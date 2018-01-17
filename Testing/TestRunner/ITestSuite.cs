using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Testing.TestRunner
{
	/// <summary>
	/// Classes implementing ITestSuite keep track of all the test fixtures contained in an assembly.
	/// </summary>
	/// <remarks>
	/// It is the responsibility of the ITestSuite.Run method to correctly ignore  
	/// fixtures and to notify any test listeners.
	/// </remarks>
	public interface ITestSuite
	{
		/// <summary>
		/// Gets or sets the name of the test suite.
		/// </summary>
		string Name { get; set; }
		// TODO remove reliance on a single assembly.
		/// <summary>
		/// Gets or sets the test assembly.
		/// </summary>
		Assembly Assembly { get; set; }
		/// <summary>
		/// Gets or sets the namespace covered by this test suite.
		/// </summary>
		string Namespace { get; set; }
		/// <summary>
		/// Gets a read-only list of all the test fixtures in the test suite.
		/// </summary>
		IFixture[] Fixtures { get; }
		/// <summary>
		/// Gets a read-only list of all the test fixtures in the test suite.
		/// </summary>
		ITestSuite[] TestSuites { get; }
		/// <summary>
		/// Gets or sets the runner that will be used to run the tests.
		/// </summary>
		ITestSuiteRunner TestSuiteRunner { get; set; }

		/// <summary>
		/// Adds the test fixture to the test suite.
		/// </summary>
		/// <param name="fixture">The test fixture to add.</param>
		void AddFixture( IFixture fixture );
		/// <summary>
		/// Removes a test fixture from the test suite.
		/// </summary>
		/// <param name="fixture">The test fixture to remove.</param>
		void RemoveFixture( IFixture fixture );

		/// <summary>
		/// Creates a new fixture instance.
		/// </summary>
		IFixture CreateFixture();

		/// <summary>
		/// Adds the test suite to the test suite.
		/// </summary>
		void AddTestSuite( ITestSuite testSuite );
		/// <summary>
		/// Removes a test suite from the test suite.
		/// </summary>
		void RemoveTestSuite( ITestSuite testSuite );

		/// <summary>
		/// Gets a count of all the tests in this component.
		/// </summary>
		int TestCount { get; }
		/// <summary>
		/// Runs all the tests in the component.
		/// </summary>
		void Run();
		/// <summary>
		/// Resets all tests to untested state.
		/// </summary>
		void Reset();

		/// <summary>
		/// Register an listener to be notified.
		/// </summary>
		void RegisterListener( ITestListener observer );
		/// <summary>
		/// Remove an listener from the notification list.
		/// </summary>
		void UnregisterListener( ITestListener observer );
		/// <summary>
		/// Notify all the currently registered listeners.
		/// </summary>
		void Begin();
		/// <summary>
		/// Notify all the currently registered listeners.
		/// </summary>
		void End();
	}
}
