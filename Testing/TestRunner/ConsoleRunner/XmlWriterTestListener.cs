using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;

namespace Testing.TestRunner.ConsoleRunner
{
	/// <summary>
	/// A helper class which outputs test results the the console.
	/// </summary>
	internal sealed class XmlWriterTestListener : ITestResultTracker
	{
		private int _passCount;
		private int _skipCount;
		private int _failCount;
		private bool _fixturePassed;
		private TextWriter _writer;
		private Stack<Writer> _writerStack;

		public XmlWriterTestListener( TextWriter writer )
		{
			if( writer == null )
			{
				throw new ArgumentNullException( "writer" );
			}
			_writer = writer;
			_writerStack = new Stack<Writer>();
		}

		private XmlTextWriter CurrentWriter
		{
			get { return _writerStack.Peek().XmlWriter; }
		}

		private Stopwatch CurrentTimer
		{
			get { return _writerStack.Peek().Timer; }
		}

		private void Push( string elementName )
		{
			Debug.WriteLine( elementName, "Start" );
			Debug.IndentLevel++;
			Writer w = new Writer();
			w.TextWriter = new StringWriter();
			w.XmlWriter = new XmlTextWriter( w.TextWriter );
			w.XmlWriter.Formatting = Formatting.Indented;
			w.Content = new StringBuilder();
			w.Timer = new Stopwatch();
			if( _writerStack.Count == 0 )
			{
				// Write document start as raw to account for dodgy MS code.
				w.XmlWriter.WriteRaw( "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"no\"?>" );
				w.XmlWriter.WriteComment( "This file represents the results of running a test suite" );
				_writerStack.Push( w );
				CurrentWriter.WriteStartElement( "test-results" );
				// TODO CurrentWriter.WriteAttributeString( "name", testSuite.Assembly.Location );
				CurrentWriter.WriteAttributeString( "date", DateTime.Now.ToString( "yyyy-MM-dd" ) );
				CurrentWriter.WriteAttributeString( "time", DateTime.Now.ToString( "HH:mm:ss" ) );
				Push( elementName );
			}
			else
			{
				_writerStack.Push( w );
				CurrentWriter.WriteStartElement( elementName );
			}
		}

		private void Pop()
		{
			Writer w = _writerStack.Pop();
			w.XmlWriter.WriteRaw( w.Content.ToString() );
			w.XmlWriter.WriteEndElement();
			w.XmlWriter.Flush();
			if( _writerStack.Count > 1 )
			{
				_writerStack.Peek().Content.AppendLine( w.TextWriter.ToString() );
			}
			else if( _writerStack.Count > 0 )
			{
				CurrentWriter.WriteAttributeString( "total", (PassCount + FailCount + SkipCount).ToString() );
				CurrentWriter.WriteAttributeString( "failures", FailCount.ToString() );
				CurrentWriter.WriteAttributeString( "not-run", SkipCount.ToString() );
				StringBuilder environment = new StringBuilder();
				environment.AppendLine( "environment" );
				environment.AppendFormat( "clr-version: {0}\n", Environment.Version.ToString() );
				environment.AppendFormat( "os-version: {0}\n", Environment.OSVersion.ToString() );
				environment.AppendFormat( "cwd: {0}\n", Environment.CurrentDirectory );
				environment.AppendFormat( "machine-name: {0}\n", Environment.MachineName );
				environment.AppendFormat( "user: {0}\n", Environment.UserName );
				environment.AppendFormat( "user-domain: {0}\n", Environment.UserDomainName );
				environment.AppendLine( "culture-info" );
				environment.AppendFormat( "current_culture: {0}\n", CultureInfo.CurrentCulture.Name );
				environment.AppendFormat( "current_uiculture: {0}\n", CultureInfo.CurrentUICulture.Name );
				CurrentWriter.WriteComment( environment.ToString() );
				_writerStack.Peek().Content.AppendLine( w.TextWriter.ToString() );
				Pop();
			}
			else
			{
				_writer.WriteLine( w.TextWriter.ToString() );
				_writer.Flush();
			}
			Debug.IndentLevel--;
			Debug.WriteLine( "", "End" );
		}

		#region ITestResultTracker Members

		public int PassCount
		{
			get { return _passCount; }
		}

		public int SkipCount
		{
			get { return _skipCount; }
		}

		public int FailCount
		{
			get { return _failCount; }
		}

		#endregion

		#region ITestListener Members

