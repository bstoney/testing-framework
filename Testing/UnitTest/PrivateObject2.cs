using System;
using System.Reflection;
using System.Security.Permissions;
using System.Collections.Generic;

namespace Testing.UnitTest
{
	/// <summary>
	/// This utility class uses reflection to wrap an instance of a
	/// class to gain access to non-public members of that class.
	/// This is an internal utility class used for unit testing.
	/// </summary>
	public class PrivateObject2
	{
		#region Constants and Static Fields
		/// <summary>
		/// BindingFalgs use to ensure we find all of the members of the type.
		/// </summary>
		private const BindingFlags Bindings = BindingFlags.Instance | BindingFlags.Static |
													BindingFlags.NonPublic | BindingFlags.Public;
		#endregion

		#region Fields
		/// <summary>
		/// Type of class to manage
		/// </summary>
		private Type _type;
		/// <summary>
		/// managed instance of type
		/// </summary>
		private object _instance;
		/// <summary>
		/// for non-public members
		/// </summary>
		private ReflectionPermission _perm;
		#endregion

		#region Constructors
		/// <summary>
		/// Takes an instance wrapping a new instance of the
		/// target type. One PrivateObject manages exactly one instance
		/// of a target instance.
		/// </summary>
		/// <param name="instance">
		/// The Instance.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// Instance can not be null.
		/// </exception>
		public PrivateObject2( object instance )
		{
			if( instance == null )
			{
				throw new ArgumentNullException( "instance" );
			}
			_type = instance.GetType();
			this._instance = instance;
		}

		/// <summary>
		/// Initializes a new instance wrapping a new instance of the
		/// target type. One PrivateObject manages exactly one instance
		/// of a target type.
		/// </summary>
		/// <param name="qualifiedTypeName">
		/// The qualified name of the type.
		/// This should include the full assembly qualified name including
		/// the namespace, for example "MyNamespace.MyType,MyAssemblyBaseName"
		/// where MyNamespace is the dotted-notation namespace, MyType is the
		/// name of the type and MyAssemblyBaseName is the base name of the
		/// assembly containing the type.
		/// </param>
		/// <param name="args">
		/// An optional array of parameters to pass to
		/// the constructor. If this argument is not specified then the
		/// default constructor is used. Otherwise, a constructor that
		/// matches the number and type of parameters is used.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// qualifiedTypeName can not be null or empty.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// args can not contain members that are null.
		/// </exception>
		/// <exception cref="System.Reflection.TargetParameterCountException">
		/// the length of args must match the number of parameters the constructor expects.
		/// </exception>
		/// <exception cref="System.Reflection.TargetInvocationException">
		/// the constructor may fail and the ConstructorInfo.Invoke will throw this exception.
		/// </exception>
		public PrivateObject2( string qualifiedTypeName, params object[] args )
			: this( Type.GetType( qualifiedTypeName ), args ) { }

		/// <summary>
		/// Initializes a new instance wrapping a new instance of the
		/// target type. One PrivateObject manages exactly one instance
		/// of a target type.
		/// </summary>
		/// <param name="type">
		/// The type.
		/// </param>
		/// <param name="args">
		/// An optional array of parameters to pass to
		/// the constructor. If this argument is not specified then the
		/// default constructor is used. Otherwise, a constructor that
		/// matches the number and type of parameters is used.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// qualifiedTypeName can not be null or empty.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// args can not contain members that are null.
		/// </exception>
		/// <exception cref="System.Reflection.TargetParameterCountException">
		/// the length of args must match the number of parameters the constructor expects.
		/// </exception>
		/// <exception cref="System.Reflection.TargetInvocationException">
		/// the constructor may fail and the ConstructorInfo.Invoke will throw this exception.
		/// </exception>
		public PrivateObject2( Type type, params object[] args )
		{
			if( type == null )
			{
				throw new ArgumentNullException( "type" );
			}

			ConstructorInfo constructorInfo;
			_perm = new ReflectionPermission( PermissionState.Unrestricted );
			_perm.Demand();

			_type = type;

			//Build the types array to use to find the ConstructorInfo
			Type[] types = new Type[args.Length];
			for( int i = 0; i < args.Length; i++ )
			{
				if( args[i] != null )
				{
					types[i] = args[i].GetType();
				}
				else
				{
					throw new ArgumentException( "Member " + i + " of args can not be null.", "args[" + i + "]" );
				}
			}

			//Find the ConstructorInfo
			try
			{
				constructorInfo = Type.GetConstructor( Bindings, null, types, null );
			}
			catch( AmbiguousMatchException )
			{
				//Exclude Static Constructor at this time
				constructorInfo = Type.GetConstructor( Bindings & ~BindingFlags.Static, null, types, null );
			}

			//Invoke the ConstructorInfo
			if( constructorInfo != null && !constructorInfo.IsStatic )
			{
				_instance = constructorInfo.Invoke( args );
			}
			else
			{
				//Don't need to keep an instance of a static type
				_instance = null;
			}
		}

