using System;
using Testing.TestRunner;
namespace ExternalTestRunner
{
	public interface IResult
	{
		string Description { get; }
		string Message { get; }
		string StackTrace { get; }
		int TestCount { get; }
		bool Skipped { get; }
		bool Failed { get; }
		bool Passed { get; }
	}
}
