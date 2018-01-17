using System;
using System.Collections.Generic;
using System.Text;

using Testing;
using Testing.UnitTest;

namespace TestingTestSuite.UnitTest
{
	/// <summary>
	/// Description of the unit tests covered in AssetTests
	/// </summary>
	[TestFixture( "A brief description of the purpose of this fixture" )]
	public class AssertTests
	{
		#region Fields

		private const string Message = null;
		private const string FormatedMessage = null;

		private string o1;
		private string o2;
		private string o3;
		private string[] objects2;
		private string[] objects1;
		private string[,] objects3;
		private string[,] objects4;

		#endregion

		#region Fixture Setup

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			o1 = "Test1";
			o2 = "Test2";
			o3 = "Test3";
			objects2 = new string[] { o1, o2 };
			objects1 = new string[] { o1 };
			objects3 = new string[,] { { o1 }, { o2 } };
			objects4 = new string[,] { { o1 }, { o3 } };
		}

		#endregion

		#region Equality Assert Tests

		[Test( "Runs all equality asserters, none should fail." )]
		public void AssertSuccessTest()
		{
			// AreEqual
			Assert.AreEqual( (decimal)1, (decimal)1 );
			Assert.AreEqual( (int)1, (int)1 );
			Assert.AreEqual( o1, o1 );
			Assert.AreEqual( (uint)1, (uint)1 );
			Assert.AreEqual( (decimal)1, (decimal)1, Message );
			Assert.AreEqual( (double)1, (double)1, 0 );
			Assert.AreEqual( (float)1, (float)1, 0 );
			Assert.AreEqual( (int)1, (int)1, Message );
			Assert.AreEqual( o1, o1, Message );
			Assert.AreEqual( (uint)1, (uint)1, Message );
			Assert.AreEqual( (decimal)1, (decimal)1, FormatedMessage, 1 );
			Assert.AreEqual( (double)1, (double)1, 0, Message );
			Assert.AreEqual( (float)1, (float)1, 0, Message );
			Assert.AreEqual( (int)1, (int)1, FormatedMessage, 1 );
			Assert.AreEqual( o1, o1, FormatedMessage, o1 );
			Assert.AreEqual( (uint)1, (uint)1, FormatedMessage, 1 );
			Assert.AreEqual( (double)1, (double)1, 0, FormatedMessage, 1 );
			Assert.AreEqual( (float)1, (float)1, 0, FormatedMessage, 1 );

			// Numeric
			Assert.AreEqual( double.NaN, double.NaN );
			Assert.AreEqual( double.PositiveInfinity, double.PositiveInfinity );
			Assert.AreEqual( double.NegativeInfinity, double.NegativeInfinity );
			Assert.AreEqual( (double)1, (byte)1 );

			// Objects
			Assert.AreEqual( null, null );
			Assert.AreEqual( objects1, objects1 );
			Assert.AreEqual( objects3, objects3 );

			// AreNotEqual
			Assert.AreNotEqual( (decimal)1, (decimal)2 );
			Assert.AreNotEqual( (double)1, (double)2 );
			Assert.AreNotEqual( (float)1, (float)2 );
			Assert.AreNotEqual( (int)1, (int)2 );
			Assert.AreNotEqual( o1, o2 );
			Assert.AreNotEqual( (uint)1, (uint)2 );
			Assert.AreNotEqual( (decimal)1, (decimal)2, Message );
			Assert.AreNotEqual( (double)1, (double)2, Message );
			Assert.AreNotEqual( (float)1, (float)2, Message );
			Assert.AreNotEqual( (int)1, (int)2, Message );
			Assert.AreNotEqual( o1, o2, Message );
			Assert.AreNotEqual( (uint)1, (uint)2, Message );
			Assert.AreNotEqual( (decimal)1, (decimal)2, FormatedMessage, 1 );
			Assert.AreNotEqual( (double)1, (double)2, FormatedMessage, 1 );
			Assert.AreNotEqual( (float)1, (float)2, FormatedMessage, 1 );
			Assert.AreNotEqual( (int)1, (int)2, FormatedMessage, 1 );
			Assert.AreNotEqual( o1, o2, FormatedMessage, 01 );
			Assert.AreNotEqual( (uint)1, (uint)2, FormatedMessage, 1 );

			// Numeric
			Assert.AreNotEqual( double.NaN, double.PositiveInfinity );
			Assert.AreNotEqual( double.NaN, double.NegativeInfinity );
			Assert.AreNotEqual( double.PositiveInfinity, double.NegativeInfinity );
			Assert.AreNotEqual( (double)1, (byte)2 );
			Assert.AreNotEqual( (double)1, "1" );

			// Objects
			Assert.AreNotEqual( o1, null );
			Assert.AreNotEqual( null, o1 );
			Assert.AreNotEqual( objects1, objects2 );
			Assert.AreNotEqual( objects1, objects3 );
			Assert.AreNotEqual( objects3, objects4 );

		}

