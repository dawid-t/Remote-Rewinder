using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace RemoteRewinder
{
	public class Communication
	{
		private byte[] receiveBuffer;
		private int tcpPort;
		private string myIP;
		private Socket socket, clientSocket;
		private UdpClient udpSocket;
		private System.Threading.Timer sendServerDataTimer, lostConnectionTimer;
		private KeysController keysController;
		private RemoteRewinderForm form;
		private Label label_StatusDesc;
		private TextBox textBox_Port;

		public Socket Socket { get { return socket; } }


		public Communication(KeysController keysController, RemoteRewinderForm form, Label label_StatusDesc, TextBox textBox_Port)
		{
			this.keysController = keysController;
			this.form = form;
			this.label_StatusDesc = label_StatusDesc;
			this.textBox_Port = textBox_Port;
			GetIP();
		}

		private void GetIP()
		{
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach(var ip in host.AddressList)
			{
				if(ip.AddressFamily == AddressFamily.InterNetwork)
				{
					myIP = ip.ToString();
				}
			}
		}

		#region TCP sockets:
		public bool StartListenning()
		{
			try
			{
				// Check ports:
				int udpPort = int.Parse(textBox_Port.Text);
				if(!CheckUDPPort(ref udpPort))
				{
					return false;
				}

				// Create TCP socket and start listening for client:
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.Bind(new IPEndPoint(IPAddress.Parse(myIP), GetFreePort(udpPort)));
				
				tcpPort = ((IPEndPoint)socket.LocalEndPoint).Port;
				socket.Listen(1);
				socket.BeginAccept(AcceptCallback, null);

				form.ChangeStatus(RemoteRewinderForm.ConnectingStatus.Waiting); // Change info about connecting status for user.
				StartSendingServerData(udpPort); // Create UDP socket and start sending server data over LAN.
				return true;
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message+": StartListenning");
				StopListenning();
				ShowErrorMessage();
				return false;
			}
		}

		private void AcceptCallback(IAsyncResult ar)
		{
			try
			{
				clientSocket = socket.EndAccept(ar);
				SetLostConnectionTimer();
				form.ChangeStatus(RemoteRewinderForm.ConnectingStatus.Online);
				form.PlaySound(true);
				StopSendingServerData();

				receiveBuffer = new byte[64];
				clientSocket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message+": AcceptCallback");
				StopListenning();
			}
		}

		private void ReceiveCallback(IAsyncResult ar)
		{
			try
			{
				int bytesCount = clientSocket.EndReceive(ar);
				if(bytesCount > 0) // If bytesCount == 0 then client is disconnected.
				{
					byte[] data = new byte[bytesCount];
					Array.Copy(receiveBuffer, data, bytesCount);
					string message = Encoding.ASCII.GetString(data);

					if(message.Equals("ping"))
					{
						SetLostConnectionTimer();
						byte[] buffer = Encoding.ASCII.GetBytes("pong");
						clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, null, null);
					}
					else if(message.Equals("minus"))
					{
						keysController.PressKey(true); // Left.
					}
					else if(message.Equals("plus"))
					{
						keysController.PressKey(false); // Right.
					}

					receiveBuffer = new byte[64];
					clientSocket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
				}
				else
				{
					if(lostConnectionTimer != null)
					{
						lostConnectionTimer.Dispose();
					}
					DisconnectClient();
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message+": ReceiveCallback");
			}
		}

		private void SetLostConnectionTimer() // This is necessary if we want to detect a loss of client connection.
		{
			if(lostConnectionTimer != null)
			{
				lostConnectionTimer.Dispose(); // Turn off the old 8-second timer because the client has sent ping to the server.
			}
			lostConnectionTimer = new System.Threading.Timer(new TimerCallback(ClientLostConnection), null, 8000, Timeout.Infinite); // New the 8-second timer.
		}

		private void ClientLostConnection(object timerState)
		{
			DisconnectClient(); // If client doesn't send a ping within 8s then server will "kick" him.
		}

		private void DisconnectClient()
		{
			if(clientSocket != null)
			{
				// Close the client socket:
				clientSocket.Close();
				clientSocket = null;
				form.PlaySound(false);

				// Start waiting for new client:
				StopListenning();
				StartListenning();
				if(socket != null) // If new TCP socket exists then UDP socket was successfully created too so change the UI again.
				{
					form.ChangeButtonOnText(Properties.Strings.Off, true);
				}
			}
		}

		public void StopListenning()
		{
			try
			{
				if(clientSocket != null)
				{
					clientSocket.Close();
					clientSocket = null;
				}
				if(socket != null)
				{
					socket.Close();
					socket = null;
				}
				StopSendingServerData();
				form.ChangeStatus(RemoteRewinderForm.ConnectingStatus.Offline);
				form.ChangeButtonOnText(Properties.Strings.On, false);
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message+": StopListenning");
				StopSendingServerData();
			}
		}
		#endregion TCP sockets.

		#region UDP sockets:
		private void StartSendingServerData(int port)
		{
			try
			{
				// Create UDP socket with free port:
				udpSocket = new UdpClient(port, AddressFamily.InterNetwork);
				IPEndPoint groupEP = new IPEndPoint(IPAddress.Broadcast, port);
				udpSocket.Connect(groupEP);

				// Send server data over LAN every second:
				sendServerDataTimer = new System.Threading.Timer(new TimerCallback(SendServerData), null, 0, 1000);
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message+": StartSendingServerData");
				StopListenning();
				ShowErrorMessage();
			}
		}

		private void SendServerData(object timerState)
		{
			try
			{
				string message = Environment.MachineName+"-"+tcpPort;
				udpSocket.Send(Encoding.ASCII.GetBytes(message), message.Length);
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message+": SendServerData");
				StopListenning();
				ShowErrorMessage();
			}
		}

		private void StopSendingServerData()
		{
			try
			{
				if(sendServerDataTimer != null)
				{
					sendServerDataTimer.Dispose();
				}
				if(udpSocket != null)
				{
					udpSocket.Close();
					udpSocket = null;
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message+": StopSendingServerData");
				StopListenning();
			}
		}
		#endregion UDP sockets.

		#region Ports:
		private bool CheckUDPPort(ref int port)
		{
			if(port < 1024) // Under port 1024 are system ports which we don't want to use. Change port to minimum available value when is too low.
			{
				textBox_Port.Text = 1024+"";
				port = 1024;
			}
			else if(port > 65535) // Max port value is 65535. Change to this value when is too high.
			{
				textBox_Port.Text = 65535+"";
				port = 65535;
			}

			if(!IsPortAvailable(port, false))
			{
				form.ShowMessage("Port is not available."+Environment.NewLine+"Please try another one.");
				return false;
			}
			else
			{
				return true;
			}
		}

		private bool IsPortAvailable(int port, bool tcpSocket)
		{
			UdpClient udpSocket = null;
			try
			{
				if(tcpSocket)
				{
					IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
					IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners(); // All ports on computer used by TCP sockets.
					
					foreach(IPEndPoint ipEndPoint in ipEndPoints)
					{
						if(ipEndPoint.Port == port)
						{
							return false;
						}
					}
				}
				else
				{
					udpSocket = new UdpClient(port, AddressFamily.InterNetwork); // Check free port by creating temporary UDP socket. This trick doesn't work with TCP sockets.
				}
				return true;
			}
			catch
			{
				return false;
			}
			finally
			{
				if(udpSocket != null)
				{
					udpSocket.Close();
				}
			}
		}

		private int GetFreePort(int port) // Finds free port "similar" to UDP port.
		{
			if(port > 65485) // 65535 is the largest available port number. If number is greater than 49101 then searching is from the smallest numbers.
			{
				port = 1024; // 1024 is the least available port number for "normal user".
			}
			for(int i = 1; i <= 50; i++)
			{
				if(IsPortAvailable(port+i, true))
				{
					return port+i;
				}
			}
			return 0; // If 50 similar ports to UDP port will not be free then IPEndPoint must find some other free port.
		}
		#endregion Ports.

		private void ShowErrorMessage()
		{
			form.ShowMessage(Properties.Strings.Error + Environment.NewLine + Properties.Strings.TryAgain);
			form.ChangeButtonOnText(Properties.Strings.On, false);
		}
	}
}
