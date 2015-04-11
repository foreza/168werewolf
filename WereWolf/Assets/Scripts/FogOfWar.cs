using UnityEngine;
using System.Collections;

public class FogOfWar : MonoBehaviour {

	public bool visible;
    public bool sendDebugMessages = false;

	// Use this for initialization
	void Start () {
	
		setInvisible ();
	}
	
	// Update is called once per frame
	void Update () {
	}


	void setInvisible()
		    {
			if(sendDebugMessages)
                print ("hiding");
			SpriteRenderer rend = this.gameObject.GetComponent<SpriteRenderer>();
			rend.color = new Color (1f, 1f, 1f, 0f);
			visible = false;
		}

		void setVisible()
		{

		SpriteRenderer rend = this.gameObject.GetComponent<SpriteRenderer>();
		rend.color = new Color (1f, 1f, 1f, 1f);
		visible = true;
        if(sendDebugMessages)
		    print (this.gameObject.name + " is visible!");

		}
}
