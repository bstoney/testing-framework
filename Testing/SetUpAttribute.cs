using System;
using System.Collections.Generic;
using System.Text;

namespace Testing
{
	/// <summary>
	/// An attribute used to define a test setup method.
	/// </summary>
	[AttributeUsage( AttributeTargets.Method )]
	public sealed class SetUpAttribute : Attribute
	{
		// TODO add support for test specific setup routines.
	}
}
