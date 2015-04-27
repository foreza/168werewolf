using UnityEngine;
using System.Collections;

public class Objectives : MonoBehaviour {

	// This script keeps track of the global game state.
	// HavenWin is a similar script, and was moved from this one to retain clarity

	public bool tower1Active;			// NorthTower active?
	public bool tower2Active;			// SouthTower active?
	public bool havenOpen;				// has haven been activated?
	public bool gameOver;				// game state

	// Use this for initialization
	void Start () {

		// All initialized as false initially.
		tower1Active = false;
		tower2Active = false;
		havenOpen = false;
		gameOver = false;
	
	}

	void modifyGlobal(string s)
	{
		// Crude. but works.
		// If any object with the interactable script is then activated/interacted with properly
		// This script recieves the message, and changes the game state accordingly.

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

	// HavenWin -> Objectives -> haven.interactable
	// This function is run by the global state to trigger Haven.
	// Haven cannot be triggered by humans directly.
	void activateHaven()
	{
		print ("Activating Haven...");
		GameObject.Find ("Haven").SendMessage ("trigger");

	}

	// This function is run to turn off Haven.
	void closeHaven()
	{
		print ("Haven closing doors...");
		GameObject.Find ("Haven").SendMessage ("reset");
	}

	// Update is called once per frame
	void Update () {

		// This is only ever called once.
		if (!havenOpen && tower1Active && tower2Active) {
			havenOpen = true;
			print ("Explorers, the Haven is open! You have __ seconds before it closes.");
			activateHaven();
			summonHorde();			// You got me. Haha.
		}
	
	}

	// Sample events/sudden death occurances would be done here.
	void summonHorde()
	{
		print ("Beware players! A horde of WereWolves have caught your scent.");

		// Spawn a horde of werewolves here.
	}


	
}
