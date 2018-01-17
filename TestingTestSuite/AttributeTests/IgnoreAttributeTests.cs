using System;
using System.Collections.Generic;
using System.Text;

using Testing;
using Testing.UnitTest;

namespace TestingTestSuite.AttributeTests
{
	/// <summary>
	/// Description of the unit tests covered in IgnoreAttributeTests
	/// </summary>
	[TestFixture( "Test the function of IgnoreAttribute" )]
	public class IgnoreAttributeTests
	{

		[Test( "Test an IgnoreAttribute" )]
		public void TestIgnoreAttribute()
		{
			IgnoreAttribute ignore = new IgnoreAttribute();
			Assert.IsNull( ignore.Reason );
			ignore = new IgnoreAttribute( "Message" );
			Assert.AreEqual( "Message", ignore.Reason );
			ignore.Reason = "Message 2";
			Assert.AreEqual( "Message 2", ignore.Reason );
		}

	}
}
