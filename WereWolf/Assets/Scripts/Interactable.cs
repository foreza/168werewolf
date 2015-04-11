using UnityEngine;
using System.Collections;

public class Interactable : MonoBehaviour {

	public bool canInteract;
	public bool triggered;

	public Sprite notTriggeredSprite;		// Sprite for triggered state
	public Sprite triggeredSprite;			// Sprite for untriggered state

	SpriteRenderer thisRender;

	// Use this for initialization
	void Start () {

		thisRender = this.gameObject.GetComponent<SpriteRenderer>();

		canInteract = false;		// objects should be non-interactable at start
		triggered = false;			// objects should not be triggered at start
	
	}
	
	// Update is called once per frame
	void Update () {

		if (canInteract) {
			thisRender.color = new Color (1f, 1f, 1f, 1f);
		}

		else {
			thisRender.color = new Color (1f, 1f, 1f, .5f);
		}


	
	}

	void trigger() // player will call this
	{
		// if the player can interact and the item has not been triggered yet
		if (canInteract == true && !triggered) {
			print ("Interactable item has been triggered!");
			thisRender.sprite = triggeredSprite;
		}




	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.name == "Interact")
		canInteract = true;

	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.name == "Interact")
		canInteract = false;
		
	}


}
