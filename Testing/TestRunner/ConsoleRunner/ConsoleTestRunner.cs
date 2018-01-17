using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace Testing.TestRunner.ConsoleRunner
{
	/// <summary>
	/// A helper class for running a test suite from the console.
	/// </summary>
	public static class ConsoleTestRunner
	{
		/// <summary>
		/// Execute all tests defined in an assembly.
		/// </summary>
		/// <remarks>
		/// Valid command arguments:
		/// &lt;pre&gt;
		/// --Namespace &lt;namespace&gt;
		///		only execute tests within the numespace.
		/// --NoHalt
		///		don't wait for a key press when the tests have complete.
		/// --XmlOutput
		///		write output in xml format.
		/// --OutputFilename &lt;filename&gt;
		///		output filename.
		/// &lt;/pre&gt;
		/// </remarks>
		/// <param name="assembly">the assembly which defines the tests.</param>
		/// <param name="commandArguments">Command line arguments.</param>
		/// <returns>A console exit code.</returns>
		public static int RunAllTests( Assembly assembly, string[] commandArguments )
		{
			RunOptions options;
			try
			{
				options = new RunOptions( commandArguments );
			}
			catch( ArgumentException exp )
			{
				Console.Error.WriteLine( exp.Message );
				return 1;
			}


			ITestSuite ts;
			ts = GetTestSuite( assembly, ref options );
			TextWriter tw = GetWriter( ref options );
			ITestResultTracker tracker = GetTracker( ref options, tw );
			ts.RegisterListener( tracker );
			try
			{
				ts.Reset();
				ts.Run();
			}
			finally
			{
				tw.Flush();
				// Close the file stream
				if( options.OutputFilename != null )
				{
					tw.Close();
					tw.Dispose();
				}
			}

			if( !options.NoHalt )
			{
				Console.WriteLine( "Press any key to continue..." );
				Console.ReadKey();
			}
			return 0;
		}

		private static ITestSuite GetTestSuite( Assembly assembly, ref RunOptions options )
		{
			TestSuiteBuilder tsb = new TestSuiteBuilder();
			ITestSuite ts;
			if( String.IsNullOrEmpty( options.Namespace ) )
			{
				ts = tsb.BuildTest( assembly );
			}
			else
			{
				ts = tsb.BuildTest( assembly, options.Namespace );
			}
			return ts;
		}

		private static ITestResultTracker GetTracker( ref RunOptions options, TextWriter tw )
		{
			ITestResultTracker tracker;
			if( !options.XmlOutput )
			{
				tracker = new TextWriterTestListener( tw );
			}
			else
			{
				tracker = new XmlWriterTestListener( tw );
			}
			return tracker;
		}

		private static TextWriter GetWriter( ref RunOptions options )
		{
			TextWriter tw;
			if( options.OutputFilename == null )
			{
				tw = Console.Out;
			}
			else
			{
				tw = new StreamWriter( new FileStream( options.OutputFilename, FileMode.Create, FileAccess.Write ) );
			}
			return tw;
		}

		private struct RunOptions
		{
			public string Namespace;
			public bool NoHalt;
			public bool XmlOutput;
			public string OutputFilename;

			public RunOptions( string[] args )
			{
				Namespace = null;
				NoHalt = false;
				XmlOutput = false;
				OutputFilename = null;

				if( args != null )
				{
					for( int i = 0; i < args.Length; i++ )
					{
						switch( args[i].ToLower() )
						{
							case "--namespace":
								if( (i + 1) < args.Length )
								{
									Namespace = args[i + 1];
									i++;
								}
								else
								{
									throw new ArgumentException( "Namesapce was not supplied." );
								}
								break;
							case "--nohalt":
								NoHalt = true;
								break;
							case "--xmloutput":
								XmlOutput = true;
								break;
							case "--outputfilename":
								if( (i + 1) < args.Length )
								{
									OutputFilename = args[i + 1];
									i++;
								}
								else
								{
									throw new ArgumentException( "Output filename was not supplied." );
								}
								break;
							default:
								throw new ArgumentException( "Unknown command argument:", args[i] );
						}
					}
				}
			}
		}
	}
}
