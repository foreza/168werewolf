using UnityEngine;
using System.Collections;

public class ServerMessageHandler : MonoBehaviour {

	GameObject player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void HandleServerMessage(string s)
	{
		// Welcome = player first message.
		// Recieve from the server the following information:
		// - Current # of players active
		// - My Player ID
		print ("Handling server message: " + s);

		if(s.Contains("welcome"))
		{
			print ("Recieved a welcome message. (HandleServerMessage)");
			string [] splitResp = s.Split('|');

			// Assigned played ID here
			this.gameObject.SendMessage("SetMyPID", splitResp[0].Substring(9));
			this.gameObject.SendMessage("SetMySpawnerPID", splitResp[0].Substring(9));

			// get the login size,
			int loginSize = int.Parse (splitResp[1]);

 
			
			print ("I was assigned this player ID: " + splitResp[0].Substring(9) + " , currently this many players: " + loginSize);
			this.gameObject.SendMessage("SetTrack", loginSize);

			// Invokes the network spawner to spawn the initial players.
			// The players will be moved to the appropriate places once an update is recieved.
			// Spawn 1 less to keep track of the player
			this.SendMessage("SpawnInitialPlayers", loginSize-1);
			player = GameObject.Find("BBPlayer");			// save the reference
			
			
		}
		
		else if (s.Contains("update"))
		{
			print("Position + score update: " + s);

			string [] split = s.Split('~');

			this.gameObject.SendMessage("UpdateTracking", split[0]);
			player.SendMessage("UpdateScoreBoard", split[1]);
		}
		
		// Update = general updates message
		// Each general update message contains the following:
		// - Location of every player, along with their PID
		// - Total # of players 
		/*
		else if (s.Contains("score"))
		{
			print("Score update: " + s);
			this.gameObject.SendMessage("UpdateScoreBoard", s);
		}
		*/
		// If there is a change in the # of players, invoke the network spawner to be able to handle it.

		
		// This method should be invoked every time an objective is reached.
		else if (s.Contains("stateUpdate"))
		{
			this.gameObject.SendMessage("UpdateStateTracking", s);
			
		}

	
		// This method is invoked at the end of the game. 
		// Response game should contain some useful information.
		else if (s.Contains("endGame"))
		{
			this.gameObject.SendMessage("EndGame", s);
		}

		
	}
}
