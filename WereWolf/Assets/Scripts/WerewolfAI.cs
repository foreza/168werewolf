using UnityEngine;
using System.Collections;

public class WerewolfAI : MonoBehaviour {

	public float speed;			// Set speed here.
	public bool chasing;
	public bool wandering;

	public SpriteRenderer currSprite;

	public Sprite forwardSprite;
	public Sprite downSprite;
	public Sprite leftSprite;
	public Sprite rightSprite;

	bool movingUp = false;
	bool movingDown = false;
	bool movingLeft = false;
	bool movingRight = false;

	float currentAngle,x,y;
	int updateTimer;

	// Use this for initialization
	void Start () {
		speed = .10f; 					// toggle this off
		chasing = false;

		updateTimer = 0;

		currSprite = this.GetComponent<SpriteRenderer>();

	}
	
	// Update is called once per frame
	void Update () {
		if (chasing) {
			chase ();
		} else if (wandering) {
			wander ();
		} else {
			//do nothing
		}
	}


	void wander (){ //Random Walk
		print (this.gameObject.name + " is wandering.");

		if (updateTimer == 0) {
			currentAngle = Mathf.Deg2Rad * (Random.value * 360); //randomly choose
			x = Mathf.Cos (currentAngle) * speed;
			y = Mathf.Sin (currentAngle) * speed;
			if (Mathf.Abs (x) > Mathf.Abs (y)) {
				if (x > 0) {
					currSprite.sprite = rightSprite;
				} else {
					currSprite.sprite = leftSprite;
				}
			} else {
				if (y > 0) {
					currSprite.sprite = forwardSprite;
				} else {
					currSprite.sprite = downSprite;
				}
			}
		}

		move(x,y);

		if(updateTimer<30){
			updateTimer++;
		} else{
			updateTimer = 0;
		}
	}

	void chase(){
		//find player position
		//calculate dx and dy
		//move(dx,dy);
	}


	void move(float dx, float dy){
		this.transform.position += new Vector3 (dx, dy, 0.0f);
	}

}
