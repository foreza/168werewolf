using UnityEngine;
using System.Collections;

public class PlayerInteract : MonoBehaviour {

	bool canInteract;
	GameObject toInteract;
	GameObject global;
	// Use this for initialization
	void Start () {

		canInteract = false;
		global = GameObject.Find ("GlobalObjectives");
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown("space") && canInteract)
		{
			// interact with building.
			print ("Attemping to interact with item.");
			toInteract.SendMessage("trigger");
			
		}

	
	}


	// Mainly for debug purposes.

	void OnTriggerEnter2D(Collider2D other)
	{
		print ("Can interact with : [" + other.gameObject.name + "]");
		canInteract = true;
		toInteract = other.gameObject;
		//revealObject (other.gameObject);
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		print ("Can no longer interact with: [" + other.gameObject.name + "]");
		canInteract = false;
		//hideObject (other.gameObject);
		
	}



}
