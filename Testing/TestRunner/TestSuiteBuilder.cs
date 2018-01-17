using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Testing.TestRunner.DefaultRunner;
using System.IO;
using Testing.UnitTest;
using Testing.Util.SymbolStore;

namespace Testing.TestRunner
{
	/// <summary>
	/// A helper class for loading the a default test suite.
	/// </summary>
	internal sealed class TestSuiteBuilder
	{

		#region Private Fields

		private ITestBuilderFactory _testBuilderFactory;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new TestSuiteBuilder.
		/// </summary>
		public TestSuiteBuilder() : this( new TestBuilderFactory() ) { }

		/// <summary>
		/// Creates a new TestSuiteBuilder.
		/// </summary>
		/// <param name="testBuilderFactory">An alternate test builder factory to use.</param>
		public TestSuiteBuilder( ITestBuilderFactory testBuilderFactory )
		{
			_testBuilderFactory = testBuilderFactory;
		}

		#endregion

		/// <summary>
		/// Builds a test suite from an assembly.
		/// </summary>
		/// <param name="assembly">The assembly which defines the test suite.</param>
		/// <returns>A new test suite.</returns>
		public ITestSuite BuildTest( Assembly assembly )
		{
			return BuildTestSuite( assembly, null );
		}

		/// <summary>
		/// Builds a test able component of the assembly.
		/// </summary>
		/// <param name="member">A member, either method or type, which defines the component.</param>
		/// <returns>A new test suite containing the component.</returns>
		public ITestSuite BuildTest( MemberInfo member )
		{
			if( member.MemberType == MemberTypes.TypeInfo )
			{
				return BuildTest( member as Type );
			}
			else if( member.MemberType == MemberTypes.Method )
			{
				return BuildTest( member as MethodInfo );
			}
			throw new ArgumentException( "Invalid member type." );
		}

		/// <summary>
		/// Builds a test fixture from a type.
		/// </summary>
		/// <param name="type">A type which defines the test fixture.</param>
		/// <returns>A new test suite containing the test fixture.</returns>
		public ITestSuite BuildTest( Type type )
		{
			ITestSuite ts = BuildTestSuite( type.Assembly, type.Namespace );
			foreach( IFixture f in ts.Fixtures )
			{
				if( f.Name != type.Name )
				{
					ts.RemoveFixture( f );
				}
			}
			// None of the child test suites are required for a single method test.
			foreach( ITestSuite cts in ts.TestSuites )
			{
				ts.RemoveTestSuite( cts );
			}
			return ts;
		}

		/// <summary>
		/// Builds a test from the method.
		/// </summary>
		/// <param name="method">A method which defines the test.</param>
		/// <returns>A new test suite containing the test.</returns>
		public ITestSuite BuildTest( MethodInfo method )
		{
			ITestSuite ts = BuildTestSuite( method.DeclaringType.Assembly, method.DeclaringType.Namespace );
			foreach( IFixture f in ts.Fixtures )
			{
				if( f.FixtureType != method.DeclaringType )
				{
					ts.RemoveFixture( f );
				}
				else
				{
					foreach( ITest t in f.Tests )
					{
						if( t.TestMethod == null || t.TestMethod.Name != method.Name )
						{
							f.RemoveTest( t );
						}
					}
				}
			}
			// None of the child test suites are required for a single method test.
			foreach( ITestSuite cts in ts.TestSuites )
			{
				ts.RemoveTestSuite( cts );
			}
			return ts;
		}

		/// <summary>
		/// Builds a test suite from an assembly, using only the test fixtures in the supplied namespace
		/// </summary>
		/// <param name="assembly">The assembly which defines the test suite.</param>
		/// <param name="nspace">The namespace which contains the test fixtures.</param>
		/// <returns>A new test suite containing the test fixtures.</returns>
		public ITestSuite BuildTest( Assembly assembly, string nspace )
		{
			return BuildTestSuite( assembly, nspace );
		}

		/// <summary>
		/// Helper function to build an entire test suite.
		/// </summary>
		private ITestSuite BuildTestSuite( Assembly assembly, string nspace )
		{
			BuildReferenceTree( assembly );
			ITestSuite testSuite = _testBuilderFactory.GetTestSuite( assembly, nspace );
			BuildTestSuite( testSuite );
			return testSuite;
		}

