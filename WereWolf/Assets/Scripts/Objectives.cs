using UnityEngine;
using System.Collections;

public class Objectives : MonoBehaviour {

	public bool tower1Active;
	public bool tower2Active;
	public bool havenOpen;
	public bool gameOver;

	// Use this for initialization
	void Start () {

		tower1Active = false;
		tower2Active = false;
		havenOpen = false;
		gameOver = false;

	
	}

	void modifyGlobal(string s)
	{
		// Crude!

		if (s == "NorthTower") {
			tower1Active = true;
			print ("The North Tower has been activated!");
		}

		else if (s == "SouthTower") {
			tower2Active = true;
			print ("The South Tower has been activated!");
		}

		else if (s == "Haven") {
			tower2Active = true;
			print ("Player [" + "defaultPlayer" + "] is victorious!");
		}


	}

	void summonHorde()
	{
		print ("Beware players! A horde of WereWolves have caught your scent.");
	}

	void activateHaven()
	{
		print ("Activating Haven...");
		GameObject.Find ("Haven").SendMessage ("trigger");

	}

	void closeHaven()
	{
		print ("Haven closing doors...");
		GameObject.Find ("Haven").SendMessage ("reset");
	}

	// Update is called once per frame
	void Update () {

		if (!havenOpen && tower1Active && tower2Active) {
			havenOpen = true;
			print ("Explorers, the Haven is open! You have __ seconds before it closes.");
			activateHaven();
			summonHorde();
		}
	
	}
}
