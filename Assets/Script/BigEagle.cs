using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BigEagle : MonoBehaviour {

    [SerializeField] bool BIG;
    private GameObject aiHome_9;
    GameObject ai21;
    
    private void Start() {
        aiHome_9 = Resources.Load<GameObject>("LFive/AIHome_9");
        ai21 = Resources.Load<GameObject>("AI/AI_21");
    }

    void YoBigEagle() {
        if (!BIG) {
            if (GameObject.Find("AIHome_9").transform.childCount == 0) {
                Destroy(GameObject.Find("AIHome_9"));
                Instantiate(aiHome_9);
                GameObject g = Instantiate(ai21, transform.position, Quaternion.Euler(0, -90, 0));
                g.transform.parent = GameObject.Find("Bosses").transform;
                GameObject.Find("Player").transform.position = new Vector3(70, 0.7f, -21);
            }
        }
    }
}
