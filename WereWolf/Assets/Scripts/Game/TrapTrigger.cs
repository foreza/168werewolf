using UnityEngine;
using System.Collections;

public class TrapTrigger : MonoBehaviour {

    public bool sendDebugMessages = false;

    public GameObject mainTrapObject;
    Trap mainTrap;

	// Use this for initialization
	void Start () 
    {
        mainTrap = mainTrapObject.GetComponent<Trap>();
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        //If a human enters the trap's trigger area, and it hasn't already been triggered...
        if (other.tag == Tags.HUMAN && !mainTrap.getTriggered())
            mainTrap.triggerTrap();
    }
}
