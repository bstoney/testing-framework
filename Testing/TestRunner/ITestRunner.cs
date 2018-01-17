using System;
using System.Collections.Generic;
using System.Text;

namespace Testing.TestRunner
{
	/// <summary>
	/// The ITestRunner interface can be implemented by an attribute class, which when associated
	/// with a test method will override the default test runner. This enables definition of 
	/// customised tests.
	/// </summary>
	public interface ITestRunner
	{
		/// <summary>
		/// This method is called by the test object to invoke the test.
		/// </summary>
		/// <param name="test">The calling test object.</param>
		void RunTest( ITest test );
	}
}
