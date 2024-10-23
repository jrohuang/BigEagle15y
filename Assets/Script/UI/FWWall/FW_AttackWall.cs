using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FW_AttackWall : MonoBehaviour {

    [SerializeField] GameObject content;
    [SerializeField] Text name, infoText, info2;
    private float[] info;
    AttackWall c;
    [SerializeField] GameObject prefab;
    int[] attackInfo = new int[3]; //saveXp, killAmount, allHurt;
    public static int[] saveItems;
    public static bool[] alreadyInsPrefab;

    void Start() {
        saveItems = new int[PlayerCtrl.PD.Items.Length];
        alreadyInsPrefab = new bool[PlayerCtrl.PD.Items.Length];

        for (int i = 0; i < alreadyInsPrefab.Length; i++) {
            alreadyInsPrefab[i] = false;
        }
        Bag.WallSir.SendMessage("SendInfo", gameObject);

        name.text = DataCtrl.English ? DataCtrl.eqbq.wall_data.Rows[(int)info[0] - 1][6].ToString() : DataCtrl.eqbq.wall_data.Rows[(int)info[0] - 1][1].ToString();

        c = Bag.WallSir.GetComponent<AttackWall>();
        attackInfo = c.GetInfo();
        if (DataCtrl.English) {
            infoText.text = "Attack Speed:" + info[1].ToString() + "s\nAttack Damage:" + info[2].ToString() + "\nStorage Capacity:" + info[3].ToString() + ".";
        }
        else {
            infoText.text = "攻擊速度:" + info[1].ToString() + "s\n攻擊傷害:" + info[2].ToString() + "\n儲存容量:" + info[3].ToString() + ".";
        }

        saveItems = c.GetSaveItems();
        for (int i = 0; i < saveItems.Length; i++) {
            if (saveItems[i] > 0) {
                GameObject g = Instantiate(prefab, content.transform, false);
                g.SendMessage("StartSetting", i);
                alreadyInsPrefab[i] = true;
            }
        }
        if (content.transform.childCount > 0) {
            RectTransform contentRT = content.GetComponent<RectTransform>();
            GridLayoutGroup glg = content.GetComponent<GridLayoutGroup>();
            float x = glg.cellSize.x;
            contentRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (x + 5) * ((content.transform.childCount + 1) / 2));
        }
    }


    void Update() {
        saveItems = c.GetSaveItems();
        info2.text = DataCtrl.English ? "Kill Count:" + attackInfo[1] + "\nTotal Damage Dealt:" + attackInfo[2] + "\nTotal Experience Gained:" + attackInfo[0]
            : "總擊殺數:" + attackInfo[1] + "\n總造成傷害:" + attackInfo[2] + "\n總獲得經驗" + attackInfo[0];
            
        //10>物品總數
        for (int i = 0; i < saveItems.Length; i++) {
            if (saveItems[i] > 0 && !alreadyInsPrefab[i]) {
                alreadyInsPrefab[i] = true;
                GameObject g = Instantiate(prefab, content.transform, false);
                g.SendMessage("StartSetting", i);
            }
            RectTransform contentRT = content.GetComponent<RectTransform>();
            GridLayoutGroup glg = content.GetComponent<GridLayoutGroup>();
            float x = glg.cellSize.x;
            contentRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (x + 5) * ((content.transform.childCount + 1) / 2));
        }

    
	}
    
    void GetInfo(float[] INFO) {
        info = INFO;
    }

    public void _PutAll() {
        for (int i = 0; i < content.transform.childCount; i++) {
            content.transform.GetChild(i).SendMessage("AllPutToBag");
        }
    }
    
}
