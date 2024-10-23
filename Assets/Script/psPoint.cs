using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class psPoint : MonoBehaviour {
    
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            transform.parent.SendMessage("te");
            Destroy(gameObject);
        }
    }
}
