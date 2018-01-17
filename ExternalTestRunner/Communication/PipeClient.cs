using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using System.IO;

namespace ExternalTestRunner.Communication
{

	public sealed class PipeClient
	{
		[DllImport( "kernel32.dll", SetLastError = true )]
		private static extern SafeFileHandle CreateFile(
		   String pipeName,
		   uint dwDesiredAccess,
		   uint dwShareMode,
		   IntPtr lpSecurityAttributes,
		   uint dwCreationDisposition,
		   uint dwFlagsAndAttributes,
		   IntPtr hTemplate );

		public const uint GENERIC_READ = (0x80000000);
		public const uint GENERIC_WRITE = (0x40000000);
		public const uint OPEN_EXISTING = 3;
		public const uint FILE_FLAG_OVERLAPPED = (0x40000000);

		public event MessageReceivedHandler MessageReceived;
		public event EventHandler Disconnected;

		public const int BUFFER_SIZE = 4096;

		private FileStream _stream;
		private SafeFileHandle _handle;
		private Thread _readThread;

		public bool Connected { get; private set; }

		public string PipeName { get; set; }

		/// <summary>
		/// Connects to the server
		/// </summary>
		public bool Connect()
		{
			_handle = CreateFile( PipeName, GENERIC_READ | GENERIC_WRITE,
				  0, IntPtr.Zero, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, IntPtr.Zero );

			//could not create handle - server probably not running
			if( _handle.IsInvalid )
			{
				return false;
			}

			Connected = true;

			//start listening for messages
			_readThread = new Thread( new ThreadStart( Read ) );
			_readThread.Start();
			return true;
		}

		public void Disconnect()
		{
			if( Connected )
			{
				// clean up resources
				if( _readThread != null )
				{
					_readThread.Abort();
					_readThread = null;
				}
				if( _stream != null )
				{
					_stream.Close();
					_stream.Dispose();
					_stream = null;
				}
				if( _handle != null )
				{
					_handle.Close();
					_handle.Dispose();
					_handle = null;
				}
				Connected = false;
			}
		}

		/// <summary>
		/// Reads data from the server
		/// </summary>
		public void Read()
		{
			_stream = new FileStream( _handle, FileAccess.ReadWrite, BUFFER_SIZE, true );
			byte[] readBuffer = new byte[BUFFER_SIZE];
			ASCIIEncoding encoder = new ASCIIEncoding();
			while( Connected )
			{
				int bytesRead = 0;

				try
				{
					bytesRead = _stream.Read( readBuffer, 0, BUFFER_SIZE );
				}
				catch
				{
					//read error occurred
					break;
				}

				//server has disconnected
				if( bytesRead == 0 )
					break;

				OnMessageReceived( readBuffer, encoder, bytesRead );
			}

			Disconnect();
			OnDisconnected();
		}

		/// <summary>
		/// Sends a message to the server
		/// </summary>
		public void SendMessage( string message )
		{
			if( _stream != null )
			{
				if( _stream.CanWrite )
				{
					ASCIIEncoding encoder = new ASCIIEncoding();
					byte[] messageBuffer = encoder.GetBytes( message );

					_stream.Write( messageBuffer, 0, messageBuffer.Length );
					_stream.Flush();
				}
				else
				{
					Disconnect();
					OnDisconnected();
				}
			}
		}

		private void OnMessageReceived( byte[] readBuffer, ASCIIEncoding encoder, int bytesRead )
		{
			//fire message received event
			if( MessageReceived != null )
			{
				MessageReceived( encoder.GetString( readBuffer, 0, bytesRead ) );
			}
		}

		private void OnDisconnected()
		{
			if( Disconnected != null )
			{
				Disconnected( this, EventArgs.Empty );
			}
		}
	}
}
