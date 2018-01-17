using System;
using Testing.TestRunner;
using Testing.TestRunner.DefaultRunner;
using System.Reflection;

namespace TestingTestSuite.MockTestFramework
{

	public class MockITestSuite : ITestSuite
	{

		public bool TestsHaveRun
		{
			get
			{
				bool testsHaveRun = true;
				foreach( MockIFixture fixture in Fixtures )
				{
					testsHaveRun = testsHaveRun && fixture.TestsHaveRun;
				}
				return testsHaveRun;
			}
		}

		#region ITestSuite Members

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

		public Assembly Assembly
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

		public string Namespace
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

		public IFixture[] Fixtures
		{
			get { throw new NotImplementedException(); }
		}

		public ITestSuite[] TestSuites
		{
			get { throw new NotImplementedException(); }
		}

		public ITestSuiteRunner TestSuiteRunner
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

		public void AddFixture( IFixture fixture )
		{
			throw new NotImplementedException();
		}

		public void RemoveFixture( IFixture fixture )
		{
			throw new NotImplementedException();
		}

		public IFixture CreateFixture()
		{
			return new MockIFixture();
		}

		public void AddTestSuite( ITestSuite testSuite )
		{
			throw new NotImplementedException();
		}

		public void RemoveTestSuite( ITestSuite testSuite )
		{
			throw new NotImplementedException();
		}

		public int TestCount
		{
			get { throw new NotImplementedException(); }
		}

		public void Run()
		{
			if( TestSuiteRunner != null )
			{
				TestSuiteRunner.RunTests( this );
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
