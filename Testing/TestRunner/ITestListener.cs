using System;
using System.Collections.Generic;
using System.Text;

using Testing.TestRunner;

namespace Testing.TestRunner
{
	/// <summary>
	/// ITestListener provides a mechanism for receiving test results.
	/// </summary>
	public interface ITestListener
	{
		
		/// <summary>
		/// Notifies the listener at the start of a test suite.
		/// </summary>
		void BeginTestSuite( ITestSuite testSuite );

		/// <summary>
		/// Notifies the listener at the end of a test suite.
		/// </summary>
		void EndTestSuite( ITestSuite testSuite );

		/// <summary>
		/// Notifies the listener at the start of a fixture.
		/// </summary>
		void BeginFixture( IFixture fixture );

		/// <summary>
		/// Notifies the listener at the end of a fixture.
		/// </summary>
		void EndFixture( IFixture fixture );

		/// <summary>
		/// Notifies the listener at the start of a test.
		/// </summary>
		void BeginTest( ITest test );

		/// <summary>
		/// Notifies the listener at the end of a test.
		/// </summary>
		void EndTest( ITest test );
	}
}
