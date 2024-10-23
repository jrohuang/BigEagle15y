using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LFiveBossesReset : MonoBehaviour {

    public GameObject[] gameObjects;
    
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            for (int i = 0; i < gameObjects.Length; i++) {
                Destroy(GameObject.Find(gameObjects[i].name));
            }
            for (int i = 0; i < gameObjects.Length; i++) {
                GameObject g = Instantiate(gameObjects[i]);
                g.name = gameObjects[i].name;
            }
        }

        
    }

}
