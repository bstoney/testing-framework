using System;
using System.Collections.Generic;
using System.Text;

using Testing;
using Testing.UnitTest;
using System.Reflection;

namespace TestingTestSuite.UnitTest
{
	/// <summary>
	/// Description of the unit tests covered in Fixture1Tests
	/// </summary>
	[TestFixture( "To Test the PrivateObject" )]
	public class PrivateObjectTests
	{

		[Test( "Test the Constructors on the PrivateObject" )]
		public void ConstructorTest()
		{
			MockObject mockObject = new MockObject();
			PrivateObject poMockObject = new PrivateObject( mockObject );
			Assert.AreEqual( mockObject, poMockObject.Instance );
			Assert.AreEqual( mockObject.GetType(), poMockObject.Type );

			poMockObject = new PrivateObject( mockObject.GetType().AssemblyQualifiedName );
			Assert.IsNotNull( poMockObject.Instance );
			Assert.AreEqual( mockObject.GetType(), poMockObject.Type );
			poMockObject = new PrivateObject( mockObject.GetType() );
			Assert.IsNotNull( poMockObject.Instance );
			Assert.AreEqual( mockObject.GetType(), poMockObject.Type );

			poMockObject = new PrivateObject( mockObject.GetType().AssemblyQualifiedName );
			poMockObject.New( new object[] { new object() } );
			Assert.IsNotNull( poMockObject.Instance );
			Assert.AreEqual( mockObject.GetType(), poMockObject.Type );
			poMockObject = new PrivateObject( mockObject.GetType() );
			poMockObject.New( new object[] { new object() } );
			Assert.IsNotNull( poMockObject.Instance );
			Assert.AreEqual( mockObject.GetType(), poMockObject.Type );

			poMockObject = new PrivateObject( typeof( MockStaticObject ).AssemblyQualifiedName );
			poMockObject.New( new object[] { } );
			Assert.IsNull( poMockObject.Instance );
			Assert.AreEqual( typeof( MockStaticObject ), poMockObject.Type );
			poMockObject = new PrivateObject( typeof( MockStaticObject ) );
			poMockObject.New( new object[] { } );
			Assert.IsNull( poMockObject.Instance );
			Assert.AreEqual( typeof( MockStaticObject ), poMockObject.Type );

			poMockObject = new PrivateObject( mockObject.GetType().AssemblyQualifiedName );
			poMockObject.New( new Type[] { }, (object[])null );
			Assert.IsNotNull( poMockObject.Instance );
			Assert.AreEqual( mockObject.GetType(), poMockObject.Type );
			poMockObject = new PrivateObject( mockObject.GetType() );
			poMockObject.New( new Type[] { }, (object[])null );
			Assert.IsNotNull( poMockObject.Instance );
			Assert.AreEqual( mockObject.GetType(), poMockObject.Type );

			poMockObject = new PrivateObject( typeof( MockStaticObject ).AssemblyQualifiedName );
			poMockObject.New( new Type[] { }, (object[])null );
			Assert.IsNull( poMockObject.Instance );
			Assert.AreEqual( typeof( MockStaticObject ), poMockObject.Type );
			poMockObject = new PrivateObject( typeof( MockStaticObject ) );
			poMockObject.New( new Type[] { }, (object[])null );
			Assert.IsNull( poMockObject.Instance );
			Assert.AreEqual( typeof( MockStaticObject ), poMockObject.Type );

			poMockObject = new PrivateObject( mockObject.GetType().AssemblyQualifiedName );
			poMockObject.New( new Type[] { typeof( object ) }, new object[] { new object() } );
			Assert.IsNotNull( poMockObject.Instance );
			Assert.AreEqual( mockObject.GetType(), poMockObject.Type );
			poMockObject = new PrivateObject( mockObject.GetType() );
			poMockObject.New( new Type[] { typeof( object ) }, new object[] { new object() } );
			Assert.IsNotNull( poMockObject.Instance );
			Assert.AreEqual( mockObject.GetType(), poMockObject.Type );

			poMockObject = new PrivateObject( typeof( MockGenericObject<> ).AssemblyQualifiedName, new Type[] { typeof( string ) } );
			poMockObject.New( new Type[] { }, null );
			Assert.IsNotNull( poMockObject.Instance );
			Assert.AreEqual( typeof( MockGenericObject<string> ), poMockObject.Type );
			poMockObject = new PrivateObject( typeof( MockGenericObject<> ), new Type[] { typeof( string ) } );
			poMockObject.New( new Type[] { }, null );
			Assert.IsNotNull( poMockObject.Instance );
			Assert.AreEqual( typeof( MockGenericObject<string> ), poMockObject.Type );

			poMockObject = new PrivateObject( typeof( MockGenericObject<> ).AssemblyQualifiedName, new Type[] { typeof( string ) } );
			poMockObject.New( new Type[] { typeof( string ) }, new object[] { typeof( string ) } );
			Assert.IsNotNull( poMockObject.Instance );
			Assert.AreEqual( typeof( MockGenericObject<string> ), poMockObject.Type );
			poMockObject = new PrivateObject( typeof( MockGenericObject<> ), new Type[] { typeof( string ) } );
			poMockObject.New( new Type[] { typeof( string ) }, new object[] { typeof( string ) } );
			Assert.IsNotNull( poMockObject.Instance );
			Assert.AreEqual( typeof( MockGenericObject<string> ), poMockObject.Type );

			poMockObject = new PrivateObject( typeof( MockStaticGenericObject<> ).AssemblyQualifiedName, new Type[] { typeof( string ) } );
			poMockObject.New( new Type[] { }, null );
			Assert.IsNull( poMockObject.Instance );
			Assert.AreEqual( typeof( MockStaticGenericObject<string> ), poMockObject.Type );
		}

