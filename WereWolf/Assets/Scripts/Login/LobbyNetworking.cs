using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


public class LobbyNetworking : MonoBehaviour {

	private const int portLobby = 11001;			// The port number for the remote lobby server.
	public static GameObject buttonHandler;			// Button handler scripts/methods reference.
	public static GameObject h;						// Reference to this game object, without using this.<name>



	// ManualResetEvent instances signal completion.
	private static ManualResetEvent connectDoneLobby = new ManualResetEvent(false);
	private static ManualResetEvent sendDoneLobby = new ManualResetEvent(false);
	private static ManualResetEvent receiveDoneLobby = new ManualResetEvent(false);
		
	// The response from the remote device.
	private static String responseLobby = String.Empty;

	private static int messageCounter = 0;

	void Start () {
		// Find scenehandler, and bind it to h.
		h = GameObject.Find("SceneHandler");

		// Find the buttonHandler, bind it to the buttonHandler.

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
	

	// Method that is called by the Networking class, with a given room name.
	public void ConnectToLobby(string r)
	{

		// Recieve the game room name, and append additional characters for ease of handling.
		MessageLobby ("joinLobby~" + r + "~");

	}


	// Handles the messages received from the remote server.
	public  static void LobbyMessageHandler(string s)
	{

		print (messageCounter + "- Handling message: " + s);
		messageCounter++;

		if(s.Contains("welcome"))
			{
				// Load the Title screen.
				Application.LoadLevel("Title");

				// Helpful debug statement.
				print ("Loading Title screen");

				// Split up and handle the response string.
				string [] split = responseLobby.Split('~');

				// Tells the local game client (GameNetworking.cs) to set the assigned port.
				h.SendMessage("setGamePort", split);

			}
	}
		

	// Connect and communicate with the remote server.
	public static void MessageLobby(String s) {
		try {
			// Print debug statement.
			print (messageCounter + " - (MessageLobby)Sending message to lobby server.");

			// Socketing essentials.
			IPHostEntry ipHostInfo = Dns.GetHostEntry(Networking.IPaddress);	// Get address of client host from DNS
			IPAddress ipAddress = ipHostInfo.AddressList[0];					// Declare type ipAddress.
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, portLobby);			// Create a remote endpoint.
				
			// Create a TCP/IP socket with given information.
			Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				
			// Connect to the remote endpoint.
			client.BeginConnect( remoteEP, new AsyncCallback(ConnectCallbackLobby), client);

			// Wait for a response. Note: 3000ms waittimeout specified here!
			connectDoneLobby.WaitOne();
				
			// Print debug statement.
			print ("Sending data to lobby: " + s);

			// Server will recieve "joinLobby or joinGame" and will process the request.
			SendLobby(client,s + "<EOF>");

			// Wait for a response. Note: 3000ms waittimeout specified here!
			sendDoneLobby.WaitOne();
				
			// Receive the responseLobby from the remote device.
			ReceiveLobby(client);

			// Wait for a response. Note: 3000ms waittimeout specified here!
			receiveDoneLobby.WaitOne();
				
			// Print/Debug the response to the console.
			print ("Response received : {0}" + responseLobby);

			// Pass response to be handled.
			LobbyMessageHandler(responseLobby);
			
			// Release and close the socket.
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

				// 

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
