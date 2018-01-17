using System;
using System.Collections.Generic;
using System.Text;

using Testing;
using Testing.UnitTest;

namespace TestingTestSuite.AttributeTests
{
	/// <summary>
	/// Description of the unit tests covered in TestFixtureSetUpAttribute
	/// </summary>
	[TestFixture( "Test the function of TestFixtureSetUpAttribute" )]
	public class TestFixtureSetUpAttributeTests
	{

		[Test( "Test a TestFixtureSetUpAttribute" )]
		public void TestFixtureSetUpAttribute()
		{
			TestFixtureSetUpAttribute setup = new TestFixtureSetUpAttribute();
		}

	}
}