		[Test( "Test to Fail the Constructors on the PrivateObject" )]
		[ExpectedExceptionCount( typeof( TargetParameterCountException ), 6 )]
		public void ConstructorTestToFailTargetParameterCountExceptionTest()
		{
			PrivateObject poMockObject;
			poMockObject = new PrivateObject( typeof( MockObject ).AssemblyQualifiedName );
			poMockObject.New( new Type[] { typeof( object ) } );
			poMockObject = new PrivateObject( typeof( MockObject ) );
			poMockObject.New( new Type[] { typeof( object ) } );

			poMockObject = new PrivateObject( typeof( MockObject ).AssemblyQualifiedName );
			poMockObject.New( new Type[] { typeof( object ) }, new object[] { } );
			poMockObject = new PrivateObject( typeof( MockObject ) );
			poMockObject.New( new Type[] { typeof( object ) }, new object[] { } );

			poMockObject = new PrivateObject( typeof( String ).AssemblyQualifiedName, new Type[] { } );
			poMockObject.New( new Type[] { typeof( String ) }, new object[] { } );
			poMockObject = new PrivateObject( typeof( String ), new Type[] { } );
			poMockObject.New( new Type[] { typeof( String ) }, new object[] { } );
		}

		[Test( "Test to Fail the Constructors on the PrivateObject" )]
		[ExpectedExceptionCount( typeof( ArgumentNullException ), 15 )]
		public void ConstructorTestToFailArgumentNullExceptionTest()
		{
			PrivateObject poMockObject;
			poMockObject = new PrivateObject( (object)null );

			poMockObject = new PrivateObject( (string)null );
			poMockObject.New( new object[] { } );
			poMockObject = new PrivateObject( (Type)null );
			poMockObject.New( new object[] { } );

			poMockObject = new PrivateObject( (string)null );
			poMockObject.New( new Type[] { }, new object[] { } );
			poMockObject = new PrivateObject( (Type)null );
			poMockObject.New( new Type[] { }, new object[] { } );

			poMockObject = new PrivateObject( (string)null, new Type[] { } );
			poMockObject.New( new Type[] { }, new object[] { } );
			poMockObject = new PrivateObject( (Type)null, new Type[] { } );
			poMockObject.New( new Type[] { }, new object[] { } );

			poMockObject = new PrivateObject( typeof( String ).AssemblyQualifiedName );
			poMockObject.New( null, new object[] { } );
			poMockObject = new PrivateObject( typeof( String ) );
			poMockObject.New( null, new object[] { } );

			poMockObject = new PrivateObject( typeof( String ).AssemblyQualifiedName );
			poMockObject.New( new Type[] { }, null );
			poMockObject = new PrivateObject( typeof( String ) );
			poMockObject.New( new Type[] { }, null );

			poMockObject = new PrivateObject( typeof( String ).AssemblyQualifiedName, new Type[] { } );
			poMockObject.New( null, new object[] { } );
			poMockObject = new PrivateObject( typeof( String ), new Type[] { } );
			poMockObject.New( null, new object[] { } );

			poMockObject = new PrivateObject( typeof( String ).AssemblyQualifiedName, (Type[])null );
			poMockObject.New( new Type[] { typeof( String ) }, new object[] { } );
			poMockObject = new PrivateObject( typeof( String ), (Type[])null );
			poMockObject.New( new Type[] { typeof( String ) }, new object[] { } );
		}

