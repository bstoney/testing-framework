using System;
using System.Collections.Generic;
using System.Text;

using Testing;
using Testing.UnitTest;

namespace TestingTestSuite.AttributeTests
{
	/// <summary>
	/// Description of the unit tests covered in TestAttribute
	/// </summary>
	[TestFixture( "Test the function of TestAttribute" )]
	public class TestAttributeTests
	{

		[Test( "Test a TestAttribute" )]
		public void TestTestAttribute()
		{
			TestAttribute test = new TestAttribute();
			Assert.IsNull( test.Description );
			test = new TestAttribute( "Message" );
			Assert.AreEqual( "Message", test.Description );
			test.Description = "Message 2";
			Assert.AreEqual( "Message 2", test.Description );
		}

	}
}
