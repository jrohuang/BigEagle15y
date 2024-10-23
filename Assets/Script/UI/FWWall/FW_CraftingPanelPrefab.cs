using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FW_CraftingPanelPrefab : MonoBehaviour {

    GameObject FW_Crafting;

    void SetFW_Crafting(GameObject g) {
        FW_Crafting = g;
    }

    public void _CraftingItem() {
        FW_Crafting.SendMessage("UpdateInfo", int.Parse(gameObject.name));
    }

}
