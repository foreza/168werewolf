using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlashArea : MonoBehaviour {

    List<HumanController> humans = new List<HumanController>();
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Human")
        {
            humans.Add(other.GetComponent<HumanController>());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Human")
        {
            humans.Remove(other.GetComponent<HumanController>());
        }

    }

    public List<HumanController> getPlayersInArea()
    {
        return humans;
    }
}
