using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Security.Policy;
using System.Diagnostics;
using ExternalTestRunner.Communication;
using ExternalTestRunner;
using System.Threading;
using TestRun.Communication;

namespace TestRun
{
	internal class InternalTestRunner : IDisposable
	{
		private const string ThreadName = "TestRunThread";

		private string _assembly;
		private string _namespace;
		private string _type;
		private string _method;
		private EnvDTE.OutputWindowPane _output;
		private EnvDTE.StatusBar _status;
		private Process _process;
		private Thread _thread;
		private int _skipCount;
		private int _failCount;
		private int _passCount;
		private long _totalSeconds;

		public InternalTestRunner() { }

		public bool IsRunning
		{
			get { return _thread != null && _thread.IsAlive; }
		}

		public void Stop()
		{
			if( IsRunning )
			{
				if( _process != null && !_process.HasExited )
				{
					_process.Kill();
				}
				else
				{
					_thread.Abort();
				}
			}
		}

		public void Start( string assembly, string nspace, string type, string method,
			EnvDTE.OutputWindowPane output, EnvDTE.StatusBar status )
		{
			if( !IsRunning )
			{
				_assembly = assembly;
				_namespace = nspace;
				_type = type;
				_method = method;
				_output = output;
				_status = status;

				_thread = new System.Threading.Thread( new ThreadStart( Run ) );
				_thread.Name = ThreadName;
				_thread.Start();
			}
		}

		public void Run()
		{
			using( TestRunnerServer server = new TestRunnerServer() )
			{
				server.Complete += new CompleteHandler( OnComplete );
				server.Result += new ResultHandler( OnResult );
				server.Error += new ErrorHandler( OnError );

				EnvDTE.Process attachedProcess;
				ProcessStartInfo psi = new ProcessStartInfo();
				Assembly asm = typeof( ITestRunnerService ).Assembly;
				psi.FileName = asm.Location;
				psi.WorkingDirectory = Path.GetDirectoryName( _assembly );
				psi.UseShellExecute = false;
				psi.RedirectStandardOutput = true;
				psi.WindowStyle = ProcessWindowStyle.Hidden;
				_process = Process.Start( psi );

				foreach( EnvDTE.Process p in _output.DTE.Debugger.LocalProcesses )
				{
					if( p.ProcessID == _process.Id )
					{
						attachedProcess = p;
						attachedProcess.Attach();
						break;
					}
				}

				string error = _process.StandardOutput.ReadToEnd();
				_process.WaitForExit();

				_output.OutputString( "\n" );
				_output.OutputString( error );

				_output.Activate();
				_status.Text = GetResultText();
			}
		}

		private void OnError( object sender, Exception ex )
		{
			_output.OutputString( ex.ToString() );
			_output.OutputString( "\n" );
		}

		private void OnResult( object sender, IResult result )
		{
			if( result != null )
			{
				if( !string.IsNullOrEmpty( result.Message ) )
				{
					_output.OutputTaskItemString( result.Message, EnvDTE.vsTaskPriority.vsTaskPriorityHigh, "Test Failure",
						EnvDTE.vsTaskIcon.vsTaskIconShortcut, "", 0, result.Description, true );
					if( !result.Message.EndsWith( "\n" ) )
					{
						_output.OutputString( "\n" );
					}
				}

				if( result.Skipped )
				{
					_skipCount += result.TestCount;
				}
				else if( result.Failed )
				{
					_failCount += result.TestCount;
				}
				else if( result.Passed )
				{
					_passCount += result.TestCount;
				}
			}
		}

		private void OnComplete( object sender, long elapsedMilliseconds )
		{
			_totalSeconds = elapsedMilliseconds / 1000;
			_output.OutputString( GetResultText() );
			_output.OutputString( "\n" );
		}

		private string GetResultText()
		{
			return string.Format( "{0} passed, {1} failed, {2} skipped, took {3:0.000} seconds.",
							_passCount, _failCount, _skipCount, _totalSeconds );
		}

		#region IDisposable Members

		public void Dispose()
		{
			if( _process != null && !_process.HasExited )
			{
				_process.Kill();
			}
		}

		#endregion
	}
}
