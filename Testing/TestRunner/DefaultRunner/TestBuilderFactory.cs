using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace Testing.TestRunner.DefaultRunner
{
	internal sealed class TestBuilderFactory : ITestBuilderFactory
	{
		public List<string> _namespaces;
		SortedDictionary<string, ITestSuiteRunner> _testSuiteRunners;

		private static MethodInfo GetMethodWithCustomAttribute( Type attribute, Type type )
		{
			MethodInfo[] methods = GetMethodsWithCustomAttribute( attribute, type );
			if( methods.Length > 0 )
			{
				return methods[0];
			}
			return null;
		}

		private static MethodInfo[] GetMethodsWithCustomAttribute( Type attribute, Type type )
		{
			List<MethodInfo> methods = new List<MethodInfo>();
			foreach( MethodInfo m in type.GetMethods() )
			{
				object[] ca = m.GetCustomAttributes( attribute, false );
				if( ca.Length != 0 )
				{
					methods.Add( m );
				}
			}
			return methods.ToArray();
		}

		private void LoadNamespaces( Assembly assembly )
		{
			_namespaces = new List<string>();
			Type[] types = assembly.GetExportedTypes();
			for( int i = 0; i < types.Length; i++ )
			{
				TestFixtureAttribute[] tfa = types[i].GetCustomAttributes( typeof( TestFixtureAttribute ),
					false ) as TestFixtureAttribute[];
				if( tfa != null && tfa.Length == 1 )
				{
					string[] namespaces = types[i].Namespace.Split( '.' );

					for( int j = namespaces.Length; j > 0; j-- )
					{
						string ns = String.Join( ".", namespaces, 0, j );
						if( !_namespaces.Contains( ns ) )
						{
							_namespaces.Add( ns );
						}
						else
						{
							break;
						}
					}
				}
			}
			_namespaces.Sort();
		}

		private void LoadTestSuiteRunners( Assembly assembly )
		{
			_testSuiteRunners = new SortedDictionary<string, ITestSuiteRunner>();

			ITestSuiteRunner[] tsr = assembly.GetCustomAttributes( typeof( ITestSuiteRunner ), false )
				as ITestSuiteRunner[];
			if( tsr != null && tsr.Length == 1 )
			{
				_testSuiteRunners[String.Empty] = tsr[0];
			}
			else
			{
				_testSuiteRunners[String.Empty] = new DefaultTestSuiteRunner();
			}

			TestSuiteAttribute[] tsa = assembly.GetCustomAttributes( typeof( TestSuiteAttribute ), false )
				as TestSuiteAttribute[];
			if( tsa != null )
			{
				for( int i = 0; i < tsa.Length; i++ )
				{
					ITestSuiteRunner runner = Activator.CreateInstance( tsa[i].TestSuiteRunner ) as ITestSuiteRunner;
					if( runner != null )
					{
						_testSuiteRunners[tsa[i].Namespace] = runner;
					}
				}
			}
		}

		private ITestSuiteRunner GetTestSuiteRunner( string namespacePath )
		{
			ITestSuiteRunner runner = null;
			string[] keys = new List<string>( _testSuiteRunners.Keys ).ToArray();
			int index = Array.BinarySearch<string>( keys, namespacePath );
			if( index >= 0 )
			{
				runner = _testSuiteRunners[keys[index]];
			}
			else
			{
				runner = _testSuiteRunners[keys[(index ^ -1) - 1]];
			}
			return runner;
		}

		#region ITestBuilderFactory Members

		public ITestSuite GetTestSuite( Assembly assembly, string nspace )
		{
			LoadNamespaces( assembly );
			LoadTestSuiteRunners( assembly );

			// TODO encapsulate test suite constructor.
			TestSuite ts = new TestSuite();
			ts.Name = assembly.GetName().FullName;
			ts.Assembly = assembly;
			ts.Namespace = (nspace != null ? nspace : String.Empty);
			ts.TestSuiteRunner = GetTestSuiteRunner( ts.Namespace );

			return ts;
		}

		/// <summary>
		/// Get a list of test suites, each child namespace is a separate test suite.
		/// </summary>
		public ITestSuite[] GetTestSuites( ITestSuite testSuite )
		{
			Regex nsMatch;
			if( String.IsNullOrEmpty( testSuite.Namespace ) )
			{
				nsMatch = new Regex( String.Concat( @"^\w+$" ) );
			}
			else
			{
				nsMatch = new Regex( String.Concat( "^", testSuite.Namespace, @"\.\w+$" ) );
			}

			Predicate<string> match = delegate( string ns ) { return nsMatch.IsMatch( ns ); };
			List<string> namespaces = _namespaces.FindAll( match );

			List<ITestSuite> testSuites = new List<ITestSuite>();
			for( int i = 0; i < namespaces.Count; i++ )
			{
				// TODO encapsulate test suite constructor.
				TestSuite ts = new TestSuite();
				ts.Name = namespaces[i];
				ts.Assembly = testSuite.Assembly;
				ts.Namespace = namespaces[i];
				ts.TestSuiteRunner = GetTestSuiteRunner( namespaces[i] );
				testSuites.Add( ts );
			}
			return testSuites.ToArray();
		}

		public IFixture[] GetFixtures( ITestSuite testSuite )
		{
			List<IFixture> fixtures = new List<IFixture>();

			Type[] types = testSuite.Assembly.GetExportedTypes();
			foreach( Type t in types )
			{
				if( t.Namespace == testSuite.Namespace )
				{
					TestFixtureAttribute[] tfa = t.GetCustomAttributes( typeof( TestFixtureAttribute ),
						false ) as TestFixtureAttribute[];
					if( tfa != null && tfa.Length == 1 )
					{
						IFixture f = testSuite.CreateFixture();
						f.Name = t.Name;
						f.Description = tfa[0].Description;
						f.FixtureType = t;

						f.StartUpMethod = GetMethodWithCustomAttribute( typeof( TestFixtureSetUpAttribute ), t );
						f.TearDownMethod = GetMethodWithCustomAttribute( typeof( TestFixtureTearDownAttribute ), t );

						IgnoreAttribute[] ia = t.GetCustomAttributes( typeof( IgnoreAttribute ), false )
							as IgnoreAttribute[];
						if( ia != null && ia.Length == 1 )
						{
							f.Ignore = true;
							f.IgnoreReason = ia[0].Reason;
						}

						IFixtureRunner[] fra = t.GetCustomAttributes( typeof( IFixtureRunner ), false )
							as IFixtureRunner[];
						if( fra != null && fra.Length == 1 )
						{
							f.FixtureRunner = fra[0];
						}
						else
						{
							f.FixtureRunner = new DefaultFixtureRunner();
						}

						fixtures.Add( f );
					}
				}
			}

			return fixtures.ToArray();
		}

		public ITest[] GetTests( IFixture fixture )
		{
			List<ITest> tests = new List<ITest>();
			MethodInfo startUp = GetMethodWithCustomAttribute( typeof( SetUpAttribute ), fixture.FixtureType );
			MethodInfo tearDown = GetMethodWithCustomAttribute( typeof( TearDownAttribute ), fixture.FixtureType );

			MethodInfo[] methods = GetMethodsWithCustomAttribute( typeof( TestAttribute ), fixture.FixtureType );
			foreach( MethodInfo m in methods )
			{
				ITest t = fixture.CreateTest();
				t.Name = String.Concat( fixture.FixtureType.FullName, ".", m.Name );
				t.TestMethod = m;
				t.StartUpMethod = startUp;
				t.TearDownMethod = tearDown;

				TestAttribute ta = m.GetCustomAttributes( typeof( TestAttribute ), false )[0] as TestAttribute;
				t.Description = ta.Description;

				IgnoreAttribute[] ia = m.GetCustomAttributes( typeof( IgnoreAttribute ), false ) as IgnoreAttribute[];
				if( ia != null && ia.Length == 1 )
				{
					t.Ignore = true;
					t.IgnoreReason = ia[0].Reason;
				}

				ITestRunner[] tra = m.GetCustomAttributes( typeof( ITestRunner ), false ) as ITestRunner[];
				if( tra != null && tra.Length == 1 )
				{
					t.TestRunner = tra[0];
				}
				else
				{
					t.TestRunner = new DefaultTestRunner();
				}

				tests.Add( t );
			}

			return tests.ToArray();
		}

		#endregion
	}
}
