using UnityEngine;
using System.Collections;

public class WereWolfSpawner : MonoBehaviour {

	public GameObject toSpawn;
	public bool periodicSpawn;

	bool canSpawn;
	float timeBetweenSpawn;
	float timeTillnextSpawn;

	// Use this for initialization
	void Start () {
		periodicSpawn = true;
		timeBetweenSpawn = 1.0f;
		timeTillnextSpawn = Time.time;
		canSpawn = true;
	}
	
	// Update is called once per frame
	void Update () {

		if (periodicSpawn && canSpawn) {
			print ("Spawning!");
			spawnSingle();
			timeTillnextSpawn = Time.time + timeBetweenSpawn;
			canSpawn = false;
		}

		else if (!canSpawn) {
			if (timeTillnextSpawn < Time.time)
			{
				canSpawn = true;
			}
		}
	
	}

	void spawnSingle()
	{
		GameObject spawned = Instantiate (toSpawn);
		spawned.transform.position = this.gameObject.transform.position;

	}

	void beginPeriodicSpawn()
	{

	}

	void stopPeriodicSpawn()
	{

	}
}
