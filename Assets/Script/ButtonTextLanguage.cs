using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTextLanguage : MonoBehaviour {

    [SerializeField] string English;
    [SerializeField] string Chinese;

    private void Awake() {
        GetComponentInChildren<Text>().text = DataCtrl.English ? English : Chinese;
    }

}
