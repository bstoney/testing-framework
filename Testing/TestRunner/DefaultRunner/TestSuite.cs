using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Testing.TestRunner.DefaultRunner
{
	/// <summary>
	/// A test suite.
	/// </summary>
	internal sealed class TestSuite : ITestSuite
	{

		#region Private Fields

		private string _name;
		private Assembly _assembly;
		private string _namespace;
		private List<IFixture> _fixtures;
		private List<ITestSuite> _testSuites;
		private ITestSuiteRunner _testSuiteRunner;
		private TestListenerCollection _listeners;

		#endregion

		/// <summary>
		/// Creates a new TestSuiteBase.
		/// </summary>
		public TestSuite()
		{
			_fixtures = new List<IFixture>();
			_testSuites = new List<ITestSuite>();
			_listeners = new TestListenerCollection();
		}

		#region ITestSuite Members

		/// <summary>
		/// Gets or sets the name of the test suite.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Gets or sets the test assembly.
		/// </summary>
		public Assembly Assembly
		{
			get { return _assembly; }
			set { _assembly = value; }
		}

		/// <summary>
		/// Gets or sets the namespace covered by this test suite.
		/// </summary>
		public string Namespace
		{
			get { return _namespace; }
			set { _namespace = value; }
		}

		/// <summary>
		/// Gets a read only list of all the test fixtures in the test suite.
		/// </summary>
		public IFixture[] Fixtures
		{
			get { return _fixtures.ToArray(); }
		}

		/// <summary>
		/// Gets a read only list of all the test suites in the test suite.
		/// </summary>
		public ITestSuite[] TestSuites
		{
			get { return _testSuites.ToArray(); }
		}

		/// <summary>
		/// Gets or sets the runner that will be used to run the tests.
		/// </summary>
		public ITestSuiteRunner TestSuiteRunner
		{
			get { return _testSuiteRunner; }
			set { _testSuiteRunner = value; }
		}

		/// <summary>
		/// Adds the test fixture to the test suite.
		/// </summary>
		/// <param name="fixture">The test fixture to add.</param>
		public void AddFixture( IFixture fixture )
		{
			if( fixture == null )
			{
				throw new ArgumentNullException( "fixture" );
			}
			_fixtures.Add( fixture );
			foreach( ITestListener l in _listeners )
			{
				fixture.RegisterListener( l );
			}
		}

		/// <summary>
		/// Removes a test fixture from the test suite.
		/// </summary>
		/// <param name="fixture">The test fixture to remove.</param>
		public void RemoveFixture( IFixture fixture )
		{
			if( _fixtures.Contains( fixture ) )
			{
				_fixtures.Remove( fixture );
				foreach( ITestListener l in _listeners )
				{
					fixture.UnregisterListener( l );
				}
			}
		}

		/// <summary>
		/// Creates a new fixture instance.
		/// </summary>
#if TEST
		public virtual
#else
		public 
#endif
			IFixture CreateFixture()
		{
			return new Fixture();
		}

		/// <summary>
		/// Adds the test suite to the test suite.
		/// </summary>
		public void AddTestSuite( ITestSuite testSuite )
		{
			if( testSuite == null )
			{
				throw new ArgumentNullException( "testSuite" );
			}
			_testSuites.Add( testSuite );
			foreach( ITestListener l in _listeners )
			{
				testSuite.RegisterListener( l );
			}
		}

		/// <summary>
		/// Removes a test suite from the test suite.
		/// </summary>
		public void RemoveTestSuite( ITestSuite testSuite )
		{
			if( _testSuites.Contains( testSuite ) )
			{
				_testSuites.Remove( testSuite );
				foreach( ITestListener l in _listeners )
				{
					testSuite.UnregisterListener( l );
				}
			}
		}

		/// <summary>
		/// Runs all the tests in the component.
		/// </summary>
#if TEST
		public virtual
#else
		public 
#endif
			void Run()
		{
			Begin();
			try
			{
				foreach( ITestSuite testSuite in TestSuites )
				{
					testSuite.Run();
				}
				TestSuiteRunner.RunTests( this );
			}
			catch( Exception exp )
			{
				foreach( IFixture fixture in Fixtures )
				{
					fixture.Begin();
					foreach( ITest test in fixture.Tests )
					{
						if( !test.Ignore && !fixture.Ignore )
						{
							test.Result.Message.AppendLine( "Test suite runner failed" );
							test.Result.WriteExceptionToOutput( exp );
							if( test.Result.Status == TestStatus.Untested )
							{
								test.Begin();
								test.Result.SetFilteredStackTrace( exp.StackTrace );
								test.End();
							}
						}
						else if( test.Result.Status == TestStatus.Untested && fixture.Ignore )
						{
							test.Begin();
							test.Result.WriteIgnoreToMessage( test, fixture.IgnoreReason );
							test.End();
						}
						else if( test.Result.Status == TestStatus.Untested )
						{
							test.Begin();
							test.Result.WriteIgnoreToMessage( test, test.IgnoreReason );
							test.End();
						}
					}
					fixture.End();
				}
			}
			finally
			{
				End();
			}
		}

		/// <summary>
		/// Gets a count of all the tests in this component.
		/// </summary>
		public int TestCount
		{
			get
			{
				int c = 0;
				foreach( ITestSuite ts in TestSuites )
				{
					c += ts.TestCount;
				}
				foreach( IFixture f in Fixtures )
				{
					c += f.TestCount;
				}
				return c;
			}
		}

		/// <summary>
		/// Register an test listener to be notified.
		/// </summary>
		public void RegisterListener( ITestListener listener )
		{
			_listeners.Add( listener );
			foreach( IFixture f in Fixtures )
			{
				f.RegisterListener( listener );
			}
			foreach( ITestSuite ts in TestSuites )
			{
				ts.RegisterListener( listener );
			}
		}

		/// <summary>
		/// Remove an listener from the notification list.
		/// </summary>
		public void UnregisterListener( ITestListener listener )
		{
			_listeners.Remove( listener );
			foreach( IFixture f in Fixtures )
			{
				f.UnregisterListener( listener );
			}
			foreach( ITestSuite ts in TestSuites )
			{
				ts.UnregisterListener( listener );
			}
		}

		/// <summary>
		/// Resets all tests to untested state.
		/// </summary>
		public void Reset()
		{
			foreach( IFixture fixtures in Fixtures )
			{
				fixtures.Reset();
			}
		}

		/// <summary>
		/// Notifies the listeners at the start of a test suite.
		/// </summary>
		public void Begin()
		{
			_listeners.NotifyBeginTestSuite( this );
		}

		/// <summary>
		/// Notifies the listeners at the end of a test suite.
		/// </summary>
		public void End()
		{
			_listeners.NotifyEndTestSuite( this );
		}

		#endregion
	}
}
