using UnityEngine;
using System.Collections;

public class PlayerInteract : MonoBehaviour {

    bool sendDebugMessages = false;
    bool havenActivated;
	bool isInHaven;
	bool canInteract;
	GameObject toInteract;
	GameObject global;



	// Use this for initialization
	void Start () {
		isInHaven = false;	
		canInteract = false;
		global = GameObject.Find ("GlobalObjectives");
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown("space") && canInteract)
		{
			// interact with building.
			
            if(sendDebugMessages) print ("Attemping to interact with item.");
			toInteract.SendMessage("trigger");
			
		}


	
	}
	// This will be broadcast to all players/werewolves!
	void havenActivate()
	{
        if (sendDebugMessages) print(this.gameObject.name + " knows that haven has been activated!");
		havenActivated = true;
	}

	void havenDeactivated()
	{	
		havenActivated = false;

        //TEMPORARY CODE ------------------------------------------------------------
        if (isInHaven)
        {
            if (sendDebugMessages) print(this.gameObject.name + " has passed!");
            //global.SendMessage("playerWin");
            Application.LoadLevel(Scenes.VICTORY);
        }
        else
        {
            if (sendDebugMessages) print("You have failed...");
            //global.SendMessage("playerLose");
            Application.LoadLevel(Scenes.GAMEOVER);
        }

        //----------------------------------------------------------------------------

	}

	// Mainly for debug purposes.

	void OnTriggerEnter2D(Collider2D other)
	{
        if (sendDebugMessages) print("Can interact with : [" + other.gameObject.name + "]");
		canInteract = true;
		toInteract = other.gameObject;
		//revealObject (other.gameObject);

		if (other.gameObject.name == "Haven" && havenActivated) {
			isInHaven = true;
		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
        if (sendDebugMessages) print("Can no longer interact with: [" + other.gameObject.name + "]");
		canInteract = false;
		//hideObject (other.gameObject);

		if (other.gameObject.name == "Haven" && havenActivated) {
			isInHaven = false;
		}
		
	}



}
