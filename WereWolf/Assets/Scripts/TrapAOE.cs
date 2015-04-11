using UnityEngine;
using System.Collections;

public class TrapAOE : MonoBehaviour {

    public GameObject mainTrapObject;
    Trap mainTrap;

	// Use this for initialization
	void Start () 
    {
        mainTrap = mainTrapObject.GetComponent<Trap>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void onTriggerEnter2D(Collider2D other)
    {

    }

    void onTriggerExit2D(Collider2D other)
    {

    }
}
