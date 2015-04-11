using UnityEngine;
using System.Collections;

public class FogOfWar : MonoBehaviour {


	public float distanceToSee;
	public bool visible;
	public SpriteRenderer thisRenderer;
	public Sprite thisSprite;

	// Use this for initialization
	void Start () {

		thisRenderer = this.gameObject.GetComponent<SpriteRenderer>();
		distanceToSee = 10.0f;
		visible = false;

	
	}
	
	// Update is called once per frame
	void Update () {


		thisRenderer.color = new Color (1f, 1f, 1f, 0f);
	
	}


	void setInvisible()
		    {
			print ("hiding");
			thisRenderer.color = new Color (1f, 1f, 1f, 0f);
			visible = false;
		}

		void setVisible()
		{
;
		thisRenderer.color = new Color (1f, 1f, 1f, 1f);
		visible = true;
		print (this.gameObject.name + " is visible!");

		}
}
