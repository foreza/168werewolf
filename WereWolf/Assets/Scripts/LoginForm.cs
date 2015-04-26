using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoginForm : MonoBehaviour {

	// Declare the array object in where we store the two values.
	string[] loginPackage;
	GameObject handler;

	// Use this for initialization
	void Start () {
	
		loginPackage = new string[2];
		handler = GameObject.Find ("SceneHandler");

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void getUserName()
	{
		InputField s = GameObject.Find ("Username").GetComponent<InputField> ();
		print ("You typed in: " + s.text);
		loginPackage [0] = s.text;

	}

	public void getPassword()
	{
		InputField s = GameObject.Find ("Password").GetComponent<InputField> ();
		print ("You typed in: " + s.text);
		loginPackage [1] = s.text;
	}

	public void submitForm()
	{
		// Access the reference to the handler and calls the function
		print ("Submitting to server: " + loginPackage [0] + " " + loginPackage [1]);
		handler.SendMessage ("handleLogIn", loginPackage);
		print ("Submitted");

	}
}
