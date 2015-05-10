using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;

public class ResponseHandler : MonoBehaviour {

    void HandleResponse(string data) {
        print("Handling Response...");
        string[] input = data.Split(':');
        if (input[0] == "<login>") {
            GameObject SceneHandler = GameObject.Find("SceneHandler");
            if (input[1] == "reject") {
                print("Rejecting Login...");
                SceneHandler.SendMessage("RejectLogin");
            }
            else {
                print("Accepting Login...");
                SceneHandler.SendMessage("AcceptLogin");
            }
        }
        else {
            print("Received data: "+data);
        }
    }

}
