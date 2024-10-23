using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class LevelClaerCollider : MonoBehaviour {

    private Image LevelClearPanel;
    private bool Clear = false;

    private void Awake() {
        LevelClearPanel = GameObject.Find("UI").transform.Find("LevelClear").GetComponent<Image>();
    }
    
    void Update () {
        if (Clear) {
            LevelClearPanel.color = Color.Lerp(LevelClearPanel.color, Color.black, 0.08f);
            if (LevelClearPanel.color.a > 0.9f) {
                Clear = false;
                Time.timeScale = 0;
                LevelClearPanel.color = Color.black;
                File.Delete(Application.persistentDataPath + "/SaveLSD(DontOpen)");
                if (int.Parse(SceneManager.GetSceneAt(0).name.Substring(6, 1)) == DataCtrl.playerRecord.Level) {
                    DataCtrl.playerRecord.Level++;
                    DataCtrl.SavePRecord();
                }
                SceneManager.LoadScene("GameMap");
            }
        }
	}
    
    float a;
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            if (SceneManager.GetSceneAt(0).name == "Level_4") {
                if (GameObject.Find("Bosses").transform.childCount > 0) {
                    PlayerCtrl.PD.HP = -100;
                    return;
                }
                else {
                    SceneManager.LoadScene("End");
                }
            }
            LevelClearPanel.gameObject.SetActive(true);
            Clear = true;
        }
    }

}
