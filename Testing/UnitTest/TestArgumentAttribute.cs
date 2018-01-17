using System;
using System.Collections.Generic;
using System.Text;
using Testing.TestRunner;

namespace Testing.UnitTest
{
	/// <summary>
	/// Provides an argument list for a single test run.
	/// </summary>
	[AttributeUsage( AttributeTargets.Method, AllowMultiple = true )]
	public sealed class TestArgumentAttribute : TestParametersAttribute
	{
		private object[] _arguments;

		/// <summary>
		/// Constructor.
		/// </summary>
		public TestArgumentAttribute( params object[] arguments )
		{
			_arguments = arguments;
		}

		/// <summary>
		/// Gets the parameters to pass to a test.
		/// </summary>
		public override List<object[]> GetParameters( ITest test )
		{
			return new List<object[]>( new object[][] { _arguments } );
		}
	}
}