		[Test( "Runs all equality asserters, all should fail." )]
		[ExpectedExceptionCount( typeof( AssertionException ), 53 )]
		public void AssertFailTest()
		{
			// AreEqual
			Assert.AreEqual( (decimal)1, (decimal)2 );
			Assert.AreEqual( (int)1, (int)2 );
			Assert.AreEqual( o1, o2 );
			Assert.AreEqual( (uint)1, (uint)2 );
			Assert.AreEqual( (decimal)1, (decimal)2, Message );
			Assert.AreEqual( (double)1, (double)2, 0 );
			Assert.AreEqual( (float)1, (float)2, 0 );
			Assert.AreEqual( (int)1, (int)2, Message );
			Assert.AreEqual( o1, o2, Message );
			Assert.AreEqual( (uint)1, (uint)2, Message );
			Assert.AreEqual( (decimal)1, (decimal)2, FormatedMessage, 1 );
			Assert.AreEqual( (double)1, (double)2, 0, Message );
			Assert.AreEqual( (float)1, (float)2, 0, Message );
			Assert.AreEqual( (int)1, (int)2, FormatedMessage, 1 );
			Assert.AreEqual( o1, o2, FormatedMessage, o1 );
			Assert.AreEqual( (uint)1, (uint)2, FormatedMessage, 1 );
			Assert.AreEqual( (double)1, (double)2, 0, FormatedMessage, 1 );
			Assert.AreEqual( (float)1, (float)2, 0, FormatedMessage, 1 );

			// Numeric
			Assert.AreEqual( double.NaN, double.PositiveInfinity );
			Assert.AreEqual( double.NaN, double.NegativeInfinity );
			Assert.AreEqual( double.PositiveInfinity, double.NegativeInfinity );
			Assert.AreEqual( (double)1, (byte)2 );
			Assert.AreEqual( (double)1, "1" );

			// Objects
			Assert.AreEqual( o1, null );
			Assert.AreEqual( null, o1 );
			Assert.AreEqual( objects1, objects2 );
			Assert.AreEqual( objects1, objects3 );
			Assert.AreEqual( objects3, objects4 );

			// AreNotEqual
			Assert.AreNotEqual( (decimal)1, (decimal)1 );
			Assert.AreNotEqual( (double)1, (double)1 );
			Assert.AreNotEqual( (float)1, (float)1 );
			Assert.AreNotEqual( (int)1, (int)1 );
			Assert.AreNotEqual( o1, o1 );
			Assert.AreNotEqual( (uint)1, (uint)1 );
			Assert.AreNotEqual( (decimal)1, (decimal)1, Message );
			Assert.AreNotEqual( (double)1, (double)1, Message );
			Assert.AreNotEqual( (float)1, (float)1, Message );
			Assert.AreNotEqual( (int)1, (int)1, Message );
			Assert.AreNotEqual( o1, o1, Message );
			Assert.AreNotEqual( (uint)1, (uint)1, Message );
			Assert.AreNotEqual( (decimal)1, (decimal)1, FormatedMessage, 1 );
			Assert.AreNotEqual( (double)1, (double)1, FormatedMessage, 1 );
			Assert.AreNotEqual( (float)1, (float)1, FormatedMessage, 1 );
			Assert.AreNotEqual( (int)1, (int)1, FormatedMessage, 1 );
			Assert.AreNotEqual( o1, o1, FormatedMessage, 01 );
			Assert.AreNotEqual( (uint)1, (uint)1, FormatedMessage, 1 );

			// Numeric
			Assert.AreNotEqual( double.NaN, double.NaN );
			Assert.AreNotEqual( double.PositiveInfinity, double.PositiveInfinity );
			Assert.AreNotEqual( double.NegativeInfinity, double.NegativeInfinity );
			Assert.AreNotEqual( (double)1, (byte)1 );

			// Objects
			Assert.AreNotEqual( null, null );
			Assert.AreNotEqual( objects1, objects1 );
			Assert.AreNotEqual( objects3, objects3 );
		}

