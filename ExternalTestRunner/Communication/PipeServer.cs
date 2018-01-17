using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using System.IO;

namespace ExternalTestRunner.Communication
{
	public sealed class PipeServer
	{
		[DllImport( "kernel32.dll", SetLastError = true )]
		private static extern SafeFileHandle CreateNamedPipe(
		   String pipeName,
		   uint dwOpenMode,
		   uint dwPipeMode,
		   uint nMaxInstances,
		   uint nOutBufferSize,
		   uint nInBufferSize,
		   uint nDefaultTimeOut,
		   IntPtr lpSecurityAttributes );

		[DllImport( "kernel32.dll", SetLastError = true )]
		private static extern int ConnectNamedPipe(
		   SafeFileHandle hNamedPipe,
		   IntPtr lpOverlapped );

		public const uint DUPLEX = (0x00000003);
		public const uint FILE_FLAG_OVERLAPPED = (0x40000000);

		public event MessageReceivedHandler MessageReceived;
		public const int BUFFER_SIZE = 4096;

		private Thread _listenThread;
		private List<Client> _clients;

		public string PipeName { get; set; }

		public bool Running { get; private set; }

		public PipeServer()
		{
			_clients = new List<Client>();
		}

		/// <summary>
		/// Starts the pipe server
		/// </summary>
		public void Start()
		{
			if( !Running )
			{
				// start the listening thread
				_listenThread = new Thread( new ThreadStart( ListenForClients ) );
				_listenThread.Start();

				Running = true;
			}
		}

		public void Stop()
		{
			if( Running )
			{
				_listenThread.Abort();

				Running = false;
			}
		}

		/// <summary>
		/// Listens for client connections
		/// </summary>
		private void ListenForClients()
		{
			while( Running )
			{
				SafeFileHandle clientHandle =
				CreateNamedPipe( PipeName, DUPLEX | FILE_FLAG_OVERLAPPED,
					 0, 255, BUFFER_SIZE, BUFFER_SIZE, 0, IntPtr.Zero );

				//could not create named pipe
				if( clientHandle.IsInvalid )
				{
					return;
				}

				int success = ConnectNamedPipe( clientHandle, IntPtr.Zero );

				//could not connect client
				if( success == 0 )
				{
					return;
				}

				Client client = new Client();
				client.Handle = clientHandle;
				client.MessageReceived += new MessageReceivedHandler( OnClientMessageReceived );
				client.Disconected += new EventHandler( OnClientDisconected );

				lock( _clients )
				{
					_clients.Add( client );
				}

				Thread readThread = new Thread( new ThreadStart( client.Read ) );
				readThread.Start();
			}
		}

		private void OnClientDisconected( object sender, EventArgs e )
		{
			lock( _clients )
			{
				_clients.Remove( (Client)sender );
			}
		}

		private void OnClientMessageReceived( string message )
		{
			if( MessageReceived != null )
			{
				MessageReceived( message );
			}
		}

		/// <summary>
		/// Sends a message to all connected clients
		/// </summary>
		/// <param name="message">the message to send</param>
		public void SendMessage( string message )
		{
			lock( _clients )
			{
				ASCIIEncoding encoder = new ASCIIEncoding();
				byte[] messageBuffer = encoder.GetBytes( message );
				foreach( Client client in _clients )
				{
					client.Stream.Write( messageBuffer, 0, messageBuffer.Length );
					client.Stream.Flush();
				}
			}
		}

		private class Client
		{
			public SafeFileHandle Handle{ get; set; }
			public FileStream Stream { get; private set; }

			public event MessageReceivedHandler MessageReceived;
			public event EventHandler Disconected;

			/// <summary>
			/// Reads incoming data from connected clients
			/// </summary>
			public void Read()
			{
				Stream = new FileStream( Handle, FileAccess.ReadWrite, BUFFER_SIZE, true );
				byte[] buffer = new byte[BUFFER_SIZE];
				ASCIIEncoding encoder = new ASCIIEncoding();

				while( true )
				{
					int bytesRead = 0;

					try
					{
						bytesRead = Stream.Read( buffer, 0, BUFFER_SIZE );
					}
					catch
					{
						// read error has occurred
						break;
					}

					// client has disconnected
					if( bytesRead == 0 )
						break;

					OnMessageReceived( buffer, encoder, bytesRead );
				}

				// clean up resources
				Stream.Close();
				Stream.Dispose();
				Stream = null;
				Handle.Close();
				Handle.Dispose();
				Handle = null;

				OnDisconnected();
			}

			private void OnDisconnected()
			{
				if( Disconected != null )
				{
					Disconected( this, EventArgs.Empty );
				}
			}

			private void OnMessageReceived( byte[] buffer, ASCIIEncoding encoder, int bytesRead )
			{
				// fire message received event
				if( MessageReceived != null )
				{
					MessageReceived( encoder.GetString( buffer, 0, bytesRead ) );
				}
			}
		}
	}
}
