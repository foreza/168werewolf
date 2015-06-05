using UnityEngine;
using System.Collections;

public class ServerMessageHandler : MonoBehaviour {

	GameObject player;				// Do not initialize until game starts!
	GameObject sceneHandler;		// Give the scenehandler once DoWelcome is called.
	bool Welcome = false;			// Boolean is set to true after DoWelcome is run.

	void Start () {
	}
	
	void Update () {
	
	}



	public void HandleServerMessage(string s)
	{
		// Welcome = player first message.
		// Recieve from the server the following information:
		// - Current # of players active
		// - My Player ID

		print (" - ServerMessageHandler: handling this server message: " + s);

		if(s.Contains("welcome") && !Welcome)
		{
			// Let DoWelcome function handle.
			DoWelcome(s);
			Welcome = true;			// let this only send once.

		}
		
		else if (s.Contains("update"))
		{
			print("Position + score update: " + s);

			string [] split = s.Split('~');

			string temp = split[0];
			string temp2 = split[1];

			print ("Attempting to communicate with playerTrack");
			sceneHandler.SendMessage("UpdatePlayerTracking", temp);
			player = GameObject.Find("BBPlayer");			// save the reference
			player.SendMessage("UpdateScoreBoard", temp2);
			print ("Should have to communicate with playerTrack");


		}

		// This method is invoked at the end of the game. 
		// Response game should contain some useful information.
		else if (s.Contains("tower"))
		{
			// End the connection.
			print ("Tower message recieved! Time to turn on the tower.");
	
		}

	

        // This method is invoked at the end of the game. 
        // Response game should contain some useful information.
        else if (s.Contains("endGame"))
        {
			// End the connection.
			print ("Closing server connection.");
			this.gameObject.SendMessage("ShutDownServerConnection");
        }


		print ("Waiting for more heartbeams.... <3");
	
		
	}


	// Called by
	public void DoWelcome(string s)	
	{
		// Helpful debug statement.
		print ("Recieved welcome message: " + s);
		
		// Get a reference 
		sceneHandler = GameObject.Find ("SceneHandler");
		
		
		string [] splitResp = s.Split('|');
		
		// Assigned played ID here (used to keep track of my ID)
		this.gameObject.SendMessage("SetMyPID", splitResp[0].Substring(9));
		
		// Set the spawner ID as well. (used by the spawner)
		this.gameObject.SendMessage("SetMySpawnerPID", splitResp[0].Substring(9));
		
		// get the login size, |"9<EOF>"



		int loginSize = int.Parse (splitResp[1].Substring(0,splitResp[1].Length-5));
		
		// Helpful debug statement.
		print (" I was assigned this player ID: " + splitResp[0].Substring(9) + " , currently this many players: " + loginSize);
		
		this.gameObject.SendMessage("SetTrack", loginSize);
		
		// Invokes the network spawner to spawn the initial players.
		// The players will be moved to the appropriate places once an update is recieved.
		// Spawn 1 less to keep track of the player
		this.SendMessage("SpawnInitialPlayers", loginSize-1);

		// Signal to the manager we are ready.
		this.gameObject.SendMessage("SignalManagerReady");

		
	}
}
