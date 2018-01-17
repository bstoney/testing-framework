using System;
using System.Collections.Generic;
using System.Text;

namespace Testing
{
	/// <summary>
	/// An attribute used to indicate a test fixture tear down method.
	/// </summary>
	[AttributeUsage( AttributeTargets.Method )]
	public sealed class TestFixtureTearDownAttribute : Attribute
	{
	}
}
