#region Copyright (c) 2002-2003, James W. Newkirk, Michael C. Two, Alexei A. Vorontsov, Charlie Poole, Philip A. Craig, Douglas de la Torre
/************************************************************************************
'
' Copyright  2002-2003 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov, Charlie Poole
' Copyright  2000-2002 Philip A. Craig
' Copyright  2001 Douglas de la Torre
'
' This software is provided 'as-is', without any express or implied warranty. In no
' event will the authors be held liable for any damages arising from the use of this
' software.
'
' Permission is granted to anyone to use this software for any purpose, including
' commercial applications, and to alter it and redistribute it freely, subject to the
' following restrictions:
'
' 1. The origin of this software must not be misrepresented; you must not claim that
' you wrote the original software. If you use this software in a product, an
' acknowledgment (see the following) in the product documentation is required.
'
' Portions Copyright  2002 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov
' Copyright  2000-2002 Philip A. Craig, or Copyright  2001 Douglas de la Torre
'
' 2. Altered source versions must be plainly marked as such, and must not be
' misrepresented as being the original software.
'
' 3. This notice may not be removed or altered from any source distribution.
'
'***********************************************************************************/
#endregion

using System;
using System.Text;
using System.IO;
using System.Collections;

namespace Testing.UnitTest
{
	/// <summary>
	/// Summary description for AssertionFailureMessage.
	/// </summary>
	public sealed class AssertionFailureMessage : StringWriter
	{
		#region Static Constants

		/// <summary>
		/// Default format for displaying an object.
		/// </summary>
		private const string ObjectFormat = "<{0}>";
		/// <summary>
		/// Number of characters before a highlighted position before
		/// clipping will occur.  Clipped text is replaced with an
		/// elipsis "..."
		/// </summary>
		private const int PreClipLength = 35;

		/// <summary>
		/// Number of characters after a highlighted position before
		/// clipping will occur.  Clipped text is replaced with an
		/// elipsis "..."
		/// </summary>
		private const int PostClipLength = 35;

		/// <summary>
		/// Prefix used to start an expected value line.
		/// Must be same length as actualPrefix.
		/// </summary>
		private const string ExpectedPrefix = "expected:";

		/// <summary>
		/// Prefix used to start an actual value line.
		/// Must be same length as expectedPrefix.
		/// </summary>
		private const string actualPrefix = " but was:";

		private const string ExpectedAndActualFormat = "\t{0} {1}";
		private const string DifferentStringLengthsFormat
			= "\tString lengths differ. Expected length={0}, but was length={1}.";
		private const string SameStringLengthsFormat
			= "\tString lengths are both {0}.";
		private const string DifferentArrayRanksFormat
			= "Array ranks differ. Expected rank={0}, but was rank={1}.";
		private const string DifferentArrayLengthsFormat
			= "Array lengths differ. Expected length={0}, but was length={1}. (Rank {2})";
		private const string SameArrayLengthsFormat
			= "Array lengths are both {0}. (Rank {1})";
		private const string StringsDifferAtIndexFormat
			= "\tStrings differ at index {0}.";
		private const string ArraysDifferAtIndexFormat
			= "Arrays differ at index {0}.";

		#endregion

		#region Constructors

		/// <summary>
		/// Construct an AssertionFailureMessage with a message
		/// and optional arguments.
		/// </summary>
		public AssertionFailureMessage( string message, params object[] args )
			: base( CreateStringBuilder( message, args ) ) { }

		/// <summary>
		/// Construct an empty AssertionFailureMessage
		/// </summary>
		public AssertionFailureMessage() : this( null, null ) { }

		#endregion

		/// <summary>
		/// Add text to the message as a new line.
		/// </summary>
		/// <param name="text">The text to add</param>
		public void AddLine( string text )
		{
			WriteLine();
			Write( text );
		}

		/// <summary>
		/// Add formatted text and arguments to the message as a new line.
		/// </summary>
		/// <param name="fmt">Format string</param>
		/// <param name="args">Arguments to use with the format</param>
		public void AddLine( string fmt, params object[] args )
		{
			WriteLine();
			Write( fmt, args );
		}

		/// <summary>
		/// Add an expected value line to the message containing
		/// the text provided as an argument.
		/// </summary>
		/// <param name="text">Text describing what was expected.</param>
		public void AddExpectedLine( string text )
		{
			AddLine( string.Format( ExpectedAndActualFormat, ExpectedPrefix, text ) );
		}

