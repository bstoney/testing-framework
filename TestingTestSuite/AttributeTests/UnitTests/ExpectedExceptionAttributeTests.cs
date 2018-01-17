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
	/// Description of the unit tests covered in ExpectedExceptionAttributeTests
	/// </summary>
	[TestFixture( "Test the function of ExpectedExceptionAttribute" )]
	public class ExpectedExceptionAttributeTests
	{
		private Type _assertionException = typeof( AssertionException );

		[Test( "Test an ExpectedExceptionAttribute" )]
		public void TestExpectedExceptionAttribute()
		{
			ExpectedExceptionAttribute expecterException;
			expecterException = new ExpectedExceptionAttribute( _assertionException );
			Assert.AreEqual( _assertionException, expecterException.ExceptionType );
			expecterException = new ExpectedExceptionAttribute( _assertionException.FullName );
			Assert.AreEqual( _assertionException, expecterException.ExceptionType );

			expecterException = new ExpectedExceptionAttribute( _assertionException, null );
			Assert.AreEqual( _assertionException, expecterException.ExceptionType );
			Assert.IsNull( expecterException.Message );
			expecterException = new ExpectedExceptionAttribute( _assertionException.FullName, null );
			Assert.AreEqual( _assertionException, expecterException.ExceptionType );
			Assert.IsNull( expecterException.Message );

			expecterException.ExceptionType = typeof( Exception );
			Assert.AreEqual( typeof( Exception ), expecterException.ExceptionType );
			expecterException.Message = "message";
			Assert.AreEqual( "message", expecterException.Message );
		}

		[Test]
		[ExpectedExceptionCount( typeof( ArgumentNullException ), 7 )]
		public void ArgumentNullExceltions()
		{
			new ExpectedExceptionAttribute( (Type)null );							// Should fail
			new ExpectedExceptionAttribute( (String)null );							// Should fail
			new ExpectedExceptionAttribute( (Type)null, null );						// Should fail
			new ExpectedExceptionAttribute( (String)null, null );					// Should fail
			new ExpectedExceptionAttribute( "BogusTypeName" );						// Should fail
			new ExpectedExceptionAttribute( "BogusTypeName", null );				// Should fail
			new ExpectedExceptionAttribute( _assertionException, null );			// Should work
			new ExpectedExceptionAttribute( _assertionException.FullName, null );	// Should work

			ExpectedExceptionAttribute expectedException = new ExpectedExceptionAttribute( _assertionException );
			expectedException.Message = null;										// Should work
			expectedException.ExceptionType = null;									// Should fail
		}

		[Test( "Test the run RunTest method" )]
		public void TestRunTestMethod()
		{
			ExpectedExceptionAttribute expectedException = new ExpectedExceptionAttribute( _assertionException );
			ITest test;
			test = MockTestingHelper.CreatePassTest();
			expectedException.RunTest( test );
			Assert.AreEqual( TestStatus.Fail, test.Result.Status );

			test = MockTestingHelper.CreateFailTest();
			expectedException.RunTest( test );
			Assert.AreEqual( TestStatus.Pass, test.Result.Status );

			expectedException.Message = "Invalid message";
			expectedException.RunTest( test );
			Assert.AreEqual( TestStatus.Fail, test.Result.Status );

			expectedException.Message = MockFixture.FailMessage;
			expectedException.RunTest( test );
			Assert.AreEqual( TestStatus.Pass, test.Result.Status );
		}
	}
}
