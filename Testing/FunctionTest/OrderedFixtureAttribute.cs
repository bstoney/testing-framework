using System;
using System.Collections.Generic;
using System.Text;
using Testing.TestRunner;

namespace Testing.FunctionTest
{
	/// <summary>
	/// A test fixture which runs the test in the order defined by the TestOrderedAttribute.
	/// </summary>
	[AttributeUsage( AttributeTargets.Class )]
	public sealed class OrderedFixtureAttribute : Attribute, IFixtureRunner
	{
		private int _index;

		/// <summary>
		/// Creates a new OrderedTextFixtureAttribute.
		/// </summary>
		public OrderedFixtureAttribute()
			: this( 0 )
		{
		}

		/// <summary>
		/// Creates a new OrderedTextFixtureAttribute.
		/// </summary>
		public OrderedFixtureAttribute( int index )
		{
			_index = index;
		}

		/// <summary>
		/// Gets or sets the index of this test fixture.
		/// </summary>
		public int Index
		{
			get { return _index; }
			set { _index = value; }
		}

		#region Methods

		private static int TestComparison( ITest x, ITest y )
		{
			return GetTestIndex( x ) - GetTestIndex( y );
		}

		private static int GetTestIndex( ITest test )
		{
			int testIndex = 0;
			if( test.TestMethod != null )
			{
				TestOrderAttribute[] testOrder = test.TestMethod.GetCustomAttributes(
					typeof( TestOrderAttribute ), false ) as TestOrderAttribute[];
				if( testOrder != null && testOrder.Length == 1 )
				{
					testIndex = testOrder[0].Index;
				}
			}
			return testIndex;
		}

		#endregion Methods

		#region IFixtureRunner Members

		/// <summary>
		/// Run the tests.
		/// </summary>
		public void RunTests( IFixture fixture )
		{
			ITest[] tests = fixture.Tests;
			Array.Sort<ITest>( tests, TestComparison );

			foreach( ITest test in tests )
			{
				test.Run();
			}
		}

		#endregion
	}
}
