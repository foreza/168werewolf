using UnityEngine;
using System.Collections;

public class ButtonHandler : MonoBehaviour {

	// This script is used by the TITLE scene.
	// Any additional button functionality should use this script. 

	GameObject instructions;									// gets a reference to the instructions object
	GameObject sceneHandler;

	void Start () {

		instructions = GameObject.Find("InstructionHelper");	// Save reference to the game object
		hideInstruction ();										// Hide the instructions at start
	
	}

	// Method that is run to start the game.
	// Activaterd through button press.
	public void startGame()
	{
		// Helpful debug statement.
		print ("Attempting to begin game instance. (ButtonHandler)");

		// Find the scenehandler.
		sceneHandler = GameObject.Find ("SceneHandler");

		// Tell the game server to begin the game.
		sceneHandler.SendMessage ("StartTheGame");

		// Place any barriers here.
		Application.LoadLevel (Scenes.CONNORSCENE);					// Change the name of scene as necessary.

	}


    public void transitionToTitle()
    {
        Application.LoadLevel(Scenes.TITLE);
    }

	// Method that is run to quit the game from title screen.
	public void exit()
	{
		Application.LoadLevel(Scenes.LOGIN);									// Add any other exit things here.
	}

	// Method that shows the instructions.
	public void viewInstruction()
	{
		instructions.SetActive(true);
	}

	// Method that hides the instructions.
	public void hideInstruction()
	{
		instructions.SetActive(false);
	}

	// Empty update loop.
	void Update () {
		
	}
}
