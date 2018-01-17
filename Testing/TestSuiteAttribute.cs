using System;
using System.Collections.Generic;
using System.Text;
using Testing.TestRunner;

namespace Testing
{
	/// <summary>
	/// An attribute to define a test suite runner for a namespace.
	/// </summary>
	public sealed class TestSuiteAttribute : Attribute
	{
		private string _namespace;
		private Type _testSuiteRunner;

		/// <summary>
		/// Constructor.
		/// </summary>
		public TestSuiteAttribute( string namespacePath, Type testSuiteRunner )
		{
			Namespace = namespacePath;
			TestSuiteRunner = testSuiteRunner;
		}

		/// <summary>
		/// Gets or sets the namespace covered by the test suite runner.
		/// </summary>
		public string Namespace
		{
			get { return _namespace; }
			set
			{
				if( value == null )
				{
					throw new ArgumentNullException( "Namespace" );
				}
				_namespace = value;
			}
		}

		/// <summary>
		/// Gets or sets the test suite runner to use.
		/// </summary>
		public Type TestSuiteRunner
		{
			get { return _testSuiteRunner; }
			set
			{
				if( value == null )
				{
					throw new ArgumentNullException( "TestSuiteRunner" );
				}
				if( !typeof( ITestSuiteRunner ).IsAssignableFrom( value ) )
				{
					throw new InvalidOperationException( "The test suite runner must implement ITestSuiteRunner" );
				}
				_testSuiteRunner = value;
			}
		}

	}
}
