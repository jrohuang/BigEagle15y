using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class elevatorButton : MonoBehaviour {
    
    private static GameObject elevator;
    private static Text text;

    private void Awake() {
        text = transform.GetChild(0).GetComponent<Text>();
    }

    public static void GetElevator(GameObject g, bool goUp) {
        text.text = goUp ? "↑" : "↓";
        elevator = g;
    }
    
    public void _Click() {
        elevator.SendMessage("GoGo");
    }



}
