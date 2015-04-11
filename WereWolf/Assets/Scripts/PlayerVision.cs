using UnityEngine;
using System.Collections;

public class PlayerVision : MonoBehaviour {

	private Vector3 playerPosition;		// Vector storing player Position 
	GameObject thePlayer;				// Store player gameobject here
	ArrayList visibleObjects;			// Array of visible objects at any time.

	// Use this for initialization
	void Start () {

		thePlayer = GameObject.Find ("Player");
	
	}
	
	// Update is called once per frame
	void Update () {
	
		playerPosition = updatePlayerPosition ();	// update Player position per loop 


	}


	Vector3 updatePlayerPosition()
	{
		return thePlayer.transform.position;
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		print ("Illuminating: [" + other.gameObject.name + "]");
		revealObject (other.gameObject);
	}

	void OnTriggerExit2D(Collider2D other)
	{
		print ("Hiding: [" + other.gameObject.name + "]");
		hideObject (other.gameObject);

	}

	void revealObject(GameObject obj)
	{
		print ("Sending message");
		obj.SendMessage ("setVisible");
	}

	void hideObject(GameObject obj)
	{
		obj.SendMessage ("setInvisible");
	}

}
