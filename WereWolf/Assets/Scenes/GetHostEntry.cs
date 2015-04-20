using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class GetHostEntry : MonoBehaviour {


	public static void DoGetHostEntry(string hostname)
	{
		IPHostEntry host;
		
		host = Dns.GetHostEntry(hostname);
		
		print("GetHostEntry({0}) returns:" + hostname);
		
		foreach (IPAddress ip in host.AddressList)
		{
			print ("    {0}" + ip);
		}
	}

	// Use this for initialization
	void Start () {
		print("Connecting to server.. getting host entry.");
		DoGetHostEntry("174.77.35.116");
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
