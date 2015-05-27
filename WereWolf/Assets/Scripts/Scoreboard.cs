using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Scoreboard: MonoBehaviour {

	GameObject scoreBoard;				// save a ref
	Text playerData;
	Text scoreData;
	string usernames;
	string scores;

	// Use this for initialization
	void Start () {
		scoreBoard = GameObject.Find ("ScoreBoardDisplay");
		playerData = GameObject.Find ("PlayerColumnData").GetComponent<Text>();
		scoreData = GameObject.Find ("ScoreColumnData").GetComponent<Text>();
		HideScoreBoard ();				// hide from start
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Will be called by client sending score update, then server responding with all scores (same as position)
	// called by ServerMessageHandler
	void UpdateScoreBoard(string s) {

		// Handle and parse the updates and put it in the score board.
		// *username|score

		print ("Scoreboard update requested!");
		usernames = "";
		scores = "";
			
		string [] split = s.Split ('*'); 			// split the string

													// start from index 1 because index 0 is part of initial msg
		for (int i = 1; i < split.Length; ++i) {
			string [] split2 = split[i].Split('|');	// split again to obtain score and user
			usernames += split[0] + "\n";			// concatenate and line break
			scores += split[1] + "\n";				// oh baby
		}
		playerData.text += usernames;
		scoreData.text += scores;

	}

	void ShowScoreBoard ()
	{
		//scoreBoard.SetActive (true);
		// scoreData.text += "TESTINGGG \n";
	}

	void HideScoreBoard ()
	{
		//scoreBoard.SetActive (false);

	}
}
