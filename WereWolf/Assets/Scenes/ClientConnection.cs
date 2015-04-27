using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


public class ClientConnection : MonoBehaviour {


		public static bool loggedIn = false;						// We are not logged in at the start.
		public bool logMessage = false;
		public static string userDisplayName;
		private const int port = 11000;						// The port number for the remote device.
		// private static bool connectToServer = false;	
		public static string address = "174.77.35.116";
		
	// ManualResetEvent instances signal completion.
	private static ManualResetEvent connectDone = 
		new ManualResetEvent(false);
	private static ManualResetEvent sendDone = 
		new ManualResetEvent(false);
	private static ManualResetEvent receiveDone = 
		new ManualResetEvent(false);
		// The response from the remote device.
		private static String response = "";
	
	// Use this for initialization
	void Start () {

		loggedIn = false;									// Redundant, but I'll do it here anyway.
		//StartClient ();

		
	}
	
	// Update is called once per frame
	void Update () {

		// Print a message once in the update loop indicating we are now logged in.
		if (loggedIn && !logMessage) {
			print("You are logged in!");
			logMessage = true;
		}

		if (!loggedIn) {
			//print("You are NOT LOGGED IN");
		}
			

	}


	// We have this method called when the user hits "login" on the client window. 
	// The method is given an object array that has two values - username and password.
	// It then searches it up in the DB.
	public void handleLogIn(String[] login){

		// Handle the parameters.
		String user = login [0];
		String pass = login [1];
		address = login [2];

		print ("yay!");
		// Pass to the database here...

		// Return confirm message on success.
		// TODO: IMPLEMENT DATABASE
		if (true) {
			userDisplayName = login[0];
			print ("Confirmed! Logging in...");
			StartCoroutine(StartClient());
		}

		print ("Incorrect! Not logging in...");
	}
	IEnumerator StartClient() {
			// Connect to a remote device.
			try {
				// Establish the remote endpoint for the socket.
				// The name of the 
				// remote device is "host.contoso.com".
			// This is hard coded in for now.
<<<<<<< HEAD
			//string address = "174.77.35.116"; //Jason's hardcoded IP
                string address = "169.234.54.128"; //Connor's hardcoded IP
=======
>>>>>>> 358ca0ddb28b8e6e086dcad42f6f1b83ab0b49b3
			print ("Starting connection. Connection: " + address);
		
			IPHostEntry ipHostInfo = Dns.GetHostEntry(address);
			// print ("Starting connection. connection: " + iNeedanAddress);

			// This is where the client will hang if it can't get the DNS host entry.
			//TODO: Have a way for the client to "safe fail" out of the state.

				// IPHostEntry ipHostInfo = Dns.Resolve("host.contoso.com");
				IPAddress ipAddress = ipHostInfo.AddressList[0];
				IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

				// Create a TCP/IP socket.
				Socket client = new Socket(AddressFamily.InterNetwork,
				                           SocketType.Stream, ProtocolType.Tcp);
				

				// Connect to the remote endpoint.
				client.BeginConnect( remoteEP, 
				                    new AsyncCallback(ConnectCallback), client);
				connectDone.WaitOne();



			print ("Connected to server! Welcome.");
				loggedIn = true;

				
				// Send test data to the remote device.
			Send(client,"Player [" + userDisplayName + "] says: This is a test! Hello, server.<EOF>");
			// YOU NEED TO END IT WITH <EOF> OMG.
			//Send(client,"This is a test<EOF>");
				sendDone.WaitOne();
				
				// Receive the response from the remote device.
				Receive(client);
				receiveDone.WaitOne();
				
				// Write the response to the console.
				print("Response received : {0}" + response);
				
				// Release the socket.
				client.Shutdown(SocketShutdown.Both);
				client.Close();

				print("Client closed connection");
				
			} catch (Exception e) {
				print(e.ToString());
			print ("FAILED HERE1");
			Application.Quit();

			}

		yield return new WaitForSeconds(1);
	}
		
		private static void ConnectCallback(IAsyncResult ar) {
			try {
				// Retrieve the socket from the state object.
				Socket client = (Socket) ar.AsyncState;
				
				// Complete the connection.
				client.EndConnect(ar);
				
				print("Socket connected to {0}" + 
				                  client.RemoteEndPoint.ToString());
				
				// Signal that the connection has been made.
				connectDone.Set();
			} catch (Exception e) {
				print(e.ToString());
			print ("FAILED HERE2");
			Application.Quit();

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
			print ("FAILED HERE3");
			Application.Quit();
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
					state.sb.Append(Encoding.ASCII.GetString(state.buffer,0,bytesRead));
					
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
			print ("FAILED HERE4");
			Application.Quit();
			}
		}
		
		private static void Send(Socket client, String data) {
			// Convert the string data to byte data using ASCII encoding.
			byte[] byteData = Encoding.ASCII.GetBytes(data);
			
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
				print("Sent {0} bytes to server." + bytesSent);
				
				// Signal that all bytes have been sent.
				sendDone.Set();
			} catch (Exception e) {
				print(e.ToString());
				print ("FAILED HERE5");
				Application.Quit();
			}
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

	

}