using System;
using Testing.TestRunner;

namespace Testing.FunctionTest
{
	/// <summary>
	/// Description of OrderTestSuiteAttribute
	/// </summary>
	public sealed class OrderedTestSuiteAttribute : Attribute, ITestSuiteRunner
	{

		#region Constructors

		/// <summary>
		/// Creates a new OrderTestSuiteAttribute.
		/// </summary>
		public OrderedTestSuiteAttribute() { }

		#endregion

		#region Methods

		private static int FixtureComparison( IFixture x, IFixture y )
		{
			return GetFixtureIndex( x ) - GetFixtureIndex( y );
		}

		private static int GetFixtureIndex( IFixture fixture )
		{
			int fixtureIndex = 0;
			if( fixture.FixtureType != null )
			{
				OrderedFixtureAttribute[] ofa = fixture.FixtureType.GetCustomAttributes(
					typeof( OrderedFixtureAttribute ), false ) as OrderedFixtureAttribute[];
				if( ofa != null && ofa.Length == 1 )
				{
					fixtureIndex = ofa[0].Index;
				}
			}
			return fixtureIndex;
		}

		#endregion

		#region ITestSuiteRunner Members

		/// <summary>
		/// This method is called by the test suite object to invoke the tests in the test suite.
		/// </summary>
		/// <param name="testSuite">The calling test suite object.</param>
		public void RunTests( ITestSuite testSuite )
		{
			IFixture[] fixtures = testSuite.Fixtures;
			Array.Sort<IFixture>( fixtures, FixtureComparison );

			foreach( IFixture f in fixtures )
			{
				f.Run();
			}
		}

		#endregion
	}
}