		[Test( "Test to Fail the Constructors on the PrivateObject" )]
		[ExpectedExceptionCount( typeof( ArgumentException ), 2 )]
		public void ConstructorTestToFailArgumentExceptionTest()
		{
			PrivateObject poMockObject;
			poMockObject = new PrivateObject( typeof( MockObject ).AssemblyQualifiedName );
			poMockObject.New( new object[] { null } );
			poMockObject = new PrivateObject( typeof( MockObject ) );
			poMockObject.New( new object[] { null } );
		}

		[Test( "Test to Fail the Constructors on the PrivateObject" )]
		[ExpectedExceptionCount( typeof( InvalidOperationException ), 2 )]
		public void ConstructorTestToFailInvalidOperationExceptionTest()
		{
			PrivateObject poMockObject;
			poMockObject = new PrivateObject( typeof( String ).AssemblyQualifiedName, new Type[] { } );
			poMockObject.New( new Type[] { }, new object[] { } );
			poMockObject = new PrivateObject( typeof( String ), new Type[] { } );
			poMockObject.New( new Type[] { }, new object[] { } );
		}

		[Test( "Test GetField on the PrivateObject" )]
		public void GetFieldTest()
		{
			PrivateObject poMockObject = new PrivateObject( typeof( MockObject ) );
			Assert.IsNull( poMockObject.GetField( "fieldInternal" ) );
			Assert.IsNull( poMockObject.GetField( "fieldProtected" ) );
			Assert.IsNull( poMockObject.GetField( "fieldProtectedInternal" ) );
			Assert.IsNull( poMockObject.GetField( "fieldPublic" ) );
			Assert.IsNull( poMockObject.GetField( "fieldPrivate" ) );

			poMockObject = new PrivateObject( typeof( MockGenericObject<> ), new Type[] { typeof( string ) } );
			poMockObject.New( new Type[] { } );
			Assert.IsNull( poMockObject.GetField( "fieldInternal" ) );
			Assert.IsNull( poMockObject.GetField( "fieldProtected" ) );
			Assert.IsNull( poMockObject.GetField( "fieldProtectedInternal" ) );
			Assert.IsNull( poMockObject.GetField( "fieldPublic" ) );
			Assert.IsNull( poMockObject.GetField( "fieldPrivate" ) );

			poMockObject = new PrivateObject( typeof( MockStaticObject ) );
			Assert.IsNull( poMockObject.GetField( "fieldInternal" ) );
			Assert.IsNull( poMockObject.GetField( "fieldPublic" ) );
			Assert.IsNull( poMockObject.GetField( "fieldPrivate" ) );

			poMockObject = new PrivateObject( typeof( MockStaticGenericObject<> ), new Type[] { typeof( string ) } );
			poMockObject.New( new Type[] { } );
			Assert.IsNull( poMockObject.GetField( "fieldInternal" ) );
			Assert.IsNull( poMockObject.GetField( "fieldPublic" ) );
			Assert.IsNull( poMockObject.GetField( "fieldPrivate" ) );
		}

		[Test( "Test to Fail GetField on the PrivateObject" )]
		[ExpectedExceptionCount( typeof( ArgumentNullException ), 4 )]
		public void GetFieldArgumentTest()
		{
			PrivateObject poMockObject = new PrivateObject( new MockObject() );
			poMockObject.GetField( null );
			poMockObject.GetField( "" );
			poMockObject.GetFieldValue( null );
			poMockObject.GetFieldValue( "" );
			poMockObject.GetStaticFieldValue( null );
			poMockObject.GetStaticFieldValue( "" );
		}

		[Test( "Test to Fail GetField on the PrivateObject" )]
		[ExpectedException( typeof( MissingFieldException ) )]
		public void GetMissingFieldTestToFailTest()
		{
			PrivateObject poMockObject = new PrivateObject( new MockObject() );
			poMockObject.GetField( "DoesNotExist" );
		}

