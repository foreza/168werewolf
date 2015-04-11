using UnityEngine;
using System.Collections;

public class TrapTrigger : MonoBehaviour {

    public GameObject mainTrapObject;
    Trap mainTrap;

	// Use this for initialization
	void Start () 
    {
        mainTrap = mainTrapObject.GetComponent<Trap>();
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        //DEBUG LOG DELETE LATER:
        Debug.Log(other.name + " has entered the trap");

        //If a human enters the trap's trigger area, and it hasn't already been triggered...
        if (other.tag == Tags.HUMAN && !mainTrap.getTriggered())
            mainTrap.triggerTrap();
    }
}
