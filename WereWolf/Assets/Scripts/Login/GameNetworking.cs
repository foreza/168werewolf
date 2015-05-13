using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


public class GameNetworking : MonoBehaviour {
	
	// The port number for the remote lobby server.
	private const int portGame = 11002;

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
	

	// ManualResetEvent instances signal completion.
	private static ManualResetEvent connectDoneGame = 
		new ManualResetEvent(false);
	private static ManualResetEvent sendDoneGame = 
		new ManualResetEvent(false);
	private static ManualResetEvent receiveDoneGame = 
		new ManualResetEvent(false);

	// The response from the remote device.
	private static String responseGame = String.Empty;

	public static String myPlayerID;


	public void BeginGame()
	{
		StartGame ("joinGame");
	}

	public void PassPosition(String s)
	{
		StartGame ("position" + "|" + myPlayerID + "|" + s + "|");
	}
	
	private static void StartGame(String s) {
		// Connect to a remote device.
		try {
			// Establish the remote endpoint for the socket.
			// The name of the 
			// remote device is "host.contoso.com".
			print ("Beginning connection to game.");
			
			IPHostEntry ipHostInfo = Dns.GetHostEntry("174.77.35.116");
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, portGame);
			
			// Create a TCP/IP socket.
			Socket client = new Socket(AddressFamily.InterNetwork,
			                           SocketType.Stream, ProtocolType.Tcp);
			
			// Connect to the remote endpoint.
			client.BeginConnect( remoteEP, new AsyncCallback(ConnectCallbackGame), client);
			connectDoneGame.WaitOne();
			
			print ("Connected to game. Sending and recieving data.");
			
			// Send test data to the remote device.
			// Server will recieve "joinGame or joinGame" and will accept the request.
			SendGame(client,s + "<EOF>");
			sendDoneGame.WaitOne();
			
			// Receive the responseGame from the remote device.
			ReceiveGame(client);
			receiveDoneGame.WaitOne();
			
			// Write the response to the console.
			print ("Response received : {0}" + responseGame);

			if(responseGame.Contains("welcome"))
			{
				// Assigned played ID here
				myPlayerID = responseGame.Substring(9);
				print ("I was assigned this player ID: " + myPlayerID);
				
			}
			else if (s =="joinGame")
			{
				print("Confirmed, beginning game instance");

			}

			// Release the socket.
			client.Shutdown(SocketShutdown.Both);
			client.Close();
			
		} catch (Exception e) {
			print(e.ToString());
		}
	}
	
	private static void ConnectCallbackGame(IAsyncResult ar) {
		try {
			// Retrieve the socket from the state object.
			Socket client = (Socket) ar.AsyncState;
			
			// Complete the connection.
			client.EndConnect(ar);
			
			print("Socket connected to {0}" + client.RemoteEndPoint.ToString());
			
			// Signal that the connection has been made.
			connectDoneGame.Set();
		} catch (Exception e) {
			print(e.ToString());
		}
	}
	
	private static void ReceiveGame(Socket client) {
		try {
			// Create the state object.
			StateObject state = new StateObject();
			state.workSocket = client;
			
			// Begin receiving the data from the remote device.
			client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,
			                    new AsyncCallback(ReceiveCallbackGame), state);
		} catch (Exception e) {
			print(e.ToString());
		}
	}
	
	private static void ReceiveCallbackGame( IAsyncResult ar ) {
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
				                    new AsyncCallback(ReceiveCallbackGame), state);
			} else {
				// All the data has arrived; put it in response.
				if (state.sb.Length > 1) {
					responseGame = state.sb.ToString();
				}
				// Signal that all bytes have been received.
				receiveDoneGame.Set();
				
				// 
				
			}
		} catch (Exception e) {
			print(e.ToString());
		}
	}
	
	private static void SendGame(Socket client, String data) {
		// Convert the string data to byte data using ASCII encoding.
		byte[] byteData = Encoding.ASCII.GetBytes(data);
		
		// Begin sending the data to the remote device.
		client.BeginSend(byteData, 0, byteData.Length, 0,
		                 new AsyncCallback(SendCallbackGame), client);
	}
	
	private static void SendCallbackGame(IAsyncResult ar) {
		try {
			// Retrieve the socket from the state object.
			Socket client = (Socket) ar.AsyncState;
			
			// Complete sending the data to the remote device.
			int bytesSent = client.EndSend(ar);
			print ("Sent {0} bytes to server." + bytesSent);
			
			// Signal that all bytes have been sent.
			sendDoneGame.Set();
		} catch (Exception e) {
			print(e.ToString());
		}
	}
	
	public static int Main(String[] args) {
		return 0;
	}
}
