using UnityEngine;
using System.Collections;

public class Trap1 : MonoBehaviour {

    public GameObject mainTrapObject;
    Trap mainTrap;

	// Use this for initialization
	void Start () 
    {
        mainTrap = mainTrapObject.GetComponent<Trap>();
	}

    void onTriggerEnter2D(Collider2D other)
    {
        //If a human enters the trap's trigger area, and it hasn't already been triggered...
        if (other.tag == Tags.HUMAN && !mainTrap.getTriggered())
            mainTrap.triggerTrap();
    }
}
