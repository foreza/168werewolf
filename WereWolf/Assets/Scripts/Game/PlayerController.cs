	using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {

    public bool sendDebugMessages = false;
    public int playerID;
    public bool isClientControlled = false;

    public float speed;			// Set speed here.

    public float startTime;

	protected Rigidbody2D thisBody;

	public SpriteRenderer currSprite;

	public Sprite fowardSprite;
	public Sprite downSprite;
	public Sprite leftSprite;
	public Sprite rightSprite;
    public Sprite deadSprite;

    public string currentlyFacing; //stupid way to do this, should either use bit calculation stuff or at least an enum.

    public bool isDead = false;
    public AudioClip deathNoise;
    protected AudioSource deathNoiseSource;

	public GameObject sceneHandler;

    Animator anim;

    void Awake() {
        deathNoiseSource = GetComponent<AudioSource>();
		sendDebugMessages = false;
        startTime = Time.time;
    }


	
	public void RequestUpdateToServer()
	{
		string position = "" + transform.position.x + "|" + transform.position.y + "";
		sceneHandler.SendMessage("PassPosition", position);

        string scoreUpdate = ((int)Math.Floor(Time.time - startTime)).ToString(); //currently keeps track of how many seconds the player is in game
        sceneHandler.SendMessage("PassScore", scoreUpdate);
		 
	}

	public void RequestScoreToServer()
	{
	
		string scoreUpdate = ((int)Math.Floor(Time.time - startTime)).ToString(); //currently keeps track of how many seconds the player is in game
		sceneHandler.SendMessage("PassScore", scoreUpdate);
	}
	
	// Use this for initialization
	protected virtual void Start () 
    {
        //if (sendDebugMessages) Debug.Log("INHERITANCE CHECK: IN PLAYER START");
		//speed = .05f; 					// toggle this off
		//dashCoolDown = 0.0f;			// Cooldown.
		//dashDuration = 1.0f;

		currSprite = this.GetComponent<SpriteRenderer>();
		thisBody = this.GetComponent<Rigidbody2D> ();

        anim = GetComponent<Animator>();

		sceneHandler = GameObject.Find ("SceneHandler");
	
		PositionUpdateRepeat ();
		ScoreUpdateRepeat ();
	}

	void PositionUpdateRepeat() {
		InvokeRepeating("RequestUpdateToServer", 2.0f, 0.2F);
	}

	void ScoreUpdateRepeat() {
		//InvokeRepeating("RequestScoreToServer", 2.0f, 5.0F);
	}

	
	// Update is called once per frame
	protected virtual void Update () 
    {
        //if (sendDebugMessages) Debug.Log("INHERITANCE CHECK: IN PLAYER UPDATE");

        if (!isClientControlled)
        {
            getControlMovement();	// function that reads basic control movement.	
            getSkill();				// function that reads basic skill usage.
        }

		// Temp Placeholder

		// Mirroed on server
		// Fomat: playerID{playerPosX|playerPosY}playerID{playerPosX|playerPosY}

        //Debug.Log(currentlyFacing);


	}




	protected virtual void getControlMovement()
	{
        //if (sendDebugMessages) Debug.Log("INHERITANCE CHECK: IN PLAYER CONTROLMOVEMENT");
        if (!isDead)
        {

            // Basic controller, allows for up/down/left/right movement.

            if (Input.GetKey("right"))
            {
                if(sendDebugMessages) Debug.Log("Right button pressed");
                currentlyFacing = "right";
                currSprite.sprite = rightSprite;

                this.transform.position += new Vector3(speed, 0.0f, 0.0f);
            }
            else if (Input.GetKey("left"))
            {
                currSprite.sprite = leftSprite;
                currentlyFacing = "left";
                this.transform.position -= new Vector3(speed, 0.0f, 0.0f);

            }

            if (Input.GetKey("up"))
            {
                currSprite.sprite = fowardSprite;
                currentlyFacing = "up";
                this.transform.position += new Vector3(0.0f, speed, 0.0f);

            }
            else if (Input.GetKey("down"))
            {
                currSprite.sprite = downSprite;
                currentlyFacing = "down";
                this.transform.position -= new Vector3(0.0f, speed, 0.0f);
            }


		// Admin controller, allows for system-wide modifications.

        }
	}



	protected virtual void getSkill()
	{
		if (Input.GetKeyDown("space"))
		{
			// interact with building.
            
                print ("Interacting with item.");

		}

		if (Input.GetKeyDown("tab"))
		{
			// interact with building.
			this.SendMessage("ShowScoreBoard");
				print ("Bringing up scoreboard");
		}

		if (Input.GetKeyUp("tab"))
		{
			// interact with building.
			this.SendMessage("HideScoreBoard");
			print ("Hiding scoreboard");
			
		}
		
	}

    void transitionSceneToGameOver()
    {
        Application.LoadLevel(Scenes.GAMEOVER); 
    }

    public void setState(float x, float y)
    {
        GetComponent<Transform>().position = new Vector3(x, y, 0);
    }

    public humanState getState()
    {
        Transform myTransform = GetComponent<Transform>();
        humanState human;
        human.x = myTransform.position.x;
        human.y = myTransform.position.y;
        human.id = playerID;

        return human;
    }


}