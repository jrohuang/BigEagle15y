using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevatorCtrl : MonoBehaviour {

    GameObject button;
    public float height;
    private Rigidbody rb;
    bool gogo = false;

    private void Update() {
        if (gogo && Time.timeScale != 0) {
            if (transform.position.y < height) {
                transform.Translate(new Vector3(0, 0.05f, 0));
                
            }
            else {
                gogo = false;
            }
        }
    }

    private void Awake() {
        button = GameObject.Find("UI").transform.Find("elevatorButton").gameObject;
        rb = transform.GetComponent<Rigidbody>();
    }
    
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            button.SetActive(true);
            if (rb.useGravity) {
                elevatorButton.GetElevator(gameObject, true);
            }
            else {
                elevatorButton.GetElevator(gameObject, false);
            }
        }
    }
    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            button.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            GoGo();
        }
    }

    void GoGo() {
        button.SetActive(false);

        if (rb.useGravity) {
            rb.useGravity = false;
            rb.isKinematic = true;
            gogo = true;
        }
        else {
            rb.useGravity = true;
            rb.isKinematic = false;
        }
    }

}
