using System;
using System.Collections.Generic;
using System.Text;

namespace Testing.FunctionTest
{
	/// <summary>
	/// Defines the index of the test with in a fixture.
	/// </summary>
	[AttributeUsage( AttributeTargets.Method )]
	public sealed class TestOrderAttribute : Attribute
	{
		private int _index;

		/// <summary>
		/// Creates a new TestOrderedAttribute.
		/// </summary>
		/// <param name="index"></param>
		public TestOrderAttribute( int index )
		{
			_index = index;
		}

		/// <summary>
		/// Gets or sets the index of this test.
		/// </summary>
		public int Index
		{
			get { return _index; }
			set { _index = value; }
		}

	}
}
