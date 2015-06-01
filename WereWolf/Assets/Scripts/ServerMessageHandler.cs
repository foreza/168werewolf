using UnityEngine;
using System.Collections;

public class ServerMessageHandler : MonoBehaviour {

	GameObject player;				// Do not initialize until game starts!
	GameObject sceneHandler;
	int msgCount = 0;
	bool Welcome = false;

	void Start () {
	}
	
	void Update () {
	
	}

	public void DoWelcome(string s)
	{
			// Helpful debug statement.
			print ("Recieved welcome message.");
			string [] splitResp = s.Split('|');

			// Assigned played ID here
			this.gameObject.SendMessage("SetMyPID", splitResp[0].Substring(9));
			this.gameObject.SendMessage("SetMySpawnerPID", splitResp[0].Substring(9));

			// get the login size,
			int loginSize = int.Parse (splitResp[1]);

 
			
			print (msgCount + " I was assigned this player ID: " + splitResp[0].Substring(9) + " , currently this many players: " + loginSize);
			this.gameObject.SendMessage("SetTrack", loginSize);

			// Invokes the network spawner to spawn the initial players.
			// The players will be moved to the appropriate places once an update is recieved.
			// Spawn 1 less to keep track of the player
			this.SendMessage("SpawnInitialPlayers", loginSize-1);
			player = GameObject.Find("BBPlayer");			// save the reference
			sceneHandler = GameObject.Find ("SceneHandler");



		msgCount++;

	}

	public void HandleServerMessage(string s)
	{
		// Welcome = player first message.
		// Recieve from the server the following information:
		// - Current # of players active
		// - My Player ID

		print (msgCount + " - ServerMessageHandler: handling this server message: " + s);

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
			//player.SendMessage("UpdateScoreBoard", temp2);
			print ("Should have to communicate with playerTrack");

		}
	
		
		// This method should be invoked every time an objective is reached.
		else if (s.Contains("stateUpdate"))
		{
			this.gameObject.SendMessage("UpdateStateTracking", s);
			
		}

        else if (s.Contains("disconnection"))
        {
        }


        // This method is invoked at the end of the game. 
        // Response game should contain some useful information.
        else if (s.Contains("endGame"))
        {
            this.gameObject.SendMessage("EndGame", s);
        }

		
	}
}
