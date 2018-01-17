using System;
using System.Collections.Generic;
using System.Text;

namespace Testing
{
	/// <summary>
	/// Excludes part of an assembly from coverage, eg. pass as the //ea CoverageExcludeAttribute parameter to NCover.
	/// </summary>
	[AttributeUsage( AttributeTargets.All )]
	public sealed class CoverageExcludeAttribute : Attribute
	{
		private string _reason;

		/// <summary>
		/// Constructor.
		/// </summary>
		public CoverageExcludeAttribute( string reason )
		{
			if( reason == null )
			{
				throw new ArgumentNullException( "reason" );
			}
			_reason = reason;
		}

		/// <summary>
		/// Gets a reason for the exclusion.
		/// </summary>
		public string Reason
		{
			get { return _reason; }
		}
	}
}
