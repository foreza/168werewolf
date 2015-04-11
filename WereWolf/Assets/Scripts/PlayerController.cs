	using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float speed;			// Set speed here.

	Rigidbody2D thisBody;

	public bool dashOnCD;		// 
	public bool dashing;
	public float dashCoolDown;
	public float dashDuration;
	public float trackDashCD;
	public float endDashTime;		// not sure if necessary
	public float timeBetweenActivation;


	public float _doubleTapTimeD1;
	public float _doubleTapTimeD2;
	public float _doubleTapTimeD3;
	public float _doubleTapTimeD4;



	public SpriteRenderer currSprite;

	public Sprite fowardSprite;
	public Sprite downSprite;
	public Sprite leftSprite;
	public Sprite rightSprite;


	// Use this for initialization
	void Start () {

		speed = .05f; 					// toggle this off
		dashCoolDown = 2.0f;			// Cooldown.
		dashDuration = 1.0f;
		trackDashCD = 0.0f;				// Init just in case.
		dashOnCD = false;				// not on CD at start!
		dashing = true;
		timeBetweenActivation = 0.2f;

		currSprite = this.GetComponent<SpriteRenderer>();
		thisBody = this.GetComponent<Rigidbody2D> ();
	
	}
	
	// Update is called once per frame
	void Update () {

		getCoolDowns ();
		getControlMovement();	// function that reads basic control movement.	
		getSkill();				// function that reads basic skill usage.

	

	}
	

	void getControlMovement()
	{

		bool doubleTapRight = false;
		bool doubleTapLeft = false;
		bool doubleTapUp = false;
		bool doubleTapDown = false;


		if (Input.GetKeyDown("right") && !dashOnCD)
		{
			if (Time.time < _doubleTapTimeD1 + timeBetweenActivation)
			{
				doubleTapRight = true;
				endDashTime = Time.time + dashDuration;
				dashing = true;
			}
			_doubleTapTimeD1 = Time.time;
		}

		if (Input.GetKeyDown("left") && !dashOnCD)
		{
			if (Time.time < _doubleTapTimeD2 + timeBetweenActivation)
			{
				doubleTapLeft = true;
				endDashTime = Time.time + dashDuration;
				dashing = true;
			}
			_doubleTapTimeD2 = Time.time;
		}

		if (Input.GetKeyDown("up") && !dashOnCD)
		{
			if (Time.time < _doubleTapTimeD3 + timeBetweenActivation)
			{
				doubleTapUp = true;
				endDashTime = Time.time + dashDuration;
				dashing = true;
			}
			_doubleTapTimeD3 = Time.time;
		}

		if (Input.GetKeyDown("down") && !dashOnCD)
		{
			if (Time.time < _doubleTapTimeD4 + timeBetweenActivation)
			{
				doubleTapDown = true;
				endDashTime = Time.time + dashDuration;
				dashing = true;
			}
			_doubleTapTimeD4 = Time.time;
		}

	
		// Dashing code.

		
			if (doubleTapRight) {
				print ("Right Dash");
				// Rigidbody2D temp = GameObject.Find ("Player").GetComponent<Rigidbody2D> ();
			thisBody.AddForce (new Vector3 (0.05f, 0.0f));	
				// AFTER ADDING FORCE, SET THE TIME
				trackDashCD = Time.time + dashCoolDown;
				dashOnCD = true;		// on coolDown
		}
		
			else if (doubleTapLeft) {
				print ("Left Dash");
				//Rigidbody2D temp = GameObject.Find ("Player").GetComponent<Rigidbody2D> ();
				thisBody.AddForce (new Vector3 (-0.05f, 0.0f));	
				trackDashCD = Time.time + dashCoolDown;
				dashOnCD = true;		// on coolDown
		}
		
		
			else if (doubleTapUp) {
				print ("Up Dash");
				//Rigidbody2D temp = GameObject.Find ("Player").GetComponent<Rigidbody2D> ();
				thisBody.AddForce (new Vector3 (0.0f, 0.05f));	
				trackDashCD = Time.time + dashCoolDown;
				dashOnCD = true;		// on coolDown
			
			}
		
			else if (doubleTapDown) {
				print ("Down Dash");
				// Rigidbody2D temp = GameObject.Find ("Player").GetComponent<Rigidbody2D> ();
				thisBody.AddForce (new Vector3 (0.0f, -0.05f));	
				trackDashCD = Time.time + dashCoolDown;
				dashOnCD = true;		// on coolDown
		}



	
		// Basic controller, allows for up/down/left/right movement.

		if (Input.GetKey ("right")) {
			currSprite.sprite = rightSprite;
			this.transform.position += new Vector3 (speed, 0.0f, 0.0f); 
		} else if (Input.GetKey ("left")) {
			currSprite.sprite = leftSprite;
			this.transform.position -= new Vector3 (speed, 0.0f, 0.0f); 
		}
		
		if (Input.GetKey ("up")) {
			currSprite.sprite = fowardSprite;
			this.transform.position += new Vector3 (0.0f, speed, 0.0f); 
		} 
		else if (Input.GetKey ("down")) {
			currSprite.sprite = downSprite;
			this.transform.position -= new Vector3 (0.0f, speed, 0.0f);  
		}





	}

	void getCoolDowns()
	{
		if (endDashTime < Time.time && dashing) {
			Rigidbody2D temp = GameObject.Find ("Player").GetComponent<Rigidbody2D> ();
			temp.velocity = new Vector3(0f,0f,0f);
			dashing = false;

		}


		if (dashOnCD && trackDashCD < Time.time) {
			dashOnCD = false;
			print ("Dash is off cooldown.");
		}



	}

	void getSkill()
	{
		if (Input.GetKeyDown("space"))
		{
			// interact with building.
			print ("Interacting with item.");

		}


	}



}