		/// <summary>
		/// Add an actual value line to the message containing
		/// the text provided as an argument.
		/// </summary>
		/// <param name="text">Text describing the actual value.</param>
		public void AddActualLine( string text )
		{
			AddLine( string.Format( ExpectedAndActualFormat, actualPrefix, text ) );
		}

		/// <summary>
		/// Add an expected value line to the message containing
		/// a string representation of the object provided.
		/// </summary>
		/// <param name="expected">An object representing the expected value</param>
		public void DisplayExpectedValue( object expected )
		{
			AddExpectedLine( FormatObjectForDisplay( expected ) );
		}

		/// <summary>
		/// Add an actual value line to the message containing
		/// a string representation of the object provided.
		/// </summary>
		/// <param name="actual">An object representing what was actually found</param>
		public void DisplayActualValue( object actual )
		{
			AddActualLine( FormatObjectForDisplay( actual ) );
		}

		/// <summary>
		/// Display two lines that communicate the expected value, and the actual value
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value found</param>
		public void DisplayExpectedAndActual( Object expected, Object actual )
		{
			DisplayExpectedValue( expected );
			DisplayActualValue( actual );
		}

		/// <summary>
		/// Draws a marker under the expected/actual strings that highlights
		/// where in the string a mismatch occurred.
		/// </summary>
		/// <param name="position">The position of the mismatch</param>
		public void DisplayPositionMarker( int position )
		{
			position = (position > 0 ? position : 0);
			AddLine( "\t{0}^", new String( '-', ExpectedPrefix.Length + position + 3 ) );
		}

		/// <summary>
		/// Called to create additional message lines when two objects have been
		/// found to be unequal.  If the inputs are strings, a special message is
		/// rendered that can help track down where the strings are different,
		/// based on differences in length, or differences in content.
		///
		/// If the inputs are not strings, the ToString method of the objects
		/// is used to show what is different about them.
		/// </summary>
		/// <param name="expected">The expected value</param>
		/// <param name="actual">The actual value</param>
		/// <param name="caseInsensitive">True if a case-insensitive comparison is being performed</param>
		public void DisplayDifferences( object expected, object actual, bool caseInsensitive )
		{
			if( InputsAreStrings( expected, actual ) )
			{
				DisplayStringDifferences(
					(string)expected,
					(string)actual,
					caseInsensitive );
			}
			else
			{
				DisplayExpectedAndActual( expected, actual );
			}
		}

		/// <summary>
		/// Display a standard message showing the differences found between
		/// two arrays that were expected to be equal.
		/// </summary>
		/// <param name="expected">The expected array value</param>
		/// <param name="actual">The actual array value</param>
		/// <param name="rank">The rank of the array to compare.</param>
		/// <param name="indices">The index at which a difference was found</param>
		public void DisplayArrayDifferences( Array expected, Array actual, int rank, int[] indices )
		{
			if( expected.Rank != actual.Rank )
			{
				AddLine( DifferentArrayRanksFormat, expected.Rank, actual.Rank );
			}
			else
			{
				if( expected.GetLength( rank ) != actual.GetLength( rank ) )
				{
					AddLine( DifferentArrayLengthsFormat,
						expected.GetLength( rank ), actual.GetLength( rank ), rank );
				}
				else
				{
					AddLine( SameArrayLengthsFormat, expected.GetLength( rank ), rank );
				}

				Converter<int, string> toString = delegate( int value ) { return value.ToString(); };
				AddLine( ArraysDifferAtIndexFormat, String.Join( ",",
					Array.ConvertAll<int, string>( indices, toString ) ) );

				if( indices[rank] < expected.GetLength( rank ) && indices[rank] < actual.GetLength( rank ) )
				{
					DisplayDifferences( expected.GetValue( indices ), actual.GetValue( indices ), false );
				}
				else
				{
					if( expected.GetLength( rank ) < actual.GetLength( rank ) )
					{
						DisplayListElements( "   extra:", actual, indices[rank], 3 );
					}
					else
					{
						DisplayListElements( " missing:", expected, indices[rank], 3 );
					}
				}
			}
		}

