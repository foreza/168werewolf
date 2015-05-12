using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateManager : MonoBehaviour {

    Dictionary<int, PlayerController> humanControllerDict = new Dictionary<int, PlayerController>();
    Dictionary<int, humanState> humanStateDict = new Dictionary<int, humanState>();

	// Use this for initialization
	void Start () 
    {
        foreach (GameObject human in GameObject.FindGameObjectsWithTag(Tags.HUMAN))
        {            
            PlayerController humanController = human.GetComponent<PlayerController>();

            humanControllerDict[humanController.playerID] = humanController;

            humanState newHumanState = humanController.getState();

            humanStateDict[humanController.playerID] = newHumanState;

        }        
	}
	
	// Update is called once per frame
	void Update () 
    {
        //****TEMPORARY CODE. THIS SHOULD NOT BE CALLED HERE, SHOULD BE CALLED BY ANOTHER CLASS.
        //receiveHumanPlayerStates(new Dictionary<int, humanState>());
	}

    public void addHumanPlayer(PlayerController newHuman)
    {
        humanControllerDict[newHuman.playerID] = newHuman;
    }

    //Maybe needed at some point? Right now, each receive state function calls its respective refresh state function.
    public void refreshAllStates()
    {
        refreshHumanPlayerStates();
    }

    public void refreshHumanPlayerStates()
    {
        foreach(KeyValuePair<int,humanState> humanStateInfo in humanStateDict)
        {
            //****This seems VERY prone to error. What if an id is present in one dict, but not the other?
            //****Should CHECK. Fix soon.
            humanControllerDict[humanStateInfo.Key].setState(humanStateInfo.Value.x, humanStateInfo.Value.y);
        }
    }

    public void receiveHumanPlayerStates(Dictionary<int,humanState> newHumanStates)
    {
        //vvvv Correct code (once some proper validation is in place). Uncomment after testing.
        //humanStateDict = newHumanStates;

        //****TEMPORARY CODE
        /*newHumanStates*Dictionary<int, humanState> tempDict = new Dictionary<int, humanState>();

        foreach(KeyValuePair<int,humanState> humanStateInfo in humanStateDict)
        {
            Debug.Log("test");
            humanState newState = humanStateInfo.Value;
            newState.x += 1;
            newState.y += 1;
            tempDict[humanStateInfo.Key] = newState;
           
        }

        humanStateDict = tempDict;*/
        humanStateDict = newHumanStates;

        //Debug.Log("Reached end of ReceiveHUmanPlayerStates()");
        refreshHumanPlayerStates();
    }
}

public struct humanState
{
    public int id;
    public float x;
    public float y;

}