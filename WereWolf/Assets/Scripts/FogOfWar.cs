using UnityEngine;
using System.Collections;

public class FogOfWar : MonoBehaviour {

	// This script is primarily in use by any non-player object.
	// All environment/traps/enemies use this script.

	// Rather than illuminate just for each particular player
	// Players illuminate for each other.
	// (Strength in numbers.)

	public bool visible;										// Indicates whether players are able to view this particular object.
 	SpriteRenderer rend;										// Gets a reference to the object's spriteRenderer


	void Start () {
		rend = this.gameObject.GetComponent<SpriteRenderer>(); 	// Bind the component
		setInvisible ();										// All objects should start as "hidden" unless revealed by player.
	}

	// Hide the object.
	void setInvisible()
		{
			rend.color = new Color (1f, 1f, 1f, 0f);			// Turn the opacity to 0, hiding the object.
			visible = false;									// No longer visible to any players.
		}

	// Show the object.
	void setVisible()
		{
		rend.color = new Color (1f, 1f, 1f, 1f);				// Turn the opacity to 0, hiding the object.
		visible = true;											// Is now visible to ALL players.
		}

			// Update is called once per frame
	void Update () {
		}

}