		#endregion

		#region Non Equality Assert Tests

		[Test( "Runs all non equality asserters, none should fail." )]
		public void NonEqualityAssertSuccessTest()
		{
			// AreNotSame
			Assert.AreNotSame( o1, o2 );
			Assert.AreNotSame( o1, o2, Message );
			Assert.AreNotSame( o1, o2, FormatedMessage, o1 );

			// AreSame
			Assert.AreSame( o1, o1 );
			Assert.AreSame( o1, o1, Message );
			Assert.AreSame( o1, o1, FormatedMessage, o1 );

			// Contains
			Assert.Contains( o1, objects1 );
			Assert.Contains( o1, objects1, Message );
			Assert.Contains( o1, objects1, FormatedMessage, o1 );

			// Counter
			Assert.AreNotEqual( 0, Assert.Counter );

			// Greater
			Assert.Greater( (decimal)3, (decimal)2 );
			Assert.Greater( (double)3, (double)2 );
			Assert.Greater( (float)3, (float)2 );
			Assert.Greater( o3, o2 );
			Assert.Greater( (int)3, (int)2 );
			Assert.Greater( (uint)3, (uint)2 );
			Assert.Greater( (decimal)3, (decimal)2, Message );
			Assert.Greater( (double)3, (double)2, Message );
			Assert.Greater( (float)3, (float)2, Message );
			Assert.Greater( o3, o2, Message );
			Assert.Greater( (int)3, (int)2, Message );
			Assert.Greater( (uint)3, (uint)2, Message );
			Assert.Greater( (decimal)3, (decimal)2, FormatedMessage, 3 );
			Assert.Greater( (double)3, (double)2, FormatedMessage, 3 );
			Assert.Greater( (float)3, (float)2, FormatedMessage, 3 );
			Assert.Greater( o3, o2, FormatedMessage, 03 );
			Assert.Greater( (int)3, (int)2, FormatedMessage, 3 );
			Assert.Greater( (uint)3, (uint)2, FormatedMessage, 3 );

			// IsAssignableFrom
			Assert.IsAssignableFrom( typeof( string ), o1 );
			Assert.IsAssignableFrom( typeof( string ), o1, Message );
			Assert.IsAssignableFrom( typeof( string ), o1, FormatedMessage, 1 );

			// IsEmpty
			Assert.IsEmpty( "" );
			Assert.IsEmpty( Type.EmptyTypes );
			Assert.IsEmpty( "", Message );
			Assert.IsEmpty( Type.EmptyTypes, Message );
			Assert.IsEmpty( "", FormatedMessage, 1 );
			Assert.IsEmpty( Type.EmptyTypes, FormatedMessage, 1 );

			// IsFalse
			Assert.IsFalse( false );
			Assert.IsFalse( false, Message );
			Assert.IsFalse( false, FormatedMessage, 1 );

			// IsInstanceOfType
			Assert.IsInstanceOfType( typeof( object ), o1 );
			Assert.IsInstanceOfType( typeof( object ), o1, Message );
			Assert.IsInstanceOfType( typeof( object ), o1, FormatedMessage, 1 );

			// IsNaN
			Assert.IsNaN( Double.NaN );
			Assert.IsNaN( Double.NaN, Message );
			Assert.IsNaN( Double.NaN, FormatedMessage, 1 );

			// IsNotAssignableFrom
			Assert.IsNotAssignableFrom( typeof( Assert ), o1 );
			Assert.IsNotAssignableFrom( typeof( Assert ), o1, Message );
			Assert.IsNotAssignableFrom( typeof( Assert ), o1, FormatedMessage, 1 );

			// IsNotEmpty
			Assert.IsNotEmpty( o1 );
			Assert.IsNotEmpty( objects1 );
			Assert.IsNotEmpty( o1, Message );
			Assert.IsNotEmpty( objects1, Message );
			Assert.IsNotEmpty( o1, FormatedMessage, 1 );
			Assert.IsNotEmpty( objects1, FormatedMessage, 1 );

			// IsNotInstanceOfType
			Assert.IsNotInstanceOfType( typeof( Assert ), o1 );
			Assert.IsNotInstanceOfType( typeof( Assert ), o1, Message );
			Assert.IsNotInstanceOfType( typeof( Assert ), o1 );

			// IsNotNull
			Assert.IsNotNull( o1 );
			Assert.IsNotNull( o1, Message );
			Assert.IsNotNull( o1, FormatedMessage, 1 );

			// IsNull
			Assert.IsNull( null );
			Assert.IsNull( null, Message );
			Assert.IsNull( null, FormatedMessage, 1 );

			// IsTrue
			Assert.IsTrue( true );
			Assert.IsTrue( true, Message );
			Assert.IsTrue( true, FormatedMessage, 1 );

			// Less
			Assert.Less( (decimal)1, (decimal)2 );
			Assert.Less( (double)1, (double)2 );
			Assert.Less( (float)1, (float)2 );
			Assert.Less( o1, o2 );
			Assert.Less( (int)1, (int)2 );
			Assert.Less( (uint)1, (uint)2 );
			Assert.Less( (decimal)1, (decimal)2, Message );
			Assert.Less( (double)1, (double)2, Message );
			Assert.Less( (float)1, (float)2, Message );
			Assert.Less( o1, o2, Message );
			Assert.Less( (int)1, (int)2, Message );
			Assert.Less( (uint)1, (uint)2, Message );
			Assert.Less( (decimal)1, (decimal)2, FormatedMessage, 1 );
			Assert.Less( (double)1, (double)2, FormatedMessage, 1 );
			Assert.Less( (float)1, (float)2, FormatedMessage, 1 );
			Assert.Less( o1, o2, FormatedMessage, 01 );
			Assert.Less( (int)1, (int)2, FormatedMessage, 1 );
			Assert.Less( (uint)1, (uint)2, FormatedMessage, 1 );
		}

