using System;
using System.Collections.Generic;
using System.Text;
using Testing;
using Testing.TestRunner;

namespace TestingTestSuite.TestRunner.DefaultRunner
{
	[TestFixture]
	[InvalidFixtureRunner]
	public class FixtureRunnerFailingTests
	{
		[Test( "This test should be skiped as the fixture runner will fail" )]
		public void Test()
		{
		}

		[Test("This test will be ignored regardless of the fixture runner")]
		[Ignore]
		public void IgnoredTest()
		{
		}
	}
}
