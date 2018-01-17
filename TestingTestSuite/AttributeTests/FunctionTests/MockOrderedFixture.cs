using System;
using System.Collections.Generic;
using System.Text;
using Testing.FunctionTest;
using Testing;

namespace TestingTestSuite.AttributeTests.FunctionTests
{
	public class MockOrderedFixture
	{
		[TestOrder( 0 )]
		public void Test()
		{
		}
	}
}