		/// <summary>
		/// Notifies the listener at the start of a test suite.
		/// </summary>
		public void BeginTestSuite( ITestSuite testSuite )
		{
			Push( "test-suite" );
			CurrentWriter.WriteAttributeString( "name", testSuite.Name );
			CurrentTimer.Start();
			Push( "results" );
		}

		/// <summary>
		/// Notifies the listener at the end of a test suite.
		/// </summary>
		public void EndTestSuite( ITestSuite testSuite )
		{
			Pop();
			CurrentTimer.Stop();
			CurrentWriter.WriteAttributeString( "time", CurrentTimer.Elapsed.TotalSeconds.ToString( "0.000" ) );
			CurrentWriter.WriteAttributeString( "success", (FailCount == 0).ToString() );
			Pop();
		}

		/// <summary>
		/// Notifies the listener at the start of a fixture.
		/// </summary>
		public void BeginFixture( IFixture fixture )
		{
			Push( "test-suite" );
			CurrentWriter.WriteAttributeString( "name", fixture.Name );
			CurrentWriter.WriteAttributeString( "description", fixture.Description );
			Push( "results" );
			_fixturePassed = true;
			CurrentTimer.Start();
		}

		/// <summary>
		/// Notifies the listener at the end of a fixture.
		/// </summary>
		public void EndFixture( IFixture fixture )
		{
			Pop();
			CurrentTimer.Stop();
			CurrentWriter.WriteAttributeString( "time", CurrentTimer.Elapsed.TotalSeconds.ToString( "0.000" ) );
			CurrentWriter.WriteAttributeString( "success", _fixturePassed.ToString() );
			Pop();
		}

		/// <summary>
		/// Notifies the listener at the start of a test.
		/// </summary>
		public void BeginTest( ITest test )
		{
		}

		/// <summary>
		/// Notifies the listener at the end of a test.
		/// </summary>s
		public void EndTest( ITest test )
		{
			CurrentWriter.WriteStartElement( "test-case" );
			CurrentWriter.WriteAttributeString( "name", test.Name );
			CurrentWriter.WriteAttributeString( "description", test.Description );
			if( test.Result.Status != (TestStatus.Untested | TestStatus.Ignore) )
			{
				CurrentWriter.WriteAttributeString( "executed", "True" );
				CurrentWriter.WriteAttributeString( "success", (test.Result.Status == TestStatus.Pass).ToString() );
				CurrentWriter.WriteAttributeString( "time", new TimeSpan( test.Result.TimeSpan ).TotalSeconds.ToString( "0.000" ) );
			}
			else
			{
				CurrentWriter.WriteAttributeString( "executed", "False" );
			}
			if( test.Result.Status == TestStatus.Ignore )
			{
				CurrentWriter.WriteStartElement( "reason" );
				CurrentWriter.WriteStartElement( "message" );
				CurrentWriter.WriteCData( test.Result.Message.ToString() );
				CurrentWriter.WriteEndElement();
				CurrentWriter.WriteEndElement();
			}
			else if( test.Result.Status == TestStatus.Fail )
			{
				CurrentWriter.WriteStartElement( "failure" );
				CurrentWriter.WriteStartElement( "message" );
				StringBuilder message = new StringBuilder();
				message.Append( test.Result.Message );
				if( test.Result.Output.Length > 0 )
				{
					message.AppendLine( "OUTPUT:" );
					message.Append( test.Result.Output );
				}
				CurrentWriter.WriteCData( message.ToString() );
				CurrentWriter.WriteEndElement();
				if( test.Result.StackTrace != null )
				{
					CurrentWriter.WriteStartElement( "stack-trace" );
					CurrentWriter.WriteCData( test.Result.StackTrace );
					CurrentWriter.WriteEndElement();
				}
				CurrentWriter.WriteEndElement();
			}
			CurrentWriter.WriteEndElement();
			switch( test.Result.Status )
			{
				case TestStatus.Untested:
				case TestStatus.Ignore:
					_skipCount += test.TestCount;
					break;
				case TestStatus.Pass:
					_passCount += test.TestCount;
					break;
				case TestStatus.Fail:
					_failCount += test.TestCount;
					_fixturePassed = false;
					break;
				default:
					break;
			}
		}

		#endregion

		private struct Writer
		{
			public XmlTextWriter XmlWriter;
			public TextWriter TextWriter;
			public Stopwatch Timer;
			public StringBuilder Content;
		}
	}
}
