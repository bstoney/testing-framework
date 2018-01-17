using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Testing.TestRunner.FilteredRunner
{
	public static class FilteredTestRunner
	{
		/// <summary>
		/// Runs the tests in an assembly.
		/// </summary>
		public static TestStatus RunAssembly( ITestListener testListener, Assembly assembly )
		{
			TestSuiteBuilder tsb = new TestSuiteBuilder();
			ITestSuite ts = tsb.BuildTest( assembly );
			return RunTests( ts, testListener );
		}
		/// <summary>
		/// Runs the tests in a Type or Method.
		/// </summary>
		public static TestStatus RunMember( ITestListener testListener, Assembly assembly, MemberInfo member )
		{
			TestSuiteBuilder tsb = new TestSuiteBuilder();
			ITestSuite ts = tsb.BuildTest( member );
			return RunTests( ts, testListener );
		}
		/// <summary>
		/// Runs the tests in a namespace.
		/// </summary>
		public static TestStatus RunNamespace( ITestListener testListener, Assembly assembly, string ns )
		{
			TestSuiteBuilder tsb = new TestSuiteBuilder();
			ITestSuite ts = tsb.BuildTest( assembly, ns );
			return RunTests( ts, testListener );
		}

		private static TestStatus RunTests( ITestSuite runnable, ITestListener testListener )
		{
			try
			{
				if ( runnable == null || runnable.TestCount > 0 )
				{
					runnable.RegisterListener( testListener );
					runnable.Reset();
					runnable.Run();
					return TestStatus.Pass;
				}
				else
				{
					return TestStatus.Untested;
				}
			}
			catch
			{
				return TestStatus.Fail;
			}
		}
	}
}
