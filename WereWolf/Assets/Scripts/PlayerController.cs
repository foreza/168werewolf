	using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float speed;			// Set speed here.	

	public float dashCoolDown;

	public float timeBetweenActivation;


	public float _doubleTapTimeD;

	// Use this for initialization
	void Start () {

		speed = .05f; 					// toggle this off
		dashCoolDown = 5.0f;			// Cooldown.
		timeBetweenActivation = 0.5f;	// time between keyPresses
		// tapTime = 0.0f;
	
	}
	
	// Update is called once per frame
	void Update () {


		getControlMovement();	// function that reads basic control movement.	
		getSkill();				// function that reads basic skill usage.

	

	}
	

	void getControlMovement()
	{

		bool doubleTapD = false;

		if (Input.GetKeyDown(KeyCode.D))
		{
			if (Time.time < _doubleTapTimeD + .3f)
			{
				doubleTapD = true;
			}
			_doubleTapTimeD = Time.time;
		}

		if (doubleTapD)
		{
			Debug.Log("DoubleTapD");
		}

		// Basic controller, allows for up/down/left/right movement.

		if (Input.GetKey ("right")) {
			// If the player pressed "right" again
			// have the player dash in the specified direction.
			// Get the time of press
			this.transform.position += new Vector3 (speed, 0.0f, 0.0f); 

		}
		
		else if (Input.GetKey("left"))
			this.transform.position -= new Vector3(speed,0.0f,0.0f); 
		
		if (Input.GetKey("up"))
			this.transform.position += new Vector3(0.0f,speed,0.0f); 
		
		else if (Input.GetKey("down"))
			this.transform.position -= new Vector3(0.0f,speed,0.0f);  


	}

	void getSkill()
	{
		if (Input.GetKeyDown("space"))
		{
			// send appropriate message to approproate script skill.
			print ("Attempting to dash");
			this.SendMessage("dash", 0);

			// Temp workaround

			// if (f == 0)  // dash up 
			Rigidbody2D temp = GameObject.Find("Player").GetComponent<Rigidbody2D>();
				temp.AddForce(new Vector3(0.0f, 0.1f,0.0f));	

		}


	}



}
