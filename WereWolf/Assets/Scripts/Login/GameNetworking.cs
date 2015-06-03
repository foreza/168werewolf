using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


public class GameNetworking : MonoBehaviour {
	
	// The port number for the remote lobby server.
	public int portGame = 0;			// this should change upon login.
	public string roomName = "";
	public GameObject myself;
    public string username;
	public bool noSpawn = true;

	public bool gameInstanceBegin;
	public int loginSize;
	public static String myPlayerID;					// Need to know my PID here.

	Socket myServer;						// Socket for the remote server.


	public int msgCount = 0;
	// The response from the remote device.
	public String responseGame = String.Empty;
	
	void Start () {
		gameInstanceBegin = false;
		myself = GameObject.Find ("SceneHandler");
	
	}
	
	void Update ()
	{
        if (Application.loadedLevel == 0)
        {
            username = GameObject.Find("Username").GetComponent<InputField>().text;
        }

	}

	public string GetMyID()
	{
		return myPlayerID;
	}

	public void setGamePort(string [] s)
	{
		portGame = int.Parse(s[2]);
		roomName = s[1];
		print ("RoomName: " + roomName + "Port set to: " + portGame);

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
	
	// ManualResetEvent instances signal completion.
	public static ManualResetEvent connectDoneGame = 
		new ManualResetEvent(false);
	public static ManualResetEvent sendDoneGame = 
		new ManualResetEvent(false);
	public static ManualResetEvent receiveDoneGame = 
		new ManualResetEvent(false);
	public static ManualResetEvent receiveDoneSpawn = 
		new ManualResetEvent(false);


	// A manualresetevent for just this threaded listener method:
	
	public ManualResetEvent allDoneGame = new ManualResetEvent(false);



	public void SetMyPID(string i)
	{
		print ("Setting my Player ID");
		myPlayerID = i;
	}

	// Is called by ButtonHandler to begin the game.
	public void StartTheGame()
	{
		if(!gameInstanceBegin)
		{
			print("Initializing Server connection!");

			//Send "join game" to tell the server to initialize initial contact
			//SendServerMessage ("joinGame");

			Thread isc = new Thread(InitializeServerConnection);
			isc.Start();

			//InitializeServerConnection();		// initialize the server connection.

			print("Game Thread active!");


			// Set gameInstanceBegin to true; message should only be sent once.
			gameInstanceBegin = true;
		}

		else
		{
			print("Game instance already active; cannot begin game again!");
		}
	}


	// 	This is run ONCE.
	public void InitializeServerConnection()
	{
		try {
			connectDoneGame.Reset();

			IPHostEntry ipHostInfo = Dns.GetHostEntry(Networking.IPaddress);
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, portGame);
			// responseGame = String.Empty;		// Start with an empty string.
			
			// Create the TCP/IP socket using my Client.
			myServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


			print ("Attempting to join game instance! + " + remoteEP.Address);

			//return;
			// Begin Connect to the remote endpoint.
			myServer.BeginConnect( remoteEP, new AsyncCallback(ConnectCallbackGame), myServer);
			connectDoneGame.WaitOne();
			
			// Server will recieve an appropriate message and respond accordingly.

			// Send the server a status message.
			SendGame(myServer,"joinGame<EOF>");
			sendDoneGame.WaitOne(2000);

			print ("Received data from remote server!!111");

			// Receive the responseGame from the remote device.
			ReceiveGame(myServer);
			receiveDoneGame.WaitOne(2000);
			
			print ("Handling initial welcome message....");


			// Pass the confirmation message to the server messagehandler.
			this.gameObject.SendMessage("HandleServerMessage", responseGame);

			// Now, begin the active listener.

			print ("Game listening is now beginning.");


			Thread lt = new Thread(StartGameListening); // Start it on a seperate thread.
			

		} catch (Exception e) {
			print(e.ToString());
		}

	}

