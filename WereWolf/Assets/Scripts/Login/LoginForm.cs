using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoginForm : MonoBehaviour {

	// Declare the array object in where we store the two values.
	string[] loginPackage;
	GameObject handler;

	// Use this for initialization
	void Start () {
	
		loginPackage = new string[3];
		handler = GameObject.Find ("SceneHandler");

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void getUserName()
	{
		InputField s = GameObject.Find ("Username").GetComponent<InputField> ();
		print ("User: You typed in: " + s.text);
		loginPackage [0] = s.text;

	}

	public void getPassword()
	{
		InputField s = GameObject.Find ("Password").GetComponent<InputField> ();
		print ("Pass: You typed in: " + s.text);
		loginPackage [1] = s.text;
	}

	public void getMyIP()
	{
		InputField s = GameObject.Find ("IpAddress").GetComponent<InputField> ();
		print ("IP: You typed in: " + s.text);
		loginPackage [2] = s.text;
	}


	public void submitForm()
	{
		// Access the reference to the handler and calls the function
		print ("Submitting to server: " + loginPackage [0] + " " + loginPackage [1] + " to IP: " + loginPackage[2]);
		handler.SendMessage ("handleLogIn", loginPackage);
		print ("Submitted");

	}
}
