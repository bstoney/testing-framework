using System;
using System.Collections.Generic;
using System.Text;
using Testing.TestRunner;

namespace TestingTestSuite.TestRunner.DefaultRunner
{
	public class InvalidFixtureRunnerAttribute : Attribute, IFixtureRunner
	{
		#region IFixtureRunner Members

		public void RunTests( IFixture fixture )
		{
			throw new Exception();
		}

		#endregion
	}

}
