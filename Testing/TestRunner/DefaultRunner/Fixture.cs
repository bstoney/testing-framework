using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Testing.TestRunner.DefaultRunner
{
	/// <summary>
	/// A test fixture.
	/// </summary>
#if TEST
	public
#else
	internal sealed
#endif
 class Fixture : IFixture
	{

		#region Private Fields

		private string _name;
		private string _description;
		private bool _ignore;
		private string _ignoreReason;
		private Type _type;
		private List<ITest> _tests;
		private IFixtureRunner _fixtureRunner;
		private MethodInfo _startUp;
		private MethodInfo _tearDown;
		private TestListenerCollection _listeners;

		private object _instance;

		#endregion

		/// <summary>
		/// Creates a new FixtureBase.
		/// </summary>
		public Fixture()
		{
			_tests = new List<ITest>();
			_listeners = new TestListenerCollection();
		}

		#region IFixture Members

		/// <summary>
		/// Gets or sets the name of the test fixture.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Gets or sets a description of the test fixture.
		/// </summary>
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		/// <summary>
		/// Indicates whether the fixture is to be ignored. Ignored fixture will not be run.
		/// </summary>
		public bool Ignore
		{
			get { return _ignore; }
			set { _ignore = value; }
		}

		/// <summary>
		/// Gets or sets the reason a fixture is to be ignored.
		/// </summary>
		public string IgnoreReason
		{
			get { return _ignoreReason; }
			set { _ignoreReason = value; }
		}

		/// <summary>
		/// Gets or sets the object type which defines the test fixture.
		/// </summary>
		public Type FixtureType
		{
			get { return _type; }
			set
			{
				_type = value;
				_instance = null;
			}
		}

		/// <summary>
		/// Gets the instance on which to invoke the tests.
		/// </summary>
		public object Instance
		{
			get
			{
				if( _instance == null )
				{
					_instance = Activator.CreateInstance( _type );
				}
				return _instance;
			}
		}

		/// <summary>
		/// Gets a list of the tests in this fixture.
		/// </summary>
		public ITest[] Tests
		{
			get { return _tests.ToArray(); }
		}

		/// <summary>
		/// Gets or sets the runner that will be used to run the tests.
		/// </summary>
		public IFixtureRunner FixtureRunner
		{
			get { return _fixtureRunner; }
			set { _fixtureRunner = value; }
		}

		/// <summary>
		/// Gets or sets a method which will be invoked before any tests are run.
		/// </summary>
		public MethodInfo StartUpMethod
		{
			get { return _startUp; }
			set { _startUp = value; }
		}

		/// <summary>
		/// Gets or sets a method which will be invoked after all tests have been run.
		/// </summary>
		public MethodInfo TearDownMethod
		{
			get { return _tearDown; }
			set { _tearDown = value; }
		}

		/// <summary>
		/// Creates a new test instance;
		/// </summary>
#if TEST
		public virtual
#else
		public 
#endif
			ITest CreateTest()
		{
			return new Test();
		}

		/// <summary>
		/// Adds the test to the test fixture.
		/// </summary>
		/// <param name="test">The test to added.</param>
		public void AddTest( ITest test )
		{
			if( test == null )
			{
				throw new ArgumentNullException( "test" );
			}
			_tests.Add( test );
			test.Fixture = this;
			foreach( ITestListener l in _listeners )
			{
				test.RegisterListener( l );
			}
		}

		/// <summary>
		/// Removes a test from the test fixture.
		/// </summary>
		/// <param name="test">The test to removed.</param>
		public void RemoveTest( ITest test )
		{
			if( _tests.Contains( test ) )
			{
				_tests.Remove( test );
				foreach( ITestListener l in _listeners )
				{
					test.UnregisterListener( l );
				}
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
				foreach( ITest t in Tests )
				{
					c += t.TestCount;
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
			foreach( ITest t in Tests )
			{
				t.RegisterListener( listener );
			}
		}

		/// <summary>
		/// Remove an listener from the notification list.
		/// </summary>
		public void UnregisterListener( ITestListener listener )
		{
			_listeners.Remove( listener );
			foreach( ITest t in Tests )
			{
				t.UnregisterListener( listener );
			}
		}

		/// <summary>
		/// Resets all tests to untested state.
		/// </summary>
		public void Reset()
		{
			foreach( ITest test in Tests )
			{
				test.Reset();
			}
		}

		/// <summary>
		/// Notifies the listeners at the start of a fixture.
		/// </summary>
		public void Begin()
		{
			_listeners.NotifyBeginFixture( this );
		}

		/// <summary>
		/// Notifies the listeners at the end of a fixture.
		/// </summary>
		public void End()
		{
			_listeners.NotifyEndFixture( this );
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
				if( !Ignore )
				{
					if( StartUpMethod != null )
					{
						try
						{
							StartUpMethod.Invoke( Instance, null );
						}
						catch( TargetInvocationException tie )
						{
							Exception exp = tie.InnerException;
							foreach( ITest test in Tests )
							{
								test.Begin();
								if( !test.Ignore )
								{
									test.Result.Message.AppendLine( "Fixture set-up failed" );
									test.Result.SetFilteredStackTrace( exp.StackTrace );
									test.Result.WriteExceptionToMessage( exp );
								}
								else
								{
									test.Result.WriteIgnoreToMessage( test, test.IgnoreReason );
								}
								test.End();
							}
							return;
						}
					}
					try
					{
						FixtureRunner.RunTests( this );
					}
					catch( Exception exp )
					{
						foreach( ITest test in Tests )
						{
							if( !test.Ignore )
							{
								test.Result.Message.AppendLine( "Fixture runner failed" );
								test.Result.WriteExceptionToOutput( exp );
								if( test.Result.Status == TestStatus.Untested )
								{
									test.Begin();
									test.Result.StackTrace = exp.StackTrace;
									test.End();
								}
							}
							else if( test.Result.Status == TestStatus.Untested )
							{
								test.Begin();
								test.Result.WriteIgnoreToMessage( test, test.IgnoreReason );
								test.End();
							}
						}
					}
					if( TearDownMethod != null )
					{
						try
						{
							TearDownMethod.Invoke( Instance, null );
						}
						catch( TargetInvocationException tie )
						{
							Exception exp = tie.InnerException;
							foreach( ITest test in Tests )
							{
								if( !test.Ignore )
								{
									test.Result.Message.AppendLine( "Fixture tear-down failed" );
									test.Result.WriteExceptionToOutput( exp );
								}
							}
						}
					}
				}
				else
				{
					foreach( ITest test in Tests )
					{
						test.Begin();
						test.Result.WriteIgnoreToMessage( test, IgnoreReason );
						test.End();
					}
				}
			}
			finally
			{
				End();
			}
		}

		#endregion

	}
}
