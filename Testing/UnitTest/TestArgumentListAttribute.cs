using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Testing.TestRunner;

namespace Testing.UnitTest
{
	/// <summary>
	/// Provdes a mechinism for retrieving a list of test arguments at runtime.
	/// </summary>
	public class TestArgumentListAttribute : TestParametersAttribute
	{
		private TestArgumentListCallback _listCallback;
		private string _listCallbackMethodName;

		/// <summary>
		/// Constructor.
		/// </summary>
		public TestArgumentListAttribute( TestArgumentListCallback listCallback )
		{
			if( listCallback == null )
			{
				throw new ArgumentNullException( "listCallback" );
			}
			_listCallback = listCallback;
		}

		/// <summary>
		/// Creates a new TestArgumentListAttribute with a callback to an instance method on the test fixture.
		/// The method should be a valid TestArgumentListCallback.
		/// </summary>
		public TestArgumentListAttribute( string listCallbackMethodName )
		{
			if( listCallbackMethodName == null )
			{
				throw new ArgumentNullException( "listCallbackMethodName" );
			}
			_listCallbackMethodName = listCallbackMethodName;
		}

		/// <summary>
		/// Gets or sets the arguments to pass to a test.
		/// </summary>
		public override List<object[]> GetParameters( ITest test )
		{
			if( _listCallback == null )
			{
				MethodInfo method = test.Fixture.FixtureType.GetMethod( _listCallbackMethodName, 
					BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
				if( !method.IsStatic )
				{
					_listCallback = (TestArgumentListCallback)Delegate.CreateDelegate(
						typeof( TestArgumentListCallback ), test.Fixture.Instance, method );
				}
				else
				{
					_listCallback = (TestArgumentListCallback)Delegate.CreateDelegate(
						typeof( TestArgumentListCallback ), method );
				}
			}
			return _listCallback();
		}
	}
}
