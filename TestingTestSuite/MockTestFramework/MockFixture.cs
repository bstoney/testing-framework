using System;
using System.Collections.Generic;
using System.Text;
using Testing.FunctionTest;
using Testing;
using Testing.UnitTest;

namespace TestingTestSuite.MockTestFramework
{
	public class MockFixture
	{
		public readonly static string FailMessage = "Test has failed";

		public void TestPass()
		{
		}

		public void TestFail()
		{
			Assert.Fail( FailMessage );
		}
	}
}
