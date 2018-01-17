using System;
using System.Collections.Generic;
using System.Text;
using Testing;
using System.Threading;
using Testing.FunctionTest;
using Testing.UnitTest;
using Testing.TestRunner;
using TestingTestSuite.MockTestFramework;

namespace TestingTestSuite.AttributeTests.FunctionTests
{
	[TestFixture]
	public class MaximumExecutionTimeAttributeTests
	{
		[Test]
		public void TestExecutionTimePass()
		{
			MaximumExecutionTimeAttribute meta = new MaximumExecutionTimeAttribute( 1000 );
			ITest t = MockTestingHelper.CreateTest( this.GetType().GetMethod( "Test" ) );
			meta.RunTest( t );

			Assert.AreEqual( TestStatus.Pass, t.Result.Status );
		}

		[Test]
		public void TestExecutionTimeFail()
		{
			MaximumExecutionTimeAttribute meta = new MaximumExecutionTimeAttribute( 1 );
			ITest t = MockTestingHelper.CreateTest( this.GetType().GetMethod( "Test" ) );
			meta.RunTest( t );

			Assert.AreEqual( TestStatus.Fail, t.Result.Status );
		}

		[Test]
		[MaximumExecutionTime( 100 )]
		public void Test()
		{
			Thread.Sleep( 10 );
		}
	}
}
