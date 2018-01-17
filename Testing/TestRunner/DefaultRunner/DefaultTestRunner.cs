using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;

namespace Testing.TestRunner.DefaultRunner
{
	public sealed class DefaultTestRunner : ITestRunner
	{
		internal DefaultTestRunner()
		{
		}

		#region ITestRunner Members

		public void RunTest( ITest test )
		{
			RunTestHelper( test );
		}

		#endregion

		public static void RunTestHelper( ITest test )
		{
			test.Result.Status = TestStatus.Pass;
			TestTimer timer = new TestTimer();
			try
			{
				timer.Start( test );
				test.TestMethod.Invoke( test.Fixture.Instance, null );
				timer.Stop();
			}
			catch( TargetInvocationException tie )
			{
				timer.Stop();
				Exception exp = tie.InnerException;
				test.Result.Status = TestStatus.Fail;
				test.Result.Message.AppendLine( exp.Message );
				test.Result.Message.Append( "EXCEPTION TYPE: " );
				test.Result.Message.AppendLine( exp.GetType().FullName );
				test.Result.SetFilteredStackTrace( exp.StackTrace );
			}
			finally
			{
				timer.Stop();
			}
		}

	}
}
