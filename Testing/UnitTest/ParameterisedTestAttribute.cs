using System;
using System.Collections.Generic;
using System.Text;
using Testing.TestRunner;
using System.Reflection;

namespace Testing.UnitTest
{
	/// <summary>
	/// Runs a test multiple times passing arguments defined by TestParametersAttribute attributes.
	/// </summary>
	[AttributeUsage( AttributeTargets.Method )]
	public sealed class ParameterisedTestAttribute : Attribute, ITestRunner
	{
		#region ITestRunner Members

		/// <summary>
		/// This method is called by the test object to invoke the test.
		/// </summary>
		/// <param name="test">The calling test object.</param>
		public void RunTest( ITest test )
		{
			// TODO use an ITestSuiteBuilder, IFixtureBuilder and ITestBuilder instead of a builder factory
			// this would allow more granular builder definitions.
			test.Result.Status = TestStatus.Pass;
			TestTimer timer = new TestTimer();

			List<object[]> parameters = new List<object[]>();
			TestParametersAttribute[] tpas = test.TestMethod.GetCustomAttributes(
				typeof( TestParametersAttribute ), false ) as TestParametersAttribute[];
			if( tpas != null )
			{
				for( int i = 0; i < tpas.Length; i++ )
				{
					parameters.AddRange( tpas[i].GetParameters( test ) );
				}
			}

			timer.Start( test );
			for( int i = 0; i < parameters.Count; i++ )
			{

				try
				{
					test.TestMethod.Invoke( test.Fixture.Instance, parameters[i] );
				}
				catch( TargetInvocationException tie )
				{
					Exception exp = tie.InnerException;
					test.Result.Status = TestStatus.Fail;
					StringBuilder sb = new StringBuilder();
					sb.AppendFormat( "Test run {0} failed with parameters: ", i + 1 );
					if( parameters[i] != null && parameters[i].Length > 0 )
					{
						sb.Append( AssertionFailureMessage.FormatObjectForDisplay( parameters[i][0] ) );
						for( int j = 1; j < parameters[i].Length; j++ )
						{
							sb.AppendFormat( ", {0}", AssertionFailureMessage.FormatObjectForDisplay( parameters[i][j] ) );
						}
					}
					test.Result.Message.AppendLine( sb.ToString() );
					test.Result.Message.AppendLine( exp.Message );
					test.Result.Message.Append( "EXCEPTION TYPE: " );
					test.Result.Message.AppendLine( exp.GetType().FullName );
					if( test.Result.StackTrace == null )
					{
						test.Result.SetFilteredStackTrace( exp.StackTrace );
					}
				}
			}
			timer.Stop();
		}

		#endregion
	}
}
