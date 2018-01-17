using System;
using System.Collections.Generic;
using System.Text;
using Testing;
using Testing.TestRunner;

namespace TestingTestSuite.TestRunner.DefaultRunner
{
	[TestFixture]
	public class TestFixtureSetUpFailingTests
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			throw new Exception();
		}

		[Test( "This test should be skipped as the fixture set up should fail" )]
		public void Test()
		{
		}

		[Test("This test will be ignored regardless of the set up")]
		[Ignore]
		public void IgnoredTest()
		{
		}
	}
}
