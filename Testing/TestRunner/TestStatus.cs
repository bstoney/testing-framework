using System;
using System.Collections.Generic;
using System.Text;

namespace Testing.TestRunner
{
	/// <summary>
	/// The restulting outcome from a test.
	/// </summary>
	public enum TestStatus
	{
		/// <summary>
		/// The test was not executed.
		/// </summary>
		Untested,
		/// <summary>
		/// The test ran and passed.
		/// </summary>
		Pass,
		/// <summary>
		/// The test has been excluded.
		/// </summary>
		Ignore,
		/// <summary>
		/// The test ran, but failed.
		/// </summary>
		Fail,
	}
}
