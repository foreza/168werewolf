using UnityEngine;
using System.Collections;

public class WerewolfController : PlayerController {

    public Sprite minimapIcon;

    SlashSkill slashAreaOfEffect;


	// Use this for initialization
	override protected void Start () 
    {
        base.Start();

        if (!isClientControlled)
        {
            //For more information on the following, see this answer:
            //http://answers.unity3d.com/questions/348974/edit-camera-culling-mask.html
            GameObject.Find("Minimap Camera").GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("MapUIWerewolf"); //Display Werewolf-Only UI elements on the minimap
            GameObject.Find("Minimap Camera").GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("MapUIHuman")); //Remove Human-Only UI elements from the minimap
        }

        GameObject minimapIconObject = transform.Find("MiniMap Icon").gameObject;
        minimapIconObject.layer = 10; //LAYER 10 == MAPUIWEREWOLF ------- SHOULD NOT BE A MAGIC NUMBER ------- CHANGE LATER
        minimapIconObject.GetComponent<SpriteRenderer>().sprite = minimapIcon;

        transform.Find("Lantern Light").gameObject.SetActive(false);
        currSprite.sprite = downSprite;

        slashAreaOfEffect = this.transform.Find("Slash Area of Effects").GetComponent<SlashSkill>();
	}
	
	// Update is called once per frame
	override protected void Update () 
    {
        base.Update();	
	}

    protected override void getSkill()
    {
        base.getSkill();

        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            skillSlash();
        }
    }

    void skillSlash()
    {
        slashAreaOfEffect.useSkill(currentlyFacing);
    }
}
