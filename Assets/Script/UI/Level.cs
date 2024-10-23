using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour {

    private static Text levelText, XPText;
    private static Image XPLine;
    private static bool textFormatPer = false;

    private void Awake() {
        levelText = transform.Find("Image/Text").GetComponent<Text>();
        XPText = transform.Find("XPLineBG/Text").GetComponent<Text>();
        XPLine = transform.Find("XPLineBG/XPLine").GetComponent<Image>();
        Bag.ItemsMaxAmountText.text = DataCtrl.English ? "Bag Capacity:" + PlayerCtrl.MaxItemAmount.ToString() : "最高負重:" + PlayerCtrl.MaxItemAmount.ToString();
        LevelTextUpdate();
    }

    void Update() {
        XPLine.fillAmount = Mathf.Lerp(XPLine.fillAmount, PlayerCtrl.PD.XP / float.Parse(DataCtrl.eqbq.player_data.Rows[PlayerCtrl.PD.Level - 1][2].ToString()), 0.05f);
    }

    public static void LevelTextUpdate() {
        levelText.text = PlayerCtrl.PD.Level.ToString();
        if (!PlayerCtrl.LevelMaxed) {
            if (textFormatPer) {
                XPText.text = (PlayerCtrl.PD.XP / float.Parse(DataCtrl.eqbq.player_data.Rows[PlayerCtrl.PD.Level - 1][2].ToString()) * 100).ToString("00") + "%";
            }
            else {
                XPText.text = PlayerCtrl.PD.XP.ToString() + " / " + DataCtrl.eqbq.player_data.Rows[PlayerCtrl.PD.Level - 1][2].ToString();
            }
        }
        else {
            XPText.text = "Max";
        }
        
    }

    public void _TextFormatChange() {
        textFormatPer = textFormatPer ? false : true;
        LevelTextUpdate();
    }

}
