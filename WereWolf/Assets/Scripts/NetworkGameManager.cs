using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This class manages the game state of the game from the networked side (aka state, player locations)

public class NetworkGameManager : MonoBehaviour {


	
	public int trackSize;								// keeps track of the current # of players. Will be compared with playersTracking.size to see if we need to track a new player.
	public string myID;									// ID of this player, to avoid excess updates. 

	public NetworkPlayerObj [] limitPlayersTracking;	// Array of players, limitation is now client side.
	public int maxLimitPlayerTracking = 100;
	// OR THIS
	public List<NetworkPlayerObj> playersTracking;		// List of networked players that the client tracks.


	void Start () {

		limitPlayersTracking = new NetworkPlayerObj[maxLimitPlayerTracking];		// Initilize the array of players.
		// OR THIS
		playersTracking = new List<NetworkPlayerObj> ();	// Initialize the list.

		trackSize = 0;										// Initial tracking size is 0.
		
	}
	
	void Update () {
	
	}

	// SETTER METHODS.

	// Sets the initial amount of players being tracked.
	public void SetTrack(int i)
	{
		
		trackSize = i;
		print ("Ready to track this many players: " + trackSize);

	}

	// Sets the initial ID of the player being tracked.
	public void SetMySpawnerPID(string d)
	{
		
		myID = d;
		print ("My ID is: " + d);

	}
	                

	// Adds a player to the current tracking list.
	public void AddPlayerToTrack(NetworkPlayerObj p)
	{
		// Get the ID of the player and store it in the array.
		limitPlayersTracking[int.Parse(p.getID())] = p;
		//OR
		//playersTracking.Add (p);
		
		print ("Player: " + p.getID() + " is now being tracked.");

	}

	// Update loop that is called. String of locations is passed in and parsed and applied.
	//Recieves the string and updates player positions as appropriate here.
	public void UpdateTracking(string s)
	{
		// Split the string.
		s = s.Substring(9);
		string [] split = s.Split('*'); 

		// Debug statement.
		print ("Number of players to process in this packet: " + split.Length);


		// Check the current size with the new size. trackSize != size, spawn a new player.	
		// We set the ID to be the trackSize. If 2 players in game [0][1] and [2] joins
		// trackSize was 2, which will then become 3 after we spawn, so we have [0][1][2] and tracksize 3.	
		if (trackSize < split.Length) {
			// Debug print statement.
			print ("New player has logged in; spawning additional player");

			// send message and add a player
			this.gameObject.SendMessage("SpawnAddPlayer", trackSize);

			// increase track size
			++trackSize;													
		}
		

		// Parse the string and split the data.
		for (int p = 0; p < split.Length; p++)
		{
			// Split the string by the | delimiter to get player ID, X, and Y.
			string [] split2 = split[p].Split('|');

			// Since we require many accesses to the player ID, save it in a variable.
			int givenID = int.Parse(split2[0]);

			// If the ID is not mine, apply the translation for the player.
			if (givenID != int.Parse(myID))
			{
				// Debug print statement.
				print ("Applying player position with this ID: " + givenID + "New loc: " + split2[1] + ", " + split2[2]);
			
				// Search into player tracking to see if they exist, and update the position.
				//playersTracking[p].updatePosition(float.Parse(split2[1]),float.Parse(split2[2]));
				// or this
				limitPlayersTracking[givenID].updatePosition(float.Parse(split2[1]),float.Parse(split2[2]));

			}
		}
	}

	// TODO: Finish this method for game state tracking.
	public void UpdateStateTracking(string s)
	{
		//Recieves the string and updates world game states as appropriate here:
		// Recieve and parse the data and apply it to the game world as necessary

		print ("Recieved Game State update message: " + s);

		
		//TODO: Handle message parsing/passing here

		// Handle case for tower down (make sure to pass ID of the tower)

		// Handle case for haven open

		// Handle case for haven close

		// Handle case for player dead?? (Or just pass player state)


		
	}


	// TODO: Finish this and allow server to call client for this.
	public void EndGame(string s)
	{
		//Recieves the string and updates world game state to END
		// Recieve and parse the end game state and print the proper response

		print ("Recieved Game End message; terminating game instance");

		
	}

	
}
