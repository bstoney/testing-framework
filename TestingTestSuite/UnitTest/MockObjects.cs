using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace TestingTestSuite.UnitTest
{
	public class MockObject
	{
		#region Fields

		internal string fieldInternal;
		protected string fieldProtected;
		protected internal string fieldProtectedInternal;
		public string fieldPublic;
		private string fieldPrivate;
		private static string fieldPrivateStatic;

		#endregion

		#region Properties

		internal string PropertyInternal
		{
			get
			{
				return fieldInternal;
			}
			set
			{
				fieldInternal = value;
			}
		}

		private string PropertyPrivate
		{
			get
			{
				return fieldPrivate;
			}
			set
			{
				fieldPrivate = value;
			}
		}

		protected string PropertyProtected
		{
			get
			{
				return fieldProtected;
			}
			set
			{
				fieldProtected = value;
			}
		}

		public string PropertyPublic
		{
			get
			{
				return fieldPublic;
			}
			set
			{
				fieldPublic = value;
			}
		}

		protected internal string PropertyProtectedInternal
		{
			get
			{
				return fieldProtectedInternal;
			}
			set
			{
				fieldProtectedInternal = value;
			}
		}

		#endregion

		#region Constructors

		public MockObject() { }
		static MockObject() { }
		public MockObject( object Object ) { }
		public MockObject( object Object1, object Object2 ) { }

		#endregion

		#region Methods
		public void Method() { }
		public void MethodThrowsException() { throw new MockException(); }
		public void MethodWithGenericParameter<T>() { }
		public void MethodWithOverLoad( string String ) { }
		public void MethodWithOverLoad( StringBuilder stringBuilder ) { }
		public void MethodWithParameter( string String ) { }
		public StringBuilder MethodWithParameterAndReturn( StringBuilder stringBuilder ) { return stringBuilder; }
		#endregion
	}

	public class MockGenericObject<T>
	{
		#region Fields

		internal T fieldInternal;
		protected T fieldProtected;
		protected internal T fieldProtectedInternal;
		public T fieldPublic;
		private T fieldPrivate;

		#endregion

		#region Properties

		internal T PropertyInternal
		{
			get
			{
				return fieldInternal;
			}
			set
			{
				fieldInternal = value;
			}
		}

		private T PropertyPrivate
		{
			get
			{
				return fieldPrivate;
			}
			set
			{
				fieldPrivate = value;
			}
		}

		protected T PropertyProtected
		{
			get
			{
				return fieldProtected;
			}
			set
			{
				fieldProtected = value;
			}
		}

		public T PropertyPublic
		{
			get
			{
				return fieldPublic;
			}
			set
			{
				fieldPublic = value;
			}
		}

		protected internal T PropertyProtectedInternal
		{
			get
			{
				return fieldProtectedInternal;
			}
			set
			{
				fieldProtectedInternal = value;
			}
		}

		#endregion

		#region Constructors

		public MockGenericObject() { }
		static MockGenericObject() { }
		public MockGenericObject( object Object ) { }
		public MockGenericObject( object Object1, object Object2 ) { }

		#endregion
	}

	public static class MockStaticObject
	{
		#region Fields

		internal static string fieldInternal;
		public static string fieldPublic;
		private static string fieldPrivate;

		#endregion

		#region Properties

		internal static string PropertyInternal
		{
			get
			{
				return fieldInternal;
			}
			set
			{
				fieldInternal = value;
			}
		}

		private static string PropertyPrivate
		{
			get
			{
				return fieldPrivate;
			}
			set
			{
				fieldPrivate = value;
			}
		}

		public static string PropertyPublic
		{
			get
			{
				return fieldPublic;
			}
			set
			{
				fieldPublic = value;
			}
		}

		#endregion

		#region Constructors

		static MockStaticObject() { }

		#endregion
	}

	public static class MockStaticGenericObject<T>
	{
		#region Fields

		internal static T fieldInternal;
		public static T fieldPublic;
		private static T fieldPrivate;

		#endregion

		#region Properties

		internal static T PropertyInternal
		{
			get
			{
				return fieldInternal;
			}
			set
			{
				fieldInternal = value;
			}
		}

		private static T PropertyPrivate
		{
			get
			{
				return fieldPrivate;
			}
			set
			{
				fieldPrivate = value;
			}
		}

		public static T PropertyPublic
		{
			get
			{
				return fieldPublic;
			}
			set
			{
				fieldPublic = value;
			}
		}

		#endregion

		#region Constructors

		static MockStaticGenericObject() { }

		#endregion Constructors
	}


	public class MockException : Exception { }

}