		[Test( "Test GetProperty on the PrivateObject" )]
		public void GetPropertyTest()
		{
			PrivateObject poMockObject = new PrivateObject( typeof( MockObject ) );
			Assert.IsNull( poMockObject.GetProperty( "PropertyInternal" ) );
			Assert.IsNull( poMockObject.GetProperty( "PropertyProtected" ) );
			Assert.IsNull( poMockObject.GetProperty( "PropertyProtectedInternal" ) );
			Assert.IsNull( poMockObject.GetProperty( "PropertyPublic" ) );
			Assert.IsNull( poMockObject.GetProperty( "PropertyPrivate" ) );

			poMockObject = new PrivateObject( typeof( MockGenericObject<> ), new Type[] { typeof( string ) } );
			poMockObject.New( new Type[] { } );
			Assert.IsNull( poMockObject.GetProperty( "PropertyInternal" ) );
			Assert.IsNull( poMockObject.GetProperty( "PropertyProtected" ) );
			Assert.IsNull( poMockObject.GetProperty( "PropertyProtectedInternal" ) );
			Assert.IsNull( poMockObject.GetProperty( "PropertyPublic" ) );
			Assert.IsNull( poMockObject.GetProperty( "PropertyPrivate" ) );

			poMockObject = new PrivateObject( typeof( MockStaticObject ) );
			Assert.IsNull( poMockObject.GetProperty( "PropertyInternal" ) );
			Assert.IsNull( poMockObject.GetProperty( "PropertyPublic" ) );
			Assert.IsNull( poMockObject.GetProperty( "PropertyPrivate" ) );

			poMockObject = new PrivateObject( typeof( MockStaticGenericObject<> ), new Type[] { typeof( string ) } );
			poMockObject.New( new Type[] { } );
			Assert.IsNull( poMockObject.GetProperty( "PropertyInternal" ) );
			Assert.IsNull( poMockObject.GetProperty( "PropertyPublic" ) );
			Assert.IsNull( poMockObject.GetProperty( "PropertyPrivate" ) );
		}

		[Test( "Test to Fail GetProperty on the PrivateObject" )]
		[ExpectedExceptionCount( typeof( ArgumentException ), 4 )]
		public void GetPropertyArgumentTest()
		{
			PrivateObject poMockObject = new PrivateObject( new MockObject() );
			poMockObject.GetProperty( null );
			poMockObject.GetProperty( "" );
			poMockObject.GetPropertyValue( null );
			poMockObject.GetPropertyValue( "" );
			poMockObject.GetStaticPropertyValue( null );
			poMockObject.GetStaticPropertyValue( "" );
		}

		[Test( "Test to Fail GetProperty on the PrivateObject" )]
		[ExpectedException( typeof( MissingMemberException ) )]
		public void GetMissingPropertyToFailTest()
		{
			PrivateObject poMockObject = new PrivateObject( new MockObject() );
			poMockObject.GetProperty( "DoesNotExist" );
		}

