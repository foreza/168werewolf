using UnityEngine;
using System.Collections;

public class Interactable : MonoBehaviour {


	// This script is placd on objects that are "interactabe"
	// Primarily towers, and maybe some NPCS if we ever choose to put any.

	public bool canInteract;				// Toggle on and off depending on distance
	public bool triggered;					// Has this been triggered?

	public Sprite notTriggeredSprite;		// Sprite for triggered state
	public Sprite triggeredSprite;			// Sprite for untriggered state

	SpriteRenderer thisRender;				// Saved ref to renderer

	GameObject global;						// Saved ref to global game object

	// Use this for initialization
	void Start () {

		thisRender = this.gameObject.GetComponent<SpriteRenderer>();		// get reference to this object's sprite renderer

		global = GameObject.Find ("GlobalObjectives");						// get the global


		canInteract = false;		// objects should be non-interactable at start
		triggered = false;			// objects should not be triggered at start
	
	}
	
	// Update is called once per frame
	void Update () {

		// If any player is able to interact with the object
		if (canInteract) {
			thisRender.color = new Color (1f, 1f, 1f, 1f);
		}

		// Otherwise, dull it to half the opacity
		else {
			thisRender.color = new Color (1f, 1f, 1f, .5f);
		}
	}

	void trigger() // player will call this
	{
		// TODO: Eventually, bind this to only the player who called it.
		// Make this more efficient by have a speerate script for the haven.

		// if the player can interact and the item has not been triggered yet
		if (canInteract == true && !triggered && this.gameObject.name != "Haven") {
			print ("Interactable item has been triggered!");
			thisRender.sprite = triggeredSprite;
			triggered = true;
			global.SendMessage("modifyGlobal", this.gameObject.name);
		}

		if (canInteract == false && !triggered && this.gameObject.name == "Haven") {
			print ("Haven has been triggered!");
			thisRender.sprite = triggeredSprite;
			triggered = true;
			global.SendMessage("beginCountDown", this.gameObject.name);
		}
	}

	// Run this function to reset the sprite/state of the interactable object

	void reset()
	{
		thisRender.sprite = notTriggeredSprite;
		triggered = false;
	}

	// Allows "trigger" or "collision" by only Players with "Interact"

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
