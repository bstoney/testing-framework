// Project: TestingTestSuite, File: PearsFixture.cs
// Namespace: TestingTestSuite, Class: PearsFixture
// Path: D:\Development\Library.2005\TestingTestSuite, Author: bstoney
// Code lines: 82, Size of file: 1.52 KB
// Creation date: 29/06/2006 10:58 AM
// Last modified: 29/06/2006 1:56 PM
// Copyright Plan B Financial Services

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
	/// Description of the unit tests covered in PearsFixture
	/// </summary>
	[TestFixture( Description = "Fixture 1" )]
	[OrderedFixture( 1 )]
	public class PearsFixture
	{
		private int _testNumber;

		#region Test Fixture Setup & Tear Down

		/// <summary>
		/// Test fixture set up
		/// </summary>
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			_testNumber = -1;
			Assert.AreEqual( 0, Globals.FixtureNumber );
		}

		/// <summary>
		/// Test fixture tear down
		/// </summary>
		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			Globals.FixtureNumber = 1;
		}

		#endregion

		#region Tests

		/// <summary>
		/// Apples
		/// </summary>
		[Test( "Test 2" )]
		[TestOrder( 2 )]
		[Ignore( "Test 2 has been ignored" )]
		public void Apples()
		{
			Assert.AreEqual( 1, _testNumber );
			_testNumber = 2;
		}

		/// <summary>
		/// Pears
		/// </summary>
		[Test( "Test 1" )]
		[TestOrder( -1, Index = 1 )]
		public void Pears()
		{
			Assert.AreEqual( 0, _testNumber );
			_testNumber = 1;
		}

		/// <summary>
		/// UnorderedTest
		/// </summary>
		[Test( Description = "Test -1" )]
		public void UnorderedTest()
		{
			Assert.AreEqual( -1, _testNumber );
			_testNumber = 0;
			Assert.Fail( "This test should fail" );
		}

		#endregion

	}
}
