using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Testing.TestRunner
{
	/// <summary>
	/// Classes implementing ITest keep track of all the information about a test.
	/// </summary>
	public interface ITest
	{
		/// <summary>
		/// Gets or sets the name of the test.
		/// </summary>
		string Name { get; set; }
		/// <summary>
		/// Gets or sets a description of the test.
		/// </summary>
		string Description { get; set; }
		/// <summary>
		/// Indicates whether the test is to be ignored. Ignored tests will not be run.
		/// </summary>
		bool Ignore { get; set; }
		/// <summary>
		/// Gets or sets the reason a test is to be ignored.
		/// </summary>
		string IgnoreReason { get; set; }
		/// <summary>
		/// Gets or sets the ITestRunner uses to execute the test.
		/// </summary>
		ITestRunner TestRunner { get; set; }
		/// <summary>
		/// Gets or sets the test fixture of this test.
		/// </summary>
		IFixture Fixture { get; set; }
		/// <summary>
		/// Gets or sets the method which defines the test.
		/// </summary>
		MethodInfo TestMethod { get; set; }
		/// <summary>
		/// Gets or sets a method which will be invoked before the test is run.
		/// </summary>
		MethodInfo StartUpMethod { get; set; }
		/// <summary>
		/// Gets or sets a method which will be invoked after the test is run.
		/// </summary>
		MethodInfo TearDownMethod { get; set; }
		/// <summary>
		/// Gets a TestResult object for storing and retrieving results.
		/// </summary>
		TestResult Result { get; }

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
