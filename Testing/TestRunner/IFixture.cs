using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Testing.TestRunner
{
	/// <summary>
	/// Classes implementing ITest keep track of all the information about a test fixture.
	/// </summary>
	/// <remarks>
	/// It is the responsibility of the IFixture.Run method to correctly ignore  
	/// tests and to notify any test listeners.
	/// </remarks>
	public interface IFixture
	{
		/// <summary>
		/// Gets or sets the name of the test fixture.
		/// </summary>
		string Name { get; set; }
		/// <summary>
		/// Gets or sets a description of the test fixture.
		/// </summary>
		string Description { get; set; }
		/// <summary>
		/// Indicates whether the fixture is to be ignored. Ignored tests will not be run.
		/// </summary>
		bool Ignore { get; set; }
		/// <summary>
		/// Gets or sets the reason a fixture is to be ignored.
		/// </summary>
		string IgnoreReason { get; set; }
		/// <summary>
		/// Gets or sets the object type which defines the test fixture.
		/// </summary>
		Type FixtureType { get; set; }
		/// <summary>
		/// Gets the instance on which to invoke the tests.
		/// </summary>
		object Instance { get; }
		/// <summary>
		/// Gets a list of the tests in this fixture.
		/// </summary>
		ITest[] Tests { get; }
		/// <summary>
		/// Gets or sets the runner that will be used to run the tests.
		/// </summary>
		IFixtureRunner FixtureRunner { get; set; }
		/// <summary>
		/// Gets or sets a method which will be invoked before any tests are run.
		/// </summary>
		MethodInfo StartUpMethod { get; set; }
		/// <summary>
		/// Gets or sets a method which will be invoked after all tests have been run.
		/// </summary>
		MethodInfo TearDownMethod { get; set; }

		/// <summary>
		/// Adds the test to the test fixture.
		/// </summary>
		/// <param name="test">The test to added.</param>
		void AddTest( ITest test );
		/// <summary>
		/// Removes a test from the test fixture.
		/// </summary>
		/// <param name="test">The test to removed.</param>
		void RemoveTest( ITest test );

		/// <summary>
		/// Creates a new test instance.
		/// </summary>
		ITest CreateTest();

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
