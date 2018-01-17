using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace ExternalTestRunner.Communication
{
	[ServiceContract( Namespace = "ExternalTestRunner.Communication" )]
	public interface ITestRunnerService
	{
		[OperationContract]
		string GetAssembly();

		[OperationContract]
		string GetNamespace();

		[OperationContract]
		string GetTypeName();

		[OperationContract]
		string GetMethod();

		[OperationContract]
		void ReportError( Exception ex );

		[OperationContract]
		void TestComplete( long elapsedMilliseconds );

		[OperationContract]
		void SendResult( IResult r );
	}
}
