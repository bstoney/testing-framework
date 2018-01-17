using System;
using System.Collections.Generic;
using System.Text;

using Testing.TestRunner.DefaultRunner;
using Testing.TestRunner;

namespace Testing
{
	/// <summary>
	/// An attribute used to indicate a test.
	/// </summary>
	[AttributeUsage( AttributeTargets.Method )]
	public sealed class TestAttribute : Attribute
	{
		private string _description;

		/// <summary>
		/// Creates a new TestAttribute.
		/// </summary>
		public TestAttribute() { }

		/// <summary>
		/// Creates a new TestAttribute.
		/// </summary>
		/// <param name="description">A description of the test.</param>
		public TestAttribute( string description )
		{
			_description = description;
		}

		/// <summary>
		/// Gets or sets a description of the test.
		/// </summary>
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}
	}
}
