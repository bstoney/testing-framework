using System;

namespace Testing.UnitTest
{
	/// <summary>
	/// Abstract base class for EqualsAsserter and NotEqualsAsserter
	/// </summary>
	public abstract class EqualityAsserter : ComparisonAsserter
	{
		private double delta;

		/// <summary>
		/// Constructor taking expected and actual values and a user message with arguments.
		/// </summary>
		protected EqualityAsserter( object expected, object actual, string message, params object[] args )
			: base( expected, actual, message, args ) { }

		/// <summary>
		/// Constructor taking expected and actual values, a tolerance
		/// and a user message and arguments.
		/// </summary>
		protected EqualityAsserter( double expected, double actual, double delta, string message, params object[] args )
			: base( expected, actual, message, args )
		{
			this.delta = delta;
		}

		/// <summary>
		/// Used to compare two objects.  Two nulls are equal and null
		/// is not equal to non-null. Comparisons between the same
		/// numeric types are fine (Int32 to Int32, or Int64 to Int64),
		/// but the Equals method fails across different types so we
		/// use <c>ToString</c> and compare the results.
		/// </summary>
		protected virtual bool ObjectsEqual( Object expected, Object actual )
		{
			if( expected == null && actual == null )
			{
				return true;
			}
			if( expected == null || actual == null )
			{
				return false;
			}

			//if ( expected.GetType().IsArray && actual.GetType().IsArray )
			//	return ArraysEqual( (System.Array)expected, (System.Array)actual );

			if( expected is double && actual is double )
			{
				if( double.IsNaN( (double)expected ) && double.IsNaN( (double)actual ) )
				{
					return true;
				}
				// handle infinity specially since subtracting two infinite values gives
				// NaN and the following test fails. mono also needs NaN to be handled
				// specially although ms.net could use either method.
				if( double.IsInfinity( (double)expected ) || double.IsNaN( (double)expected ) || double.IsNaN( (double)actual ) )
				{
					return expected.Equals( actual );
				}
				else
				{
					return Math.Abs( (double)expected - (double)actual ) <= delta;
				}
			}

			//			if ( expected is float && actual is float )
			//			{
			//				// handle infinity specially since subtracting two infinite values gives
			//				// NaN and the following test fails. mono also needs NaN to be handled
			//				// specially although ms.net could use either method.
			//				if (float.IsInfinity((float)expected) || float.IsNaN((float)expected) || float.IsNaN((float)actual))
			//					return (float)expected == (float)actual;
			//				else
			//					return Math.Abs((float)expected-(float)actual) <= (float)this.delta;
			//			}

			if( expected.GetType() != actual.GetType() &&
				IsNumericType( expected ) && IsNumericType( actual ) )
			{
				//
				// Convert to strings and compare result to avoid
				// issues with different types that have the same
				// value
				//
				string sExpected = expected.ToString();
				string sActual = actual.ToString();
				return sExpected.Equals( sActual );
			}
			return expected.Equals( actual );
		}

		/// <summary>
		/// Checks the type of the object, returning true if
		/// the object is a numeric type.
		/// </summary>
		/// <param name="obj">The object to check</param>
		/// <returns>true if the object is a numeric type</returns>
		private bool IsNumericType( Object obj )
		{
			switch( obj.GetType().UnderlyingSystemType.Name )
			{
				case "Byte":
				case "SByte":
				case "Decimal":
				case "Double":
				case "Single":
				case "Int16":
				case "Int32":
				case "Int64":
				case "UInt16":
				case "UInt32":
					return true;
				default:
					return false;
			}
		}

		/// <summary>
		/// Helper method to compare two arrays
		/// </summary>
		protected virtual bool ArraysEqual( Array expected, Array actual )
		{
			if( expected.Rank != actual.Rank )
			{
				return false;
			}

			int rank = 0;
			int[] indices = new int[actual.Rank];
			return ArrayRankEqual( expected, actual, ref rank, indices );
		}

		/// <summary>
		/// Recursively traverses an array to check equality.
		/// </summary>
		protected bool ArrayRankEqual( Array expected, Array actual, ref int rank, int[] indices )
		{
			if( expected.GetLength( rank ) != actual.GetLength( rank ) )
			{
				indices[rank] = Math.Min( expected.GetLength( rank ), actual.GetLength( rank ) );
				return false;
			}

			int nextRank = rank + 1;
			for( int i = 0; i < actual.GetLength( rank ); i++ )
			{
				indices[rank] = i;
				if( rank == actual.Rank - 1 )
				{
					if( !ObjectsEqual( expected.GetValue( indices ), actual.GetValue( indices ) ) )
					{
						return false;
					}
				}
				else if( !ArrayRankEqual( expected, actual, ref nextRank, indices ) )
				{
					rank = nextRank;
					return false;
				}
			}
			return true;
		}
	}
}
