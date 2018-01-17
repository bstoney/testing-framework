using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Testing.TestRunner
{
	/// <summary>
	/// Helper class for timing tests.
	/// </summary>
	public sealed class TestTimer
	{
		private ITest _test;
		private Stopwatch _timer = new Stopwatch();

		/// <summary>
		/// Start the timer.
		/// </summary>
		public void Start( ITest test )
		{
			if( !_timer.IsRunning )
			{
				if( test == null )
				{
					throw new ArgumentException( "test" );
				}
				_test = test;
				_timer.Reset();
				_timer.Start();
			}
		}

		/// <summary>
		/// Stop the timer and update the test.
		/// </summary>
		public void Stop()
		{
			_timer.Stop();
			if( _test != null )
			{
				_test.Result.TimeSpan = _timer.ElapsedTicks;
			}
		}
	}
}
