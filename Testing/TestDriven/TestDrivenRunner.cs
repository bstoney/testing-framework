using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using TestDriven.Framework;
using System.Diagnostics;
using System.Threading;
using Testing.TestRunner.FilteredRunner;

namespace Testing.TestDriven
{
	/// <summary>
	/// Helper class to run tests from TestDriven
	/// </summary>
	[CoverageExclude( "Used only for intergration with VS, no need to test" )]
	public sealed class TestDrivenRunner : ITestRunner
	{
		#region ITestRunner Members

		/// <summary>
		/// Runs the tests in an assembly.
		/// </summary>
		public TestRunState RunAssembly( ITestListener testListener, Assembly assembly )
		{
			switch ( FilteredTestRunner.RunAssembly( new ProxyListener( testListener ), assembly ) )
			{
				case Testing.TestRunner.TestStatus.Pass:
					return TestRunState.Success;
				case Testing.TestRunner.TestStatus.Untested:
				case Testing.TestRunner.TestStatus.Ignore:
					return TestRunState.NoTests;
				case Testing.TestRunner.TestStatus.Fail:
				default:
					return TestRunState.Error;
			}
		}
		/// <summary>
		/// Runs the tests in a Type or Method.
		/// </summary>
		public TestRunState RunMember( ITestListener testListener, Assembly assembly, MemberInfo member )
		{
			switch ( FilteredTestRunner.RunMember( new ProxyListener( testListener ), assembly, member ) )
			{
				case Testing.TestRunner.TestStatus.Pass:
					return TestRunState.Success;
				case Testing.TestRunner.TestStatus.Untested:
				case Testing.TestRunner.TestStatus.Ignore:
					return TestRunState.NoTests;
				case Testing.TestRunner.TestStatus.Fail:
				default:
					return TestRunState.Error;
			}
		}
		/// <summary>
		/// Runs the tests in a namespace.
		/// </summary>
		public TestRunState RunNamespace( ITestListener testListener, Assembly assembly, string ns )
		{
			switch ( FilteredTestRunner.RunNamespace( new ProxyListener( testListener ), assembly, ns ) )
			{
				case Testing.TestRunner.TestStatus.Pass:
					return TestRunState.Success;
				case Testing.TestRunner.TestStatus.Untested:
				case Testing.TestRunner.TestStatus.Ignore:
					return TestRunState.NoTests;
				case Testing.TestRunner.TestStatus.Fail:
				default:
					return TestRunState.Error;
			}
		}

		#endregion

		private class ProxyListener : Testing.TestRunner.ITestListener
		{
			private ITestListener _listener;

			public ProxyListener( ITestListener listener )
			{
				_listener = listener;
			}

			#region ITestListener Members

			public void BeginTestSuite( Testing.TestRunner.ITestSuite testSuite )
			{
			}

			public void EndTestSuite( Testing.TestRunner.ITestSuite testSuite )
			{
			}

			public void BeginFixture( Testing.TestRunner.IFixture fixture )
			{
			}

			public void EndFixture( Testing.TestRunner.IFixture fixture )
			{
			}

			public void BeginTest( Testing.TestRunner.ITest test )
			{
			}

			public void EndTest( Testing.TestRunner.ITest test )
			{
				lock ( _listener )
				{
					TestResult tr = new TestResult();
					tr.FixtureType = test.Fixture.Instance.GetType();
					StringBuilder sb = new StringBuilder();
					if ( test.Result.Message.Length > 0 )
					{
						if ( test.Result.Status != Testing.TestRunner.TestStatus.Ignore )
						{
							sb.Append( "MESSAGE: " );
						}
						else
						{
							sb.Append( "IGNORED: " );
						}
						sb.Append( test.Result.Message );
					}
					if ( sb.Length > 0 )
					{
						tr.Message = sb.ToString();
					}
					tr.TimeSpan = new TimeSpan( test.Result.TimeSpan );
					tr.Method = test.TestMethod;
					tr.StackTrace = test.Result.StackTrace;
					tr.Name = test.Name;
					switch ( test.Result.Status )
					{
						case Testing.TestRunner.TestStatus.Untested:
							tr.State = TestState.Ignored;
							break;
						case Testing.TestRunner.TestStatus.Pass:
							tr.State = TestState.Passed;
							break;
						case Testing.TestRunner.TestStatus.Ignore:
							tr.State = TestState.Ignored;
							break;
						case Testing.TestRunner.TestStatus.Fail:
							tr.State = TestState.Failed;
							break;
						default:
							break;
					}
					tr.TotalTests = test.TestCount;
					_listener.TestFinished( tr );

					// Write long output to the debug window.
					if ( test.Result.Output.Length > 0 && test.Result.Status != Testing.TestRunner.TestStatus.Pass )
					{
						sb.Append( test.Result.Output );
						Console.WriteLine( sb.ToString() );
					}
				}
			}

			#endregion
		}
	}
}
