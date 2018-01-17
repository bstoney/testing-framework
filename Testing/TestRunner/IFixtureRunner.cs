using System;
using System.Collections.Generic;
using System.Text;

namespace Testing.TestRunner
{
	/// <summary>
	/// The IFixtureRunner interface can be implemented by an attribute class, which when associated
	/// with a test fixture will override the default fixture runner. This enables the definition 
	/// of customised fixtures.
	/// </summary>
	public interface IFixtureRunner
	{
		/// <summary>
		/// This method is called by the fixture object to invoke the tests in the fixture.
		/// </summary>
		/// <param name="fixture">The calling fixture object.</param>
		void RunTests( IFixture fixture );
	}
}
