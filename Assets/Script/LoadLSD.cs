using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class LoadLSD : MonoBehaviour {
    private DataCtrl cd;

    private void Awake() {
        cd = GameObject.Find("Data").GetComponent<DataCtrl>();
        if (DataCtrl.LoadData) {
            cd.LoadLSD();
        }
        
        InvokeRepeating("AutoSave", 0, 20);
    }

    private void Start() {
        
    }

    private void AutoSave() {
        SaveLSD(false);
    }

    public static void SaveLSD(bool Exit) {
        //儲存遊戲
        LevelSaveData LSD = new LevelSaveData();

        //player位置、資料、快捷鍵設定
        LSD.playerPos = GameObject.Find("Player").transform.position;
        LSD.pd = PlayerCtrl.PD;
        LSD.fastItemsNumber = FastItems.GetFastItemsNumber();
        LSD.LevelMaxed = PlayerCtrl.LevelMaxed;
        LSD.Level = int.Parse(SceneManager.GetSceneAt(0).name.Substring(6, 1).ToString());

        //AIs
        int AIsCount = 0;
        GameObject[] AIHomes = GameObject.FindGameObjectsWithTag("AIHome");
        for (int i = 0; i < AIHomes.Length; i++) {
            AIsCount += AIHomes[i].transform.childCount;
        }
        LSD.AIs = new AIInfo[AIsCount];

        int k = 0;
        for (int i = 0; i < AIHomes.Length; i++) {
            for (int j = 0; j < AIHomes[i].transform.childCount; j++) {
                AICtrl a = AIHomes[i].transform.GetChild(j).GetComponent<AICtrl>();
                LSD.AIs[k] = a.GetAiInfo();
                k++;
            }
        }

        //boss
        GameObject bosses = GameObject.Find("Bosses");
        LSD.bosses = new AIInfo[bosses.transform.childCount];
        for (int i = 0; i < bosses.transform.childCount; i++) {
            AICtrl a = bosses.transform.GetChild(i).GetComponent<AICtrl>();
            LSD.bosses[i] = a.GetAiInfo();
        }

        //equip
        LSD.EquipDurable = new float[Bag.equipInfo.Length];
        for (int i = 0; i < LSD.EquipDurable.Length; i++) {
            LSD.EquipDurable[i] = Bag.equipInfo[i].DurableRecord;
        }
        LSD.equip = new int[Bag.equip.Length];
        for (int i = 0; i < LSD.equip.Length; i++) {
            LSD.equip[i] = Bag.equip[i].UsingEquipNumber;
            if (Bag.equip[i].UsingEquipNumber > -1) {
                LSD.EquipDurable[Bag.equip[i].UsingEquipNumber] = Bag.equip[i].Durable;
            }
        }
        LSD.EBNumber = Bag.EB.BulletNumber;

        //skill
        LSD.UsingAttackSkillNumber = AttackSkill.usingSkillNumber;

        //FW
        Transform allFW = GameObject.Find("AllFW").transform;
        LSD.FW = new FWWallInfo[allFW.childCount];
        for (int i = 0; i < LSD.FW.Length; i++) {
            LSD.FW[i] = new FWWallInfo();

            LSD.FW[i].Pos = allFW.GetChild(i).position;
            LSD.FW[i].Number = int.Parse(allFW.GetChild(i).name.Substring(3, 1));

            if (LSD.FW[i].Number == 3 || LSD.FW[i].Number == 8) {
                
                LSD.FW[i].f3 = new F3();
                AttackWall aw = allFW.GetChild(i).gameObject.GetComponent<AttackWall>();
                LSD.FW[i].f3.AttackInfo = aw.GetInfo();
                LSD.FW[i].f3.SaveItems = aw.GetSaveItems();
            }
            else if (LSD.FW[i].Number == 4 || LSD.FW[i].Number == 9) {
                LSD.FW[i].FBox = new FBox();
                BoxWall bw = allFW.GetChild(i).gameObject.GetComponent<BoxWall>();
                LSD.FW[i].FBox.Item = bw.Items;
            }
        }
        
        ///////save
        string s = DataCtrl.StrEncryption(JsonUtility.ToJson(LSD).ToString(), "526moon");
        StreamWriter file = new StreamWriter(Application.persistentDataPath + "/SaveLSD(DontOpen)");
        file.Write(s);
        file.Close();

        if (Exit) {
            SceneManager.LoadScene("Menu");
        }
    }


}