		/// <summary>
		/// Initializes a new instance wrapping a new instance of the
		/// target type. One PrivateObject manages exactly one instance
		/// of a target type.
		/// </summary>
		/// <param name="qualifiedTypeName">
		/// The qualified name of the type.
		/// This should include the full assembly qualified name including
		/// the namespace, for example "MyNamespace.MyType,MyAssemblyBaseName"
		/// where MyNamespace is the dotted-notation namespace, MyType is the
		/// name of the type and MyAssemblyBaseName is the base name of the
		/// assembly containing the type.
		/// </param>
		/// <param name="argTypes">
		/// An array of the Types of the parameters to pass to the constructor.
		/// The number of Types listed must match the number of parameters provided.
		/// </param>
		/// <param name="args">
		/// An optional array of parameters to pass to
		/// the constructor. If this argument is not specified then the
		/// default constructor is used. Otherwise, a constructor that
		/// matches the number and type of parameters is used.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// type can not be null. argTypes can not be null.
		/// </exception>
		/// <exception cref="System.Reflection.TargetParameterCountException">
		/// the length of typeArgs must match the length of args.
		/// the length of args must match the number of parameters the constructor expects.
		/// </exception>
		/// <exception cref="System.Reflection.TargetInvocationException">
		/// the constructor may fail and the ConstructorInfo.Invoke will throw this exception.
		/// </exception>
		public PrivateObject2( string qualifiedTypeName, Type[] argTypes, params object[] args )
			: this( Type.GetType( qualifiedTypeName ), argTypes, args ) { }

		/// <summary>
		/// Initializes a new instance wrapping a new instance of the
		/// target type. One PrivateObject manages exactly one instance
		/// of a target type.
		/// </summary>
		/// <param name="type">
		/// The Type.
		/// </param>
		/// <param name="argTypes">
		/// An array of the Types of the parameters to pass to the constructor.
		/// The number of Types listed must match the number of parameters provided.
		/// </param>
		/// <param name="args">
		/// An optional array of parameters to pass to
		/// the constructor. If this argument is not specified then the
		/// default constructor is used. Otherwise, a constructor that
		/// matches the number and type of parameters is used.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// qualifiedTypeName can not be null or empty. argTypes can not be null.
		/// </exception>
		/// <exception cref="System.Reflection.TargetParameterCountException">
		/// the length of typeArgs must match the length of args.
		/// the length of args must match the number of parameters the constructor expects.
		/// </exception>
		/// <exception cref="System.Reflection.TargetInvocationException">
		/// the constructor may fail and the ConstructorInfo.Invoke will throw this exception.
		/// </exception>
		public PrivateObject2( Type type, Type[] argTypes, params object[] args )
		{
			if( type == null )
				throw new ArgumentNullException( "type is null.", "type" );
			if( argTypes == null )
				throw new ArgumentNullException( "argTypes is null.", "argTypes" );
			if( args != null || argTypes.Length != 0 )
			{
				if( args.Length != argTypes.Length )
				{
					throw new TargetParameterCountException( "argTypes and args must be the same length." );
				}
			}

			ConstructorInfo constructorInfo;
			_perm = new ReflectionPermission( PermissionState.Unrestricted );
			_perm.Demand();

			_type = type;

			//Find the ConstructorInfo
			try
			{
				constructorInfo = Type.GetConstructor( Bindings, null, argTypes, null );
			}
			catch( AmbiguousMatchException )
			{
				//Exclude Static Constructor at this time
				constructorInfo = Type.GetConstructor( Bindings & ~BindingFlags.Static, null, argTypes, null );
			}

			//Invoke the ConstructorInfo
			if( constructorInfo != null && !constructorInfo.IsStatic )
			{
				_instance = constructorInfo.Invoke( args );
			}
			else
			{
				//Don't need to keep an instance of a static type
				_instance = null;
			}
		}

