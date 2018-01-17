using System;
using System.Collections.Generic;
using System.Text;

using Testing;
using Testing.UnitTest;
using Testing.FunctionTest;

/// <summary>
/// Testing test suite
/// </summary>
namespace TestingTestSuite.OrderedTests
{
	/// <summary>
	/// Description of the unit tests covered in OrangesFixture
	/// </summary>
	[TestFixture( "Fixture 2 - unordered test" )]
	public class OrangesFixture
	{
		private int _testNumber;

		#region Test Fixture Setup & Tear Down

		/// <summary>
		/// Test fixture set up
		/// </summary>
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			_testNumber = 0;
		}

		/// <summary>
		/// Test fixture tear down
		/// </summary>
		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			Assert.AreEqual( 1, _testNumber );
		}

		#endregion

		#region Tests

		/// <summary>
		/// Apples
		/// </summary>
		[Test( "Test 2" )]
		public void Apples()
		{
			Assert.AreEqual( 0, _testNumber );
			_testNumber = 2;
		}

		/// <summary>
		/// Pears
		/// </summary>
		[Test( "Test 1" )]
		public void Pears()
		{
			Assert.AreEqual( 2, _testNumber );
			_testNumber = 1;
		}

		#endregion

	}
}
