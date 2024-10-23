using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DestroyWithBoss : MonoBehaviour {

    public string BossName;

    private void Start() {
        StartCoroutine(Setting());
    }

    IEnumerator Setting() {
        yield return new WaitForSeconds(0.5f);
        try {
            GameObject.Find("Bosses/" + BossName).SendMessage("AddDWM", gameObject);

        }
        catch {
            Destroy(gameObject);
        }
    }

}
