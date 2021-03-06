using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using VSLangProj;
using System.IO;
using System.Threading;

namespace TestRun
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{

		private DTE2 _applicationObject;
		private AddIn _addInInstance;

		/// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
		public Connect()
		{
		}

		/// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
		/// <param term='application'>Root object of the host application.</param>
		/// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
		/// <param term='addInInst'>Object representing this Add-in.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection( object application, ext_ConnectMode connectMode, object addInInst, ref Array custom )
		{
			_applicationObject = (DTE2)application;
			_addInInstance = (AddIn)addInInst;
			if( connectMode == ext_ConnectMode.ext_cm_UISetup )
			{
				object[] contextGUIDS = new object[] { };
				Commands commands = _applicationObject.Commands;
				CommandBars commandBars = ((CommandBars)_applicationObject.CommandBars);

				// When run, the Add-in wizard prepared the registry for the Add-in.
				// At a later time, the Add-in or its commands may become unavailable for reasons such as:
				//   1) You moved this project to a computer other than which is was originally created on.
				//   2) You chose 'Yes' when presented with a message asking if you wish to remove the Add-in.
				//   3) You add new commands or modify commands already defined.
				// You will need to re-register the Add-in by building the FunctionsAddInSetup project,
				// right-clicking the project in the Solution Explorer, and then choosing install.
				// Alternatively, you could execute the ReCreateCommands.reg file the Add-in Wizard generated in
				// the project directory, or run 'devenv /setup' from a command prompt.
				try
				{
					CommandBar commandBar = (CommandBar)commandBars["Code Window"];
					Command command;
					CommandBarControl commandBarControl;
					command = commands.AddNamedCommand( _addInInstance,
						"RunTest", "Run Test", "Build the project and run the selected test or tests.",
						true, 186, ref contextGUIDS,
						(int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled );
					commandBarControl = ((CommandBarControl)command.AddControl( commandBar, 1 ));

					command = commands.AddNamedCommand( _addInInstance,
						"StopTest", "Stop Test", "Stop the currently running test.",
						true, 184, ref contextGUIDS,
						(int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled );
					commandBarControl = ((CommandBarControl)command.AddControl( commandBar, 1 ));
				}
				catch( Exception /*e*/)
				{
				}
			}
		}

		/// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
		/// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection( ext_DisconnectMode disconnectMode, ref Array custom )
		{
		}

		/// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />		
		public void OnAddInsUpdate( ref Array custom )
		{
		}

		/// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete( ref Array custom )
		{
		}

		/// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown( ref Array custom )
		{
		}

		/// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
		/// <param term='commandName'>The name of the command to determine state for.</param>
		/// <param term='neededText'>Text that is needed for the command.</param>
		/// <param term='status'>The state of the command in the user interface.</param>
		/// <param term='commandText'>Text requested by the neededText parameter.</param>
		/// <seealso class='Exec' />
		public void QueryStatus( string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText )
		{
			if( neededText == EnvDTE.vsCommandStatusTextWanted.vsCommandStatusTextWantedNone )
			{
				string docLanguage = _applicationObject.ActiveDocument.Language;

				if( commandName == "TestRun.Connect.RunTest" )
				{
					if( docLanguage.Equals( "CSharp" ) && !TestRunHelper.Instance.IsTestRunning )
					{
						status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
					}
					else
					{
						status = (vsCommandStatus)vsCommandStatus.vsCommandStatusUnsupported;
					}
				}
				else if( commandName == "TestRun.Connect.StopTest" )
				{
					if( docLanguage.Equals( "CSharp" ) && TestRunHelper.Instance.IsTestRunning )
					{
						status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
					}
					else
					{
						status = (vsCommandStatus)vsCommandStatus.vsCommandStatusUnsupported;
					}
				}
			}
		}

		/// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
		/// <param term='commandName'>The name of the command to execute.</param>
		/// <param term='executeOption'>Describes how the command should be run.</param>
		/// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
		/// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
		/// <param term='handled'>Informs the caller if the command was handled or not.</param>
		/// <seealso class='Exec' />
		public void Exec( string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled )
		{
			handled = false;
			if( executeOption == EnvDTE.vsCommandExecOption.vsCommandExecOptionDoDefault )
			{
				if( commandName == "TestRun.Connect.RunTest" )
				{
					TestRunHelper.Instance.RunTest( _applicationObject );
					handled = true;
					return;
				}
				else if( commandName == "TestRun.Connect.StopTest" )
				{
					TestRunHelper.Instance.StopTest( _applicationObject );
					handled = true;
					return;
				}
			}
		}
	}
}