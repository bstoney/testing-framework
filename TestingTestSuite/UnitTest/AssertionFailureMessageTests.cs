using System;
using System.Collections.Generic;
using System.Text;

using Testing;
using Testing.UnitTest;
using System.Collections;

namespace TestingTestSuite.UnitTest
{
	/// <summary>
	/// Description of the unit tests covered in AssertionFailureMessageTests
	/// </summary>
	[TestFixture( "A brief description of the purpose of this fixture" )]
	public class AssertionFailureMessageTests
	{
		const string TestLine = "Line 1";
		const string SpecialCharaterTestLine = "\r\n\t\"\\";
		const string LongTestLine = "This is a very long test line, much longer than 'TestLine', has 110 characters, and contains some punctuation!";

		[Test( "Test AddLine methods" )]
		public void AddLineTest()
		{
			AssertionFailureMessage afm;
			afm = new AssertionFailureMessage();
			Assert.AreEqual( String.Empty, afm.ToString() );
			afm = new AssertionFailureMessage( TestLine );
			Assert.AreEqual( TestLine, afm.ToString() );
			afm = new AssertionFailureMessage( "Line {0}", 1 );
			Assert.AreEqual( TestLine, afm.ToString() );
			afm = new AssertionFailureMessage();
			afm.AddLine( TestLine );
			Assert.AreEqual( String.Concat( afm.NewLine, TestLine ), afm.ToString() );
			afm = new AssertionFailureMessage();
			afm.AddLine( "Line {0}", 1 );
			Assert.AreEqual( String.Concat( afm.NewLine, TestLine ), afm.ToString() );
			afm = new AssertionFailureMessage();
			afm.AddExpectedLine( TestLine );
			Assert.AreEqual( String.Concat( afm.NewLine, "\texpected: ", TestLine ), afm.ToString() );
			afm = new AssertionFailureMessage();
			afm.AddActualLine( TestLine );
			Assert.AreEqual( String.Concat( afm.NewLine, "\t but was: ", TestLine ), afm.ToString() );
		}

		[Test( "Test display functions" )]
		public void DisplayMethodTest()
		{
			AssertionFailureMessage afm;
			AssertionFailureMessage afm2;
			afm = new AssertionFailureMessage();
			afm.DisplayExpectedValue( TestLine );
			afm2 = new AssertionFailureMessage();
			afm2.AddExpectedLine( String.Concat( "<\"", TestLine, "\">" ) );
			Assert.AreEqual( afm2.ToString(), afm.ToString() );

			afm = new AssertionFailureMessage();
			afm.DisplayActualValue( TestLine );
			afm2 = new AssertionFailureMessage();
			afm2.AddActualLine( String.Concat( "<\"", TestLine, "\">" ) );
			Assert.AreEqual( afm2.ToString(), afm.ToString() );

			afm = new AssertionFailureMessage();
			afm.DisplayExpectedAndActual( TestLine, TestLine );
			afm2 = new AssertionFailureMessage();
			afm2.DisplayExpectedValue( TestLine );
			afm2.DisplayActualValue( TestLine );
			Assert.AreEqual( afm2.ToString(), afm.ToString() );
		}

		[Test( "Test DisplayPositionMarker method" )]
		public void DisplayPositionMarker()
		{
			AssertionFailureMessage afm;
			afm = new AssertionFailureMessage();
			afm.DisplayPositionMarker( -1 );
			Assert.AreEqual( String.Concat( afm.NewLine, "\t------------^" ), afm.ToString() );
			afm = new AssertionFailureMessage();
			afm.DisplayPositionMarker( 0 );
			Assert.AreEqual( String.Concat( afm.NewLine, "\t------------^" ), afm.ToString() );
			afm = new AssertionFailureMessage();
			afm.DisplayPositionMarker( 3 );
			Assert.AreEqual( String.Concat( afm.NewLine, "\t---------------^" ), afm.ToString() );
		}

		[Test( "Test FormatObjectForDisplay function" )]
		[ParameterisedTest]
		[TestArgument( null, "<(null)>" )]
		[TestArgument( new string[] { }, "<(empty)>" )]
		[TestArgument( TestLine, "<\"Line 1\">" )]
		[TestArgument( 'M', "<'M'>" )]
		[TestArgument( '\n', "<'\\n'>" )]
		[TestArgument( SpecialCharaterTestLine, "<\"\\r\\n\\t\\\"\\\\\">" )]
		[TestArgument( 3.33333333333333333333d, "<3.3333333333333335>" )]
		[TestArgument( 3.33333333333333333333f, "<3.33333325>" )]
		[TestArgument( 3, "<3>" )]
		public void FormatObjectForDisplay( object value, string result )
		{
			Assert.AreEqual( result, AssertionFailureMessage.FormatObjectForDisplay( value ) );
		}

