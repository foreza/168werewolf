using UnityEngine;
using System.Collections;

public class PlayerDash : MonoBehaviour {

	Rigidbody2D thisBody;

	// Use this for initialization
	void Start () {
		print ("Located body");
		thisBody = GameObject.Find("Player").GetComponent<Rigidbody2D>();	// getsthe rigidbody 2d component of the player.
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void dash(int f)		// will be called from the player controller script
	{
		print ("Dashing");

		if (f == 0)  // dash up 
			thisBody.AddForce(new Vector3(0.0f, 1.0f,0.0f));	
	}
}
