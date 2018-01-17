// Sample to load PDB and dump as XML.
// Author: Mike Stall  (http://blogs.msdn.com/jmstall)
// UPDATED on 1/26/05, uses PDB wrappers from MDBG sample.
// must include reference to Mdbg (such as MdbgCore.dll)
using System;
using System.Xml;

namespace Testing.Util.SymbolStore
{

	/// <summary>
	/// Harness to drive PDB to XML reader.
	/// </summary>
	class PDB2XML
	{
		static void Main( string[] args )
		{
			Console.WriteLine( "Test harness for PDB2XML." );

			if( args.Length < 2 )
			{
				Console.WriteLine( "Usage: Pdb2Xml <input managed exe> <output xml>" );
				Console.WriteLine( "This will load the pdb for the managed exe, and then spew the pdb contents to an XML file." );
				return;
			}
			string stInputExe = args[0];
			string stOutputXml = args[1];

			// Get a Text Writer to spew the PDB to.
			XmlDocument doc = new XmlDocument();
			XmlWriter xw = doc.CreateNavigator().AppendChild();

			// Do the pdb for the exe into an xml stream.
			Pdb2XmlConverter p = new Pdb2XmlConverter( xw, stInputExe );
			p.ReadPdbAndWriteToXml();
			xw.Close();

			// Print the XML we just generated and save it to a file for more convenient viewing.
			Console.WriteLine( doc.OuterXml );

			doc.Save( stOutputXml );
			{
				// Proove that it's valid XML by reading it back in...
				XmlDocument d2 = new XmlDocument();
				d2.Load( stOutputXml );
			}

			// Now demonstrate some different queries.
			XmlNode root = doc.DocumentElement;

			DoQuery( QueryForStartRow( "Foo.Main", 3 ), root );
			DoQuery( QueryForEntryName(), root );
			DoQuery( QueryForAllLocalsInMethod( "Foo.Main" ), root );
		} // end Main

		#region Sample Queries
		// Some sample queries
		static string QueryForEntryName()
		{
			return @"/symbols/methods/method[@token=/symbols/EntryPoint/methodref]/@name";
		}
		static string QueryForAllLocalsInMethod( string stMethod )
		{
			return "/symbols/methods/method[@name=\"" + stMethod + "\"]/locals/local/@name";
		}
		static string QueryForStartRow( string stMethod, int exactILOffset )
		{
			return "/symbols/methods/method[@name=\"" + stMethod + "\"]/sequencepoints/entry[@il_offset=\"" + exactILOffset + "\"]/@start_row";
		}

#if false
		            // Here are more sample queries:
		            // Get all locals that are active at a given line?
		            @"/symbols/methods/method/locals/local[@il_start<=""2"" and @il_end>=""2""]/@name";

		            @"/symbols/files/file/@name"; // get all filenames referenced from PDB.

		            // ** All methods that have code in a given filename.
		            @"/symbols/methods/method[sequencepoints/entry/@file_ref=/symbols/files/file[@name=""c:\temp\t.cs""]/@id]/@name";

		            @"/symbols/files/file[@name=""c:\temp\t.cs""]/@id"; // File ID for a given filename:

		            @"/symbols/EntryPoint/methodref"; // entry point token.
		            @"/symbols/methods/method/@name";  // ** select all names of all methods
		            @"/symbols/methods/method[@token=""0x6000001""]";  // entire XML snippet for method with specified token value
		            @"/symbols/methods/method[@token=""0x6000001""]/@name"; // ** just the name of method with the given token value
		            @"/symbols/methods/method[@token=/symbols/EntryPoint/methodref]/@name";   // *** method name of the entry point token!!
		            @"/symbols/methods/method[@name=""Foo.Main""]/locals/local/@name"; // name of all locals in method Foo.Main
		            @"/symbols/methods/method/sequencepoints/entry[@il_offset=""0x12""]"; // sp entry for IL offset 12 (in all methods)
		            @"/symbols/methods/method[@name=""Foo.Main""]/sequencepoints/entry/@start_row"; // get all source rows for method Foo.Main
		            @"/symbols/methods/method[@name=""Foo.Main""]/sequencepoints/entry[@il_offset=""0x4""]/@start_row"; // get start row for for IL offset 4 in method Foo.Main

		            // Lookup method name + IL offset given source file + line
		            // Queries only return 1 result, so there's not a good way to get the (name, IL offset) pair back with a single query.
		            @"/symbols/methods/method/sequencepoints/entry[@start_row<=""26"" and @end_row>=""26"" and @file_ref=/symbols/files/file[@name=""c:\temp\t.cs""]/@id]/@il_offset";
		            @"/symbols/methods/method[sequencepoints/entry[@start_row<=""26"" and @end_row>=""26"" and @file_ref=/symbols/files/file[@name=""c:\temp\t.cs""]/@id]]/@name";
#endif

		#endregion


		// Helper to execute a query and print out to console.
		static void DoQuery( string stQuery, XmlNode root )
		{
			Console.WriteLine( "Query:{0}", stQuery );

			XmlNodeList nodeList = root.SelectNodes( stQuery );
			Console.WriteLine( "Found {0} item(s) in query.", nodeList.Count );
			Console.WriteLine( "(outer)-----------" );
			foreach( XmlNode x in nodeList )
			{
				Console.WriteLine( x.OuterXml );
			}
			Console.WriteLine( "(inner)-----------" );
			foreach( XmlNode x in nodeList )
			{
				Console.WriteLine( x.InnerText );
			}
			Console.WriteLine( "------------------" );
		}
	}

}
