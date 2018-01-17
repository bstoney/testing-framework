// Sample to load PDB and dump as XML.
// Author: Mike Stall  (http://blogs.msdn.com/jmstall)
// UPDATED on 1/26/05, uses PDB wrappers from MDBG sample.
// must include reference to Mdbg (such as MdbgCore.dll)
using System;
using System.Runtime.InteropServices;

namespace Testing.Util.SymbolStore
{
	/// <summary>
	/// Since we're just blindly passing this interface through managed code to the Symbinder, we don't care about actually
	/// importing the specific methods.
	/// This needs to be public so that we can call Marshal.GetComInterfaceForObject() on it to get the
	/// underlying metadata pointer.
	/// </summary>
	[Guid( "7DAC8207-D3AE-4c75-9B67-92801A497D44" ), InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
	[ComVisible( true )]
	// [CLSCompliant( true )]
	public interface IMetadataImport
	{
		/// <summary>
		///  Just need a single placeholder method so that it doesn't complain about an empty interface.
		/// </summary>
		void Placeholder();
	}

}
