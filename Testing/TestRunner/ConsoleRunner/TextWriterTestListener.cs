using System;
using System.IO;
using System.Diagnostics;

namespace Testing.TestRunner.ConsoleRunner
{
	/// <summary>
	/// A helper class which outputs test results the the console.
	/// </summary>
	internal sealed class TextWriterTestListener : ITestResultTracker
	{
		private int _passCount;
		private int _skipCount;
		private int _failCount;
		private TextWriter _writer;
		private Stopwatch _timer;
		private int _testSuiteLevel;

		public TextWriterTestListener( TextWriter writer )
		{
			if( writer == null )
			{
				throw new ArgumentNullException( "writer" );
			}
			_writer = writer;
			_timer = new Stopwatch();
		}

		#region ITestResultTracker Members

		public int PassCount
		{
			get { return _passCount; }
		}

		public int SkipCount
		{
			get { return _skipCount; }
		}

		public int FailCount
		{
			get { return _failCount; }
		}

		#endregion

		#region ITestListener Members

		/// <summary>
		/// Notifies the listener at the start of a test suite.
		/// </summary>
		public void BeginTestSuite( ITestSuite testSuite )
		{
			if( _testSuiteLevel == 0 )
			{
				_timer.Start(); 
			}
			_testSuiteLevel++;
		}

		/// <summary>
		/// Notifies the listener at the end of a test suite.
		/// </summary>
		public void EndTestSuite( ITestSuite testSuite )
		{
			_testSuiteLevel--;
			if( _testSuiteLevel == 0 )
			{
				_timer.Stop();
				_writer.WriteLine( String.Format( "{0} passed, {1} failed, {2} skipped, took {3:0.000} seconds.",
					_passCount, _failCount, _skipCount, _timer.Elapsed.TotalSeconds ) ); 
			}
		}

		/// <summary>
		/// Notifies the listener at the start of a fixture.
		/// </summary>
		public void BeginFixture( IFixture fixture )
		{
		}

		/// <summary>
		/// Notifies the listener at the end of a fixture.
		/// </summary>
		public void EndFixture( IFixture fixture )
		{
		}

		/// <summary>
		/// Notifies the listener at the start of a test.
		/// </summary>
		public void BeginTest( ITest test )
		{
		}

		/// <summary>
		/// Notifies the listener at the end of a test.
		/// </summary>s
		public void EndTest( ITest test )
		{
			if( test.Result.Status != TestStatus.Pass || test.Result.Output.Length > 0 )
			{
				_writer.WriteLine( test.Name );
				if( test.Result.Message.Length > 0 )
				{
					if( test.Result.Status != TestStatus.Ignore )
					{
						_writer.Write( "MESSAGE: " );
					}
					else
					{
						_writer.Write( "IGNORED: " );
					}
					_writer.WriteLine( test.Result.Message );
				}
				if( test.Result.Output.Length > 0 )
				{
					_writer.WriteLine( "OUTPUT: " );
					_writer.WriteLine( test.Result.Output );
				}
			}
			switch( test.Result.Status )
			{
				case TestStatus.Untested:
					_skipCount += test.TestCount;
					break;
				case TestStatus.Pass:
					_passCount += test.TestCount;
					break;
				case TestStatus.Ignore:
					_skipCount += test.TestCount;
					break;
				case TestStatus.Fail:
					_failCount += test.TestCount;
					break;
				default:
					break;
			}
		}

		#endregion
	}
}
