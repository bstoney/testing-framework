using System;
using System.Collections.Generic;
using System.Text;

namespace Testing
{
	/// <summary>
	/// An attribute used to defined a test teardown method.
	/// </summary>
	[AttributeUsage( AttributeTargets.Method )]
	public sealed class TearDownAttribute : Attribute
	{
	}
}