		/// <summary>
		/// Initializes a new instance wrapping a new instance of the
		/// target type. One PrivateObject manages exactly one instance
		/// of a target type.
		/// </summary>
		/// <param name="qualifiedTypeName">
		/// The qualified name of the type.
		/// This should include the full assembly qualified name including
		/// the namespace, for example "MyNamespace.MyType,MyAssemblyBaseName"
		/// where MyNamespace is the dotted-notation namespace, MyType is the
		/// name of the type and MyAssemblyBaseName is the base name of the
		/// assembly containing the type.
		/// </param>
		/// <param name="argTypes">
		/// An array of the Types of the parameters to pass to the constructor.
		/// The number of Types listed must match the number of parameters provided.
		/// </param>
		/// <param name="genericArgs">
		/// An array of the Types of the generic arguments required for the type.
		/// </param>
		/// <param name="args">
		/// An optional array of parameters to pass to
		/// the constructor. If this argument is not specified then the
		/// default constructor is used. Otherwise, a constructor that
		/// matches the number and type of parameters is used.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// qualifiedTypeName can not be null or empty. argTypes can not be null. genericArgs can not be null.
		/// </exception>
		/// <exception cref="System.Reflection.TargetParameterCountException">
		/// the length of typeArgs must match the length of args.
		/// the length of args must match the number of parameters the constructor expects.
		/// </exception>
		/// <exception cref="System.Reflection.TargetInvocationException">
		/// the constructor may fail and the ConstructorInfo.Invoke will throw this exception.
		/// </exception>
		public PrivateObject2( string qualifiedTypeName, Type[] argTypes, Type[] genericArgs, params object[] args )
			: this( Type.GetType( qualifiedTypeName ), argTypes, genericArgs, args ) { }

		/// <summary>
		/// Initializes a new instance wrapping a new instance of the
		/// target type. One PrivateObject manages exactly one instance
		/// of a target type.
		/// </summary>
		/// <param name="type">
		/// The Type.
		/// </param>
		/// <param name="argTypes">
		/// An array of the Types of the parameters to pass to the constructor.
		/// The number of Types listed must match the number of parameters provided.
		/// </param>
		/// <param name="genericArgs">
		/// An array of the Types of the generic arguments required for the type.
		/// </param>
		/// <param name="args">
		/// An optional array of parameters to pass to
		/// the constructor. If this argument is not specified then the
		/// default constructor is used. Otherwise, a constructor that
		/// matches the number and type of parameters is used.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// type can not be null. argTypes can not be null. genericArgs can not be null.
		/// </exception>
		/// <exception cref="System.Reflection.TargetParameterCountException">
		/// the length of typeArgs must match the length of args.
		/// the length of args must match the number of parameters the constructor expects.
		/// </exception>
		/// <exception cref="System.Reflection.TargetInvocationException">
		/// the constructor may fail and the ConstructorInfo.Invoke will throw this exception.
		/// </exception>
		public PrivateObject2( Type type, Type[] argTypes, Type[] genericArgs, params object[] args )
		{
			if( type == null )
				throw new ArgumentNullException( "type is null.", "type" );
			if( argTypes == null )
				throw new ArgumentNullException( "argTypes is null.", "argTypes" );
			if( genericArgs == null )
				throw new ArgumentNullException( "genericArgs is null.", "genericArgs" );
			if( args != null || argTypes.Length != 0 )
			{
				if( args.Length != argTypes.Length )
				{
					throw new TargetParameterCountException( "argTypes and args must be the same length." );
				}
			}

			ConstructorInfo constructorInfo;
			_perm = new ReflectionPermission( PermissionState.Unrestricted );
			_perm.Demand();

			_type = type.MakeGenericType( genericArgs );

			//Find the ConstructorInfo
			try
			{
				constructorInfo = Type.GetConstructor( Bindings, null, argTypes, null );
			}
			catch( AmbiguousMatchException )
			{
				//Exclude Static Constructor at this time
				constructorInfo = Type.GetConstructor( Bindings & ~BindingFlags.Static, null, argTypes, null );
			}

			//Invoke the ConstructorInfo
			if( constructorInfo != null && !constructorInfo.IsStatic )
			{
				_instance = constructorInfo.Invoke( args );
			}
			else
			{
				//Don't need to keep an instance of a static type
				_instance = null;
			}
		}
		#endregion // PrivateObject(instance)

