using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using ExternalTestRunner.Communication;
using ExternalTestRunner;
using System.Threading;

namespace TestRun.Communication
{
	[ServiceBehavior( InstanceContextMode = InstanceContextMode.Single )]
	internal class TestRunnerServer : ITestRunnerService, IDisposable
	{
		private ServiceHost _host;
		private string _assembly;
		private string _namespace;
		private string _type;
		private string _method;
		public bool _running;

		public event CompleteHandler Complete;
		public event ResultHandler Result;
		public event ErrorHandler Error;

		public void Start( string assembly, string nsapace, string type, string method )
		{
			_assembly = assembly;
			_namespace = nsapace;
			_type = type;
			_method = method;
			_host = new ServiceHost( this, new Uri( TestRunnerServiceConfig.Uri ) );
			_host.AddServiceEndpoint( typeof( TestRunnerServer ), new BasicHttpBinding(), TestRunnerServiceConfig.PipeName );
			_host.Open();
		}

		#region ITestRunnerService Members

		public string GetAssembly()
		{
			return _assembly;
		}

		public string GetNamespace()
		{
			return _namespace;
		}

		public string GetTypeName()
		{
			return _type;
		}

		public string GetMethod()
		{
			return _method;
		}

		public void ReportError( Exception ex )
		{
			if( Error != null )
			{
				Error( this, ex );
			}
		}

		public void TestComplete( long elapsedMilliseconds )
		{
			if( Complete != null )
			{
				Complete( this, elapsedMilliseconds );
			}
		}

		public void SendResult( IResult r )
		{
			if( Result != null )
			{
				Result( this, r );
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if( _host != null )
			{
				_host.Close();
			}
		}

		#endregion
	}
}