		[Test( "Runs all non equality asserters, all should fail." )]
		[ExpectedExceptionCount( typeof( AssertionException ), 90 )]
		public void NonEqualityAssertFailTest()
		{
			// AreNotSame
			Assert.AreNotSame( o1, o1 );
			Assert.AreNotSame( o1, o1, Message );
			Assert.AreNotSame( o1, o1, FormatedMessage, o1 );

			// AreSame
			Assert.AreSame( o1, o2 );
			Assert.AreSame( o1, o2, Message );
			Assert.AreSame( o1, o2, FormatedMessage, o1 );

			// Contains
			Assert.Contains( o2, objects1 );
			Assert.Contains( o2, objects1, Message );
			Assert.Contains( o2, objects1, FormatedMessage, o1 );

			// Fail
			Assert.Fail();
			Assert.Fail( Message );
			Assert.Fail( FormatedMessage, 1 );
			Assert.Fail( "{0}", 1 );

			// Greater
			Assert.Greater( (decimal)1, (decimal)2 );
			Assert.Greater( (double)1, (double)2 );
			Assert.Greater( (float)1, (float)2 );
			Assert.Greater( o1, o2 );
			Assert.Greater( (int)1, (int)2 );
			Assert.Greater( (uint)1, (uint)2 );
			Assert.Greater( (decimal)1, (decimal)2, Message );
			Assert.Greater( (double)1, (double)2, Message );
			Assert.Greater( (float)1, (float)2, Message );
			Assert.Greater( o1, o2, Message );
			Assert.Greater( (int)1, (int)2, Message );
			Assert.Greater( (uint)1, (uint)2, Message );
			Assert.Greater( (decimal)1, (decimal)2, FormatedMessage, 1 );
			Assert.Greater( (double)1, (double)2, FormatedMessage, 1 );
			Assert.Greater( (float)1, (float)2, FormatedMessage, 1 );
			Assert.Greater( o1, o2, FormatedMessage, 01 );
			Assert.Greater( (int)1, (int)2, FormatedMessage, 1 );
			Assert.Greater( (uint)1, (uint)2, FormatedMessage, 1 );

			// IsAssignableFrom
			Assert.IsAssignableFrom( typeof( Assert ), o1 );
			Assert.IsAssignableFrom( typeof( Assert ), o1, Message );
			Assert.IsAssignableFrom( typeof( Assert ), o1, FormatedMessage, 1 );

			// IsEmpty
			Assert.IsEmpty( o1 );
			Assert.IsEmpty( objects1 );
			Assert.IsEmpty( o1, Message );
			Assert.IsEmpty( objects1, Message );
			Assert.IsEmpty( o1, FormatedMessage, 1 );
			Assert.IsEmpty( objects1, FormatedMessage, 1 );

			// IsFalse
			Assert.IsFalse( true );
			Assert.IsFalse( true, Message );
			Assert.IsFalse( true, FormatedMessage, 1 );

			// IsInstanceOfType
			Assert.IsInstanceOfType( typeof( Assert ), o1 );
			Assert.IsInstanceOfType( typeof( Assert ), o1, Message );
			Assert.IsInstanceOfType( typeof( Assert ), o1, FormatedMessage, 1 );

			// IsNaN
			Assert.IsNaN( 1 );
			Assert.IsNaN( 1, Message );
			Assert.IsNaN( 1, FormatedMessage, 1 );

			// IsNotAssignableFrom
			Assert.IsNotAssignableFrom( typeof( string ), o1 );
			Assert.IsNotAssignableFrom( typeof( string ), o1, Message );
			Assert.IsNotAssignableFrom( typeof( string ), o1, FormatedMessage, 1 );

			// IsNotEmpty
			Assert.IsNotEmpty( "" );
			Assert.IsNotEmpty( Type.EmptyTypes );
			Assert.IsNotEmpty( "", Message );
			Assert.IsNotEmpty( Type.EmptyTypes, Message );
			Assert.IsNotEmpty( "", FormatedMessage, 1 );
			Assert.IsNotEmpty( Type.EmptyTypes, FormatedMessage, 1 );

			// IsNotInstanceOfType
			Assert.IsNotInstanceOfType( typeof( object ), o1 );
			Assert.IsNotInstanceOfType( typeof( object ), o1, Message );
			Assert.IsNotInstanceOfType( typeof( object ), o1 );

			// IsNotNull
			Assert.IsNotNull( null );
			Assert.IsNotNull( null, Message );
			Assert.IsNotNull( null, FormatedMessage, 1 );

			// IsNull
			Assert.IsNull( o1 );
			Assert.IsNull( o1, Message );
			Assert.IsNull( o1, FormatedMessage, 1 );

			// IsTrue
			Assert.IsTrue( false );
			Assert.IsTrue( false, Message );
			Assert.IsTrue( false, FormatedMessage, 1 );

			// Less
			Assert.Less( (decimal)3, (decimal)2 );
			Assert.Less( (double)3, (double)2 );
			Assert.Less( (float)3, (float)2 );
			Assert.Less( o3, o2 );
			Assert.Less( (int)3, (int)2 );
			Assert.Less( (uint)3, (uint)2 );
			Assert.Less( (decimal)3, (decimal)2, Message );
			Assert.Less( (double)3, (double)2, Message );
			Assert.Less( (float)3, (float)2, Message );
			Assert.Less( o3, o2, Message );
			Assert.Less( (int)3, (int)2, Message );
			Assert.Less( (uint)3, (uint)2, Message );
			Assert.Less( (decimal)3, (decimal)2, FormatedMessage, 3 );
			Assert.Less( (double)3, (double)2, FormatedMessage, 3 );
			Assert.Less( (float)3, (float)2, FormatedMessage, 3 );
			Assert.Less( o3, o2, FormatedMessage, 03 );
			Assert.Less( (int)3, (int)2, FormatedMessage, 3 );
			Assert.Less( (uint)3, (uint)2, FormatedMessage, 3 );

			// Unused methods
			Assert.Equals( o1, o1 );
			Assert.ReferenceEquals( o1, o1 );

		}

		#endregion

		#region Assert Exception Tests

		[Test( "Test the AssertionException constructors" )]
		public void AssertExceptionTest()
		{
			AssertionException exp;
			exp = new AssertionException( null );
			exp = new AssertionException( null, new Exception() );
		}

		#endregion

		#region Other Equality Tests

		[Test( "Test the handling of null in ObjectsEqual method" )]
		public void NullObjectsEqualTest()
		{
			EqualAsserter equalAsserter = new EqualAsserter( null, null, null );
			PrivateObject po = new PrivateObject( equalAsserter );
			Assert.IsTrue( (bool)po.Invoke( "ObjectsEqual", null, null ) );
			Assert.IsFalse( (bool)po.Invoke( "ObjectsEqual", o1, null ) );
			Assert.IsFalse( (bool)po.Invoke( "ObjectsEqual", null, o1 ) );
		}


		#endregion Other Equality Tests

	}
}
