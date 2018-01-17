using System;
using System.Collections.Generic;
using System.Text;

using Testing;
using Testing.UnitTest;
using Testing.FunctionTest;
using Testing.TestRunner;
using TestingTestSuite.MockTestFramework;

/// <summary>
/// Testing test suite. attribute tests. function tests
/// </summary>
namespace TestingTestSuite.AttributeTests.FunctionTests
{
	/// <summary>
	/// Description of the unit tests covered in OrderedTestSuiteAttribute
	/// </summary>
	[TestFixture( "Test the function of OrderedTestSuiteAttribute" )]
	public class OrderedTestSuiteAttributeTests
	{

		[Test( "Test an OrderedTestSuiteAttribute" )]
		public void TestOrderedTestSuiteAttribute()
		{
			new OrderedTestSuiteAttribute();
		}

		[Test( "Test the run RunTest method" )]
		public void TestRunTests()
		{
			MockITestSuite testSuite = new MockITestSuite();
			IFixture fixture = testSuite.CreateFixture();
			{
				fixture.FixtureType = typeof( MockOrderedFixture );
				fixture.FixtureRunner = new OrderedFixtureAttribute();
				ITest test = fixture.CreateTest();
				test.TestRunner = new MockTestRunner();
				test.TestMethod = fixture.FixtureType.GetMethod( "Test" );
				fixture.AddTest( test );
			}
			testSuite.AddFixture( fixture );
			testSuite.AddFixture( fixture );
			testSuite.TestSuiteRunner = new OrderedTestSuiteAttribute();
			testSuite.Run();
			Assert.IsTrue( testSuite.TestsHaveRun );
		}
	}
}