		#region Properties

		/// <summary>
		/// Gets the instance of the managed object.
		/// </summary>
		public object Instance
		{
			get { return _instance; }
		}

		/// <summary>
		/// Gets the Type of the managed object.
		/// </summary>
		public Type Type
		{
			get { return _type; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets the value of a field (member variable) of the
		/// managed type.
		/// </summary>
		/// <param name="name">
		/// The name of the field to interrogate.
		/// </param>
		/// <returns>
		/// A value whose type is specific to the field.
		/// </returns>
		/// <exception cref="System.ArgumentException">
		/// name can not be null or empty.
		/// name must be a valid Field name.
		/// </exception>
		public object GetField( string name )
		{
			return GetField( Type, name ).GetValue( _instance );
		}

		/// <summary>
		/// Gets the value of a property of the
		/// managed type.
		/// </summary>
		/// <param name="name">
		/// The name of the property to interrogate.
		/// </param>
		/// <returns>
		/// A value whose type is specific to the property.
		/// </returns>
		/// <exception cref="System.ArgumentException">
		/// name can not be null or empty.
		/// name must be a valid Property name.
		/// </exception>
		public object GetProperty( string name )
		{
			return GetProperty( Type, name ).GetValue( _instance, null );
		}

		/// <summary>
		/// Invokes the method of the managed type.
		/// </summary>
		/// <param name="name">
		/// The name of the method to invoke.
		/// </param>
		/// <param name="args">
		/// And optional array of typed parameters to pass to the
		/// method.  If this argument is not specified then the routine searches
		/// for a method with a signature that contains not parameters.  Otherwise,
		/// the procedure searches for a method with the number and type of parameters
		/// specified.
		/// </param>
		/// <returns>
		/// A value who type is specific to the invoked method.
		/// </returns>
		/// <exception cref="System.ArgumentException">
		/// name can not be null or empty.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// No suitable method could be found.
		/// </exception>
		/// <exception cref="System.Reflection.AmbiguousMatchException">
		/// Too many matching methods could be found.
		/// </exception>
		public object Invoke( string name, params object[] args )
		{
			if( args == null )
			{
				args = new object[] { null };
			}
			try
			{
				return GetMethod( Type, name, args ).Invoke( _instance, args );
			}
			catch( TargetInvocationException tie )
			{
				throw tie.InnerException;
			}
		}

		/// <summary>
		/// Invokes the method of the managed type.
		/// </summary>
		/// <param name="name">
		/// The name of the method to invoke.
		/// </param>
		/// <param name="types">
		/// An Array of Types for the parameters to pass to the method.
		/// </param>
		/// <param name="args">
		/// And optional array of typed parameters to pass to the
		/// method.  If this argument is not specified then the routine searches
		/// for a method with a signature that contains not parameters.  Otherwise,
		/// the procedure searches for a method with the number and type of parameters
		/// specified.
		/// </param>
		/// <returns>
		/// A value who type is specific to the invoked method.
		/// </returns>
		/// <exception cref="System.ArgumentException">
		/// name can not be null or empty.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// types can not be null.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// No suitable method could be found.
		/// </exception>
		/// <exception cref="System.Reflection.AmbiguousMatchException">
		/// Too many matching methods could be found.
		/// </exception>
		public object InvokeExplict( string name, Type[] types, params object[] args )
		{
			if( types == null )
			{
				throw new ArgumentNullException( "types" );
			}
			if( args == null )
			{
				args = new object[] { null };
			}
			try
			{
				return GetMethodExplict( Type, name, types ).Invoke( _instance, args );
			}
			catch( TargetInvocationException tie )
			{
				throw tie.InnerException;
			}

		}

		/// <summary>
		/// Sets the value of a field (member variable) of the managed type.
		/// </summary>
		/// <param name="name">
		/// The name of the field to modify.
		/// </param>
		/// <param name="val">
		/// A value whose type is specific to the field.
		/// </param>
		/// <exception cref="System.ArgumentException">
		/// name can not be null or empty.
		/// name must be a valid Field name.
		/// </exception>
		public void SetField( string name, object val )
		{
			GetField( Type, name ).SetValue( _instance, val );
		}

		/// <summary>
		/// Sets the value of a property of the managed type.
		/// </summary>
		/// <param name="name">
		/// The name of the property to modify.
		/// </param>
		/// <param name="val">
		/// A value whose type is specific to the property.
		/// </param>
		/// <exception cref="System.ArgumentException">
		/// name can not be null or empty.
		/// name must be a valid Property name.
		/// </exception>
		public void SetProperty( string name, object val )
		{
			GetProperty( Type, name ).SetValue( _instance, val, null );
		}

		#endregion

		/// <summary>
		/// Gets a field of the type. An MissingFieldException is thrown if the field can not be found.
		/// </summary>
		public static FieldInfo GetField( Type objectType, string fieldName )
		{
			if( objectType == null )
			{
				throw new ArgumentNullException( "objectType" );
			}
			if( String.IsNullOrEmpty( fieldName ) )
			{
				throw new ArgumentException( "fieldName is null or empty.", "fieldName" );
			}

			FieldInfo fieldInfo = objectType.GetField( fieldName, Bindings );

			if( fieldInfo == null )
			{
				throw new MissingFieldException( String.Format( "Field '{0}' does not exist", fieldName ) );
			}

			return fieldInfo;
		}
		/// <summary>
		/// Gets a property of the type. An MissingMemberException is thrown if the property can not be found.
		/// </summary>
		public static PropertyInfo GetProperty( Type objectType, string propertyName )
		{
			if( objectType == null )
			{
				throw new ArgumentNullException( "objectType" );
			}
			if( String.IsNullOrEmpty( propertyName ) )
			{
				throw new ArgumentException( "propertyName is null or empty.", "propertyName" );
			}

			PropertyInfo propertyInfo = objectType.GetProperty( propertyName, Bindings );

			if( propertyInfo == null )
			{
				throw new MissingMemberException( String.Format( "Property '{0}' does not exist", propertyName ) );
			}

			return propertyInfo;
		}

		/// <summary>
		/// Gets a method of the type. An MissingMethodException is thrown if the method can not be found.
		/// </summary>
		public static MethodInfo GetMethod( Type objectType, string methodName, params object[] methodArguments )
		{
			Type[] argumentTypes;
			if( methodArguments == null )
			{
				argumentTypes = new Type[] { null };
			}
			else
			{
				// Load types from arguments.
				argumentTypes = (Type[])Array.CreateInstance( typeof( Type ), methodArguments.Length );
				for( int i = 0; i < methodArguments.Length; i++ )
				{
					argumentTypes[i] = (methodArguments[i] == null ? null : methodArguments[i].GetType());
				}
			}

			return GetMethodExplict( objectType, methodName, argumentTypes );
		}

		/// <summary>
		/// Gets a method of the type. An MissingMethodException is thrown if the method can not be found.
		/// </summary>
		public static MethodInfo GetMethodExplict( Type objectType, string methodName, params Type[] argumentTypes )
		{
			if( objectType == null )
			{
				throw new ArgumentNullException( "objectType" );
			}
			if( String.IsNullOrEmpty( methodName ) )
			{
				throw new ArgumentException( "methodName is null or empty.", "methodName" );
			}
			if( argumentTypes == null )
			{
				argumentTypes = new Type[] { null };
			}

			MemberFilter filter = delegate( MemberInfo member, Object criteria )
			{
				bool match = false;
				MethodInfo method = member as MethodInfo;
				if( method != null && method.Name == methodName )
				{
					ParameterInfo[] parameters = method.GetParameters();
					if( parameters.Length == argumentTypes.Length )
					{
						match = true;
						for( int j = 0; j < parameters.Length; j++ )
						{
							if( argumentTypes[j] != null && !parameters[j].ParameterType.IsAssignableFrom( argumentTypes[j] ) )
							{
								return false;
							}
						}
					}
				}
				return match;
			};

			MemberInfo[] members = objectType.FindMembers( MemberTypes.Method, Bindings, filter, null );

			if( members.Length == 1 )
			{
				return members[0] as MethodInfo;
			}
			else if( members.Length == 0 )
			{
				throw new MissingMethodException( String.Format( "Method '{0}' does not exist", methodName ) );
			}
			else
			{
				throw new AmbiguousMatchException( String.Format( "Miltible method matches where found for '{0}'", methodName ) );
			}
		}
	}
}