	// Belongs to the initialize game thread. This is good.
	public  void ConnectCallbackGame(IAsyncResult ar) {
		try {
			// Retrieve the socket from the state object.
			Socket client = (Socket) ar.AsyncState;
			
			// Complete the connection.
			client.EndConnect(ar);
			
			print(msgCount + " - Socket connected to {0}" + client.RemoteEndPoint.ToString());
			
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
	
	public  void ReceiveCallbackGame( IAsyncResult ar ) {
		try {
			// Retrieve the state object and the client socket 
			// from the asynchronous state object.
			StateObject state = (StateObject) ar.AsyncState;
			Socket client = state.workSocket;
			
			//print ("Waiting for message!");
			// Read data from the remote device.
			int bytesRead = client.EndReceive(ar);

			print ("ReceiveCallbackGame hit.");
			
			if (bytesRead > 0) {

				print ("Waitin on more data!" );

				// There might be more data, so store the data received so far.
				state.sb.Append(Encoding.ASCII.GetString(state.buffer,0,bytesRead));
				
				// Get the rest of the data.
				client.BeginReceive(state.buffer,0,StateObject.BufferSize,0,
				                    new AsyncCallback(ReceiveCallbackGame), state);
			} else {
				// All the data has arrived; put it in response.
				if (state.sb.Length > 1) {
					responseGame = state.sb.ToString();

					print ("ReceiveCallbackGame done, result: " + responseGame);


				}
				// Signal that all bytes have been received.
				receiveDoneGame.Set();
				
				// 
				
			}
		} catch (Exception e) {
			print(e.ToString());
		}
	}






	// This begins a listener on the client that receive and processes updates.
	public  void StartGameListening() {
		// This is dangerous; make sure to run the Game listener before the status checks.
		
		Console.WriteLine("Client now listening for updates.");
		
		// Data buffer for incoming data.
		byte[] bytes = new Byte[1024];



		IPHostEntry lgameipHostInfo;
		IPAddress lgameipAddress;
		IPEndPoint serverEndPoint;


		// Establish the local endpoint for the server socket on the client side.
		lgameipHostInfo = Dns.Resolve(Dns.GetHostName());
		lgameipAddress = lgameipHostInfo.AddressList[0];
		serverEndPoint = new IPEndPoint(lgameipAddress, portGame);
		
		// Create a TCP/IP socket that is essentially the same as myServer, but will be used only by this THREAD.
		Socket listener = new Socket(AddressFamily.InterNetwork,
		                             SocketType.Stream, ProtocolType.Tcp);
		
		// Bind the socket to the local endpoint and listen for incoming connections.
		try {
			listener.Bind(serverEndPoint);
			listener.Listen(100);
			
			// Handle Game entrances here.
			while (true) {
				// Set the event to nonsignaled state.
				allDoneGame.Reset();
				
				//Console.WriteLine("Game room: [" + RoomName + "] is active. :)");
				// Start an asynchronous socket to listen for connections.
				listener.BeginAccept(new AsyncCallback(AcceptGameCallback), listener);
				
				// Wait until a connection is made before continuing.
				allDoneGame.WaitOne();
				
			}
			
		}
		catch (Exception e) {
			Console.WriteLine(e.ToString());
		}
		
		Console.WriteLine("\nPress ENTER to continue...");
		Console.Read();
		
	}

	// Only run/called by the GameListening thread/method.
	public  void AcceptGameCallback(IAsyncResult ar) {
		// Signal the main thread to continue.
		allDoneGame.Set();
		
		// Get the socket that handles the client request.
		Socket alistener = (Socket)ar.AsyncState;
		Socket ahandler = alistener.EndAccept(ar);
		
		// Create the state object.
		StateObject astate = new StateObject();
		astate.workSocket = ahandler;
		ahandler.BeginReceive(astate.buffer, 0, StateObject.BufferSize, 0,
		                     new AsyncCallback(ReadGameCallback), astate);
	}

	// Only run/called by the GameListening thread/method.
	public void ReadGameCallback(IAsyncResult ar) {
		String content = String.Empty;
		
		// Retrieve the state object and the handler socket
		// from the asynchronous state object.
		StateObject astate = (StateObject)ar.AsyncState;
		Socket ahandler = astate.workSocket;
		
		// Read data from the client socket. 
		int bytesRead = ahandler.EndReceive(ar);
		
		if (bytesRead > 0) {
			// There  might be more data, so store the data received so far.
			astate.sb.Append(Encoding.ASCII.GetString(
				astate.buffer, 0, bytesRead));
			
			// Check for end-of-file tag. If it is not there, read 
			// more data.
			content = astate.sb.ToString();

			if (content.IndexOf("<EOF>") > -1) {
				// All the data has been read from the server.


				if (content.Contains("update")) {
					// Let HandleServerMessage process the update and move on.
					print("Received update from server; letting server message handler handle.");
					this.gameObject.SendMessage("HandleServerMessage", content);

				}
				

			}
			else {
				// Not all data received. Get more.
				ahandler.BeginReceive(astate.buffer, 0, StateObject.BufferSize, 0,
				                     new AsyncCallback(ReadGameCallback), astate);
			}
		}
	}



	public void ShutDownServerConnection()
	{
		// Release the socket.
		myServer.Shutdown(SocketShutdown.Both);
		myServer.Close();
	}


	// Main form of connection with remote server.
	// Note: Make sure myServer is define first by initializing the connection with InitializeServerConnection
	public void SendServerMessage(String s) {
	
		try {

			// Send the server a message.
			SendGame(myServer,s + "<EOF>");
			sendDoneGame.WaitOne();

			// Receive the responseGame from the remote device. If we have a particular message, flag it down here.
			ReceiveGame(myServer);
			receiveDoneGame.WaitOne();

			print (msgCount + " - Response received:" + responseGame);

			// Pass the message to the server messagehandler.
			this.gameObject.SendMessage("HandleServerMessage", responseGame);

			msgCount++;
			
		} catch (Exception e) {
			print(e.ToString());
		}
	}


	public  void SendGame(Socket client, String data) {
		// Convert the string data to byte data using ASCII encoding.
		byte[] byteData = Encoding.ASCII.GetBytes(data);
		
		// Begin sending the data to the remote device.
		client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallbackGame), client);
	}
	
