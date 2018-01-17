using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testing.TestRunner;

namespace ExternalTestRunner
{
	[Serializable]
	internal class Result : IResult
	{

		#region IResult Members

		public string Message { get; set; }

		public string Description { get; set; }

		public string StackTrace { get; set; }

		public int TestCount { get; set; }

		public bool Skipped { get; set; }

		public bool Failed { get; set; }

		public bool Passed { get; set; }

		#endregion
	}
}
