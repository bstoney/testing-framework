using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExternalTestRunner.Communication;
using System.ServiceModel;
using System.Threading;
using System.IO;

namespace ExternalTestRunner
{
	class Program
	{
		static void Main( string[] args )
		{
			try
			{
				EndpointAddress ea = new EndpointAddress( string.Concat(
					TestRunnerServiceConfig.Uri, "/", TestRunnerServiceConfig.PipeName ) );
				ITestRunnerService server = ChannelFactory<ITestRunnerService>.CreateChannel( new BasicHttpBinding(), ea );

				try
				{
					string assembly = server.GetAssembly();
					string nspace = server.GetNamespace();
					string type = server.GetTypeName();
					string method = server.GetMethod();

					ExternalTestRunner etr = new ExternalTestRunner();
					etr.BeginRun( server, assembly, nspace, type, method );
				}
				catch( Exception ex )
				{
					server.ReportError( ex );
				}
			}
			catch( Exception ex )
			{
				Console.WriteLine( ex.ToString() );
			}
		}
	}
}