		[Test( "Test SetField on the PrivateObject" )]
		public void SetFieldTest()
		{
			PrivateObject poMockObject = new PrivateObject( typeof( MockObject ) );
			Assert.IsNull( poMockObject.GetField( "fieldInternal" ) );
			poMockObject.SetFieldValue( "fieldInternal", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetField( "fieldInternal" ) );

			Assert.IsNull( poMockObject.GetField( "fieldProtected" ) );
			poMockObject.SetFieldValue( "fieldProtected", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetField( "fieldProtected" ) );

			Assert.IsNull( poMockObject.GetField( "fieldProtectedInternal" ) );
			poMockObject.SetFieldValue( "fieldProtectedInternal", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetField( "fieldProtectedInternal" ) );

			Assert.IsNull( poMockObject.GetField( "fieldPublic" ) );
			poMockObject.SetFieldValue( "fieldPublic", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetField( "fieldPublic" ) );

			Assert.IsNull( poMockObject.GetField( "fieldPrivate" ) );
			poMockObject.SetFieldValue( "fieldPrivate", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetField( "fieldPrivate" ) );

			poMockObject = new PrivateObject( typeof( MockGenericObject<> ), new Type[] { typeof( string ) } );
			poMockObject.New( new Type[] { } );
			Assert.IsNull( poMockObject.GetField( "fieldInternal" ) );
			poMockObject.SetFieldValue( "fieldInternal", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetField( "fieldInternal" ) );

			Assert.IsNull( poMockObject.GetField( "fieldProtected" ) );
			poMockObject.SetFieldValue( "fieldProtected", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetField( "fieldProtected" ) );

			Assert.IsNull( poMockObject.GetField( "fieldProtectedInternal" ) );
			poMockObject.SetFieldValue( "fieldProtectedInternal", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetField( "fieldProtectedInternal" ) );

			Assert.IsNull( poMockObject.GetField( "fieldPublic" ) );
			poMockObject.SetFieldValue( "fieldPublic", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetField( "fieldPublic" ) );

			Assert.IsNull( poMockObject.GetField( "fieldPrivate" ) );
			poMockObject.SetFieldValue( "fieldPrivate", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetField( "fieldPrivate" ) );

			poMockObject = new PrivateObject( typeof( MockStaticObject ) );
			Assert.IsNull( poMockObject.GetField( "fieldInternal" ) );
			poMockObject.SetFieldValue( "fieldInternal", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetField( "fieldInternal" ) );
			poMockObject.SetFieldValue( "fieldInternal", null );

			Assert.IsNull( poMockObject.GetField( "fieldPublic" ) );
			poMockObject.SetFieldValue( "fieldPublic", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetField( "fieldPublic" ) );
			poMockObject.SetFieldValue( "fieldPublic", null );

			Assert.IsNull( poMockObject.GetField( "fieldPrivate" ) );
			poMockObject.SetFieldValue( "fieldPrivate", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetField( "fieldPrivate" ) );
			poMockObject.SetFieldValue( "fieldPrivate", null );

			poMockObject = new PrivateObject( typeof( MockStaticGenericObject<> ), new Type[] { typeof( string ) } );
			poMockObject.New( new Type[] { } );
			Assert.IsNull( poMockObject.GetField( "fieldInternal" ) );
			poMockObject.SetFieldValue( "fieldInternal", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetField( "fieldInternal" ) );
			poMockObject.SetFieldValue( "fieldInternal", null );

			Assert.IsNull( poMockObject.GetField( "fieldPublic" ) );
			poMockObject.SetFieldValue( "fieldPublic", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetField( "fieldPublic" ) );
			poMockObject.SetFieldValue( "fieldPublic", null );

			Assert.IsNull( poMockObject.GetField( "fieldPrivate" ) );
			poMockObject.SetFieldValue( "fieldPrivate", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetField( "fieldPrivate" ) );
			poMockObject.SetFieldValue( "fieldPrivate", null );

			Assert.IsNull( poMockObject.GetField( "fieldPrivateStatic" ) );
			poMockObject.SetFieldValue( "fieldPrivate", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetStaticFieldValue( "fieldPrivateStatic" ) );
			poMockObject.SetStaticFieldValue( "fieldPrivateStatic", null );
		}

		[Test( "Test to Fail SetField on the PrivateObject" )]
		[ExpectedExceptionCount( typeof( ArgumentException ), 2 )]
		public void SetFieldTestToFailTest()
		{
			PrivateObject poMockObject = new PrivateObject( new MockObject() );
			poMockObject.SetFieldValue( null, null );
			poMockObject.SetFieldValue( "", null );
		}

		[Test( "Test to Fail SetField on the PrivateObject" )]
		[ExpectedException( typeof( MissingFieldException ) )]
		public void SetMissingFieldTestToFailTest()
		{
			PrivateObject poMockObject = new PrivateObject( new MockObject() );
			poMockObject.SetFieldValue( "DoesNotExist", null );
		}

