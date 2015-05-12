using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateManager : MonoBehaviour {

    Dictionary<int, PlayerController> humanDict = new Dictionary<int, PlayerController>();

	// Use this for initialization
	void Start () 
    {
        foreach (GameObject human in GameObject.FindGameObjectsWithTag(Tags.HUMAN))
        {
            PlayerController humanController = human.GetComponent<PlayerController>();

            humanDict[humanController.playerID] = humanController;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void refreshAllStates()
    {

    }

    public void humanPlayerStates()
    {

    }
}
