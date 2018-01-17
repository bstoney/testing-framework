using System;
using System.Collections.Generic;
using System.Text;

namespace Testing
{
	/// <summary>
	/// An attribute used to indicate a test fixture setup method.
	/// </summary>
	[AttributeUsage( AttributeTargets.Method )]
	public sealed class TestFixtureSetUpAttribute : Attribute
	{
	}
}