		[Test( "Test SetProperty on the PrivateObject" )]
		public void SetPropertyTest()
		{
			PrivateObject poMockObject = new PrivateObject( typeof( MockObject ) );
			Assert.IsNull( poMockObject.GetProperty( "PropertyInternal" ) );
			poMockObject.SetPropertyValue( "PropertyInternal", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetProperty( "PropertyInternal" ) );

			Assert.IsNull( poMockObject.GetProperty( "PropertyProtected" ) );
			poMockObject.SetPropertyValue( "PropertyProtected", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetProperty( "PropertyProtected" ) );

			Assert.IsNull( poMockObject.GetProperty( "PropertyProtectedInternal" ) );
			poMockObject.SetPropertyValue( "PropertyProtectedInternal", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetProperty( "PropertyProtectedInternal" ) );

			Assert.IsNull( poMockObject.GetProperty( "PropertyPublic" ) );
			poMockObject.SetPropertyValue( "PropertyPublic", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetProperty( "PropertyPublic" ) );

			Assert.IsNull( poMockObject.GetProperty( "PropertyPrivate" ) );
			poMockObject.SetPropertyValue( "PropertyPrivate", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetProperty( "PropertyPrivate" ) );

			poMockObject = new PrivateObject( typeof( MockGenericObject<> ), new Type[] { typeof( string ) } );
			poMockObject.New( new Type[] { } );
			Assert.IsNull( poMockObject.GetProperty( "PropertyInternal" ) );
			poMockObject.SetPropertyValue( "PropertyInternal", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetProperty( "PropertyInternal" ) );

			Assert.IsNull( poMockObject.GetProperty( "PropertyProtected" ) );
			poMockObject.SetPropertyValue( "PropertyProtected", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetProperty( "PropertyProtected" ) );

			Assert.IsNull( poMockObject.GetProperty( "PropertyProtectedInternal" ) );
			poMockObject.SetPropertyValue( "PropertyProtectedInternal", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetProperty( "PropertyProtectedInternal" ) );

			Assert.IsNull( poMockObject.GetProperty( "PropertyPublic" ) );
			poMockObject.SetPropertyValue( "PropertyPublic", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetProperty( "PropertyPublic" ) );

			Assert.IsNull( poMockObject.GetProperty( "PropertyPrivate" ) );
			poMockObject.SetPropertyValue( "PropertyPrivate", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetProperty( "PropertyPrivate" ) );

			poMockObject = new PrivateObject( typeof( MockStaticObject ) );
			Assert.IsNull( poMockObject.GetProperty( "PropertyInternal" ) );
			poMockObject.SetPropertyValue( "PropertyInternal", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetProperty( "PropertyInternal" ) );

			Assert.IsNull( poMockObject.GetProperty( "PropertyPublic" ) );
			poMockObject.SetPropertyValue( "PropertyPublic", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetProperty( "PropertyPublic" ) );

			Assert.IsNull( poMockObject.GetProperty( "PropertyPrivate" ) );
			poMockObject.SetPropertyValue( "PropertyPrivate", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetProperty( "PropertyPrivate" ) );

			poMockObject = new PrivateObject( typeof( MockStaticGenericObject<> ), new Type[] { typeof( string ) } );
			poMockObject.New( new Type[] { } );
			Assert.IsNull( poMockObject.GetProperty( "PropertyInternal" ) );
			poMockObject.SetPropertyValue( "PropertyInternal", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetProperty( "PropertyInternal" ) );

			Assert.IsNull( poMockObject.GetProperty( "PropertyPublic" ) );
			poMockObject.SetPropertyValue( "PropertyPublic", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetProperty( "PropertyPublic" ) );

			Assert.IsNull( poMockObject.GetProperty( "PropertyPrivate" ) );
			poMockObject.SetPropertyValue( "PropertyPrivate", string.Empty );
			Assert.AreEqual( string.Empty, poMockObject.GetProperty( "PropertyPrivate" ) );
		}

		[Test( "Test to Fail SetProperty on the PrivateObject" )]
		[ExpectedExceptionCount( typeof( ArgumentException ), 2 )]
		public void SetPropertyToFailTest()
		{
			PrivateObject poMockObject = new PrivateObject( new MockObject() );
			poMockObject.SetPropertyValue( null, null );
			poMockObject.SetPropertyValue( "", null );
		}

		[Test( "Test to Fail SetProperty on the PrivateObject" )]
		[ExpectedException( typeof( MissingMemberException ) )]
		public void SetMissingPropertyToFailTest()
		{
			PrivateObject poMockObject = new PrivateObject( new MockObject() );
			poMockObject.SetPropertyValue( "DoesNotExist", null );
		}

		[Test( "Test Invoke on the PrivateObject" )]
		public void InvokeTest()
		{
			PrivateObject poMockObject = new PrivateObject( new MockObject() );
			poMockObject.Invoke( "Method" );
			poMockObject.Invoke( "MethodWithParameter", null );
			poMockObject.Invoke( "MethodWithParameter", "" );
			poMockObject.Invoke( "MethodWithOverLoad", "" );
		}

