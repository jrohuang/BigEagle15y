using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGift : MonoBehaviour {

    private Color gray = new Color(120f / 220, 120f / 220, 120f / 220, 1);
    private Image[] image = new Image[5];
    bool[] imageCheck = new bool[5];
    private int level;
    [SerializeField] Sprite check, unCheck;
    private Text text;
    
    private void Start() {
        Time.timeScale = 0;

        text = transform.Find("Text").GetComponent<Text>();
        level = int.Parse(SceneManager.GetSceneAt(0).name.ToString().Substring(6, 1));
        text.text = DataCtrl.English ? "Gifts * " + level.ToString() : "初始獎勵(最多可選" + level.ToString() + "項)";
        for (int i = 0; i < 5; i++) {
            image[i] = transform.Find("I" + i).GetComponent<Image>();
            imageCheck[i] = false;
        }

        if (DataCtrl.LoadData) {
            gameObject.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void _ClickImage(int number) {
        if (level > 0) {
            if (image[number].sprite != check) {
                int a, b;
                a = 0;
                b = 0;
                for (int i = 0; i < 5; i++) {
                    if (imageCheck[i]) {
                        a++;
                        b = i;
                    }
                }
                if (a == level) {
                    image[b].sprite = unCheck;
                    imageCheck[b] = false;
                }
            }
            image[number].sprite = image[number].sprite == check ? unCheck : check;
            imageCheck[number] = imageCheck[number] ? false : true;
        }
    }

    public void PlayGame() {

        for (int i = 0; i < 5; i++) {
            if (imageCheck[i]) {
                if (i < 2) {
                    if (PlayerCtrl.PD.Level == 1) {
                        PlayerCtrl.GetXP(10);
                    }
                    else {
                        PlayerCtrl.GetXP(50);
                    }
                }
                else if(i == 4) {
                    PlayerCtrl.PD.Items[0] += 20;
                }
                else {
                    PlayerCtrl.PD.Items[20] += 5;
                }
            }
        }
        

        Time.timeScale = 1;
        gameObject.SetActive(false);
    }


}
