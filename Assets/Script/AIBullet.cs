using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBullet : MonoBehaviour {

    [SerializeField] float speed;
    private bool HurtHP;

    private Rigidbody rb;
    private float hurt;
    
    void AIBulletStartSetting(float HURT) {
        transform.right = GameObject.Find("Player").transform.position - transform.position;
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.right * speed;
        hurt = HURT;
    }

    //true:HP, false:hunger
    void SetHurtKind(bool b) {
        HurtHP = b ? true : false;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            if (HurtHP) {
                PlayerCtrl.GetHurt(hurt);
            }
            else {
                PlayerCtrl.PD.Hunger -= hurt;
            }
        }
        Destroy(gameObject);
    }

}
