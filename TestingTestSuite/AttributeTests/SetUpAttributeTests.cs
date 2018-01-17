using System;
using System.Collections.Generic;
using System.Text;

using Testing;
using Testing.UnitTest;

namespace TestingTestSuite.AttributeTests
{
	/// <summary>
	/// Description of the unit tests covered in SetUpAttribute
	/// </summary>
	[TestFixture( "Test the function of SetUpAttribute" )]
	public class SetUpAttributeTests
	{

		[Test( "Test a SetUpAttribute" )]
		public void TestSetUpAttribute()
		{
			SetUpAttribute setup = new SetUpAttribute();
		}

	}
}
