using System;
using System.Collections.Generic;
using System.Text;

#if Testing
using Testing;
using Testing.UnitTest;
#elif Zanebug
using Adapdev.UnitTest;
#endif

namespace TestingTestSuite.AttributeTests
{
	/// <summary>
	/// Description of the unit tests covered in CoverageExcludeAttributeTests
	/// </summary>
	[TestFixture( "Test the operation of CoverageExcludeAttributeTests" )]
	public class CoverageExcludeAttributeTests
	{

		[Test( "Test CoverageExcludeAttributeTests constructor" )]
		public void ExampleTest()
		{
			string reason = "Test";
			CoverageExcludeAttribute cea = new CoverageExcludeAttribute( reason );
			Assert.IsNotNull(cea);
			Assert.AreEqual( reason, cea.Reason );
		}

		[Test( "Test CoverageExcludeAttributeTests constructor ArgumentNullException" )]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TestMethod()
		{
			new CoverageExcludeAttribute( null );
		}

	}
}
