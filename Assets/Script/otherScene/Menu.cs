using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class Menu : MonoBehaviour {

    [SerializeField] Button ResumeGameButton;
    [SerializeField] Image panel;
    [SerializeField] Sprite C, E, C2, E2;
    [SerializeField] GameObject setting, teachingPanel;

    private void Awake() {

        ResumeGameButton.interactable = File.Exists((Path.Combine(Application.persistentDataPath, "SaveLSD(DontOpen)"))) ? true : false;
        if (DataCtrl.playerRecord.Level == 5) {
            panel.sprite = DataCtrl.English ? E2 : C2;
        }
        else {
            panel.sprite = DataCtrl.English ? E : C;
        }

        setting.SetActive(true);
        teachingPanel.SetActive(true);
    }

    public void _LoadGameMap() {
        SceneManager.LoadScene("GameMap");
    }
    public void _ResumeGame() {
        if (File.Exists(Path.Combine(Application.persistentDataPath, "SaveLSD(DontOpen)"))) {
            GameObject.Find("Data").SendMessage("LoadLSDScene");
        }
        else {
            ResumeGameButton.interactable = false;
        }
    }
    public void _ChangeLanguage() {
        DataCtrl.playerRecord.English = DataCtrl.playerRecord.English ? false : true;
        DataCtrl.English = DataCtrl.playerRecord.English ? true : false;
        DataCtrl.SavePRecord();
        SceneManager.LoadScene("Menu");
    }

    public void _FeedEagle() {
        SceneManager.LoadScene("FeedEagle");
    }
    public void _article() {
        SceneManager.LoadScene("Me");
    }

}
