using System;
using System.Collections.Generic;
using System.Text;

using Testing;
using Testing.UnitTest;

namespace TestingTestSuite.AttributeTests
{
	/// <summary>
	/// Description of the unit tests covered in TestFixtureTearDownAttribute
	/// </summary>
	[TestFixture( "Test the function of TestFixtureTearDownAttribute" )]
	public class TestFixtureTearDownAttributeTests
	{

		[Test( "Test a TestFixtureTearDownAttribute" )]
		public void TestFixtureTearDownAttribute()
		{
			TestFixtureTearDownAttribute tearDown = new TestFixtureTearDownAttribute();
		}

	}
}
