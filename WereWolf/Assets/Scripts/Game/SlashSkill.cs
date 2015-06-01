using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlashSkill : MonoBehaviour {

    SlashArea upArea;
    SlashArea downArea;
    SlashArea leftArea;
    SlashArea rightArea;
	
    // Use this for initialization
	void Start () 
    {
        upArea = transform.Find("Up Area").GetComponent<SlashArea>();
        downArea = transform.Find("Down Area").GetComponent<SlashArea>();
        leftArea = transform.Find("Left Area").GetComponent<SlashArea>();
        rightArea = transform.Find("Right Area").GetComponent<SlashArea>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void useSkill(string currentlyFacing)
    {
        //Debug.Log("slashslash");
        List<HumanController> humans = new List<HumanController>();
        if(currentlyFacing == "up")
        {
            humans = upArea.getPlayersInArea();
        }
        if (currentlyFacing == "down")
        {
            humans = downArea.getPlayersInArea();
        }
        if (currentlyFacing == "left")
        {
            humans = leftArea.getPlayersInArea();
        }
        if (currentlyFacing == "right")
        {
            humans = rightArea.getPlayersInArea();
        }

        foreach(HumanController human in humans)
        {
            human.triggerDeath();
        }
    }
}
