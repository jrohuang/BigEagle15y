using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class GameMap : MonoBehaviour {

    private Button[] loadLevelButton = new Button[5];
    private GameObject loadingPanel;
    private Text progressText;
    private AsyncOperation async;
    private bool LoadingScene = false;

    private void Awake() {
        loadingPanel = transform.Find("LoadingPanel").gameObject;
        loadingPanel.SetActive(false);
        progressText = loadingPanel.transform.GetChild(0).GetComponent<Text>();
        for (int i = 0; i < loadLevelButton.Length; i++) {
            loadLevelButton[i] = transform.GetChild(0).GetChild(i).GetComponent<Button>();
            loadLevelButton[i].interactable = DataCtrl.playerRecord.Level >= i ? true : false;
        }

        AudioListener.volume = DataCtrl.playerRecord.SettingValue[0];
    }

    void Update () {
        if (LoadingScene) {
            progressText.text = "Loading..." + (async.progress * 100).ToString("0.0") + "%";
        }
    }

    public void _LoadLevel(int level) {
        File.Delete(Application.persistentDataPath + "/SaveLSD(DontOpen)");

        loadingPanel.SetActive(true);
        async = SceneManager.LoadSceneAsync("Level_" + level.ToString(), LoadSceneMode.Single);
        async.allowSceneActivation = true;
        LoadingScene = true;
    }

    public void _BackToMenu() {
        SceneManager.LoadScene("Menu");
    }
}
