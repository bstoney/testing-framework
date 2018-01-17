// Project: Testing, File: Test.cs
// Namespace: Testing.TestRunner.DefaultRunner, Class: Test
// Path: D:\Development\Library.2005\Testing\TestRunner\DefaultRunner, Author: bstoney
// Code lines: 62, Size of file: 1.37 KB
// Creation date: 26/06/2006 8:19 AM
// Last modified: 29/06/2006 3:18 PM
// Copyright Plan B Financial Services

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Testing.TestRunner.DefaultRunner
{
	/// <summary>
	/// A test.
	/// </summary>
#if TEST
	public
#else
	internal sealed
#endif
		class Test : ITest
	{
		#region Private Fields

		private string _name;
		private string _description;
		private bool _ignore;
		private string _ignoreReason;
		private ITestRunner _testRunner;
		private IFixture _fixture;
		private MethodInfo _test;
		private MethodInfo _startUp;
		private MethodInfo _tearDown;
		private TestResult _result;
		private TestListenerCollection _listeners;

		#endregion

		/// <summary>
		/// Creates a new TestBase.
		/// </summary>
		public Test()
		{
			_result = new TestResult();
			_listeners = new TestListenerCollection();
		}

		#region ITest Members

		/// <summary>
		/// Gets or sets the name of the test.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// Gets or sets a descriptin of the test.
		/// </summary>
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		/// <summary>
		/// Indicates whether the test is to be ignored. Ignored tests will not be run.
		/// </summary>
		public bool Ignore
		{
			get { return _ignore; }
			set { _ignore = value; }
		}

		/// <summary>
		/// Gets or sets the reason a test is to be ignored.
		/// </summary>
		public string IgnoreReason
		{
			get { return _ignoreReason; }
			set { _ignoreReason = value; }
		}

		/// <summary>
		/// Gets or sets the ITestRunner uses to execute the test.
		/// </summary>
		public ITestRunner TestRunner
		{
			get { return _testRunner; }
			set { _testRunner = value; }
		}

		/// <summary>
		/// Gets or sets the test fixture of this test.
		/// </summary>
		public IFixture Fixture
		{
			get { return _fixture; }
			set { _fixture = value; }
		}

		/// <summary>
		/// Gets or sets the method which defines the test.
		/// </summary>
		public MethodInfo TestMethod
		{
			get { return _test; }
			set { _test = value; }
		}

		/// <summary>
		/// Gets or sets a method which will be invoked before the test is run.
		/// </summary>
		public MethodInfo StartUpMethod
		{
			get { return _startUp; }
			set { _startUp = value; }
		}

		/// <summary>
		/// Gets or sets a method which will be invoked after the test is run.
		/// </summary>
		public MethodInfo TearDownMethod
		{
			get { return _tearDown; }
			set { _tearDown = value; }
		}

		/// <summary>
		/// Gets a TestResult object for storing and retrieving results.
		/// </summary>
		public TestResult Result
		{
			get { return _result; }
		}

		/// <summary>
		/// Gets a count of all the tests in this component.
		/// </summary>
		public int TestCount
		{
			get { return 1; }
		}

		/// <summary>
		/// Resets the tests to untested state.
		/// </summary>
		public void Reset()
		{
			Result.Reset();
		}

		/// <summary>
		/// Notifies the listeners at the start of a test.
		/// </summary>
		public void Begin()
		{
			_listeners.NotifyBeginTest( this );
		}

		/// <summary>
		/// Notifies the listeners at the end of a test.
		/// </summary>
		public void End()
		{
			_listeners.NotifyEndTest( this );
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
					bool setupFailed = false;
					if( StartUpMethod != null )
					{
						try
						{
							StartUpMethod.Invoke( Fixture.Instance, null );
						}
						catch( TargetInvocationException tie )
						{
							Exception exp = tie.InnerException;
							Result.Status = TestStatus.Untested;
							Result.Message.AppendLine( "Test set-up failed" );
							Result.SetFilteredStackTrace( exp.StackTrace );
							Result.WriteExceptionToMessage( exp );
							setupFailed = true;
						}
					}
					if( !setupFailed )
					{
						try
						{
							TestRunner.RunTest( this );
						}
						catch( Exception exp )
						{
							Result.Status = TestStatus.Untested;
							Result.Message.AppendLine( "Test runner failed" );
							Result.StackTrace = exp.StackTrace;
							Result.WriteExceptionToOutput( exp );
						}
						if( TearDownMethod != null )
						{
							try
							{
								TearDownMethod.Invoke( Fixture.Instance, null );
							}
							catch( TargetInvocationException tie )
							{
								Exception exp = tie.InnerException;
								Result.Message.AppendLine( "Test tear-down failed" );
								Result.WriteExceptionToOutput( exp );
							}
						}
					}
				}
				else
				{
					Result.WriteIgnoreToMessage( this, IgnoreReason );
				}
			}
			finally
			{
				End();
			}
		}

		/// <summary>
		/// Register an test listener to be notified.
		/// </summary>
		public void RegisterListener( ITestListener listener )
		{
			_listeners.Add( listener );
		}

		/// <summary>
		/// Remove an listener from the notification list.
		/// </summary>
		public void UnregisterListener( ITestListener listener )
		{
			_listeners.Remove( listener );
		}

		#endregion
	}
}
