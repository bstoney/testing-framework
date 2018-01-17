// Sample to load PDB and dump as XML.
// Author: Mike Stall  (http://blogs.msdn.com/jmstall)
// UPDATED on 1/26/05, uses PDB wrappers from MDBG sample.
// must include reference to Mdbg (such as MdbgCore.dll)
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;

namespace Testing.Util.SymbolStore
{

	/// <summary>
	/// Class to write out XML for a PDB.
	/// </summary>
	class Pdb2XmlConverter
	{
		/// <summary>
		/// Initialize the Pdb to Xml converter. Actual conversion happens in ReadPdbAndWriteToXml.
		/// Passing a filename also makes it easy for us to use reflection to get some information
		/// (such as enumeration)
		/// </summary>
		/// <param name="writer">XmlWriter to spew to.</param>
		/// <param name="fileName">Filename for exe/dll. This class will find the pdb to match.</param>
		public Pdb2XmlConverter( XmlWriter writer, string fileName )
		{
			m_writer = writer;
			m_fileName = fileName;
		}

		// The filename that the pdb is for.
		string m_fileName;
		XmlWriter m_writer;

		// Keep assembly so we can query metadata on it.
		System.Reflection.Assembly m_assembly;

		// Maps files to ids.
		Dictionary<string, int> m_fileMapping = new Dictionary<string, int>();

		/// <summary>
		/// Load the PDB given the parameters at the ctor and spew it out to the XmlWriter specified
		/// at the ctor.
		/// </summary>
		public void ReadPdbAndWriteToXml()
		{
			// Actually load the files
			ISymbolReader reader = SymUtil.GetSymbolReaderForFile( m_fileName, null );
			m_assembly = Assembly.ReflectionOnlyLoadFrom( m_fileName );

			// Begin writing XML.
			m_writer.WriteStartDocument();
			m_writer.WriteComment( "This is an XML file representing the PDB for '" + m_fileName + "'" );
			m_writer.WriteStartElement( "symbols" );


			// Record what input file these symbols are for.
			m_writer.WriteAttributeString( "file", m_fileName );

			WriteDocList( reader );
			WriteEntryPoint( reader );
			WriteAllMethods( reader );

			m_writer.WriteEndElement(); // "Symbols";
		}

		// Dump all of the methods in the given ISymbolReader to the XmlWriter provided in the ctor.
		void WriteAllMethods( ISymbolReader reader )
		{
			m_writer.WriteComment( "This is a list of all methods in the assembly that matches this PDB." );
			m_writer.WriteComment( "For each method, we provide the sequence tables that map from IL offsets back to source." );

			m_writer.WriteStartElement( "methods" );

			// Use reflection to enumerate all methods
			foreach( Type t in m_assembly.GetTypes() )
			{
				foreach( MethodInfo methodReflection in t.GetMethods( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly ) )
				{
					int token = methodReflection.MetadataToken;
					ISymbolMethod methodSymbol = null;


					m_writer.WriteStartElement( "method" );
					{
						m_writer.WriteAttributeString( "name", t.FullName + "." + methodReflection.Name );
						m_writer.WriteAttributeString( "token", Util.AsToken( token ) );
						try
						{
							methodSymbol = reader.GetMethod( new SymbolToken( token ) );
							WriteSequencePoints( methodSymbol );
							WriteLocals( methodSymbol );
						}
						catch( COMException e )
						{
							m_writer.WriteComment( String.Concat( "No symbol info", e.Message ) );
						}
					}
					m_writer.WriteEndElement(); // method
				}
			}
			m_writer.WriteEndElement();
		}

		// Write all the locals in the given method out to an XML file.
		// Since the symbol store represents the locals in a recursive scope structure, we need to walk a tree.
		// Although the locals are technically a hierarchy (based off nested scopes), it's easiest for clients
		// if we present them as a linear list. We will provide the range for each local's scope so that somebody
		// could reconstruct an approximation of the scope tree. The reconstruction may not be exact.
		// (Note this would still break down if you had an empty scope nested in another scope.
		void WriteLocals( ISymbolMethod method )
		{
			m_writer.WriteStartElement( "locals" );
			{
				// If there are no locals, then this element will just be empty.
				WriteLocalsHelper( method.RootScope );
			}
			m_writer.WriteEndElement();
		}

		// Helper method to write the local variables in the given scope.
		// Scopes match an IL range, and also have child scopes.
		void WriteLocalsHelper( ISymbolScope scope )
		{
			foreach( ISymbolVariable l in scope.GetLocals() )
			{
				m_writer.WriteStartElement( "local" );
				{
					m_writer.WriteAttributeString( "name", l.Name );

					// Each local maps to a unique "IL Index" or "slot" number.
					// This index is what you pass to ICorDebugILFrame::GetLocalVariable() to get
					// a specific local variable.
					Debug.Assert( l.AddressKind == SymAddressKind.ILOffset );
					int slot = l.AddressField1;
					m_writer.WriteAttributeString( "il_index", slot.ToString() );

					// Provide scope range
					m_writer.WriteAttributeString( "il_start", Util.AsIlOffset( scope.StartOffset ) );
					m_writer.WriteAttributeString( "il_end", Util.AsIlOffset( scope.EndOffset ) );
				}
				m_writer.WriteEndElement(); // local
			}

			foreach( ISymbolScope childScope in scope.GetChildren() )
			{
				WriteLocalsHelper( childScope );
			}
		}

