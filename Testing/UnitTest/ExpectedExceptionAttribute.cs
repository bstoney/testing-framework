using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Testing.TestRunner;
using Testing.Util.SymbolStore;

namespace Testing.UnitTest
{
	/// <summary>
	/// This test runner will execute the test method and catch any exception thrown. The test
	/// passes if an exception is thrown which exactly matches the expected type and message.
	/// </summary>
	[AttributeUsage( AttributeTargets.Method )]
	public sealed class ExpectedExceptionAttribute : Attribute, ITestRunner
	{
		#region Private Fields

		private Type _exceptionType;
		private string _message;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new ExpectedExceptionAttribute.
		/// </summary>
		/// <param name="exceptionType">The expected type of a thrown exception.</param>
		public ExpectedExceptionAttribute( Type exceptionType )
		{
			if( exceptionType == null )
			{
				throw new ArgumentNullException( "exceptionType is null.", "exceptionType" );
			}
			_exceptionType = exceptionType;
		}

		/// <summary>
		/// Creates a new ExpectedExceptionAttribute.
		/// </summary>
		/// <param name="exceptionTypeName">The expected type of a thrown exception.</param>
		public ExpectedExceptionAttribute( string exceptionTypeName )
			: this( Type.GetType( exceptionTypeName ) ) { }

		/// <summary>
		/// Creates a new ExpectedExceptionAttribute.
		/// </summary>
		/// <param name="exceptionType">The expected type of a thrown exception.</param>
		/// <param name="message">The expected exception message.</param>
		public ExpectedExceptionAttribute( Type exceptionType, string message )
		{
			if( exceptionType == null )
			{
				throw new ArgumentNullException( "exceptionType is null.", "exceptionType" );
			}
			_exceptionType = exceptionType;
			_message = message;
		}


		/// <summary>
		/// Creates a new ExpectedExceptionAttribute.
		/// </summary>
		/// <param name="exceptionTypeName">The expected type of a thrown exception.</param>
		/// <param name="message">The expected exception message.</param>
		public ExpectedExceptionAttribute( string exceptionTypeName, string message )
			: this( Type.GetType( exceptionTypeName ), message ) { }

		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the expected type of a thrown exception.
		/// </summary>
		public Type ExceptionType
		{
			get
			{
				return _exceptionType;
			}
			set
			{
				if( value == null )
				{
					throw new ArgumentNullException( "exceptionType is null.", "exceptionType" );
				}
				_exceptionType = value;
			}
		}

		/// <summary>
		/// Gets or sets the expected exception message.
		/// </summary>
		public string Message
		{
			get { return _message; }
			set { _message = value; }
		}

		#endregion

		#region ITestRunner Members

		/// <summary>
		/// Runn the test.
		/// </summary>
		public void RunTest( ITest test )
		{
			TestTimer timer = new TestTimer();
			try
			{
				timer.Start( test );
				test.TestMethod.Invoke( test.Fixture.Instance, null );
				timer.Stop();
			}
			catch( TargetInvocationException tie )
			{
				timer.Stop();
				Type thrownExceptionType = tie.InnerException.GetType();
				test.Result.Message.AppendFormat( "Expected Exception: {0}", ExceptionType.FullName );
				if( thrownExceptionType.Equals( ExceptionType ) && (Message == null || tie.InnerException.Message == Message) )
				{
					test.Result.Status = TestStatus.Pass;
					test.Result.Message.AppendLine( " was thrown." );
				}
				else
				{
					test.Result.Status = TestStatus.Fail;
					test.Result.Message.AppendLine( " was NOT thrown." );
				}
				test.Result.Message.Append( "Message: " );
				test.Result.Message.AppendLine( tie.InnerException.Message );
				test.Result.Message.Append( "Exception Type: " );
				test.Result.Message.AppendLine( thrownExceptionType.FullName );
				test.Result.SetFilteredStackTrace( tie.InnerException.StackTrace );
			}
			finally
			{
				timer.Stop();
				if( test.Result.Status == TestStatus.Untested )
				{
					// No exception has been thrown.

					test.Result.Status = TestStatus.Fail;
					test.Result.Message.AppendFormat( "Expected Exception: {0}", ExceptionType.FullName );
					test.Result.Message.AppendLine( " was NOT thrown." );
					SequenceManager sm = new SequenceManager( test.TestMethod );
					if( sm.IsSourceAvailable() )
					{
						test.Result.StackTrace = sm.GetStackTrace( 0 );
					}
				}
			}
		}

		#endregion
	}
}
