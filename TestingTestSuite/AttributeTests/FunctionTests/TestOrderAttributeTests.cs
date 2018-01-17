using System;
using System.Collections.Generic;
using System.Text;

using Testing;
using Testing.UnitTest;
using Testing.FunctionTest;

namespace TestingTestSuite.AttributeTests.FunctionTests
{
	/// <summary>
	/// Description of the unit tests covered in TestOrderAttribute
	/// </summary>
	[TestFixture( "Test the function of TestOrderAttribute" )]
	public class TestOrderAttributeTests
	{

		[Test( "Test an TestOrderAttribute" )]
		public void TestTestOrderAttribute()
		{
			TestOrderAttribute testOrder = new TestOrderAttribute( 0 );
			Assert.AreEqual( 0, testOrder.Index );
			testOrder.Index = 2;
			Assert.AreEqual( 2, testOrder.Index );
		}

	}
}
