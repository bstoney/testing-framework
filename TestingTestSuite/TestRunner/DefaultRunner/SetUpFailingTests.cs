using System;
using System.Collections.Generic;
using System.Text;
using Testing;
using Testing.TestRunner;

namespace TestingTestSuite.TestRunner.DefaultRunner
{
	[TestFixture]
	public class SetUpFailingTests
	{
		[SetUp]
		public void SetUp()
		{
			throw new Exception();
		}

		[Test( "This test should be skipped as the set up should fail" )]
		public void Test()
		{
		}
	}
}
