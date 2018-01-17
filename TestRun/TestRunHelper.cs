using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using System.IO;
using System.Threading;
using EnvDTE80;

namespace TestRun
{
	public class TestRunHelper
	{
		private static TestRunHelper _instance;

		private InternalTestRunner _testRunner;

		private TestRunHelper() { }

		public static TestRunHelper Instance
		{
			get
			{
				if( _instance == null )
				{
					_instance = new TestRunHelper();
				}
				return _instance;
			}
		}

		public bool IsTestRunning
		{
			get { return (_testRunner != null && _testRunner.IsRunning); }
		}

		public void StopTest( DTE2 applicationObject )
		{
			if( IsTestRunning )
			{
				_testRunner.Stop();
				_testRunner.Dispose();

				OutputWindowPane output = GetOutputPane( applicationObject );
				output.OutputString( "Test stopped by user.\n" );
				output.Activate();
			}
		}

		public void RunTest( DTE2 applicationObject )
		{
			if( IsTestRunning )
			{
				return;
			}

			OutputWindowPane output = GetOutputPane( applicationObject );
			output.Clear();
			output.Activate();
			GetOutputWindow( applicationObject ).Activate();
			output.OutputString( "Begining Test Run.\n" );

			string assembly = null;
			string nspace = null;
			string type = null;
			string method = null;

			// Find code element.
			try
			{
				GetElement( applicationObject, ref nspace, ref type, ref method );
			}
			catch( Exception ex )
			{
				output.OutputString( "Unable to find code element.\n" );
				output.OutputString( ex.ToString() );
				return;
			}

			// Build the solution.
			try
			{
				Solution s = applicationObject.Solution;
				s.SolutionBuild.Build( true );
				if( s.SolutionBuild.LastBuildInfo != 0 )
				{
					output.OutputString( "Build Failed" );
					return;
				}
			}
			catch( Exception ex )
			{
				output.OutputString( "Unable to build the project.\n" );
				output.OutputString( ex.ToString() );
				return;
			}
			output.Activate();
			GetOutputWindow( applicationObject ).Activate();

			// Load assembly.
			try
			{
				assembly = GetProjectAssembly( applicationObject );
				if( assembly == null )
				{
					output.OutputString( "Unable to load assembly.\n" );
					return;
				}
			}
			catch( Exception ex )
			{
				output.OutputString( "Unable to load assembly.\n" );
				output.OutputString( ex.ToString() );
				return;
			}

			string message;
			if( nspace != null )
			{
				if( type != null )
				{
					if( method != null )
					{
						message = string.Format( "Running tests for method: {0}.{1}.{2} in", nspace, type, method );
					}
					else
					{
						message = string.Format( "Running tests for type: {0}.{1} in", nspace, type );
					}
				}
				else
				{
					message = string.Format( "Running tests for namespace: {0} in", nspace );
				}
			}
			else
			{
				message = "Running all tests from";
			}
			output.OutputString( string.Format( "{0} assembly {1}.\n\n", message, Path.GetFileNameWithoutExtension( assembly ) ) );
			try
			{
				_testRunner = new InternalTestRunner();
				_testRunner.Start( assembly, nspace, type, method, output, applicationObject.StatusBar );
			}
			catch( Exception ex )
			{
				output.OutputString( string.Concat( ex.Message, "\n" ) );
			}
		}

		private OutputWindowPane GetOutputPane( DTE2 applicationObject )
		{
			return GetWindowPane( applicationObject, "Testing" );
		}

		private void GetElement( DTE2 applicationObject, ref string nspace, ref string type, ref string method )
		{
			TextDocument td = applicationObject.ActiveDocument.Object( "TextDocument" ) as TextDocument;
			TextPoint tp = td.Selection.ActivePoint;
			CodeElements ces = applicationObject.ActiveDocument.ProjectItem.FileCodeModel.CodeElements;
			CodeElement ce;
			ce = CodeElementFromPoint( vsCMElement.vsCMElementFunction, ces, td.Selection.TopPoint );
			if( ce != null )
			{
				method = ce.Name;
			}
			ce = CodeElementFromPoint( vsCMElement.vsCMElementClass, ces, td.Selection.TopPoint );
			if( ce != null )
			{
				type = ce.Name;
			}
			ce = CodeElementFromPoint( vsCMElement.vsCMElementNamespace, ces, td.Selection.TopPoint );
			if( ce != null )
			{
				nspace = ce.FullName;
			}
		}

		private string GetProjectAssembly( DTE2 applicationObject )
		{
			Project project = applicationObject.ActiveDocument.ProjectItem.ProjectItems.ContainingProject;

			string relativeOutputPath = project.ConfigurationManager.ActiveConfiguration.Properties.Item( "OutputPath" ).Value.ToString();
			string projectPath = Path.GetDirectoryName( project.FileName );
			string outputPath = Path.Combine( projectPath, relativeOutputPath );
			return Path.Combine( outputPath, project.Properties.Item( "OutputFileName" ).Value.ToString() );
		}

		private CodeElement CodeElementFromPoint( vsCMElement elementType, CodeElements codeElements, TextPoint point )
		{
			CodeElement element = null;
			if( codeElements != null )
			{
				foreach( CodeElement ce in codeElements )
				{
					if( ce.StartPoint.LessThan( point ) && ce.EndPoint.GreaterThan( point ) )
					{
						if( ce.Kind == elementType )
						{
							element = ce;
						}

						// Check for inner ellements of a class or namespace.
						if( ce.Kind == vsCMElement.vsCMElementNamespace || ce.Kind == vsCMElement.vsCMElementClass )
						{
							CodeElement innerElement = CodeElementFromPoint( elementType, ce.Children, point );
							if( innerElement != null )
							{
								element = innerElement;
							}
						}
						break;
					}
				}
			}
			return element;
		}

		private OutputWindowPane GetWindowPane( DTE2 applicationObject, string title )
		{
			Window winItem = GetOutputWindow( applicationObject );
			OutputWindow window = (OutputWindow)winItem.Object;

			try
			{
				// Try to return an existing output pane which has the matching title.
				return window.OutputWindowPanes.Item( title );
			}
			catch( ArgumentException )
			{
				// Output pane was not found and it must be added.
				return window.OutputWindowPanes.Add( title );
			}
		}

		private static Window GetOutputWindow( DTE2 applicationObject )
		{
			return applicationObject.Windows.Item( Constants.vsWindowKindOutput );
		}
	}
}
