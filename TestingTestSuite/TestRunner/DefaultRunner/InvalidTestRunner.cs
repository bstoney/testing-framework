using System;
using System.Collections.Generic;
using System.Text;
using Testing.TestRunner;

namespace TestingTestSuite.TestRunner.DefaultRunner
{
	public class InvalidTestRunnerAttribute : Attribute, ITestRunner
	{
		#region ITestRunner Members

		public void RunTest( ITest test )
		{
			throw new Exception();
		}

		#endregion
	}

}
