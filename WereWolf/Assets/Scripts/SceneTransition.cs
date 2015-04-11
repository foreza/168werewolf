using UnityEngine;
using System.Collections;

public class SceneTransition : MonoBehaviour {

	public void transitionSceneToLevelOne()
    {
        Application.LoadLevel(Scenes.CONNORSCENE);
    }
}
