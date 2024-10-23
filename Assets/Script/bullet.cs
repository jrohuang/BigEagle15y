using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour {

    float hurt;
    void SetHurt(float HURT) {
        hurt = HURT;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("AI")) {
            AICtrl a = other.GetComponent<AICtrl>();
            a.bulletGO = transform.parent.gameObject;
            other.SendMessage("GetBulletHurt", hurt);
        }
        else if(other.gameObject == transform.parent.gameObject) {
            return;
        }
        Destroy(gameObject);
    }

}
