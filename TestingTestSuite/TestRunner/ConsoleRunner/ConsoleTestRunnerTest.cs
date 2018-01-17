using System;
using System.Collections.Generic;
using System.Text;
using Testing;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Testing.UnitTest;
using System.Xml;
using System.Xml.Schema;
using Testing.TestRunner.ConsoleRunner;

namespace TestingTestSuite.TestRunner.DefaultRunner
{
	[TestFixture]
	public class ConsoleTestRunnerTest
	{
		private const string TestFilename = "ConsolTestRunnerTestOutput.xml";
		private TextWriter _writer;
		Process _process;

		[SetUp]
		public void SetUp()
		{
			_writer = new StringWriter();
			_process = Process.GetCurrentProcess();
			_process.OutputDataReceived += new DataReceivedEventHandler( OnDataRecieved );
		}

		[TearDown]
		public void TearDown()
		{
			_process.OutputDataReceived -= new DataReceivedEventHandler( OnDataRecieved );
			_writer.Close();
			_writer.Dispose();
			_writer = null;
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			if( File.Exists( TestFilename ) )
			{
				File.Delete( TestFilename );
			}
		}

		[Test( "Test loading and running a test suite from the console." )]
		public void LoadAndRunTests()
		{
			int retVal = ConsoleTestRunner.RunAllTests( Assembly.GetExecutingAssembly(),
				new string[] { "--NoHalt", "--Namespace", "TestingTestSuite.TestRunner.ConsoleRunner.TestNamespace" } );
			Assert.AreEqual( 0, retVal );
		}

		[Test( "Test loading and running a test suite from the console." )]
		public void LoadAndRunWithXmlOutputTests()
		{
			int retVal = ConsoleTestRunner.RunAllTests( Assembly.GetExecutingAssembly(),
				new string[] { "--NoHalt", 
					"--XmlOutput", "--OutputfileName", TestFilename,
					"--Namespace", "TestingTestSuite.TestRunner.ConsoleRunner.TestNamespace" } );
			Assert.AreEqual( 0, retVal );

			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ValidationType = ValidationType.Schema;
			settings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings;
			settings.Schemas.Add( XmlSchema.Read( Assembly.GetExecutingAssembly().GetManifestResourceStream(
				"TestingTestSuite.TestRunner.ConsoleRunner.Results.xsd" ), null ) );
			XmlReader validator = XmlReader.Create( TestFilename, settings );
			while( validator.Read() )
			{
			}
		}

		private void OnValidationEvent( object sender, ValidationEventArgs args )
		{
			if( args.Severity == XmlSeverityType.Error )
			{
				Assert.Fail( args.Message );
			}
		}

		private void OnDataRecieved( object sender, DataReceivedEventArgs args )
		{
			_writer.Write( args.Data );
		}
	}
}
