using UnityEngine;
using System.Collections;

public class Escape : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.LoadLevel(Scenes.TITLE);
        }
	}
}
