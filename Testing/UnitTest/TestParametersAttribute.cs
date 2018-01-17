using System;
using System.Collections.Generic;
using System.Text;
using Testing.TestRunner;

namespace Testing.UnitTest
{
	/// <summary>
	/// Provides a set of patameters to pass to a test.
	/// </summary>
	[AttributeUsage( AttributeTargets.Method )]
	public abstract class TestParametersAttribute : Attribute
	{
		/// <summary>
		/// Gets the parameters to pass to a test.
		/// </summary>
		public abstract List<object[]> GetParameters( ITest test );
	}
}
