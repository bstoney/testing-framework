using System;
using System.Collections.Generic;
using System.Text;
using Testing;
using Testing.UnitTest;

namespace TestingTestSuite.TestRunner.ConsoleRunner.TestNamespace
{
	[TestFixture]
	public class TestTest
	{
		[Test( "Test to pass" )]
		public void TestToPass()
		{
		}
		[Test( "Test to fail" )]
		public void TestToFail()
		{
			Assert.Fail( "Test to fail" );
		}
		[Test( "Test to ignore" )]
		[Ignore]
		public void TestToIgnore()
		{
			Assert.Fail( "Test to ignore" );
		}
	}
}
