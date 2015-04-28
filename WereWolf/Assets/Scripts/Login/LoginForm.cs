using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

public class LoginForm : MonoBehaviour {

	// Declare the array object in where we store the two values.
	string[] loginPackage;
	GameObject handler;
    byte[] salt = GetBytes("string of letters that could be anything we want i guess");

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
        var hmacMD5 = new HMACMD5(salt); //Hashes Password using MD5 and salt.
        loginPackage[1] =  GetString(hmacMD5.ComputeHash(GetBytes(s.text)));
	}

    static byte[] GetBytes(string str) { //Converts string to byte array
        byte[] bytes = new byte[str.Length * sizeof(char)];
        System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }
    static string GetString(byte[] bytes) { //Converts byte array to string
        char[] chars = new char[bytes.Length / sizeof(char)];
        System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
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
