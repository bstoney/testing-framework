using System;
using System.Collections.Generic;
using System.Text;

#if Testing
using Testing;
using Testing.UnitTest;
#elif Zanebug
using Adapdev.UnitTest;
#endif

namespace TestingTestSuite.AttributeTests.UnitTests
{
	/// <summary>
	/// Description of the unit tests covered in ParameterisedTestAttributeTests
	/// </summary>
	[TestFixture( "A test of the ParameterisedTestAttributeTests test runner" )]
	public class ParameterisedTestAttributeTests
	{
		[Test( "Parameterised test with no parameters" )]
		[ParameterisedTest]
		public void NoPatametersTest()
		{
			Assert.Fail( "This test should not fail." );
		}

		[Test( "Parameterised test with one parameters" )]
		[ParameterisedTest]
		[TestArgument( 1 )]
		public void OnePatametersTest( int parameter )
		{
			Assert.AreEqual( 1, parameter );
		}

		[Test( "Failing parameterised test with two parameters" )]
		[ParameterisedTest]
		[TestArgument( 1, 0 )]
		[TestArgument( 2, 0 )]
		public void TwoPatametersTest( int parameter1, int parameter2 )
		{
			Assert.Fail( "This test should fail twice, failure #{0}", parameter1 );
		}

		[Test( "Parameterised test with parameters loaded from an instance method" )]
		[ParameterisedTest]
		[TestArgumentList( "InstanceCallbackMethod" )]
		public void InstancePatametersTest( int parameter )
		{
			Assert.AreEqual( 1, parameter );
		}

		private List<object[]> InstanceCallbackMethod()
		{
			return new List<object[]>( new object[][] { new object[] { 1 } } );
		}

		[Test( "Parameterised test with parameters loaded from a static method" )]
		[ParameterisedTest]
		[TestArgumentList( "StaticCallbackMethod" )]
		public void StaticPatametersTest( int parameter )
		{
			Assert.AreEqual( 1, parameter );
		}

		private static List<object[]> StaticCallbackMethod()
		{
			return new List<object[]>( new object[][] { new object[] { 1 } } );
		}
	}
}
