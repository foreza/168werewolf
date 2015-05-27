using UnityEngine;
using System.Collections;

public class NetworkSpawner : MonoBehaviour {

	// This class has methods that is invoked by GameNetworking
	

	// Use this for initialization
	void Start () {


	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	// Spawns the player objects in the initial game.
	// TODO: Implement so I can spawn at a certain location
	public void SpawnInitialPlayers(int n)
	{
		print ("Will spawn this many players at start: " + n);
		
		for (int i = 0; i < n; i++)
		{
			// Create a new network player obj
			//NetworkPlayerObj player = new NetworkPlayerObj();
			//player.g = new GameObject();

			// player.g = Instantiate ((GameObject)Resources.Load ("OtherPlayer"));
			// Initialize and invoke the spawn on it.
			//player.playerID = i.ToString();
			// Add several Player objects to aid in expansion!
			//this.gameObject.SendMessage("AddPlayerToTrack", player);

			print ("Spawned a player object.");
		}
		
	}
	// TODO: FIX THIS
	// Spawns a single player (to keep focus)
	public void SpawnAddPlayer()
	{
		print ("New player has entered; spawning");
		
		// Make a new game object.
		GameObject go = new GameObject();
		// Instantiate it.
		go = (GameObject)Instantiate(Resources.Load("OtherPlayer"));
		// Make a new player object, and give it the reference to the game object.
		NetworkPlayerObj p = new NetworkPlayerObj("100");
		// Add this person to the list of players tracking.
		this.gameObject.SendMessage("AddPlayerToTrack", p);

	}

}
