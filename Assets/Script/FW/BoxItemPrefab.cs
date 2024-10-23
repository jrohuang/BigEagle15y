using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxItemPrefab : MonoBehaviour {

    [SerializeField] GameObject amountSBar;
    private Image image;
    private Text text;
    private int number;
    private BoxWall boxWall;

    void StartSetting(int n) {
        number = n;
        image = transform.Find("Image").GetComponent<Image>();
        text = transform.Find("Image/image/Text").GetComponent<Text>();

        image.sprite = Bag.itemSprite[n];
        boxWall = FW_BoxWall.boxWall;
    }
    
	void Update () {
        text.text = boxWall.Items[number].ToString();
        if (boxWall.Items[number] < 1) {
            boxWall.alreadyInsPrefab[number] = false;
            transform.GetComponentInParent<FW_BoxWall>().SendMessage("UpdateContentSize");
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
                asb.SetInfo(1, k >= boxWall.Items[number] ? boxWall.Items[number] : k, 1, 1);
                AmountScrollBar.whenCheck = PutItemToBag;
            }
        }
    }
    
    private bool Clicking = false;
    private bool usingASB = false;
    private float time;
    
    void PutItemToBag(int amount) {
        if (PlayerCtrl.PD.Items[number] < PlayerCtrl.MaxItemAmount) {
            boxWall.Items[number] -= amount;
            PlayerCtrl.PD.Items[number] += amount;
        }
        usingASB = false;
    }

    public void _Click() {
        if (!usingASB) {
            if (PlayerCtrl.PD.Items[number] < PlayerCtrl.MaxItemAmount) {
                boxWall.Items[number]--;
                PlayerCtrl.PD.Items[number]++;
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


}
