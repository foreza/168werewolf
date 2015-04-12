using UnityEngine;
using System.Collections;

public class WerewolfAI : MonoBehaviour {

    public bool sendDebugMessages = false;
    public float speed;			// Set speed here.
    public float chaseRange;
	bool chasing;

	public SpriteRenderer currSprite;

	public Sprite forwardSprite;
	public Sprite downSprite;
	public Sprite leftSprite;
	public Sprite rightSprite;

    public GameObject target;
    
	float currentAngle,x,y;
	int updateTimer;

	// Use this for initialization
	void Start () {
		updateTimer = 0;
        target = GameObject.FindWithTag(Tags.HUMAN);

		currSprite = this.GetComponent<SpriteRenderer>();

	}
	
	// Update is called once per frame
	void Update () {
        if (Vector3.Distance(this.transform.position, target.transform.position) < chaseRange) {
            chasing = true;
        } else {
            chasing = false;
        }
		if (chasing) {
			chase ();
		} else {
			wander ();
		}
	}


	void wander (){ //Random Walk
		// print (this.gameObject.name + " is wandering.");

		if (updateTimer == 0) {
			currentAngle = Mathf.Deg2Rad * (Random.value * 360); //randomly choose
			x = Mathf.Cos (currentAngle) * speed;
			y = Mathf.Sin (currentAngle) * speed;
		}

		move(x,y);

		if(updateTimer<30){
			updateTimer++;
		} else{
			updateTimer = 0;
		}
	}

	void chase(){
        // print(this.name + " is chasing " + target.name + "!!");

        Vector3 toMove = Vector3.MoveTowards(this.transform.position, target.transform.position, speed);
        move(toMove.x - this.transform.position.x, toMove.y - this.transform.position.y);
	}


	void move(float dx, float dy){
        if (Mathf.Abs(dx) > Mathf.Abs(dy)){
            if (dx > 0){
                currSprite.sprite = rightSprite;
            } else {
                currSprite.sprite = leftSprite;
            }
        } else {
            if (dy > 0) {
                currSprite.sprite = forwardSprite;
            } else {
                currSprite.sprite = downSprite;
            }
        }

		this.transform.position += new Vector3 (dx, dy, 0.0f);
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == Tags.HUMAN)
        {
            if (sendDebugMessages) Debug.Log(this.name + " has eaten " + other.name);

            other.GetComponent<PlayerController>().triggerDeath();
        }
    }
}
