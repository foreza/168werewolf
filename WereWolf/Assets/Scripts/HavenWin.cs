using UnityEngine;
using System.Collections;

public class HavenWin : MonoBehaviour {

	// A semi-redundant script that should be part of Objectives, but was kept seperate to avoid confusion.
	// This script (original intention) was to keep track of all players who have made it into the objective.
	// This script is also in charge of setting the count-down, and opening/closing the end objective.


	// Public vars
	public float timeSet;	// Length of the duration of Haven opening time, in seconds.

	// Private Vars
	bool activated;			// activated?
	float timeToClose;		// This time will reflect the exact time Haven closes.
		
	
	ArrayList humansInsideHaven;	// array of players who are inside Haven


	// Use this for initialization
	void Start () {

		activated = false;	// Starts deactivated.
		timeSet = 10.0f;	// default time is 30s, modify as necessary.
		
	}
	
	void Update () {

		if (activated) {
			print ("Time left before haven closes: " + (timeToClose - Time.time));
			checkHavenClose();
		}
	
	}

	// This method is called to start a global countdown.

	void beginCountDown()
	{
		activated = true;											// Activated
		print ("Players have 30 seconds to reach the endpoint."); 	
		timeToClose = Time.time + timeSet; 							// Get current time, add time set, and use that as our reference
		GameObject.Find ("Interact").SendMessage ("havenActivate");	// Send a message to the player/players indicating that "Haven is active!"

		// TODO: Optimize this so that the countdown is sent to all players, not just one.
		// GameObject.FindGameObjectsWithTag("Interact").SendMessage ("havenActivate");
	}

	// This method is called to end the global countdown and reset the Haven.
	void checkHavenClose()
	{
		if (timeToClose < Time.time) {
			activated = false;
			print ("Haven has closed.");
			this.SendMessage("closeHaven");			// Sends a message to the globalobjectives tracker 
			GameObject.Find ("Interact").SendMessage ("havenDeactivated"); // Send message to the player/players that Havfen is deactivated"
		
		}
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == Tags.HUMAN)
            humansInsideHaven.Add(other.gameObject);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == Tags.HUMAN)
            humansInsideHaven.Remove(other.gameObject);
    }
}
