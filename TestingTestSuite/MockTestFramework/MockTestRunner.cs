using System;
using System.Collections.Generic;
using System.Text;
using Testing.TestRunner;

namespace TestingTestSuite.MockTestFramework
{
	public class MockTestRunner : ITestRunner
	{
		#region ITestRunner Members

		public void RunTest( ITest test )
		{
			if( test.TestMethod != null )
			{
				test.TestMethod.Invoke( test.Fixture.Instance, null );
				test.Result.Status = TestStatus.Pass;
			}
		}

		#endregion
	}
}
