using UnityEngine;
using System.Collections;

public class Objectives : MonoBehaviour {

	public bool tower1Active;
	public bool tower2Active;
	public bool havenOpen;

	// Use this for initialization
	void Start () {

		tower1Active = false;
		tower2Active = false;
		havenOpen = false;

	
	}

	void modifyGlobal(string s)
	{
		// Crude!

		if (s == "NorthTower") {
			tower1Active = true;
			print ("The North Tower has been activated!");
		}

		if (s == "SouthTower") {
			tower2Active = true;
			print ("The South Tower has been activated!");
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
