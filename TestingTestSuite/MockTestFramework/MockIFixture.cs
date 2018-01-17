using System;
using Testing.TestRunner;
using Testing.TestRunner.DefaultRunner;
using System.Reflection;

namespace TestingTestSuite.MockTestFramework
{

	public class MockIFixture : IFixture
	{

		public bool TestsHaveRun
		{
			get
			{
				bool testsHaveRun = true;
				foreach( MockITest test in Tests )
				{
					testsHaveRun = testsHaveRun && test.TestHasRun;
				}
				return testsHaveRun;
			}
		}

		#region IFixture Members

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

		public Type FixtureType
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

		public object Instance
		{
			get { throw new NotImplementedException(); }
		}

		public ITest[] Tests
		{
			get { throw new NotImplementedException(); }
		}

		public IFixtureRunner FixtureRunner
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

		public MethodInfo StartUpMethod
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

		public MethodInfo TearDownMethod
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

		public void AddTest( ITest test )
		{
			throw new NotImplementedException();
		}

		public void RemoveTest( ITest test )
		{
			throw new NotImplementedException();
		}

		public ITest CreateTest()
		{
			return new MockITest();
		}

		public int TestCount
		{
			get { throw new NotImplementedException(); }
		}

		public void Run()
		{
			if( FixtureRunner != null )
			{
				FixtureRunner.RunTests( this );
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
