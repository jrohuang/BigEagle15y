using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackWallPanelPrefab : MonoBehaviour {

    private Image image;
    private Text text;
    private int number;
    
    void StartSetting(int n) {
        number = n;
        image = transform.Find("Image").GetComponent<Image>();
        text = transform.Find("Image/image/Text").GetComponent<Text>();

        image.sprite = Bag.itemSprite[n];
    }
    
	void Update () {
        text.text = FW_AttackWall.saveItems[number].ToString();
        if (FW_AttackWall.saveItems[number] < 1) {
            FW_AttackWall.alreadyInsPrefab[number] = false;
            Destroy(gameObject);
        }

        if (Clicking) {
            if (Time.time - 0.3f > time) {
                Clicking = false;
                usingASB = true;
                GameObject g = GameObject.Find("UI").transform.Find("AmountScrollBar").gameObject;
                g.SetActive(true);
                g.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y + 100, Input.mousePosition.z);
                AmountScrollBar asb = g.GetComponent<AmountScrollBar>();
                int k = PlayerCtrl.MaxItemAmount - PlayerCtrl.PD.Items[number];
                asb.SetInfo(0, k >= FW_AttackWall.saveItems[number] ? FW_AttackWall.saveItems[number] : k, 1, 1);
                AmountScrollBar.whenCheck = PutToBag;
            }
        }
    }

    private float time;
    private bool Clicking;
    private bool usingASB;

    void PutToBag(int amount) {
        PlayerCtrl.PD.Items[number] += amount;
        FW_AttackWall.saveItems[number] -= amount;

        if (FW_AttackWall.saveItems[number] < 1) {
            Destroy(gameObject);
            FW_AttackWall.alreadyInsPrefab[number] = false;
        }
        usingASB = false;
    }

    public void Click() {
        if (!usingASB) {
            if (PlayerCtrl.PD.Items[number] < PlayerCtrl.MaxItemAmount) {
                PlayerCtrl.PD.Items[number]++;
                FW_AttackWall.saveItems[number]--;
            }
            if (FW_AttackWall.saveItems[number] < 1) {
                Destroy(gameObject);
                FW_AttackWall.alreadyInsPrefab[number] = false;
            }
        }
    }
    public void _PointerDown() {
        time = Time.time;
        Clicking = true;
    }
    public void _ClickingFalse() {
        Clicking = false;
    }

    void AllPutToBag() {
        int a = PlayerCtrl.MaxItemAmount - PlayerCtrl.PD.Items[number];
        if (a >= FW_AttackWall.saveItems[number]) {
            PlayerCtrl.PD.Items[number] += FW_AttackWall.saveItems[number];
            FW_AttackWall.saveItems[number] = 0;
        }
        else {
            PlayerCtrl.PD.Items[number] += a;
            FW_AttackWall.saveItems[number] -= a;
        }
    }

}
