using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hunger : MonoBehaviour {

    private Image image;
    private Text hungerText;
    private float maxHunger;
    private bool textFormatPer = false;

    private void Start() {
        image = transform.Find("Hunger").GetComponent<Image>();
        hungerText = image.transform.GetChild(0).GetComponent<Text>();
    }

    void Update () {

        image.fillAmount = Mathf.Lerp(image.fillAmount, PlayerCtrl.PD.Hunger / PlayerCtrl.PD.MaxHunger, 0.05f);

        if (textFormatPer) {
            hungerText.text = (PlayerCtrl.PD.Hunger / PlayerCtrl.PD.MaxHunger * 100).ToString("00.00") + "%";
        }
        else {
            hungerText.text = Mathf.Floor(PlayerCtrl.PD.Hunger).ToString("0") + " / " + PlayerCtrl.PD.MaxHunger.ToString();
        }
	}

    public void _TextFormatChange() {
        textFormatPer = textFormatPer ? false : true;
    }

}
