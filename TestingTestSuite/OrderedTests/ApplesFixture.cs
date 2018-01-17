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
	/// Description of the unit tests covered in ApplesFixture
	/// </summary>
	[TestFixture( "Fixture 4" )]
	[OrderedFixture( Index = 4 )]
	[Ignore( "Previous fixture should have failed" )]
	public class ApplesFixture
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
			Assert.AreEqual( 3, Globals.FixtureNumber );
		}

		/// <summary>
		/// Test fixture tear down
		/// </summary>
		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			Globals.FixtureNumber = 4;
		}

		#endregion

		#region Tests

		/// <summary>
		/// Apples
		/// </summary>
		[Test( "Test 2" )]
		[TestOrder( 2 )]
		[Ignore]
		public void Apples()
		{
			Assert.AreEqual( 1, _testNumber );
			_testNumber = 2;
		}

		/// <summary>
		/// Pears
		/// </summary>
		[Test( "Test 1" )]
		[TestOrder( 1 )]
		[Ignore( Reason = "Test 1 has been ignored" )]
		public void Pears()
		{
			Assert.AreEqual( 0, _testNumber );
			_testNumber = 1;
		}

		#endregion

	}
}
