using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MeScene : MonoBehaviour {

    [SerializeField] Text text;
    [SerializeField] GameObject panel;

    private void Awake() {
        text.text = "\n";
        text.text += DataCtrl.English ? Resources.Load<TextAsset>("E").ToString() : Resources.Load<TextAsset>("C").ToString();

        AudioListener.volume = DataCtrl.playerRecord.SettingValue[0];
    }

    private void Start() {
        
    }

    public void _BackToMenu() {
        SceneManager.LoadScene("Menu");
    }

}
