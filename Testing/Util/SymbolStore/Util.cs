// Sample to load PDB and dump as XML.
// Author: Mike Stall  (http://blogs.msdn.com/jmstall)
// UPDATED on 1/26/05, uses PDB wrappers from MDBG sample.
// must include reference to Mdbg (such as MdbgCore.dll)
using System;
using System.Globalization;

namespace Testing.Util.SymbolStore
{
	// Random utility methods.
	static class Util
	{
		// Format a token to a string. Tokens are in hex.
		public static string AsToken( int i )
		{
			return String.Format( CultureInfo.InvariantCulture, "0x{0:x}", i );
		}

		// Since we're spewing this to XML, spew as a decimal number.
		public static string AsIlOffset( int i )
		{
			return i.ToString();
		}
	}

}
