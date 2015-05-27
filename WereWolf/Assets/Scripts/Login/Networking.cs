using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


// 169.234.32.66
public class Networking : MonoBehaviour {

	// The port number for the remote device.
	private const int port = 11000;
    // IP address for remote device;
    public static string IPaddress;
    public static string username;
    public static string password;
	public static string roomName;

	// A test integer so we can track the message numbers.
	public static int test = 0;

	public static GameObject g;
	void Start () {
		//StartClient();

		g = GameObject.Find("SceneHandler");
		DontDestroyOnLoad(g);
	}

	void BeginLogin (string[] loginPackage)
	{

        username = loginPackage[0];
        password = loginPackage[1];
        IPaddress = loginPackage[2];        
		roomName = loginPackage [3];

        StartClient();

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
	


		
		// ManualResetEvent instances signal completion.
		private static ManualResetEvent connectDone = 
			new ManualResetEvent(false);
		private static ManualResetEvent sendDone = 
			new ManualResetEvent(false);
		private static ManualResetEvent receiveDone = 
			new ManualResetEvent(false);
		private static ManualResetEvent loginDone = 
		new ManualResetEvent(false);

	
	// The response from the remote device.
		private static String response = String.Empty;
		
		private static void StartClient() {
			// Connect to a remote device.
			try {
				// Establish the remote endpoint for the socket.
				// The name of the 
				// remote device is "host.contoso.com".
                IPHostEntry ipHostInfo = Dns.GetHostEntry(IPaddress);
				IPAddress ipAddress = ipHostInfo.AddressList[0];
				IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
				
				// Create a TCP/IP socket.
				Socket client = new Socket(AddressFamily.InterNetwork,
				                           SocketType.Stream, ProtocolType.Tcp);
				
				// Connect to the remote endpoint.
				client.BeginConnect( remoteEP, new AsyncCallback(ConnectCallback), client);
				connectDone.WaitOne(1000);
				
				print ("Connected. Sending data.");

				// Send test data to the remote device.
				Send(client,username+":"+password+":"+roomName+":<EOF>");
				sendDone.WaitOne(1000);
				
				// Receive the response from the remote device.
				Receive(client);
				receiveDone.WaitOne(1000);
				
				// Write the response to the console.
				print ("Response received : {0}" + response);
				// Release the socket.

	
                try {
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
                catch (Exception e){
                    print("SOCKET ALREADY CLOSED");
                }

				// Handle the instance.

			if (response.Contains("success"))
			    { 
				print ("Allow user to login! Waiting 2 seconds...");
				// Log player into the lobby
				loginDone.WaitOne(2000);
				g.SendMessage("ConnectToLobby" , roomName);
			}

			else
			{
				print ("Incorrect login.");
			}
				
				
			} catch (Exception e) {
				print(e.ToString());
			}
		}
		
		private static void ConnectCallback(IAsyncResult ar) {
			try {
				// Retrieve the socket from the state object.
				Socket client = (Socket) ar.AsyncState;
				
				// Complete the connection.
				client.EndConnect(ar);
				
				print("Socket connected to {0}" + client.RemoteEndPoint.ToString());
				
				// Signal that the connection has been made.
				connectDone.Set();
			} catch (Exception e) {
				print(e.ToString());
			}
		}
		
		private static void Receive(Socket client) {
			try {
				// Create the state object.
				StateObject state = new StateObject();
				state.workSocket = client;
				
				// Begin receiving the data from the remote device.
				client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,
				                    new AsyncCallback(ReceiveCallback), state);
			} catch (Exception e) {
				print(e.ToString());
			}
		}
		
		private static void ReceiveCallback( IAsyncResult ar ) {
			try {
				// Retrieve the state object and the client socket 
				// from the asynchronous state object.
				StateObject state = (StateObject) ar.AsyncState;
				Socket client = state.workSocket;
				
				// Read data from the remote device.
				int bytesRead = client.EndReceive(ar);
				
				if (bytesRead > 0) {
					// There might be more data, so store the data received so far.
					state.sb.Append(Encoding.Unicode.GetString(state.buffer,0,bytesRead));
					
					// Get the rest of the data.
					client.BeginReceive(state.buffer,0,StateObject.BufferSize,0,
					                    new AsyncCallback(ReceiveCallback), state);
				} else {
					// All the data has arrived; put it in response.
					if (state.sb.Length > 1) {
						response = state.sb.ToString();
					}
					// Signal that all bytes have been received.
					receiveDone.Set();
				}
			} catch (Exception e) {
				print(e.ToString());
			}
		}
		
		private static void Send(Socket client, String data) {
			// Convert the string data to byte data using Unicode encoding.
			byte[] byteData = Encoding.Unicode.GetBytes(data);
			
			// Begin sending the data to the remote device.
			client.BeginSend(byteData, 0, byteData.Length, 0,
			                 new AsyncCallback(SendCallback), client);
		}
		
		private static void SendCallback(IAsyncResult ar) {
			try {
				// Retrieve the socket from the state object.
				Socket client = (Socket) ar.AsyncState;
				
				// Complete sending the data to the remote device.
				int bytesSent = client.EndSend(ar);
				print ("Sent {0} bytes to server." + bytesSent);
				
				// Signal that all bytes have been sent.
				sendDone.Set();
			} catch (Exception e) {
				print(e.ToString());
			}


		}

}
		

