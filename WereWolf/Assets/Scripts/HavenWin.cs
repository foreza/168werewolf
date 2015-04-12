using UnityEngine;
using System.Collections;

public class HavenWin : MonoBehaviour {

	GameObject players;		// array of players who have successfully made it to the haven.

	bool activated;			// activated?
	float timeToClose;		// time is set when countDown begins.
	float timeSet;			// set time before end of haven.
		

	// Use this for initialization
	void Start () {

		activated = false;
		timeSet = 20.0f;		// default time is 30s
		
	}
	
	// Update is called once per frame
	void Update () {

		if (activated) {
			print ("Time left before haven closes: " + (timeToClose - Time.time));
			checkHavenClose();
		}
	
	}

	void beginCountDown()
	{
		activated = true;
		print ("Players have 30 seconds to reach the endpoint.");
		timeToClose = Time.time + timeSet; 		// time

	}

	void checkHavenClose()
	{
		if (timeToClose < Time.time) {
			activated = false;
			print ("Haven has closed.");
			this.SendMessage("closeHaven");
		}
	}
}
