using System;
using System.Collections.Generic;
using System.Text;
using Testing.TestRunner;
using System.Reflection;

namespace TestingTestSuite.MockTestFramework
{
	public static class MockTestingHelper
	{
		public static ITest CreateTest( MethodInfo method )
		{
			ITest test = new MockITest();
			test.Fixture = CreateFixture( method.DeclaringType );
			test.TestMethod = method;
			return test;
		}

		public static ITest CreatePassTest()
		{
			return CreateTest( typeof( MockFixture ).GetMethod( "TestPass" ) );
		}

		public static ITest CreateFailTest()
		{
			return CreateTest( typeof( MockFixture ).GetMethod( "TestFail" ) );
		}

		public static IFixture CreateFixture()
		{
			return CreateFixture( typeof( MockFixture ) );
		}

		public static IFixture CreateFixture( Type fixtureType )
		{
			IFixture fixture = new MockIFixture();
			fixture.FixtureType = fixtureType;
			return fixture;
		}
	}
}
