using System;
using System.Collections.Generic;
using System.Text;
using Testing.TestRunner;
using System.Threading;
using System.Reflection;
using Testing.TestRunner.DefaultRunner;

namespace Testing.FunctionTest
{
	public sealed class MaximumExecutionTimeAttribute : Attribute, ITestRunner
	{
		private int _timeout = -1;

		/// <summary>
		/// Creates a new MaximumExecutionTime test runner.
		/// </summary>
		/// <param name="timeout">Maxinum time in milliseconds to allow the test to run.</param>
		public MaximumExecutionTimeAttribute( int timeout )
		{
			if( timeout < -1 )
			{
				throw new ArgumentOutOfRangeException( "timeout" );
			}
			_timeout = timeout;
		}

		#region ITestRunner Members

		/// <summary>
		/// RunTest.
		/// </summary>
		public void RunTest( ITest test )
		{
			ManualResetEvent mre = new ManualResetEvent( false );
			Thread t = new Thread( new ThreadStart( delegate() {
				DefaultTestRunner.RunTestHelper( test );
				mre.Set();
			} ) );
			t.Start();
			if( !WaitHandle.WaitAll( new WaitHandle[] { mre }, _timeout, false ) )
			{
				t.Abort();
				test.Result.Status = TestStatus.Fail;
				test.Result.Message.AppendFormat( "Maximim execution time {0}ms exceeded.", _timeout );
			}
		}

		#endregion
	}
}