		/// <summary>
		/// Displays elements from a list on a line
		/// </summary>
		/// <param name="label">Text to prefix the line with</param>
		/// <param name="list">The list of items to display</param>
		/// <param name="index">The index in the list of the first element to display</param>
		/// <param name="maximumElements">The maximum number of elements to display</param>
		public void DisplayListElements( string label, IList list, int index, int maximumElements )
		{
			AddLine( FormatListForDisplay( label, list, index, maximumElements ) );
		}

		/// <summary>
		/// Reports whether the string lengths are the same or different, and
		/// what the string lengths are.
		/// </summary>
		/// <param name="sExpected">The expected string</param>
		/// <param name="sActual">The actual string value</param>
		private void BuildStringLengthReport( string sExpected, string sActual )
		{
			if( sExpected.Length != sActual.Length )
			{
				AddLine( DifferentStringLengthsFormat, sExpected.Length, sActual.Length );
			}
			else
			{
				AddLine( SameStringLengthsFormat, sExpected.Length );
			}
		}

		/// <summary>
		/// Constructs a message that can be displayed when the content of two
		/// strings are different, but the string lengths are the same.  The
		/// message will clip the strings to a reasonable length, centered
		/// around the first position where they are mismatched, and draw
		/// a line marking the position of the difference to make comparison
		/// quicker.
		/// </summary>
		/// <param name="sExpected">The expected string value</param>
		/// <param name="sActual">The actual string value</param>
		/// <param name="caseInsensitive">True if a case-insensitive comparison is being performed</param>
		private void DisplayStringDifferences( string sExpected, string sActual, bool caseInsensitive )
		{
			//
			// If they mismatch at a specified position, report the
			// difference.
			//
			int iPosition = (caseInsensitive
				? FindMismatchPosition( sExpected.ToLower(), sActual.ToLower(), 0 )
				: FindMismatchPosition( sExpected, sActual, 0 ));
			//
			// If the lengths differ, but they match up to the length,
			// show the difference just past the length of the shorter
			// string
			//
			if( iPosition == -1 )
			{
				iPosition = Math.Min( sExpected.Length, sActual.Length );
			}

			BuildStringLengthReport( sExpected, sActual );

			AddLine( StringsDifferAtIndexFormat, iPosition );

			//
			// Clips the strings, then turns any hidden whitespace into visible
			// characters
			//
			string sClippedExpected = ConvertEscapedCharacters( ClipAroundPosition( sExpected, iPosition ) );
			string sClippedActual = ConvertEscapedCharacters( ClipAroundPosition( sActual, iPosition ) );

			DisplayExpectedAndActual(
				sClippedExpected,
				sClippedActual );

			// Add a line showing where they differ.  If the string lengths are
			// different, they start differing just past the length of the
			// shorter string
			DisplayPositionMarker( caseInsensitive
				? FindMismatchPosition( sClippedExpected.ToLower(), sClippedActual.ToLower(), 0 )
				: FindMismatchPosition( sClippedExpected, sClippedActual, 0 ) );
		}

		#region Static Methods

		/// <summary>
		/// Formats an object for display in a message line
		/// </summary>
		/// <param name="obj">The object to be displayed</param>
		/// <returns></returns>
		public static string FormatObjectForDisplay( object obj )
		{
			if( obj == null )
			{
				return String.Format( ObjectFormat, "(null)" );
			}
			if( obj is string )
			{
				return String.Format( ObjectFormat, String.Concat( "\"", ConvertEscapedCharacters( obj.ToString() ), "\"" ) );
			}
			if( obj is char )
			{
				return String.Format( ObjectFormat, String.Concat( "'", ConvertEscapedCharacters( obj.ToString() ), "'" ) );
			}
			if( obj is double )
			{
				return String.Format( ObjectFormat, ((double)obj).ToString( "G17" ) );
			}
			if( obj is float )
			{
				return String.Format( ObjectFormat, ((float)obj).ToString( "G9" ) );
			}
			IList list = obj as IList;
			if( list != null )
			{
				if( list.Count == 0 )
				{
					return String.Format( ObjectFormat, "(empty)" );
				}
				else
				{
					return FormatListForDisplay( null, list, 0, 3 );
				}
			}
			return string.Format( ObjectFormat, obj );
		}

