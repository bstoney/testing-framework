using System;
using System.Collections.Generic;
using System.Text;
using Testing;
using Testing.TestRunner;

namespace TestingTestSuite.TestRunner.DefaultRunner
{
	[TestFixture]
	public class TestFixtureTearDownFailingTests
	{
		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			throw new Exception();
		}

		[Test( "Thie test should succeed but the fixture tear down will fail" )]
		public void Test()
		{
		}
	}
}
