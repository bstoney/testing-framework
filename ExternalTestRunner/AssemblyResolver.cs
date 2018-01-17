using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace ExternalTestRunner
{
	internal class AssemblyResolver 
	{
		public Assembly OnResolveAssembly( object sender, ResolveEventArgs args )
		{
			return null;
		}

		public static ResolveEventHandler CreateResolver( AppDomain domain, string basePath )
		{
			AssemblyName an = new AssemblyName( "TestRunAssembly" );
			AssemblyBuilder ab = domain.DefineDynamicAssembly( an, AssemblyBuilderAccess.Run );
			ModuleBuilder mb = ab.DefineDynamicModule( "TestRunModule" );
			MethodBuilder mbr = mb.DefineGlobalMethod( "OnResolveAssembly", MethodAttributes.Static | MethodAttributes.Public,
				typeof( Assembly ), new Type[] { typeof( object ), typeof( ResolveEventArgs ) } );
			ILGenerator ilg = mbr.GetILGenerator();
			ilg.Emit( OpCodes.Ret );
			mb.CreateGlobalFunctions();

			MethodInfo mi = mb.GetMethod( "OnResolveAssembly" );

			return (ResolveEventHandler)Delegate.CreateDelegate( typeof( ResolveEventHandler ), mi );

		}
	}
}
