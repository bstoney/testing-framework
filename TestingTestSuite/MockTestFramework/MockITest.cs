using System;
using Testing.TestRunner;
using Testing.TestRunner.DefaultRunner;

namespace TestingTestSuite.MockTestFramework
{

	public class MockITest : ITest
	{
		public bool TestHasRun
		{
			get { return Result.Status != TestStatus.Untested; }
		}

		#region ITest Members

		public string Name
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string Description
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool Ignore
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string IgnoreReason
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public ITestRunner TestRunner
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public IFixture Fixture
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public System.Reflection.MethodInfo TestMethod
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public System.Reflection.MethodInfo StartUpMethod
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public System.Reflection.MethodInfo TearDownMethod
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public TestResult Result
		{
			get { throw new NotImplementedException(); }
		}

		public int TestCount
		{
			get { throw new NotImplementedException(); }
		}

		public void Run()
		{
			if( TestRunner != null )
			{
				TestRunner.RunTest( this );
			}
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}

		public void RegisterListener( ITestListener observer )
		{
			throw new NotImplementedException();
		}

		public void UnregisterListener( ITestListener observer )
		{
			throw new NotImplementedException();
		}

		public void Begin()
		{
			throw new NotImplementedException();
		}

		public void End()
		{
			throw new NotImplementedException();
		}

		#endregion
	}

}
