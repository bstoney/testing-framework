using System;
using System.Reflection;

using Testing.TestRunner.ConsoleRunner;

namespace TestingTestSuite
{
	class RunAllTests
	{
		static void Main( string[] args )
		{
			Environment.Exit( ConsoleTestRunner.RunAllTests( Assembly.GetExecutingAssembly(), args ) );
		}
	}
}
