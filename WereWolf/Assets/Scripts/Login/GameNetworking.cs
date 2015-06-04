using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


public class GameNetworking : MonoBehaviour {
	
	public int portGame = 0;							// The port number for the remote lobby server - this should change upon login.
	public int loginSize = 0;							// # of players already available in the server.

	public bool gameInstanceBegin = false;				// Use this to track whether the game instance has sucessfully been launched.

	Socket myServer;									// Socket for the remote server.

	public string responseGame = String.Empty;			// String that is used to store incoming messages from the remote device.	
	public string responseUpdate = String.Empty;
	public string username = "";						// Username of the playe that can be used in various ways (scoreboard)
	public string roomName = "";						// Name of the room. Set when remote server sends message.

	public string myPlayerID = "";						// Need to know my PID here.

	// ManualResetEvent instances signal completion - used for initial connection w/ server.
	public static ManualResetEvent connectDoneGame = new ManualResetEvent(false);
	public static ManualResetEvent sendDoneGame = new ManualResetEvent(false);
	public static ManualResetEvent receiveDoneGame = new ManualResetEvent(false);
	// For the main listener thread
	public static ManualResetEvent allDoneGameClient = new ManualResetEvent(false);
	public static ManualResetEvent receiveDoneGameClient = new ManualResetEvent(false);

	bool newUpdate = false;
	
	public GameObject g;

	
	void Start () {
		g = GameObject.Find ("SceneHandler");
	}

	// Does nothing at this point; just saves username.
	void Update ()
	{
        if (Application.loadedLevel == 0)
        {
            username = GameObject.Find("Username").GetComponent<InputField>().text;
        }


		// If new update flag aka flag dirty.
		if (newUpdate) {
			print("THERE IS A NEW UPDATE OH MY GOD");			// HELPFUL PRINT STATEMENT.
			StartHeartBeatListen();			// Listen again
			g.SendMessage ("HandleServerMessage", responseGame);			// handle the response
		}

		print ("No new updates.");
	}
	
	// Method called by LobbyNetworking to set the game port. (NOT CALLED BY THE NETWORK MANAGER)
	public void setGamePort(string [] s)
	{
		portGame = int.Parse(s[2]);
		roomName = s[1];
		print ("RoomName: " + roomName + "Port set to: " + portGame);

	}

	// PID is set.
	public void SetMyPID(string i)
	{
		print ("Setting my Player ID as :" + i);
		myPlayerID = i;
	}

	// Is called by ButtonHandler to begin the game.
	public void StartTheGame()
	{
		if(!gameInstanceBegin)
		{
			print("Initializing Server connection!");

			//Thread isc = new Thread(InitializeServerConnection);
			//isc.Start();

			InitializeServerConnection();

			//print("Game Thread active!");


		}

		else
		{
			print("Game instance already active; cannot begin game again!");
		}
	}


	// 	This is run ONCE.
	public void InitializeServerConnection()
	{

		if (!gameInstanceBegin) {
			try {
				connectDoneGame.Reset ();
				receiveDoneGame.Reset();

				IPHostEntry ipHostInfo = Dns.GetHostEntry (Networking.IPaddress);
				IPAddress ipAddress = ipHostInfo.AddressList [0];
				IPEndPoint remoteEP = new IPEndPoint (ipAddress, portGame);

				// Create the TCP/IP socket using my Client.
				myServer = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

				print ("Attempting to join game instance! + " + remoteEP.Address);

				// Begin Connect to the remote endpoint.
				myServer.BeginConnect (remoteEP, new AsyncCallback (ConnectCallbackGame), myServer);
				connectDoneGame.WaitOne ();

				// Send the server a status message.
				SendGame (myServer, "joinGame<EOF>");
				//sendDoneGame.WaitOne();

				print ("Received data from remote server!!111");

				// Receive the responseGame from the remote device.
				ReceiveGame (myServer);
				bool signal = receiveDoneGame.WaitOne (2000);
				if (!signal)
					print ("Game failed");
				print ("Handling initial welcome message....");


				// Set gameInstanceBegin to true; message should only be sent once.
				gameInstanceBegin = true;

				// Pass the confirmation message to the server messagehandler.


				g.SendMessage ("HandleServerMessage", responseGame);
				print ("No heartbeams???");
				// Begin client heartbeat.


				StartHeartBeatListen();


			} catch (Exception e) {
				print (e.ToString ());
			}
		} else
			print ("Tried to run another game instance. Why?");

	}