		// Write the sequence points for the given method
		// Sequence points are the map between IL offsets and source lines.
		// A single method could span multiple files (use C#'s #line directive to see for yourself).
		void WriteSequencePoints( ISymbolMethod method )
		{
			m_writer.WriteStartElement( "sequencepoints" );

			int count = method.SequencePointCount;
			m_writer.WriteAttributeString( "total", count.ToString() );

			// Get the sequence points from the symbol store.
			// We could cache these arrays and reuse them.
			int[] offsets = new int[count];
			ISymbolDocument[] docs = new ISymbolDocument[count];
			int[] startColumn = new int[count];
			int[] endColumn = new int[count];
			int[] startRow = new int[count];
			int[] endRow = new int[count];
			method.GetSequencePoints( offsets, docs, startRow, startColumn, endRow, endColumn );

			// Write out sequence points
			for( int i = 0; i < count; i++ )
			{
				m_writer.WriteStartElement( "entry" );
				m_writer.WriteAttributeString( "il_offset", Util.AsIlOffset( offsets[i] ) );

				// If it's a special 0xFeeFee sequence point (eg, "hidden"),
				// place an attribute on it to make it very easy for tools to recognize.
				// See http://blogs.msdn.com/jmstall/archive/2005/06/19/FeeFee_SequencePoints.aspx
				if( startRow[i] == 0xFeeFee )
				{
					m_writer.WriteAttributeString( "hidden", XmlConvert.ToString( true ) );
				}
				else
				{
					m_writer.WriteAttributeString( "start_row", startRow[i].ToString() );
					m_writer.WriteAttributeString( "start_column", startColumn[i].ToString() );
					m_writer.WriteAttributeString( "end_row", endRow[i].ToString() );
					m_writer.WriteAttributeString( "end_column", endColumn[i].ToString() );
					m_writer.WriteAttributeString( "file_ref", this.m_fileMapping[docs[i].URL].ToString() );
				}
				m_writer.WriteEndElement();
			}

			m_writer.WriteEndElement(); // sequencepoints
		}

		// Write all docs, and add to the m_fileMapping list.
		// Other references to docs will then just refer to this list.
		void WriteDocList( ISymbolReader reader )
		{
			m_writer.WriteComment( "This is a list of all source files referred by the PDB." );

			int id = 0;
			// Write doc list
			m_writer.WriteStartElement( "files" );
			{
				ISymbolDocument[] docs = reader.GetDocuments();
				foreach( ISymbolDocument doc in docs )
				{
					string url = doc.URL;

					// Symbol store may give out duplicate documents. We'll fold them here
					if( m_fileMapping.ContainsKey( url ) )
					{
						m_writer.WriteComment( "There is a duplicate entry for: " + url );
						continue;
					}
					id++;
					m_fileMapping.Add( doc.URL, id );

					m_writer.WriteStartElement( "file" );
					{
						m_writer.WriteAttributeString( "id", id.ToString() );
						m_writer.WriteAttributeString( "name", doc.URL );
					}
					m_writer.WriteEndElement(); // file
				}
			}
			m_writer.WriteEndElement(); // files
		}

		// Write out a reference to the entry point method (if one exists)
		void WriteEntryPoint( ISymbolReader reader )
		{
			try
			{
				// If there is no entry point token (such as in a dll), this will throw.
				SymbolToken token = reader.UserEntryPoint;
				ISymbolMethod m = reader.GetMethod( token );

				Debug.Assert( m != null ); // would have thrown by now.

				// Should not throw past this point
				m_writer.WriteComment(
					"This is the token for the 'entry point' method, which is the method that will be called when the assembly is loaded." +
					" This usually corresponds to 'Main'" );

				m_writer.WriteStartElement( "EntryPoint" );
				WriteMethod( m );
				m_writer.WriteEndElement();
			}
			catch( System.Runtime.InteropServices.COMException )
			{
				// If the Symbol APIs fail when looking for an entry point token, there is no entry point.
				m_writer.WriteComment(
					"There is no entry point token such as a 'Main' method. This module is probably a '.dll'" );
			}
		}

		// Write out XML snippet to refer to the given method.
		void WriteMethod( ISymbolMethod method )
		{
			m_writer.WriteElementString( "methodref", Util.AsToken( method.Token.GetToken() ) );
		}
	}

}
