using System;
using System.Collections.Generic;
using System.Text;

using Testing;
using Testing.UnitTest;

namespace TestingTestSuite.AttributeTests
{
	/// <summary>
	/// Description of the unit tests covered in TearDownAttribute
	/// </summary>
	[TestFixture( "Test the function of TearDownAttribute" )]
	public class TearDownAttributeTests
	{

		[Test( "Test a TearDownAttribute" )]
		public void TestTearDownAttribute()
		{
			TearDownAttribute tearDown = new TearDownAttribute();
		}

	}
}
