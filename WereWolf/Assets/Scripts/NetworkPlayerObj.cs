using UnityEngine;
using System.Collections;

public class NetworkPlayerObj {
	
		public string playerID;
		public GameObject g;
		public Transform t;
		// To add in
		public int facingDirection;			// facing direction: 0 -> right, 1 -> left
		public bool isHuman;					// Is this player a human or werewolf
		
	public NetworkPlayerObj()
		{
			// Empty constructor.
			playerID = "new";
		}
	public NetworkPlayerObj(string id)
		{
			playerID = id;

			
		}
		
	public void ping()
	{
		//print ("You have pinged this player: " + playerID);
		g.transform.position += new Vector3 (1.0f, 0.0f);
	}

		public void updatePosition(float x, float y)
		{

			g.transform.position = new Vector3 (x, y);
			// print ("Moving other player (not me): " + g.transform.position);
		}
		
		public string getID()
		{
			return playerID;
		}
		
		public GameObject getObj()
		{
			return g;
		}


		// Shouldn't need to get the position, we're just settting.
		
		public void BecomeWereWolf()
		{
			// Load werewolf sprite here
			//print (this.gameObject.name + " has become a werewolf!");
			isHuman = false;
		}

        public void Deactivate()
        {
            g.SetActive(false);
        }
        
		// Use this for initialization
		void Start () {
	
		}
	
		// Update is called once per frame
		void Update () {
	
	}
}	
