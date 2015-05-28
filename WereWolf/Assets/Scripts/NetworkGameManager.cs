using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkGameManager : MonoBehaviour {

	// This class manages the game state of the game from the networked side (aka state, player locations)

	public List<NetworkPlayerObj> playersTracking;

	public int trackSize;				// keeps track of the # of players that i'm tracking right now.
	public string myID;
	
	// Use this for initialization
	void Start () {
		// Initialize the arraylist.

		playersTracking = new List<NetworkPlayerObj> ();

		trackSize = 0;

		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetTrack(int i)
	{
		trackSize = i;
		print ("Ready to track this many players: " + trackSize);
	}

	public void SetMySpawnerPID(string d)
	{
		print ("My ID is: " + d);
		myID = d;
	}
	                           

	public void AddPlayerToTrack(NetworkPlayerObj p)
	{
		// Adds a new player to the tracking list.
		print ("Player: " + p.getID() + " is now being tracked.");
		playersTracking.Add (p);
		// trackSize++;			// don't increment track size yet.
	}

	// If you add more to packet, change this function
	// TODO: Track more stats
	public void UpdateTracking(string s)
	{
		//Recieves the string and updates player positions as appropriate here:
		// Recieve and parse the data and apply it to the game world as necessary
	
		s = s.Substring(9);
		string [] split = s.Split('*'); 

		//print ("GameManager processing this tracking update: " + s);
		print ("number of players active: " + split.Length);


		// Split[0] = [player|pos1|pos2|EOF
		// * denotes each player.		print ("Recieved Game End message; terminating game instance");
		// Split.length gives you the total amount of players.

		// Check the current size with the new size.
		/*
		if (trackSize < split.Length) {
			// Spawn a new player, and increment trackSize
			print ("New player has arrived; spawning additional player");
			this.gameObject.SendMessage("SpawnAddPlayer");		// send message and add a player
			++trackSize;										// increase track size

		}
		*/

		// Parse the string and split the data.
		for (int p = 0; p < split.Length; p++)
		{

			// Split the string by the | delimiter to get player ID, X, and Y.
			string [] split2 = split[p].Split('|');
			int givenID = int.Parse(split2[0]);
			if (givenID != int.Parse(myID))
			{
			print ("Applying player position with this ID: " + givenID + "New loc: " + split2[1] + ", " + split2[2]);
				// Search into player tracking to see if they exist, and update the position.
			playersTracking[p].updatePosition(float.Parse(split2[1]),float.Parse(split2[2]));
			print ("Player updated[" + p + "]");
			 //((NetworkPlayerObj)playersTracking[givenID]).updatePosition(float.Parse(split2[1]),float.Parse(split2[2]));
			}
		}
	}

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

	public void EndGame(string s)
	{
		//Recieves the string and updates world game state to END
		// Recieve and parse the end game state and print the proper response

		print ("Recieved Game End message; terminating game instance");

		
	}

	
}
