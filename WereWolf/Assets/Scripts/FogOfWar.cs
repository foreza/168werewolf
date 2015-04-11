using UnityEngine;
using System.Collections;

public class FogOfWar : MonoBehaviour {


	public float distanceToSee;
	public SpriteRenderer thisRenderer;

	// Use this for initialization
	void Start () {

		thisRenderer = this.gameObject.GetComponent<SpriteRenderer>();
	
	}
	
	// Update is called once per frame
	void Update () {


		// Very not optimal, works for now.
		Vector3 playerPos = GameObject.Find ("Player").transform.position;
		float distX = Mathf.Abs(this.gameObject.transform.position.x - playerPos.x);
		float distY = Mathf.Abs(this.gameObject.transform.position.y - playerPos.y);


		print (distX + ", " + distY);
		if (distX < distanceToSee && distY < distanceToSee) {
			setVisible ();
		} else
			setInvisible ();

	
	}


	void setInvisible()
		    {
			thisRenderer.color = new Color (1f, 1f, 1f, 0f);

		}

		void setVisible()
		{
			thisRenderer.color = new Color (1f, 1f, 1f, 1f);

		}
}
