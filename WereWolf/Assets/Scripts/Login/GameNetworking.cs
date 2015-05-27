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

	public int loginSize;
	public String myPlayerID;					// Need to know my PID here.

	// The response from the remote device.
	public String responseGame = String.Empty;
	
	void Start () {

		//
		myself = GameObject.Find ("SceneHandler");
	
	}
	
	void Update ()
	{
        if (Application.loadedLevel == 0)
        {
            username = GameObject.Find("Username").GetComponent<InputField>().text;
        }

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


	public void SetMyPID(string i)
	{
		print ("Setting my Player ID");
		myPlayerID = i;
	}

	public void BeginGame()
	{
		// Start the game. Send "join game" to tell the server to initialize initial contact
		SendServerMessage ("joinGame");
		print ("Send server message. Joining game!");
	}

	public void PassPosition(String s)
	{
		// Called by the main game loop to pass positions.
	
		// Old deprecated move method for milestone 3
		//SendServerMessage ("position" + "|" + s + "|"+username+"|0|"); //the zero is a test score


		SendServerMessage ("position" + "|" + myPlayerID + "|" + s + "|");
		// TODO: Incorporate player facing direction and various other things LATER

	}

    public void PassScore(String scoreUpdate)
    {
		print ("Passing this score: " + scoreUpdate);
		SendServerMessage("scoreUpdate"+"|"+username+"|"+scoreUpdate+"|");
    }


	// Method call by game engine to change the state of the game.
	public void PassStateChange(String s)
	{
		// Called by the main game loop to pass positions.
		SendServerMessage ("stateUpdate" + "|" + myPlayerID + "|" + s + "|");



	}
	

	public void SendServerMessage(String s) {
		// Connect to a remote device.
		try {
			
			IPHostEntry ipHostInfo = Dns.GetHostEntry(Networking.IPaddress);
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, portGame);
			// responseGame = String.Empty;		// Start with an empty string.
			
			// Create a TCP/IP socket.
			Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			
			// Connect to the remote endpoint.
			client.BeginConnect( remoteEP, new AsyncCallback(ConnectCallbackGame), client);
			connectDoneGame.WaitOne();

			// Server will recieve an appropriate message and respond accordingly.
			print ("About to send: " + s);
			SendGame(client,s + "<EOF>");
			sendDoneGame.WaitOne();

			// Receive the responseGame from the remote device.
			ReceiveGame(client);
			receiveDoneGame.WaitOne();
			
			// Write the response to the console.
			print ("Response received:" + responseGame);

			// Pass the message to the server messagehandler.
			this.gameObject.SendMessage("HandleServerMessage", responseGame);
	
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

			print ("Waiting for message!");
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
		client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallbackGame), client);
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