using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Testing.TestRunner;
using System.Diagnostics.SymbolStore;
using System.IO;
using Testing.Util.SymbolStore;

namespace Testing.Util.SymbolStore
{
	/// <summary>
	/// A class for managing IL and source code sequence points.
	/// </summary>
	public class SequenceManager
	{
		#region Fields
		private MemberInfo _method;
		private bool _isSourceAvailable;
		private int _count;
		private int[] _offsets;
		private ISymbolDocument[] _documents;
		private int[] _startColumns;
		private int[] _endColumns;
		private int[] _startRows;
		private int[] _endRows;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the method which this sequence is based on.
		/// </summary>
		public MemberInfo Method
		{
			get { return _method; }
		}

		/// <summary>
		/// Gets the number of sequence points.
		/// </summary>
		public int Count
		{
			get { return _count; }
		}

		/// <summary>
		/// Gets an array of IL code offsets.
		/// </summary>
		public int[] Offsets
		{
			get { return _offsets; }
		}

		/// <summary>
		/// Gets an array of source files for each offset.
		/// </summary>
		public ISymbolDocument[] Documents
		{
			get { return _documents; }
		}

		/// <summary>
		/// Gets an array of start columns for each offset.
		/// </summary>
		public int[] StartColumns
		{
			get { return _startColumns; }
		}

		/// <summary>
		/// Gets an array of end columns for each offset.
		/// </summary>
		public int[] EndColumns
		{
			get { return _endColumns; }
		}

		/// <summary>
		/// Gets an array of start rows for each offset.
		/// </summary>
		public int[] StartRows
		{
			get { return _startRows; }
		}

		/// <summary>
		/// Gets an array of end rows for each offset.
		/// </summary>
		public int[] EndRows
		{
			get { return _endRows; }
		}
		#endregion

		/// <summary>
		/// Creates a new sequence point manager.
		/// </summary>
		public SequenceManager( MemberInfo method )
		{
			_method = method;
			_isSourceAvailable = GetSourceReference();
		}

		/// <summary>
		/// Gets information about the source if it is available.
		/// </summary>
		private bool GetSourceReference()
		{
			try
			{
				ISymbolReader sr = SymUtil.GetSymbolReaderForFile( _method.Module.Assembly.Location, null );
				ISymbolMethod sm = sr.GetMethod( new SymbolToken( _method.MetadataToken ) );
				_count = sm.SequencePointCount;
				_offsets = new int[_count];
				_documents = new ISymbolDocument[_count];
				_startColumns = new int[_count];
				_endColumns = new int[_count];
				_startRows = new int[_count];
				_endRows = new int[_count];
				sm.GetSequencePoints( _offsets, _documents, _startRows, _startColumns, _endRows, _endColumns );
				return true;
			}
			catch
			{
				_count = 0;
				_offsets = null;
				_documents = null;
				_startColumns = null;
				_endColumns = null;
				_startRows = null;
				_endRows = null;
				return false;
			}
		}

		/// <summary>
		/// Indicates whether the source information is available.
		/// </summary>
		public bool IsSourceAvailable()
		{
			return _isSourceAvailable;
		}

		/// <summary>
		/// Indicates whether the source is available for the assembly.
		/// </summary>
		public static bool IsSourceAvailable( Assembly assembly )
		{
			bool sourceIsAvailable = true;
			try
			{
				ISymbolReader sr = SymUtil.GetSymbolReaderForFile( assembly.Location, null );
			}
			catch
			{
				sourceIsAvailable = false;
			}
			return sourceIsAvailable;
		}

		/// <summary>
		/// Gets a stack trace string for the first line of the method.
		/// </summary>
		public string GetStackTrace( int offset )
		{
			if( offset < 0 || offset >= _count )
			{
				throw new ArgumentOutOfRangeException( "offset" );
			}
			string stackTrace = null;
			if( _isSourceAvailable )
			{
				stackTrace = String.Format( "   at {0}.{1}() in {2}:line {3}",
					 _method.DeclaringType.FullName, _method.Name, _documents[offset].URL, _startRows[offset] );
			}
			return stackTrace;
		}
	}

}