		/// <summary>
		/// Helper function to build the fixtures for a test suite.
		/// </summary>
		private void BuildTestSuite( ITestSuite testSuite )
		{
			IFixture[] fixtures = _testBuilderFactory.GetFixtures( testSuite );
			foreach( IFixture fixture in fixtures )
			{
				ITest[] tests = _testBuilderFactory.GetTests( fixture );
				foreach( ITest test in tests )
				{
					fixture.AddTest( test );
				}
				testSuite.AddFixture( fixture );
			}
			// Build child suites.
			ITestSuite[] testSuites = _testBuilderFactory.GetTestSuites( testSuite );
			foreach( ITestSuite ts in testSuites )
			{
				testSuite.AddTestSuite( ts );
				BuildTestSuite( ts );
			}
		}

		#region Assembly Resolution

		private List<string> _resolverPaths = new List<string>();

		private void BuildReferenceTree( Assembly assembly )
		{
			List<string> references = new List<string>();
			Stack<Assembly> assemblies = new Stack<Assembly>();
			assemblies.Push( assembly );

			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler( CurrentDomain_AssemblyResolve );

			while( assemblies.Count > 0 )
			{
				Assembly parentAssembly = assemblies.Pop();
				if( !_resolverPaths.Contains( parentAssembly.Location ) )
				{
					_resolverPaths.Add( Path.GetDirectoryName( parentAssembly.Location ) );
				}
				references.Add( parentAssembly.FullName );
				foreach( AssemblyName childAssembly in parentAssembly.GetReferencedAssemblies() )
				{
					if( !references.Contains( childAssembly.FullName ) )
					{
						Assembly asm = Assembly.Load( childAssembly );
						if( SequenceManager.IsSourceAvailable( asm ) )
						{
							assemblies.Push( asm );
						}
					}
				}
			}
			foreach( string reference in references )
			{
				Type[] types = AppDomain.CurrentDomain.Load( reference ).GetTypes();
			}
		}

		private Assembly CurrentDomain_AssemblyResolve( object sender, ResolveEventArgs args )
		{
			string[] possibleMatches = new string[] {
				@"{0}\{1}.dll",
				@"{0}\{1}.exe",
				@"{0}\{1}\{1}.dll",
				@"{0}\{1}\{1}.exe",
			};
			Assembly assembly = null;
			AssemblyName assemblyName = new AssemblyName( args.Name );
			for( int i = 0; i < _resolverPaths.Count && assembly == null; i++ )
			{
				for( int j = 0; j < possibleMatches.Length; j++ )
				{
					string filename = string.Format( possibleMatches[j], _resolverPaths[i], assemblyName.Name );
					if( File.Exists( filename ) )
					{
						assembly = Assembly.LoadFrom( filename );
						break;
					}
				}
			}
			return assembly;
		}

		#endregion

		// TODO load tests in separate AppDomain.
		//public static TestSuiteBuilder GetTestSuiteBuilder( string assemblyFile )
		//{
		//    // TODO make this method usable by other test suite builders.
		//    Type tsbType = typeof( TestSuiteBuilder );
		//    AppDomain tsbDomain = AssemblyResolver.MakeAppDomain( assemblyFile );

		//    // Inject assembly resolver into remote domain to help locate our assemblies
		//    AssemblyResolver assemblyResolver = (AssemblyResolver)tsbDomain.CreateInstanceFromAndUnwrap(
		//        typeof( AssemblyResolver ).Assembly.CodeBase, typeof( AssemblyResolver ).FullName );

		//    // Tell resolver to use our core assembly in the test domain
		//    assemblyResolver.AddFile( tsbType.Assembly.Location );

		//    object obj = tsbDomain.CreateInstanceAndUnwrap( tsbType.Assembly.FullName, tsbType.FullName,
		//        false, BindingFlags.Default, null, null, null, null, null );

		//    TestSuiteBuilder tsb = (TestSuiteBuilder)obj;
		//    tsb._domain = tsbDomain;

		//    return tsb;
		//}

	}
}
