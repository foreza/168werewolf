using UnityEngine;
using System.Collections;

public class HumanController : PlayerController {

    public bool dashOnCD;		
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

	// Use this for initialization
	override protected void Start () 
    {
        //if(sendDebugMessages) Debug.Log("INHERITANCE CHECK: IN HUMAN START");
        base.Start();
        
        trackDashCD = 0.0f;				// Init just in case.
        dashOnCD = false;				// not on CD at start!
        dashing = true;
        timeBetweenActivation = 0.2f;	
	}
	
	// Update is called once per frame
	override protected void Update () 
    {
        //if (sendDebugMessages) Debug.Log("INHERITANCE CHECK: IN HUMAN UPDATE");
        base.Update();
        getCoolDowns();
        if(Input.GetKeyDown(KeyCode.LeftAlt))
        {
            transformToWerewolf();
        }
	}

    protected override void getControlMovement()
    {
        //if (sendDebugMessages) Debug.Log("INHERITANCE CHECK: IN HUMAN CONTROLMOVEMENT");
        base.getControlMovement();

        bool doubleTapRight = false;
        bool doubleTapLeft = false;
        bool doubleTapUp = false;
        bool doubleTapDown = false;

        if (!isDead)
        {
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


            if (doubleTapRight)
            {
                if (sendDebugMessages)
                    print("Right Dash");
                // Rigidbody2D temp = GameObject.Find ("Player").GetComponent<Rigidbody2D> ();
                thisBody.AddForce(new Vector3(0.05f, 0.0f));
                // AFTER ADDING FORCE, SET THE TIME
                trackDashCD = Time.time + dashCoolDown;
                dashOnCD = true;		// on coolDown
            }

            else if (doubleTapLeft)
            {
                if (sendDebugMessages)
                    print("Left Dash");
                //Rigidbody2D temp = GameObject.Find ("Player").GetComponent<Rigidbody2D> ();
                thisBody.AddForce(new Vector3(-.25f, 0.0f));
                trackDashCD = Time.time + dashCoolDown;
                dashOnCD = true;		// on coolDown
            }


            else if (doubleTapUp)
            {
                if (sendDebugMessages)
                    print("Up Dash");
                //Rigidbody2D temp = GameObject.Find ("Player").GetComponent<Rigidbody2D> ();
                thisBody.AddForce(new Vector3(0.0f, 0.05f));
                trackDashCD = Time.time + dashCoolDown;
                dashOnCD = true;		// on coolDown

            }

            else if (doubleTapDown)
            {
                if (sendDebugMessages)
                    print("Down Dash");
                // Rigidbody2D temp = GameObject.Find ("Player").GetComponent<Rigidbody2D> ();
                thisBody.AddForce(new Vector3(0.0f, -0.05f));
                trackDashCD = Time.time + dashCoolDown;
                dashOnCD = true;		// on coolDown
            }
        }
    }

    void getCoolDowns()
    {
        if (endDashTime < Time.time && dashing)
        {
            Rigidbody2D temp = this.GetComponent<Rigidbody2D>();
            temp.velocity = new Vector3(0f, 0f, 0f);
            dashing = false;
        }

        if (dashOnCD && trackDashCD < Time.time)
        {
            dashOnCD = false;
            if (sendDebugMessages)
                print("Dash is off cooldown.");
        }
    }

    void transformToWerewolf()
    {
        GetComponent<WerewolfController>().enabled = true;
        this.enabled = false;
    }
}
