using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace ExternalTestRunner.Communication
{
	public static class TestRunnerServiceConfig
	{
		public static readonly string PipeName = "\\\\.\\pipe\\ExternalTestRunner";

	    public static readonly string Uri = "";
	}
}
