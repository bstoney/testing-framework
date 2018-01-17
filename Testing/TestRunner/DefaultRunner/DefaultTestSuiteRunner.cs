using System;
using System.Collections.Generic;
using System.Text;

namespace Testing.TestRunner.DefaultRunner
{
	/// <summary>
	/// The default test suite runner runner. Runs each test in turn.
	/// </summary>
	public class DefaultTestSuiteRunner : ITestSuiteRunner
	{
		#region ITestSuiteRunner Members

		/// <summary>
		/// This method is called by the test suite object to invoke the tests in the test suite.
		/// </summary>
		/// <param name="testSuite">The calling test suite object.</param>
		public void RunTests( ITestSuite testSuite )
		{
			foreach( IFixture f in testSuite.Fixtures )
			{
				f.Run();
			}
		}

		#endregion
	}
}
