using System;
using System.Collections.Generic;
using System.Text;

namespace Testing
{
	/// <summary>
	/// An attribute which defines a test fixture.
	/// </summary>
	[AttributeUsage( AttributeTargets.Class )]
	public sealed class TestFixtureAttribute : Attribute
	{
		private string _description;

		/// <summary>
		/// Creates a new TestFixtureAttribute.
		/// </summary>
		public TestFixtureAttribute() { }

		/// <summary>
		/// Creates a new TestFixtureAttribute.
		/// </summary>
		/// <param name="description">A description of the test fixture.</param>
		public TestFixtureAttribute( string description )
		{
			_description = description;
		}

		/// <summary>
		/// Gets or sets a description of the test fixture.
		/// </summary>
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

	}
}