	public  void SendCallbackGame(IAsyncResult ar) {
		try {
			// Retrieve the socket from the state object.
			Socket client = (Socket) ar.AsyncState;
			
			// Complete sending the data to the remote device.
			int bytesSent = client.EndSend(ar);
			//print ("Sent {0} bytes to server." + bytesSent);
			
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
		SendServerMessage("score"+"|"+username+"|"+scoreUpdate+"|");
		
		// TODO: Do more fun things with the score.
		
	}


}





/*
 * 
 * 
 * 
 * Thinking of new way to recieve messages and spawn.
 * 
 * First step is INITIALIZATION - when the client first connects.
 * The information player gets from the server is:
 *  - a player ID
 * 	- total number of players already connected
 *  -- TODO --
 * ~ POSITION OF ALL PLAYERS/WOLVES (not really needed, gets fixed in update loop ~ // may come in handy next time though
 * Location of all buildings/and their statuses
 *  
 * Next step is UPDATE PER LOOP - the information that is passed every game loop (werewolf transformation can be handled here too..)
 * 
 * 
 * 
 * Another step is event Trigger - the info that is passed when something interesting happens (werewolf changes, game over, lighttower activated etc)
 * 
 * 
 * Crucial step is handle DC - game should run smoothly / have no dependency (assume player dead if they d/c, or something)
 * 
 * Final step is end game - should handle game end.
 * 
 * More things to consider - multiple game servers, etc.
 * 
 */