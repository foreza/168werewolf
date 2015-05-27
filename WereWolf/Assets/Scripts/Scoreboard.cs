using UnityEngine;
using System.Collections;

public class Scoreboard: MonoBehaviour {

	GameObject scoreBoard;

	// Use this for initialization
	void Start () {
		scoreBoard = GameObject.Find ("ScoreBoardDisplay");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Will be called by client sending score update, then server responding with all scores (same as position)
	// called by ServerMessageHandler
	void UpdateScoreBoard() {

	}

	void ShowScoreBoard ()
	{
		scoreBoard.SetActive (true);
	}

	void HideScoreBoard ()
	{
		scoreBoard.SetActive (false);

	}
}
