using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPS : MonoBehaviour {
    [SerializeField] float time;
    private void Awake() {
        Invoke("D", time);
    }
    void D() {
        Destroy(gameObject);
    }
}
