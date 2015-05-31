using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkSpawner : MonoBehaviour {

	void Start () {
	
	}

	void Update () {
	
	}


	// Spawns the player objects in the initial game.
	// TODO: Implement so I can spawn at a certain location
	public void SpawnInitialPlayers(int n)
	{
		print ("Will 'spawn' this many players at start: " + n);


// handle my id
		GameObject s = GameObject.Find ("SceneHandler");
		
		for (int i = 0; i < n; i++)
		{
			// Create a new network player obj
			NetworkPlayerObj player = new NetworkPlayerObj();

			// Instantiate the prefab and set it to the game object.
			player.g = Instantiate ( (GameObject)Resources.Load ("OtherPlayer"));

			// Parent the game object to not lose reference.
			player.g.transform.SetParent(s.transform);

			// Set the playerID based on the spawn #.
			player.playerID = i.ToString();
			//Add several Player objects to aid in expansion!
			this.gameObject.SendMessage("AddPlayerToTrack", player);

			print ("Spawned player object with ID: " + player.playerID);
		}
		
		
	}

	// Spawns a single player (to keep focus)
	public void SpawnAddPlayer(string i)
	{

		GameObject s = GameObject.Find ("SceneHandler");


			print ("New player has entered; spawning");
		
			// Create a new network player obj
			NetworkPlayerObj player = new NetworkPlayerObj();

			// Instantiate the prefab and set it to the game object.
			player.g = Instantiate ( (GameObject)Resources.Load ("OtherPlayer"));

			// Parent the game object to not lose reference.
			player.g.transform.SetParent(s.transform);

			// Set the playerID to the string value.
			player.playerID = i.ToString();

			// Add this person to the list of players tracking.
			this.gameObject.SendMessage("AddPlayerToTrack", player);

	}

}
