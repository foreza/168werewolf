using UnityEngine;
using System.Collections;

public class ButtonHandler : MonoBehaviour {

	bool showInstruction;
	GameObject instructions;

	// Use this for initialization
	void Start () {

		showInstruction = false;
		instructions = GameObject.Find("InstructionHelper");
		hideInstruction ();
	
	}
	
	// Update is called once per frame
	void Update () {

	
	}

	public void startGame()
	{
		Application.LoadLevel ("TestScene");
	}

	public void exit()
	{
		Application.Quit ();
	}


	public void viewInstruction()
	{
		showInstruction = true;
		instructions.SetActive(true);
	}

	public void hideInstruction()
	{
		showInstruction = false;
		instructions.SetActive(false);
	}
}