		[Test( "Test private static helper functions" )]
		public void PrivateHelperFunctionTest()
		{
			PrivateObject po = new PrivateObject( typeof( AssertionFailureMessage ) );
			// InputsAreStrings
			Assert.IsTrue( (bool)po.Invoke( "InputsAreStrings", TestLine, TestLine ) );
			Assert.IsFalse( (bool)po.Invoke( "InputsAreStrings", null, TestLine ) );
			Assert.IsFalse( (bool)po.Invoke( "InputsAreStrings", TestLine, null ) );
			Assert.IsFalse( (bool)po.Invoke( "InputsAreStrings", null, null ) );
			Assert.IsFalse( (bool)po.Invoke( "InputsAreStrings", 3, TestLine ) );
			Assert.IsFalse( (bool)po.Invoke( "InputsAreStrings", TestLine, 3 ) );
			// CreateStringBuilder
			StringBuilder sb;
			sb = po.Invoke( "CreateStringBuilder", null, new object[] { null } ) as StringBuilder;
			Assert.AreEqual( "", sb.ToString() );
			sb = po.Invoke( "CreateStringBuilder", null, new object[] { TestLine } ) as StringBuilder;
			Assert.AreEqual( "", sb.ToString() );
			sb = po.Invoke( "CreateStringBuilder", TestLine, new object[] { null } ) as StringBuilder;
			Assert.AreEqual( TestLine, sb.ToString() );
			sb = po.Invoke( "CreateStringBuilder", "{0}{1}", new object[] { TestLine, TestLine } ) as StringBuilder;
			Assert.AreEqual( TestLine + TestLine, sb.ToString() );
			// ClipAroundPosition
			Assert.AreEqual( String.Empty, po.Invoke( "ClipAroundPosition", null, 0 ) );
			Assert.AreEqual( String.Empty, po.Invoke( "ClipAroundPosition", String.Empty, 0 ) );
			Assert.AreEqual( TestLine, po.Invoke( "ClipAroundPosition", TestLine, 0 ) );
			Assert.AreEqual( String.Concat( LongTestLine.Substring( 0, 35 ), "..." ),
				po.Invoke( "ClipAroundPosition", LongTestLine, 0 ) );
			Assert.AreEqual( String.Concat( LongTestLine.Substring( 0, 35 ), "..." ),
				po.Invoke( "ClipAroundPosition", LongTestLine, -40 ) );
			Assert.AreEqual( String.Concat( "...", LongTestLine.Substring( LongTestLine.Length - 35 ) ),
				po.Invoke( "ClipAroundPosition", LongTestLine, LongTestLine.Length ) );
			Assert.AreEqual( String.Concat( "...", LongTestLine.Substring( LongTestLine.Length - 35 ) ),
				po.Invoke( "ClipAroundPosition", LongTestLine, LongTestLine.Length + 40 ) );
			Assert.AreEqual( String.Concat( "...", LongTestLine.Substring( 15, 70 ), "..." ),
				po.Invoke( "ClipAroundPosition", LongTestLine, 50 ) );
			// FindMismatchPosition
			Assert.AreEqual( -1, po.Invoke( "FindMismatchPosition", TestLine, TestLine, 0 ) );
			Assert.AreEqual( TestLine.Length, po.Invoke( "FindMismatchPosition", TestLine, String.Concat( TestLine, "_" ), 0 ) );
			Assert.AreEqual( 0, po.Invoke( "FindMismatchPosition", TestLine, String.Concat( "_", TestLine ), 0 ) );
			Assert.AreEqual( TestLine.Length, po.Invoke( "FindMismatchPosition", String.Concat( TestLine, "_" ), TestLine, 0 ) );
			Assert.AreEqual( 0, po.Invoke( "FindMismatchPosition", String.Concat( "_", TestLine ), TestLine, 0 ) );
			// ConvertWhitespace
			Assert.IsNull( po.Invoke( "ConvertEscapedCharacters", null ) );
			Assert.AreEqual( "\\r", po.Invoke( "ConvertEscapedCharacters", "\r" ) );
			Assert.AreEqual( "\\n", po.Invoke( "ConvertEscapedCharacters", "\n" ) );
			Assert.AreEqual( "\\t", po.Invoke( "ConvertEscapedCharacters", "\t" ) );
			Assert.AreEqual( "\\\"", po.Invoke( "ConvertEscapedCharacters", "\"" ) );
			Assert.AreEqual( "\\\\", po.Invoke( "ConvertEscapedCharacters", "\\" ) );

		}

		[Test( "Test display output for lists" )]
		[ParameterisedTest]
		[TestArgument( null, null, 0, 0, "\r\n<(null)>" )]
		[TestArgument( "Label", null, 0, 0, "\r\nLabel:<(null)>" )]
		[TestArgument( null, new int[] { }, 0, 0, "\r\n<(empty)>" )]
		[TestArgument( null, new int[] { 1, 2, 3, 4 }, 0, 4, "\r\n<<1>,<2>,<3>,<4>>" )]
		[TestArgument( null, new int[] { 1, 2, 3, 4 }, 0, 2, "\r\n<<1>,<2>,...>" )]
		[TestArgument( null, new int[] { 1, 2, 3, 4 }, 2, 4, "\r\n<...,<3>,<4>>" )]
		[TestArgument( null, new int[] { 1, 2, 3, 4 }, 1, 2, "\r\n<...,<2>,<3>,...>" )]
		[TestArgument( null, new int[] { 1, 2, 3, 4 }, -1, 5, "\r\n<<1>,<2>,<3>,<4>>" )]
		[TestArgumentList( "GetAssertionFailureMessageLists" )]
		public void DisplayListElementsTest( string label, IList list, int index, int max, string result )
		{
			AssertionFailureMessage afm = new AssertionFailureMessage();
			afm.DisplayListElements( label, list, index, max );
			Assert.AreEqual( result, afm.ToString() );
		}

		/// <summary>
		/// Helper method for DisplayListElementsTest to create non static arguments.
		/// </summary>
		private List<object[]> GetAssertionFailureMessageLists()
		{
			return new List<object[]>( new object[][] {
				 new object[] { null, new int[][] { new int[] { 1, 2 }, new int[]{ 3, 4 } }, 0, 2, "\r\n<<<1>,<2>>,<<3>,<4>>>" },
				 new object[] { null, new int[,] { { 1, 2 }, { 3, 4 } }, 0, 2, "\r\n<System.Int32[,]>" }
			} );
		}
	}
}
