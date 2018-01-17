using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Reflection;
using Testing.TestRunner.FilteredRunner;
using ExternalTestRunner.Communication;
using System.Threading;
using System.Configuration;
using System.IO;
using Testing.TestRunner;
using System.Diagnostics;

namespace ExternalTestRunner
{
	internal class ExternalTestRunner : ITestListener
	{
		private Stopwatch _timer = new Stopwatch();
		private int _testSuiteLevel;

		private ITestRunnerService _service;
		private string _assembly;
		private string _namespace;
		private string _type;
		private string _method;

		public void BeginRun( ITestRunnerService service, string assembly, string nspace, string type, string method )
		{
			_service = service;
			_assembly = assembly;
			_namespace = nspace;
			_type = type;
			_method = method;

			Thread t = new Thread( new ThreadStart( Run ) );
			t.Start();
		}

		private void Run()
		{
			string basePath = Path.GetDirectoryName( _assembly );
			AppDomainSetup ads = new AppDomainSetup();
			ads.PrivateBinPath = basePath;
			ads.ApplicationBase = basePath;
			ads.ConfigurationFile = string.Concat( Path.GetFileName( _assembly ), ".config" );
			AppDomain domain = AppDomain.CreateDomain( "External Test Run", null, ads );

			// TODO use assembly resolver to load assemblies instead of copying locally.
			//Type t = typeof( RunClient );

			//string copyAssembly = Path.Combine( basePath, Path.GetFileName( t.Assembly.Location ) );
			//File.Copy( t.Assembly.Location, copyAssembly, true );

			try
			{
				//_listener = (RunClient)domain.CreateInstanceAndUnwrap( t.Assembly.FullName, t.FullName );
				//_listener.Run( _assembly, _namespace, _type, _method );
				Run( _assembly, _namespace, _type, _method );
			}
			catch( Exception ex )
			{
				_service.ReportError( ex );
			}
			finally
			{
				//try
				//{
				//    File.Delete( copyAssembly );
				//}
				//catch( Exception ex )
				//{
				//    _service.ReportError( ex );
				//}
			}

			_service.TestComplete( _timer.ElapsedMilliseconds );
		}

		private void Run( string assembly, string nspace, string type, string method )
		{
			Assembly asm = Assembly.LoadFrom( assembly );
			if( string.IsNullOrEmpty( nspace ) )
			{
				FilteredTestRunner.RunAssembly( this, asm );
			}
			else if( string.IsNullOrEmpty( type ) )
			{
				FilteredTestRunner.RunNamespace( this, asm, nspace );
			}
			else
			{
				Type t = asm.GetType( string.Concat( nspace, ".", type ) );
				if( t == null )
				{
					throw new Exception( "Unable to load type." );
				}
				MemberInfo mi;
				if( string.IsNullOrEmpty( method ) )
				{
					mi = t;
				}
				else
				{
					mi = t.GetMethod( method );
					if( mi == null )
					{
						throw new Exception( "Unable to load function." );
					}
				}
				FilteredTestRunner.RunMember( this, asm, mi );
			}
		}

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
		/// </summary>
		public void EndTest( ITest test )
		{
			TestResult tr = test.Result;
			Result r = new Result();
			switch( tr.Status )
			{
				case TestStatus.Untested:
					r.Skipped = true;
					break;
				case TestStatus.Pass:
					r.Passed = true;
					break;
				case TestStatus.Ignore:
					r.Skipped = true;
					break;
				case TestStatus.Fail:
					r.Failed = true;
					break;
				default:
					break;
			}
			r.Description = test.Name;
			r.StackTrace = tr.StackTrace;
			r.TestCount = test.TestCount;

			if( tr.Status != TestStatus.Pass || tr.Output.Length > 0 )
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendLine( test.Name );
				if( tr.Message.Length > 0 )
				{
					if( tr.Status != TestStatus.Ignore )
					{
						sb.Append( "MESSAGE: " );
					}
					else
					{
						sb.Append( "IGNORED: " );
					}
					sb.AppendLine( tr.Message.ToString() );
				}
				if( tr.Output.Length > 0 )
				{
					sb.AppendLine( "OUTPUT:" );
					sb.AppendLine( tr.Output.ToString() );
				}
				r.Message = sb.ToString();
			}
			_service.SendResult( r );
		}

		#endregion
	}
}
