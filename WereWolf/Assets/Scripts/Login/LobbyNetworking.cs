using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


public class LobbyNetworking : MonoBehaviour {

	// The port number for the remote lobby server.
	private const int portLobby = 11001;

	// A test integer so we can track the message numbers.
	public static int test = 0;

	void Start () {
	}

	void Update ()
	{
	}


	// State object for receiving data from remote device.
	public class StateObject {
		// Client socket.
		public Socket workSocket = null;
		// Size of receive buffer.
		public const int BufferSize = 256;
		// Receive buffer.
		public byte[] buffer = new byte[BufferSize];
		// Received data string.
		public StringBuilder sb = new StringBuilder();
	}
	

	public void ConnectToLobby()
	{
		StartLobby ();
	}
		
		// ManualResetEvent instances signal completion.
		private static ManualResetEvent connectDoneLobby = 
			new ManualResetEvent(false);
	private static ManualResetEvent sendDoneLobby = 
			new ManualResetEvent(false);
	private static ManualResetEvent receiveDoneLobby = 
			new ManualResetEvent(false);
		
		// The response from the remote device.
	private static String responseLobby = String.Empty;
		
		private static void StartLobby() {
			// Connect to a remote device.
			try {
				// Establish the remote endpoint for the socket.
				// The name of the 
				// remote device is "host.contoso.com".
				print ("Beginning connection to lobby.");

				IPHostEntry ipHostInfo = Dns.GetHostEntry("174.77.35.116");
				IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, portLobby);
				
				// Create a TCP/IP socket.
				Socket client = new Socket(AddressFamily.InterNetwork,
				                           SocketType.Stream, ProtocolType.Tcp);
				
				// Connect to the remote endpoint.
			client.BeginConnect( remoteEP, new AsyncCallback(ConnectCallbackLobby), client);
			connectDoneLobby.WaitOne(1000);
				
				print ("Connected to lobby. Sending data.");

				// Send test data to the remote device.
			SendLobby(client,"[" + test + "]ActiveInLobby<EOF>");
			sendDoneLobby.WaitOne(1000);
				
			// Receive the responseLobby from the remote device.
			ReceiveLobby(client);
			receiveDoneLobby.WaitOne(1000);
				
				// Write the response to the console.
			print ("Response received : {0}" + responseLobby + test);
				test++;
				// Release the socket.
				client.Shutdown(SocketShutdown.Both);
				client.Close();
				
			} catch (Exception e) {
				print(e.ToString());
			}
		}
		
	private static void ConnectCallbackLobby(IAsyncResult ar) {
			try {
				// Retrieve the socket from the state object.
				Socket client = (Socket) ar.AsyncState;
				
				// Complete the connection.
				client.EndConnect(ar);
				
				print("Socket connected to {0}" + client.RemoteEndPoint.ToString());
				
				// Signal that the connection has been made.
			connectDoneLobby.Set();
			} catch (Exception e) {
				print(e.ToString());
			}
		}
		
	private static void ReceiveLobby(Socket client) {
			try {
				// Create the state object.
				StateObject state = new StateObject();
				state.workSocket = client;
				
				// Begin receiving the data from the remote device.
				client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,
			                    new AsyncCallback(ReceiveCallbackLobby), state);
			} catch (Exception e) {
				print(e.ToString());
			}
		}
		
	private static void ReceiveCallbackLobby( IAsyncResult ar ) {
			try {
				// Retrieve the state object and the client socket 
				// from the asynchronous state object.
				StateObject state = (StateObject) ar.AsyncState;
				Socket client = state.workSocket;
				
				// Read data from the remote device.
				int bytesRead = client.EndReceive(ar);
				
				if (bytesRead > 0) {
					// There might be more data, so store the data received so far.
					state.sb.Append(Encoding.ASCII.GetString(state.buffer,0,bytesRead));
					
					// Get the rest of the data.
					client.BeginReceive(state.buffer,0,StateObject.BufferSize,0,
				                    new AsyncCallback(ReceiveCallbackLobby), state);
				} else {
					// All the data has arrived; put it in response.
					if (state.sb.Length > 1) {
					responseLobby = state.sb.ToString();
					}
					// Signal that all bytes have been received.
				receiveDoneLobby.Set();
				}
			} catch (Exception e) {
				print(e.ToString());
			}
		}
		
	private static void SendLobby(Socket client, String data) {
			// Convert the string data to byte data using ASCII encoding.
			byte[] byteData = Encoding.ASCII.GetBytes(data);
			
			// Begin sending the data to the remote device.
		client.BeginSend(byteData, 0, byteData.Length, 0,
		                 new AsyncCallback(SendCallbackLobby), client);
		}
		
	private static void SendCallbackLobby(IAsyncResult ar) {
			try {
				// Retrieve the socket from the state object.
				Socket client = (Socket) ar.AsyncState;
				
				// Complete sending the data to the remote device.
				int bytesSent = client.EndSend(ar);
				print ("Sent {0} bytes to server." + bytesSent);
				
				// Signal that all bytes have been sent.
				sendDoneLobby.Set();
			} catch (Exception e) {
				print(e.ToString());
			}
		}
		
		public static int Main(String[] args) {
			return 0;
		}
	}
