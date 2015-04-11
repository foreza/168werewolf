using UnityEngine;
using System.Collections;

public class TrapAOE : MonoBehaviour {

    public GameObject mainTrapObject;
    Trap mainTrap;

    ArrayList humansInside = new ArrayList();    

	// Use this for initialization
	void Start () 
    {
        mainTrap = mainTrapObject.GetComponent<Trap>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == Tags.HUMAN)
            humansInside.Add(other.gameObject);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == Tags.HUMAN)
            humansInside.Remove(other.gameObject);
    }

    public ArrayList getHumansInTrapAOE()
    {
        return humansInside;
    }
}
