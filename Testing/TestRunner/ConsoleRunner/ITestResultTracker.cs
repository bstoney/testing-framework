using System;
namespace Testing.TestRunner.ConsoleRunner
{
	internal interface ITestResultTracker : ITestListener
	{
		int FailCount { get; }
		int PassCount { get; }
		int SkipCount { get; }
	}
}
