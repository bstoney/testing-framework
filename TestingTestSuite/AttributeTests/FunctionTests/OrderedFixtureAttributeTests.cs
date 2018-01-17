using System;
using System.Collections.Generic;
using System.Text;

using Testing;
using Testing.UnitTest;
using Testing.FunctionTest;
using Testing.TestRunner;
using TestingTestSuite.MockTestFramework;

namespace TestingTestSuite.AttributeTests.FunctionTests
{
	/// <summary>
	/// Description of the unit tests covered in OrderedFixtureAttribute
	/// </summary>
	[TestFixture( "Test the function of OrderedFixtureAttribute" )]
	public class OrderedFixtureAttributeTests
	{

		[Test( "Test an OrderedFixtureAttribute" )]
		public void TestOrderedFixtureAttribute()
		{
			OrderedFixtureAttribute orderedFixture = new OrderedFixtureAttribute();
			Assert.AreEqual( 0, orderedFixture.Index );
			orderedFixture = new OrderedFixtureAttribute( 1 );
			Assert.AreEqual( 1, orderedFixture.Index );
			orderedFixture.Index = 2;
			Assert.AreEqual( 2, orderedFixture.Index );
		}

		[Test( "Test the run RunTest method" )]
		public void TestRunTests()
		{
			MockIFixture fixture = new MockIFixture();
			fixture.FixtureType = typeof( MockOrderedFixture );
			ITest test = fixture.CreateTest();
			test.TestRunner = new MockTestRunner();
			test.TestMethod = fixture.FixtureType.GetMethod( "Test" );
			fixture.AddTest( test );
			fixture.AddTest( test );
			fixture.FixtureRunner = new OrderedFixtureAttribute();
			fixture.Run();
			Assert.IsTrue( fixture.TestsHaveRun );
		}
	}
}
