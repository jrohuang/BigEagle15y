using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FW_AttackSkill : MonoBehaviour {

    [SerializeField] Button button;
    [SerializeField] Text text;
    private string s;
    private int[,] need;
    private int needHunger, needLevel;

    private void Awake() {

        UpdateInfo();

    }

    private bool b;

    void Update () {

        if (PlayerCtrl.PD.skillLevel == 3) {
            button.interactable = false;
            text.text = DataCtrl.English ? "Learn all skills" : "已學完所有技能";
            return;
        }

        b = true;
        text.text = s;
        text.text += DataCtrl.English ? "\nLearning Requirements:\n" : "\n學習需求:\n";
        for (int i = 0; i < need.GetLength(0); i++) {
            if (PlayerCtrl.PD.Items[need[i, 0]] >= need[i, 1]) {
                text.text += "<color=#000000>";
            }
            else {
                text.text += "<color=#FF0000>";
                b = false;
            }
            
            text.text += DataCtrl.English ? DataCtrl.eqbq.item_data.Rows[need[i, 0]][6] + "(" + PlayerCtrl.PD.Items[need[i, 0]] + "/" + need[i, 1].ToString() + ")</color>     "
                : DataCtrl.eqbq.item_data.Rows[need[i, 0]][1] + "(" + PlayerCtrl.PD.Items[need[i, 0]] + "/" + need[i, 1].ToString() + ")</color>     ";
        }

        text.text += DataCtrl.English ? "Hunger Cost" : "消耗飢餓度:";
        if (PlayerCtrl.PD.Hunger > needHunger) {
            text.text += needHunger.ToString(); ;
        }
        else {
            text.text += "<color=#FF0000>" + needHunger + "</color>";
            b = false;
        }

        text.text += DataCtrl.English ? "\nLevel Requirements:" : "\n等級需求:";
        if (PlayerCtrl.PD.Level >= needLevel) {
            text.text += needLevel.ToString();
        }
        else {
            text.text += "<color=#FF0000>" + needLevel + "</color>";
            b = false;
        }
        

        button.interactable = b ? true : false;

    }

    private void UpdateInfo() {

        if (PlayerCtrl.PD.skillLevel == 3) {
            s = DataCtrl.English ? "Learn all skills" : "已學完所有技能";
            return;
        }

        s = "";
        string[] array = DataCtrl.English ? DataCtrl.eqbq.skill_data.Rows[PlayerCtrl.PD.skillLevel][7].ToString().Split('%') : DataCtrl.eqbq.skill_data.Rows[PlayerCtrl.PD.skillLevel][1].ToString().Split('%');
        for (int i = 0; i < array.Length; i++) {
            s += array[i] + "\n";
        }

        array = DataCtrl.eqbq.skill_data.Rows[PlayerCtrl.PD.skillLevel][2].ToString().Split('/');
        
        need = new int[array.Length, 2];
        for (int i = 0; i < array.Length; i++) {
            int b = array[i].IndexOf("*");
            need[i, 0] = int.Parse(array[i].Substring(0, b));
            need[i, 1] = int.Parse(array[i].Substring(b + 1, array[i].Length - b - 1));
        }
        needHunger = int.Parse(DataCtrl.eqbq.skill_data.Rows[PlayerCtrl.PD.skillLevel][3].ToString());
        needLevel = int.Parse(DataCtrl.eqbq.skill_data.Rows[PlayerCtrl.PD.skillLevel][4].ToString());

    }

    public void _LearnSkill() {
        for (int i = 0; i < need.GetLength(0); i++) {
            PlayerCtrl.PD.Items[need[i, 0]] -= need[i, 1];
        }
        PlayerCtrl.PD.Hunger -= needHunger;

        PlayerCtrl.PD.skillLevel++;
        UpdateInfo();
    }

}
