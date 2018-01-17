using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Testing.TestRunner
{
	/// <summary>
	/// ITestBuilderFactory provides an method to extract sub-components of a testing environment.
	/// This is currently confined to a single assembly, but could be expanded.
	/// </summary>
	internal interface ITestBuilderFactory
	{
		/// <summary>
		/// Constructs a new test suite from an assembly.
		/// </summary>
		/// <param name="assembly">The assembly which defines the tests.</param>
		/// <param name="nspace">The root namespace to search from.</param>
		/// <returns>A new test suite.</returns>
		ITestSuite GetTestSuite( Assembly assembly, string nspace );
		/// <summary>
		/// Gets a list of all the test suites within a test suite.
		/// </summary>
		/// <param name="testSuite">The test suite to examine.</param>
		/// <returns>A list of new test suites in the test suite.</returns>
		ITestSuite[] GetTestSuites( ITestSuite testSuite );
		/// <summary>
		/// Gets a list of all the test fixtures within a test suite.
		/// </summary>
		/// <param name="testSuite">The test suite to examine.</param>
		/// <returns>A list of new test fixtures in the test suite.</returns>
		IFixture[] GetFixtures( ITestSuite testSuite );
		/// <summary>
		/// Gets a list of all the tests within a test fixture.
		/// </summary>
		/// <param name="fixture">The test fixture to examine.</param>
		/// <returns>A list of new tests in the test fixture.</returns>
		ITest[] GetTests( IFixture fixture );
	}
}
