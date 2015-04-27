using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour {

    //This is the base class from which all traps will inherit. 

    public bool sendDebugMessages = false;

    //All traps require an "activation" trigger collider. If the players enter this trigger, the trap is activated.
    //The trigger collider is attached to a gameobject that has a TrapTrigger script, which sends a message when a player enters it.
    public GameObject triggerZone;

    //All traps require an "area of effect" trigger collider. This represents the effected area of a trap's effect; if a player is in this area when the trap is dealing damage, the player dies. 
    //This trigger collier is attached to a gameobject that has a TrapAOE script, which can be queried for an array players currently inside the zone.
    public GameObject areaOfEffectZone;

    //Associated scripts of the two above objects, to be assigned at start.
    TrapTrigger triggerZoneScript;
    TrapAOE AreaOfEffectZoneScript;

    //Traps have a duration of time between being triggered and dealing damage. (Though for some traps, it may be zero)
    public float timeToActivate;

    //SFX to be played upon trap being triggered
    public AudioClip triggerSFX;

    //SFX to be played upon trap activation
    public AudioClip activeSFX;

    //Once activated, can this trap eventually reactivate?
    public bool canReactivate;

    //Time it takes for trap to reactivate, if at all
    public float timeToReactivate;
    
    //Whether a trap has been triggered or not
    bool triggered = false;

    //Whether a trap has the potential to be triggered or not
    bool canActivate = true;

    //Timestamp for being active
    float timestampForActivation;

    //Timestamp for reactivation
    float timestampForReactivation;

    Animator anim;

    // Use this for initialization
	void Start () 
    {
        triggerZoneScript = triggerZone.GetComponent<TrapTrigger>();
        AreaOfEffectZoneScript = areaOfEffectZone.GetComponent<TrapAOE>();

        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
    {

        if (triggered && canActivate && Time.time > timestampForActivation)
        {
            activateTrap();
        }
	}

    void activateTrap()
    {
        ArrayList humans = AreaOfEffectZoneScript.getHumansInTrapAOE();

        //For each human inside the trap's death radius...
        foreach (GameObject human in humans)
        {
            if (sendDebugMessages) Debug.Log(human.name + " has been slain!");

            //Get the human's player controller, and use it to trigger their death.
            human.GetComponent<PlayerController>().triggerDeath();

        }

        anim.SetBool("Activated", true);
        canActivate = false;
    }
    
    public void triggerTrap()
    {
        if (sendDebugMessages) Debug.Log(this.gameObject.name + ": Trap.cs : triggerTrap()");
        triggered = true;
        timestampForActivation = Time.time + timeToActivate;
    }

    public bool getTriggered()
    {
        return triggered;
    }
}
