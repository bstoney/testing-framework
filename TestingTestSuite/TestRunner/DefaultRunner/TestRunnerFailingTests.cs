using System;
using System.Collections.Generic;
using System.Text;
using Testing;
using Testing.TestRunner;

namespace TestingTestSuite.TestRunner.DefaultRunner
{
	[TestFixture]
	public class TestRunnerFailingTests
	{
		[Test( "This test should be skipped as the test runner should fail" )]
		[InvalidTestRunner]
		public void Test()
		{
		}
	}
}
