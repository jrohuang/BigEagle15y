using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackSkill : MonoBehaviour {

    private static GameObject AttackSkill_1_UI;
    private Text T_info, T_usingSkillInfo;
    private Button B_next, B_previous, B_enable1, B_enable2, B_disenable;
    private int infoNumber = 1;
    public static int[] usingSkillNumber = new int[2];
    private GameObject sight;

    private void Awake() {
        T_info = transform.Find("Info_2").GetComponent<Text>();
        T_usingSkillInfo = transform.Find("Info_3").GetComponent<Text>();
        B_next = transform.Find("Next").GetComponent<Button>();
        B_previous = transform.Find("Previous").GetComponent<Button>();
        B_enable1 = transform.Find("Enable-1").GetComponent<Button>();
        B_enable2 = transform.Find("Enable-2").GetComponent<Button>();
        B_disenable = transform.Find("Disenable").GetComponent<Button>();
        sight = transform.parent.Find("Sight").gameObject;
        AttackSkill_1_UI = transform.parent.Find("AttackSkill_1").gameObject;

        gameObject.SetActive(false);
        UpdateInfo();
    }

    public static void IfLoadData() {
        if (usingSkillNumber[0] == 1 || usingSkillNumber[1] == 1) {
            AttackSkill_1_UI.SetActive(true);
            AttackSkill_1_UI.SendMessage("ChangeAttackMode");
        }
    }
    
    private void UpdateInfo() {
        
        if (PlayerCtrl.PD.skillLevel == 0) {
            T_info.text = DataCtrl.English ? "Have not learn any skill" : "尚未學習任何技能";

            B_disenable.interactable = false;
            B_enable1.interactable = false;
            B_enable2.interactable = false;
            B_next.interactable = false;
            B_previous.interactable = false;
            return;
        }
        else{
            string[] array = DataCtrl.English ? DataCtrl.eqbq.skill_data.Rows[infoNumber - 1][7].ToString().Split('%') : DataCtrl.eqbq.skill_data.Rows[infoNumber - 1][1].ToString().Split('%');
            T_info.text = "";
            for (int i = 0; i < array.Length; i++) {
                T_info.text += array[i] + "\n";
            }
        }
        
        T_usingSkillInfo.text = "";
        if (DataCtrl.English) {
            for (int i = 0; i < 2; i++) {
                T_usingSkillInfo.text += i == 0 ? "Using Skill" : "\nUsing Skill";
                if (usingSkillNumber[i] > 0) {
                    T_usingSkillInfo.text += (i + 1).ToString() + ":" + DataCtrl.eqbq.skill_data.Rows[usingSkillNumber[i] - 1][6].ToString();
                }
                else {
                    T_usingSkillInfo.text += (i + 1).ToString() + ":";
                }
            }
        }
        else{
            for (int i = 0; i < 2; i++) {
                if (DataCtrl.English) {
                    T_usingSkillInfo.text += i == 0 ? "Skill" : "\nSkill";
                }
                else {
                    T_usingSkillInfo.text += i == 0 ? "技能" : "\n技能";
                }
                if (usingSkillNumber[i] > 0) {
                    T_usingSkillInfo.text += (i + 1).ToString() + ":" + DataCtrl.eqbq.skill_data.Rows[usingSkillNumber[i] - 1][5].ToString();
                }
                else {
                    T_usingSkillInfo.text += (i + 1).ToString() + ":";
                }
            }
        }


        B_next.interactable = PlayerCtrl.PD.skillLevel > infoNumber ? true : false;
        B_previous.interactable = infoNumber > 1 ? true : false;

        B_enable1.interactable = infoNumber != usingSkillNumber[0] && infoNumber != usingSkillNumber[1] ? true : false;
        B_enable2.interactable = infoNumber != usingSkillNumber[0] && infoNumber != usingSkillNumber[1] ? true : false;
        B_disenable.interactable = infoNumber == usingSkillNumber[0] || infoNumber == usingSkillNumber[1] ? true : false;
    }

    public void _EnableButton(int ButtonNumber) {

        int a = infoNumber;

        infoNumber = usingSkillNumber[ButtonNumber];
        if (infoNumber != 0) {
            _DisenableButton();
        }

        infoNumber = a;
        usingSkillNumber[ButtonNumber] = infoNumber;
        UpdateInfo();


        switch (infoNumber) {
            case 1:
                AttackSkill_1_UI.SetActive(true);
                AttackSkill_1_UI.SendMessage("ChangeAttackMode");
                break;
        }

        
    }
    public void _DisenableButton() {

        switch (infoNumber) {
            case 1:
                AttackSkill_1_UI.SetActive(false);
                break;
        }

        for (int i = 0; i < 2; i++) {
            if (usingSkillNumber[i] == infoNumber) {
                usingSkillNumber[i] = 0;
                UpdateInfo();
            }
        }
    }
    public void _NextButton() {
        infoNumber++;
        UpdateInfo();
    }
    public void _PreviousButton() {
        infoNumber--;
        UpdateInfo();
    }
    public void _OpenPanel() {
        UpdateInfo();
        gameObject.SetActive(true);
        CameraCtrl.TouchCtrl = false;
    }
    public void _ClosePanel() {
        gameObject.SetActive(false);
        CameraCtrl.TouchCtrl = true;
    }
}