	// Belongs to the initialize game thread. This is good.
	public  void ConnectCallbackGame(IAsyncResult ar) {
		try {
			// Retrieve the socket from the state object.
			Socket client = (Socket) ar.AsyncState;
			
			// Complete the connection.
			client.EndConnect(ar);
			
			print(" - Socket connected to {0}" + client.RemoteEndPoint.ToString());
			
			// Signal that the connection has been made.
			connectDoneGame.Set();
		} catch (Exception e) {
			print(e.ToString());
		}
	}
	// Belongs to the initialize game thread.!!!!!
	
	public  void ReceiveGame(Socket client) {
		try {
			// Create the state object.


			StateObject state = new StateObject();
			state.workSocket = client;
			
			print ("RecieveGame hit.");
			
			// Begin receiving the data from the remote device.
			client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,
			                    new AsyncCallback(ReceiveCallbackGame), state);
		} catch (Exception e) {
			print(e.ToString());
		}
	}
	// Recieve.
	public  void ReceiveCallbackGame( IAsyncResult ar ) {
		try {
			// Retrieve the state object and the client socket 
			// from the asynchronous state object.
			StateObject state = (StateObject) ar.AsyncState;
			Socket client = state.workSocket;
			String content = String.Empty;

			
			//print ("Waiting for message!");
			// Read data from the remote device.
			int bytesRead = client.EndReceive(ar);


			state.sb.Append(Encoding.ASCII.GetString(
				state.buffer, 0, bytesRead));


			content = state.sb.ToString();

			print ("ReceiveCallbackGame hit: " + content);


			if (content.IndexOf("<EOF>") > -1) {

				responseGame = content;			// store the response string in here!

				newUpdate = true;				// signal to main game loop that there is something new!

			}

			else {
				// All the data has arrived; put it in response.
				if (state.sb.Length > 1) {
					responseGame = state.sb.ToString();

					print ("ReceiveCallbackGame done, result: " + responseGame);


				}
				// Signal that all bytes have been received.

				// 
				
			}

			// Set done recieve.
			receiveDoneGame.Set();



		} catch (Exception e) {
			print(e.ToString());
		}
	}
	

	public void ShutDownServerConnection()
	{
		// Release the socket.
		myServer.Shutdown(SocketShutdown.Both);
		myServer.Close();
	}


	// Main form of connection with remote server.
	// Note: Make sure myServer is defined first by initializing the connection with InitializeServerConnection
	public void SendServerMessage(String s) {
	
		try {

			// Send the server a message, append <EOF> to ensure that remote server sees the end.
			s += "<EOF>";

			// Helpful debug statement.
			 // print ("Sending message: " + s);

			// Send async.
			myServer.BeginSend(Encoding.ASCII.GetBytes(s), 0, Encoding.ASCII.GetBytes(s).Length, 0, null, null);

			
		} catch (Exception e) {
			print(e.ToString());
		}
	}

	// This send method is run just once.
	public  void SendGame(Socket client, String data) {
		
		try {
			print ("Attempting to send join game message.");
			myServer.BeginSend(Encoding.ASCII.GetBytes(data), 0, Encoding.ASCII.GetBytes(data).Length, 0, null, null);
			print ("Async call begun.");
		
		} catch (Exception e) {
			print(e.ToString());
		}
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


    void OnApplicationQuit()
    {
        print("Quitting application...");
        SendServerMessage("goodbye|" + myPlayerID + "|" + username + "|");
    }


	// OTHER METHODS THAT HELP


	// Method called to pass current player position to the server.
	public void PassPosition(String s)
	{
		// Called by the main game loop to pass positions.
		SendServerMessage ("position" + "|" + myPlayerID + "|" + s + "|");
		
		// TODO: Incorporate player facing direction and various other things LATER
		
	}
	
	
	// Method called to pass current player score to the server.
	public void PassScore(String scoreUpdate)
	{
		// Called by the main game loop to pass positions.
		//SendServerMessage("score"+"|"+username+"|"+scoreUpdate+"|");
		
		// TODO: Do more fun things with the score.
		
	}

	// State object for receiving data from remote device.
	public class StateObject {
		// Client socket.
		public Socket workSocket = null;
		// Size of receive buffer.
		public const int BufferSize = 1024;
		// Receive buffer.
		public byte[] buffer = new byte[BufferSize];
		// Received data string.
		public StringBuilder sb = new StringBuilder();
	}

	// This allows players to enter the Game, storing their information into an available data structure of players that the game instances can run on.



	public void StartHeartBeatListen()
	{

			Console.WriteLine ("Now listening for updates. <3 HEARTBEAT@!");
			newUpdate = false;

			//receiveDoneGame.Reset ();

			ReceiveGame (myServer);

			//receiveDoneGame.WaitOne ();

	}



	
	
}


