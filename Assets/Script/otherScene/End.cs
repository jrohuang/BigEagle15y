using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class End : MonoBehaviour {

    private void Awake() {
        DataCtrl.playerRecord.Level = 5;
        DataCtrl.SavePRecord();

        if (File.Exists(Path.Combine(Application.persistentDataPath + "SaveLSD(DontOpen)"))) {
            File.Delete(Path.Combine(Application.persistentDataPath + "SaveLSD(DontOpen)"));
        }
    }

    public void BackToMenu() {
        SceneManager.LoadScene("Menu");
    }

}
