using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Testing.TestRunner.DefaultRunner
{
	/// <summary>
	/// The default fixture runner. Runs each test in turn.
	/// </summary>
	internal sealed class DefaultFixtureRunner : IFixtureRunner
	{

		#region IFixtureRunner Members

		/// <summary>
		/// Run the tests.
		/// </summary>
		public void RunTests( IFixture fixture )
		{
			foreach( ITest test in fixture.Tests )
			{
				test.Run();
			}
		}

		#endregion

	}
}
