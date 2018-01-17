using System;
using System.Collections.Generic;
using System.Text;
using Testing;
using Testing.TestRunner;

namespace TestingTestSuite.TestRunner.DefaultRunner
{
	[TestFixture]
	public class TearDownFailingTests
	{
		[TearDown]
		public void TearDown()
		{
			throw new Exception();
		}

		[Test( "This test should succeed but the tear down should fail" )]
		public void Test()
		{
		}
	}
}