		private static string FormatListForDisplay( string label, IList list, int index, int maximumElements )
		{

			string value = null;
			if( list == null || list.Count == 0 )
			{
				value = FormatObjectForDisplay( list );
			}
			else
			{
				Array ary = list as Array;
				if( ary != null && ary.Rank > 1 )
				{
					return string.Format( ObjectFormat, list );
				}

				StringBuilder sb = new StringBuilder();
				if( index > 0 )
				{
					sb.Append( "...," );
				}
				else if( index < 0 )
				{
					index = 0;
				}
				for( int i = 0; i < maximumElements && index < list.Count; i++ )
				{
					sb.Append( FormatObjectForDisplay( list[index++] ) );

					if( index < list.Count )
					{
						sb.Append( "," );
					}
				}
				if( index < list.Count )
				{
					sb.Append( "..." );
				}
				value = String.Format( ObjectFormat, sb.ToString() );
			}
			if( String.IsNullOrEmpty( label ) )
			{
				return value;
			}
			else
			{
				return String.Concat( label, ":", value );
			}
		}

		/// <summary>
		/// Tests two objects to determine if they are strings.
		/// </summary>
		private static bool InputsAreStrings( Object expected, Object actual )
		{
			return expected as string != null && actual as string != null;
		}

		/// <summary>
		/// Used to create a StringBuilder that is used for constructing
		/// the output message when text is different.  Handles initialization
		/// when a message is provided.  If message is null, an empty
		/// StringBuilder is returned.
		/// </summary>
		private static StringBuilder CreateStringBuilder( string message, params object[] args )
		{
			StringBuilder sb = new StringBuilder();
			if( message != null )
			{
				if( args != null )
				{
					sb.AppendFormat( message, args );
				}
				else
				{
					sb.Append( message );
				}
			}
			return sb;
		}

		/// <summary>
		/// Renders up to M characters before, and up to N characters after
		/// the specified index position.  If leading or trailing text is
		/// clipped, and elipses "..." is added where the missing text would
		/// be.
		///
		/// Clips strings to limit previous or post newline characters,
		/// since these mess up the comparison
		/// </summary>
		private static string ClipAroundPosition( string text, int position )
		{
			if( String.IsNullOrEmpty( text ) )
			{
				return String.Empty;
			}

			position = (position > 0 ? position : 0);
			position = (position < text.Length ? position : text.Length);

			StringBuilder sb = new StringBuilder();
			int start = 0;
			if( position > PreClipLength )
			{
				sb.Append( "..." );
				start = position - PreClipLength;
			}
			int length = text.Length - start;
			if( position + PostClipLength < text.Length )
			{
				length = position + PostClipLength - start;
				sb.Append( text.Substring( start, length ) );
				sb.Append( "..." );
			}
			else
			{
				sb.Append( text.Substring( start ) );
			}

			return sb.ToString();
		}

		/// <summary>
		/// Shows the position two strings start to differ.  Comparison
		/// starts at the start index.
		/// </summary>
		/// <returns>-1 if no mismatch found, or the index where mismatch found</returns>
		private static int FindMismatchPosition( string sExpected, string sActual, int iStart )
		{
			int iLength = Math.Min( sExpected.Length, sActual.Length );
			for( int i = Math.Max( iStart, 0 ); i < iLength; i++ )
			{
				//
				// If they mismatch at a specified position, report the
				// difference.
				//
				if( sExpected[i] != sActual[i] )
				{
					return i;
				}
			}
			//
			// Strings have same content up to the length of the shorter string.
			// Mismatch occurs because string lengths are different, so show
			// that they start differing where the shortest string ends
			//
			if( sExpected.Length != sActual.Length )
			{
				return iLength;
			}

			//
			// Same strings
			//
			Assert.IsTrue( sExpected.Equals( sActual ) );
			return -1;
		}

		/// <summary>
		/// Turns CR, LF, or TAB into visual indicator to preserve visual marker
		/// position. This is done by replacing the '\r' into '\\' and 'r'
		/// characters, and the '\n' into '\\' and 'n' characters, and '\t' into
		/// '\\' and 't' characters.
		///
		/// Thus the single character becomes two characters for display.
		/// </summary>
		private static string ConvertEscapedCharacters( string sInput )
		{
			if( null != sInput )
			{
				sInput = sInput.Replace( "\\", "\\\\" );
				sInput = sInput.Replace( "\r", "\\r" );
				sInput = sInput.Replace( "\n", "\\n" );
				sInput = sInput.Replace( "\t", "\\t" );
				sInput = sInput.Replace( "\"", "\\\"" );
			}
			return sInput;
		}
		#endregion
	}
}
