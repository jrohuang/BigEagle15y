using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class psPoints : MonoBehaviour {

    [SerializeField] GameObject g;

    void te() {
        if (transform.childCount < 2) {
            Destroy(g);
            Destroy(gameObject);
        }
    }

}
