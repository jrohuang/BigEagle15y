using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightChange : MonoBehaviour{
    
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Camera.main.SendMessage("ChangeHieght", false);
        }
    }
}