using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Data;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class Setting : MonoBehaviour {

    private Image me;
    private Light light;
    private Color halfA = new Color(1, 1, 1, 0.5f);
    private Scrollbar audiO, camerA, touchL;
    
    private void Start() {
        audiO = transform.Find("AudioS").GetComponent<Scrollbar>();
        camerA = transform.Find("CameraS").GetComponent<Scrollbar>();
        touchL = transform.Find("TouchS").GetComponent<Scrollbar>();

        me = transform.GetComponent<Image>();
        if (SceneManager.GetSceneAt(0).name.Substring(0, 2) == "Le") {
            light = Camera.main.transform.GetChild(0).GetComponent<Light>();
            camerA.value = DataCtrl.playerRecord.SettingValue[1];
            touchL.value = DataCtrl.playerRecord.SettingValue[2];
        }
        else {
            if (transform.parent.gameObject.activeSelf) {
                transform.parent.gameObject.SetActive(false);
            }
        }
        audiO.value = DataCtrl.playerRecord.SettingValue[0];
        me.color = Color.white;
    }

    
    public void _CameraSettingPointerUp() {
        DataCtrl.playerRecord.SettingValue[1] = camerA.value;
        me.color = Color.white;
    }

    public void _AudioSetting() {
        AudioListener.volume = audiO.value;
        DataCtrl.playerRecord.SettingValue[0] = audiO.value;
    }
    public void _TocuhLevelSetting() {
        CameraCtrl.TouchSensitivity = touchL.value + 0.5f;
        DataCtrl.playerRecord.SettingValue[2] = touchL.value;

    }

    public void _CameraDisSetting() {
        
        float a = CameraCtrl.CameraMinDis - CameraCtrl.CameraMaxDis;
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,
            a * camerA.value + CameraCtrl.CameraMinDis - a, Camera.main.transform.position.z);
        light.range = camerA.value * (CameraCtrl.LightMinRange - CameraCtrl.LightMaxRange) + CameraCtrl.LightMaxRange;
        me.color = halfA;
    }
    
    public void _SaveAndExit() {
        LoadLSD.SaveLSD(true);
    }
    public void _Exit() {
        File.Delete(Application.persistentDataPath + "/SaveLSD(DontOpen)");
        SceneManager.LoadScene("Menu");
    }

    public void _Open() {
        transform.parent.gameObject.SetActive(true);
        Time.timeScale = 0;
    }
    public void _Close() {
        DataCtrl.SavePRecord();
        transform.parent.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
