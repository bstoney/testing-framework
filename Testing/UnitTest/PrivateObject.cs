using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Testing.UnitTest
{

	/// <summary>
	/// This utility class uses reflection to wrap an instance of a
	/// class to gain access to non-public members of that class.
	/// This is an internal utility class used for unit testing.
	/// </summary>
	public sealed class PrivateObject
	{
		/// <summary>
		/// Construct a new PrivateObject instance initialised with an instance of the object to examine.
		/// </summary>
		public PrivateObject( object instance )
		{
			if( instance == null )
			{
				throw new ArgumentNullException( "instance" );
			}
			throw new NotImplementedException();
		}

		/// <summary>
		/// Construct a new PrivateObject instance for the object type.
		/// </summary>
		public PrivateObject( string objectType )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Construct a new PrivateObject instance for the object type.
		/// </summary>
		public PrivateObject( Type objectType )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Construct a new PrivateObject instance for the object type with the supplied generic parameters.
		/// </summary>
		public PrivateObject( string objectType, Type[] genericParameters )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Construct a new PrivateObject instance for the object type with the supplied generic parameters.
		/// </summary>
		public PrivateObject( Type objectType, Type[] genericParameters )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the object type which will be access.
		/// </summary>
		public Type Type { get; private set; }

		/// <summary>
		/// Gets the instance of the object which will be accessed.
		/// </summary>
		public object Instance { get; private set; }

		/// <summary>
		/// Creates, sets and returns the instance of the object type using the supplied arguments.
		/// </summary>
		public object New( params object[] arguments )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates, sets and returns the instance of the object type using the supplied arguments.
		/// </summary>
		public object New( Type[] argumentTypes, params object[] arguments )
		{
			throw new NotImplementedException();
		}

		#region Field Manipulation

		/// <summary>
		/// Gets the info about a field.
		/// </summary>
		public FieldInfo GetField( string fieldName )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the value of a field.
		/// </summary>
		public object GetFieldValue( string fieldName )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the value of a static field.
		/// </summary>
		public object GetStaticFieldValue( string fieldName )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sets the value of a field.
		/// </summary>
		public void SetFieldValue( string fieldName, object value )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sets the value of a static field.
		/// </summary>
		public void SetStaticFieldValue( string fieldName, object value )
		{
			throw new NotImplementedException();
		}

		#endregion Field Manipulation

		#region Property Manipulation

		/// <summary>
		/// Gets the info about a property.
		/// </summary>
		public PropertyInfo GetProperty( string propertyName )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the value of a property.
		/// </summary>
		public object GetPropertyValue( string propertyName )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the value of a static property.
		/// </summary>
		public object GetStaticPropertyValue( string propertyName )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sets the value of a property.
		/// </summary>
		public void SetPropertyValue( string propertyName, object value )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sets the value of a static property.
		/// </summary>
		public void SetStaticPropertyValue( string propertyName, object value )
		{
			throw new NotImplementedException();
		}

		#endregion Property Manipulation

		#region Method Invokation

		/// <summary>
		/// Invokes a method on the instance of the object.
		/// </summary>
		public object Invoke( string methodName, params object[] args )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Invokes a static method on the object type.
		/// </summary>
		public object InvokeStatic( string methodName, params object[] args )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Invokes a method on the instance of the object.
		/// </summary>
		public object InvokeExplicit( string methodName, params object[] args )
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Invokes a static method on the object type.
		/// </summary>
		public object InvokeStaticExplicit( string methodName, params object[] args )
		{
			throw new NotImplementedException();
		}

		#endregion Method Invokation
	}
}
