using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


public class GameNetworking : MonoBehaviour {
	
	// The port number for the remote lobby server.
	public const int portGame = 11002;
	public GameObject myself;
	public bool noSpawn = true;
	public int loginSize;

	public  ArrayList playersTracking; // fill with PlayerObjects and iterate.
	void Start () {
	
		myself = GameObject.Find ("SceneHandler");
		playersTracking = new ArrayList();
	}
	
	void Update ()
	{

		if (Input.GetMouseButtonDown (0)) { 
		}
	}

	public class PlayerObj
	{
		public String playerID;
		public GameObject g;

		public PlayerObj()
		{
			// Empty constructor.
			playerID = "new";
		}
		public PlayerObj(String id, GameObject game)
		{
			playerID = id;
			g = game;
		}

		public void updatePosition(float x, float y)
		{
			g.transform.position = new Vector3 (x, y);
			print ("Moving other player (not me): " + g.transform.position);
		}

		public String getID()
		{
			return playerID;
		}

		public GameObject getObj()
		{
			return g;
		}
		// Shouldn't need to get the position, we're just settting.

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
	public static ManualResetEvent connectDoneGame = 
		new ManualResetEvent(false);
	public static ManualResetEvent sendDoneGame = 
		new ManualResetEvent(false);
	public static ManualResetEvent receiveDoneGame = 
		new ManualResetEvent(false);
	public static ManualResetEvent receiveDoneSpawn = 
		new ManualResetEvent(false);

	// The response from the remote device.
	public String responseGame = String.Empty;

	public  String myPlayerID;


	public void BeginGame()
	{
		StartGame ("joinGame");
	}

	public void PassPosition(String s)
	{
		StartGame ("position" + "|" + myPlayerID + "|" + s + "|");
	}

	public GameObject toSpawn;

	public void SpawnNew(int n)
	{
		print ("Recieved param: " + n);

		for (int i = 0; i < n; i++)
		{

			GameObject go = new GameObject();
			go = (GameObject)Instantiate(Resources.Load("OtherPlayer")); ;
			PlayerObj p = new PlayerObj("new", go);
			playersTracking.Add(p); // Add several Player objects to aid in expansion!
		}

	}
	
	public void StartGame(String s) {
		// Connect to a remote device.
		try {
			// Establish the remote endpoint for the socket.
			// The name of the 
			// remote device is "host.contoso.com".
			print ("Beginning connection to game.");
			
			IPHostEntry ipHostInfo = Dns.GetHostEntry(Networking.IPaddress);
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
				String [] splitResp = responseGame.Split('|');

				myPlayerID = splitResp[0].Substring(9);
				loginSize = int.Parse (splitResp[1]);
				print ("I was assigned this player ID: " + myPlayerID + " , currently this many players: " + loginSize);

				receiveDoneSpawn.WaitOne(1000);

				
			}
			else if (responseGame.Contains("update"))
			{
				// Recieve and parse the data and apply it to the game world as necessary
				if(noSpawn)
				{
				noSpawn = false;
				int trimm = loginSize - 1;
					SpawnNew(trimm);
				}

				print ("I'm processing this: " + responseGame);
				responseGame = responseGame.Substring(9);
				String [] split = responseGame.Split('*'); 
				// Split[0] = [player|pos1|pos2|EOF
				// * denotes each player.
				//int newSize = int.Parse(split[0]);			// Check if we need to force refresh.
				//if(newSize > loginSize)
				{
					//SpawnNew(1);
					//loginSize = newSize;
				}

				for (int p = 0; p < split.Length; p++)
				{
					// Split the string by the | delimiter to get player ID, X, and Y.
					String [] split2 = split[p].Split('|');
					int givenID = int.Parse(split2[0]);
					String givenX = split2[1];
					String givenY = split2[2];
					// If it isn't my player ID, I need to either add the player to the game world, or update their position.
					//if (givenID != myPlayerID) // 
					{
						// Search into player tracking to see if they exist.
						((PlayerObj)playersTracking[givenID]).updatePosition(float.Parse(split2[1]),float.Parse(split2[2]));
						}
				}
			}


			// Release the socket.
			client.Shutdown(SocketShutdown.Both);
			client.Close();
			
		} catch (Exception e) {
			print(e.ToString());
		}
	}
	
	public  void ConnectCallbackGame(IAsyncResult ar) {
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
	
	public  void ReceiveGame(Socket client) {
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
	
	public  void ReceiveCallbackGame( IAsyncResult ar ) {
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
	
	public  void SendGame(Socket client, String data) {
		// Convert the string data to byte data using ASCII encoding.
		byte[] byteData = Encoding.ASCII.GetBytes(data);
		
		// Begin sending the data to the remote device.
		client.BeginSend(byteData, 0, byteData.Length, 0,
		                 new AsyncCallback(SendCallbackGame), client);
	}
	
	public  void SendCallbackGame(IAsyncResult ar) {
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
	

}
