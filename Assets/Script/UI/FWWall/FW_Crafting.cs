using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class FW_Crafting : MonoBehaviour {

    [SerializeField] GameObject Content, prefab;
    [SerializeField] Text text, name;
    [SerializeField] Button button, changeAmountButton;
    private int CraftingWallNumber;
    private int CraftingAmount;
    private int x = 1;

    private void Start() {

        string s = Bag.WallSir.name;
        int w = gameObject.name.IndexOf("_");
        CraftingWallNumber = int.Parse(s.Substring(w + 1, s.Length - w - 1));
        for (int i = 0; i < DataCtrl.ic.Length; i++) {
            if (DataCtrl.ic[i].wall == CraftingWallNumber) {
                if (PlayerCtrl.PD.Level >= DataCtrl.ic[i].level) {
                    GameObject g = Instantiate(prefab, Content.transform, false);
                    g.name = i.ToString();
                    g.SendMessage("SetFW_Crafting", gameObject);
                    g.GetComponentInChildren<Text>().text = DataCtrl.English ? DataCtrl.ic[i].name_E : DataCtrl.ic[i].name;
                    g.transform.Find("Image").GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Items/Item" + DataCtrl.ic[i].number.ToString());
                }
            }

        }

        name.text = DataCtrl.English ? DataCtrl.eqbq.wall_data.Rows[CraftingWallNumber - 1][6].ToString() : DataCtrl.eqbq.wall_data.Rows[CraftingWallNumber - 1][1].ToString();

        RectTransform rt = Content.transform as RectTransform;
        float x = (prefab.transform as RectTransform).sizeDelta.x;
        rt.sizeDelta = new Vector2((x + 2) * Content.transform.childCount, rt.sizeDelta.y);
    }

    private int[,] need;
    private bool selectItem = false;
    private int ItemNumber;
    private int ICNumber;

    void UpdateInfo(int Number) {
        x = 1;
        changeAmountButton.interactable = true;
        button.interactable = false;
        selectItem = true;
        ItemNumber = DataCtrl.ic[Number].number;
        ICNumber = Number;

        string[] array = Regex.Split(DataCtrl.ic[Number].need, ",", RegexOptions.IgnoreCase);
        need = new int[array.Length, 2];
        
        for (int i = 0; i < array.Length; i++) {
            int a = array[i].IndexOf(".");
            need[i, 0] = int.Parse(array[i].Substring(0, a));
            need[i, 1] = int.Parse(array[i].Substring(a + 1, array[i].Length - a - 1));
        }
        
        CraftingAmount = DataCtrl.ic[ICNumber].Amount;
    }

    void Update () {
        if (!selectItem) {return;}
        bool b = true;
        
        text.text = DataCtrl.English ? "Requirement to make " + CraftingAmount * x + " copies of items:\n" : "合成" + CraftingAmount * x + "份所需:\n";
        for (int i = 0; i < need.GetLength(0); i++) {
            if (PlayerCtrl.PD.Items[need[i, 0]] >= need[i, 1] * x) {
                text.text += "<color=#000000>";
            }
            else {
                text.text += "<color=#FF0000>";
                b = false;
            }
            text.text += DataCtrl.English ? DataCtrl.eqbq.item_data.Rows[need[i, 0]][6] + "(" + PlayerCtrl.PD.Items[need[i, 0]] + "/" + (need[i, 1] * x).ToString() + ")</color>  "
                : DataCtrl.eqbq.item_data.Rows[need[i, 0]][1] + "(" + PlayerCtrl.PD.Items[need[i, 0]] + "/" + (need[i, 1] * x).ToString() + ")</color>  ";
        }
        text.text += "\n";

        string[] array = DataCtrl.English ? DataCtrl.eqbq.item_data.Rows[ItemNumber][8].ToString().Split('%') : DataCtrl.eqbq.item_data.Rows[ItemNumber][3].ToString().Split('%');
        for (int i = 0; i < array.Length; i++) {
            text.text += array[i] + "\n";
        }

        i = new int[need.GetLength(0) + 1];
        for (int j = 0; j < need.GetLength(0); j++) {
            if (PlayerCtrl.PD.Items[need[j, 0]] > 0) {
                i[j] = PlayerCtrl.PD.Items[need[j, 0]] / need[j, 1];
            }
            else {
                i[j] = 0;
            }
        }
        i[i.Length - 1] = PlayerCtrl.MaxItemAmount / CraftingAmount;

        if (PlayerCtrl.PD.Items[ItemNumber] + CraftingAmount > PlayerCtrl.MaxItemAmount) {
            b = false;
        }


        changeAmountButton.interactable = Mathf.Min(i) == 0 ? false : true;
        button.interactable = b ? true : false;
    }

    int[] i;

    public void _CraftingItem() {
        for (int i = 0; i < need.GetLength(0); i++) {
            PlayerCtrl.PD.Items[need[i, 0]] -= need[i, 1] * x;
        }
        PlayerCtrl.PD.Items[ItemNumber] += CraftingAmount * x;
    }

    public void _ChangeCraftingAmount() {
        GameObject g = GameObject.Find("UI").transform.Find("AmountScrollBar").gameObject;
        g.SetActive(true);
        g.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y + 100, Input.mousePosition.z);
        AmountScrollBar asb = g.GetComponent<AmountScrollBar>();
        
        if (PlayerCtrl.MaxItemAmount - PlayerCtrl.PD.Items[ItemNumber] > Mathf.Min(i) * CraftingAmount) {
            asb.SetInfo(CraftingAmount, CraftingAmount * Mathf.Min(i), CraftingAmount, CraftingAmount);
        }
        else {
            asb.SetInfo(CraftingAmount, (PlayerCtrl.MaxItemAmount - PlayerCtrl.PD.Items[ItemNumber]) / CraftingAmount, CraftingAmount, CraftingAmount);
        }

        AmountScrollBar.whenCheck = CCA;
    }

    void CCA(int amount) {
        x = amount / CraftingAmount;
    }

}
