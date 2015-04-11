using UnityEngine;
using System.Collections;

public class PlayerInteract : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown("space"))
		{
			// interact with building.
			print ("Attemping to interact with item.");
			
		}

	
	}


	// Mainly for debug purposes.

	void OnTriggerEnter2D(Collider2D other)
	{
		print ("Can interact with : [" + other.gameObject.name + "]");

		//revealObject (other.gameObject);
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		print ("Can no longer interact with: [" + other.gameObject.name + "]");
		//hideObject (other.gameObject);
		
	}



}
