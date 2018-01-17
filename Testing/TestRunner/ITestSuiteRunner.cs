using System;
using System.Collections.Generic;
using System.Text;

namespace Testing.TestRunner
{
	/// <summary>
	/// The ITestSuiteRunner interface can be implemented by an attribute class, which when associated
	/// with an assembly will override the default test suite runner. This enables the definition
	/// of customised test suites.
	/// </summary>
	public interface ITestSuiteRunner
	{
		/// <summary>
		/// This method is called by the test suite object to invoke the tests in the test suite.
		/// </summary>
		/// <param name="testSuite">The calling test suite object.</param>
		void RunTests( ITestSuite testSuite );
	}
}
