using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour {

    [SerializeField] GameObject boom;
    private float hurt;
    private Rigidbody rb;
    private float speed;

    void SetHurt(float h) {
        hurt = h;
    }

    private void OnTriggerEnter(Collider other) {

        if (other.CompareTag("AI")) {
            other.gameObject.SendMessage("GetHurt", hurt);
        }

        if (AttackSkill.usingSkillNumber[0] == 2 || AttackSkill.usingSkillNumber[1] == 2) {
            Instantiate(boom, transform.position, Quaternion.identity);
            Collider[] colliders = Physics.OverlapSphere(transform.position, 5, LayerMask.GetMask("AI"));
            for (int i = 0; i < colliders.Length; i++) {
                if (colliders[i].gameObject.name.Substring(0, 2) == "AI") {
                    colliders[i].gameObject.SendMessage("GetHurt", hurt / 2);
                }
            }
        }
        
        Destroy(gameObject);
    }

    void CloneBullet() {
        gameObject.SetActive(false);
        Invoke("TurnActiveTrue", 0.1f);
    }

    private void TurnActiveTrue() {
        gameObject.SetActive(true);
    }

}
