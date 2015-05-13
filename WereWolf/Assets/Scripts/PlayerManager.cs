using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

    public int myID;

    public GameObject humanPrefab;

    int spawnX = 0;
    int spawnY = 0;

    //int playerWidth = 5;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void instantiateNewPlayer(int newPlayerID)
    {
        instantiateNewPlayer(newPlayerID, spawnX, spawnY);
    }

    public void instantiateNewPlayer(int newPlayerID, float x_coord, float y_coord)
    {
        GameObject newPlayer = (GameObject)Object.Instantiate(humanPrefab, new Vector3(x_coord, y_coord), Quaternion.identity);
        PlayerController newPlayerController = newPlayer.GetComponent<PlayerController>();
        newPlayerController.playerID = newPlayerID;
        if (myID != newPlayerID)
        {
            newPlayerController.isClientControlled = false;
            newPlayer.GetComponentInChildren<Camera>().enabled = false;

        }

        else
        {
            newPlayerController.isClientControlled = true;
        }
    }
}
