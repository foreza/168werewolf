using UnityEngine;
using System.Collections;

public class PlayerVision : MonoBehaviour {

	public bool sendDebugMessages = false;
    private Vector3 playerPosition;		// Vector storing player Position 
	GameObject thePlayer;				// Store player gameobject here
	ArrayList visibleObjects;			// Array of visible objects at any time.

	// Use this for initialization
	void Start () {

		thePlayer = GameObject.Find ("Player");
	
	}
	
	// Update is called once per frame
	void Update () {

	}
	

	void OnTriggerEnter2D(Collider2D other)
	{
		if (sendDebugMessages)
            print ("Illuminating: [" + other.gameObject.name + "]");
		if(other.GetComponent<FogOfWar>())
            revealObject (other.gameObject);
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (sendDebugMessages)
            print ("Hiding: [" + other.gameObject.name + "]");
        if (other.GetComponent<FogOfWar>())
		    hideObject (other.gameObject);

	}

	void revealObject(GameObject obj)
	{
		if (sendDebugMessages)
            print ("Sending message");
		obj.SendMessage ("setVisible");
	}

	void hideObject(GameObject obj)
	{
		obj.SendMessage ("setInvisible");
	}

}
