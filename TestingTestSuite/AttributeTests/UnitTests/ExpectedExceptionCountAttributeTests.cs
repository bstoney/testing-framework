using System;
using System.Collections.Generic;
using System.Text;

using Testing;
using Testing.UnitTest;
using Testing.TestRunner;
using TestingTestSuite.MockTestFramework;

namespace TestingTestSuite.AttributeTests.UnitTests
{
	/// <summary>
	/// Description of the unit tests covered in ExpectedExceptionCountAttributeTests
	/// </summary>
	[TestFixture( "Test the function of ExpectedExceptionCountAttribute" )]
	public class ExpectedExceptionCountAttributeTests
	{
		private Type _assertionException = typeof( AssertionException );

		[Test( "Test an ExpectedExceptionCountAttribute" )]
		public void TestExpectedExceptionCountAttribute()
		{
			ExpectedExceptionCountAttribute expectedException;
			expectedException = new ExpectedExceptionCountAttribute( _assertionException, 1 );
			Assert.AreEqual( _assertionException, expectedException.ExceptionType );
			Assert.AreEqual( 1, expectedException.ExceptionCount );
			Assert.IsTrue( expectedException.FailOnOtherExceptions );
			expectedException = new ExpectedExceptionCountAttribute( _assertionException.FullName, 1 );
			Assert.AreEqual( _assertionException, expectedException.ExceptionType );
			Assert.AreEqual( 1, expectedException.ExceptionCount );
			Assert.IsTrue( expectedException.FailOnOtherExceptions );
			expectedException = new ExpectedExceptionCountAttribute( _assertionException.FullName, 1, true );
			Assert.AreEqual( _assertionException, expectedException.ExceptionType );
			Assert.AreEqual( 1, expectedException.ExceptionCount );
			Assert.IsTrue( expectedException.FailOnOtherExceptions );
			expectedException = new ExpectedExceptionCountAttribute( _assertionException.FullName, 1, false );
			Assert.AreEqual( _assertionException, expectedException.ExceptionType );
			Assert.AreEqual( 1, expectedException.ExceptionCount );
			Assert.IsFalse( expectedException.FailOnOtherExceptions );

			Type exceptionType = typeof( Exception );
			expectedException.ExceptionType = exceptionType;
			Assert.AreEqual( typeof( Exception ), expectedException.ExceptionType );
			expectedException.ExceptionCount = 2;
			Assert.AreEqual( 2, expectedException.ExceptionCount );
			expectedException.FailOnOtherExceptions = false;
			Assert.IsFalse( expectedException.FailOnOtherExceptions );
		}

		[Test]
		[ExpectedExceptionCount( typeof( ArgumentNullException ), 4 )]
		public void ArgumentNullExceptionTest()
		{
			new ExpectedExceptionCountAttribute( (Type)null, 0 );			// Should fail
			new ExpectedExceptionCountAttribute( (String)null, 0 );			// Should fail
			new ExpectedExceptionCountAttribute( "BogusTypeName", 0 );		// Should fail

			ExpectedExceptionCountAttribute expectedException = new ExpectedExceptionCountAttribute( _assertionException, 0 );
			expectedException.ExceptionType = null;							// Should fail
		}

		[Test( "Test the run RunTest method" )]
		public void TestRunTestMethod()
		{
			ExpectedExceptionCountAttribute expectedException = new ExpectedExceptionCountAttribute( typeof( Exception ), 0 );
			ITest test;
			test = MockTestingHelper.CreatePassTest();
			expectedException.RunTest( test );
			Assert.AreEqual( TestStatus.Pass, test.Result.Status );

			test = MockTestingHelper.CreateFailTest();
			expectedException.RunTest( test );
			Assert.AreEqual( TestStatus.Fail, test.Result.Status );

			expectedException.FailOnOtherExceptions = false;
			expectedException.RunTest( test );
			Assert.AreEqual( TestStatus.Pass, test.Result.Status );

			expectedException.ExceptionCount = 1;
			expectedException.RunTest( test );
			Assert.AreEqual( TestStatus.Fail, test.Result.Status );

			expectedException.ExceptionType = _assertionException;
			expectedException.RunTest( test );
			Assert.AreEqual( TestStatus.Pass, test.Result.Status );

			expectedException.ExceptionCount = 0;
			expectedException.RunTest( test );
			Assert.AreEqual( TestStatus.Fail, test.Result.Status );
		}

		[Test( "Test the run RunTest method with an invalid method" )]
		[ExpectedException( typeof( InvalidOperationException ) )]
		public void TestRunInvalidTestMethod()
		{
			ExpectedExceptionCountAttribute expectedException = new ExpectedExceptionCountAttribute( typeof( Exception ), 1 );
			ITest test;
			test = MockTestingHelper.CreatePassTest();
			test.Fixture.FixtureType = typeof( ExpectedExceptionCountAttributeTests );
			test.TestMethod = test.Fixture.FixtureType.GetMethod( "InvalidTest" );
			expectedException.RunTest( test );
		}

		public void InvalidTest()
		{
			int j = 0;
			for( int i = 0; i < 10; i++ )
			{
				j++;
			}
		}
	}
}
