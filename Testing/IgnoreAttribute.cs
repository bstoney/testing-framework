using System;
using System.Collections.Generic;
using System.Text;

namespace Testing
{
	/// <summary>
	/// An attribute used to exclude a test.
	/// </summary>
	[AttributeUsage( AttributeTargets.Method | AttributeTargets.Class )]
	public sealed class IgnoreAttribute : Attribute
	{
		private string _reason;

		/// <summary>
		/// Creates a new IgnoreAttribute.
		/// </summary>
		public IgnoreAttribute() { }

		/// <summary>
		/// Creates a new IgnoreAttribute.
		/// </summary>
		/// <param name="reason">A message indicating why the test has been excluded.</param>
		public IgnoreAttribute( string reason )
		{
			_reason = reason;
		}

		/// <summary>
		/// Gets or sets a message indicating why the test has been excluded.
		/// </summary>
		public string Reason
		{
			get { return _reason; }
			set { _reason = value; }
		}

	}
}