		[Test( "Test To Fail Invoke on the PrivateObject" )]
		[ExpectedExceptionCount( typeof( ArgumentException ), 4 )]
		public void InvokeArgumentTest()
		{
			PrivateObject poMockObject = new PrivateObject( new MockObject() );
			poMockObject.Invoke( null );
			poMockObject.Invoke( "" );
			poMockObject.InvokeStatic( null );
			poMockObject.InvokeStatic( "" );
		}

		[Test( "Test To Fail Invoke on the PrivateObject" )]
		[ExpectedException( typeof( MissingMethodException ) )]
		public void InvokeTestToFailMissingMethodTest()
		{
			PrivateObject poMockObject = new PrivateObject( new MockObject() );
			poMockObject.Invoke( "DoesNotExist" );
		}

		[Test( "Test To Fail Invoke on the PrivateObject" )]
		[ExpectedException( typeof( MissingMethodException ) )]
		public void InvokeTestToFailAmbiguousMethodTest()
		{
			PrivateObject poMockObject = new PrivateObject( new MockObject() );
			poMockObject.Invoke( "MethodWithOverLoad" );
		}

		[Test( "Test To Fail Invoke on the PrivateObject" )]
		[ExpectedException( typeof( AmbiguousMatchException ) )]
		public void InvokeTestToFailAmbiguousMatchExceptionTest()
		{
			PrivateObject poMockObject = new PrivateObject( new MockObject() );
			poMockObject.Invoke( "MethodWithOverLoad", null );
		}

		[Test( "Test To Fail Invoke on the PrivateObject" )]
		[ExpectedException( typeof( MockException ) )]
		public void InvokeTestToFailTargetInvocationExceptionTest()
		{
			PrivateObject poMockObject = new PrivateObject( new MockObject() );
			poMockObject.Invoke( "MethodThrowsException" );
		}

		[Test( "Test InvokeExplict on the PrivateObject" )]
		public void InvokeExplictTest()
		{
			PrivateObject poMockObject = new PrivateObject( new MockObject() );
			poMockObject.InvokeExplicit( "Method", new Type[] { } );
			poMockObject.InvokeExplicit( "MethodWithParameter", new Type[] { typeof( string ) }, null );
			poMockObject.InvokeExplicit( "MethodWithParameter", new Type[] { typeof( string ) }, "" );
			poMockObject.InvokeExplicit( "MethodWithOverLoad", new Type[] { typeof( string ) }, "" );
		}

		[Test( "Test To Fail InvokeExplict on the PrivateObject" )]
		[ExpectedExceptionCount( typeof( ArgumentException ), 4 )]
		public void InvokeExplictArgumentTest()
		{
			PrivateObject poMockObject = new PrivateObject( new MockObject() );
			poMockObject.InvokeExplicit( null, new Type[] { } );
			poMockObject.InvokeExplicit( "", new Type[] { } );
			poMockObject.InvokeStaticExplicit( null, new Type[] { } );
			poMockObject.InvokeStaticExplicit( "", new Type[] { } );
		}

		[Test( "Test To Fail InvokeExplict on the PrivateObject" )]
		[ExpectedExceptionCount( typeof( ArgumentNullException ), 6 )]
		public void InvokeExplictArgumentNullTest()
		{
			PrivateObject poMockObject = new PrivateObject( new MockObject() );
			poMockObject.InvokeExplicit( null, null );
			poMockObject.InvokeExplicit( "", null );
			poMockObject.InvokeExplicit( "Method", null );
			poMockObject.InvokeStaticExplicit( null, null );
			poMockObject.InvokeStaticExplicit( "", null );
			poMockObject.InvokeStaticExplicit( "Method", null );
		}

		[Test( "Test To Fail InvokeExplict on the PrivateObject" )]
		[ExpectedException( typeof( MissingMethodException ) )]
		public void InvokeExplictTestToFailMissingMethodTest()
		{
			PrivateObject poMockObject = new PrivateObject( new MockObject() );
			poMockObject.InvokeExplicit( "DoesNotExist", new Type[] { } );
		}

		[Test( "Test To Fail InvokeExplict on the PrivateObject" )]
		[ExpectedException( typeof( MockException ) )]
		public void InvokeExplictTestToFailTargetInvocationExceptionTest()
		{
			PrivateObject poMockObject = new PrivateObject( new MockObject() );
			poMockObject.InvokeExplicit( "MethodThrowsException", new Type[] { } );
		}
	}
}
