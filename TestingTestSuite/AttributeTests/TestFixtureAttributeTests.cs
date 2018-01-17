using System;
using System.Collections.Generic;
using System.Text;

using Testing;
using Testing.UnitTest;

namespace TestingTestSuite.AttributeTests
{
	/// <summary>
	/// Description of the unit tests covered in TestFixtureAttribute
	/// </summary>
	[TestFixture( "Test the function of TestFixtureAttribute" )]
	public class TestFixtureAttributeTests
	{

		[Test( "Test a TestFixtureAttribute" )]
		public void TestTestFixtureAttribute()
		{
			TestFixtureAttribute testFixture = new TestFixtureAttribute();
			Assert.IsNull( testFixture.Description );
			testFixture = new TestFixtureAttribute( "Message" );
			Assert.AreEqual( "Message", testFixture.Description );
			testFixture.Description = "Message 2";
			Assert.AreEqual( "Message 2", testFixture.Description );
		}

	}
}
