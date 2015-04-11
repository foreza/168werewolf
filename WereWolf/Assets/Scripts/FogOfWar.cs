using UnityEngine;
using System.Collections;

public class FogOfWar : MonoBehaviour {

	public bool visible;


	// Use this for initialization
	void Start () {
	
		setInvisible ();
	}
	
	// Update is called once per frame
	void Update () {
	}


	void setInvisible()
		    {
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
		print (this.gameObject.name + " is visible!");

		}
}
