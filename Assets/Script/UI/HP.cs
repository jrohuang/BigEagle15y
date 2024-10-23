using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour {

    private Image image;
    private Text hungerText;
    private float maxHunger;
    private bool textFormatPer = false;

    private void Start() {
        image = transform.Find("HP").GetComponent<Image>();
        hungerText = image.transform.GetChild(0).GetComponent<Text>();
    }

    void Update () {

        image.fillAmount = Mathf.Lerp(image.fillAmount, PlayerCtrl.PD.HP / PlayerCtrl.PD.MaxHP, 0.05f);

        if (textFormatPer) {
            hungerText.text = (PlayerCtrl.PD.HP / PlayerCtrl.PD.MaxHP * 100).ToString("00.00") + "%";
        }
        else {
            hungerText.text = Mathf.Floor(PlayerCtrl.PD.HP).ToString("0") + " / " + PlayerCtrl.PD.MaxHP.ToString();
        }
	}

    public void _TextFormatChange() {
        textFormatPer = textFormatPer ? false : true;
    }

}
