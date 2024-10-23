using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FastItems : MonoBehaviour {

    private Image[] image = new Image[3];
    private Text[] text = new Text[3];
    private static Animator[] animators = new Animator[3];
    private static int[] itemsNumber = new int[3];
    private static bool ChangingItem;
    Bag bag;
    
    private void Awake() {
        bag = transform.parent.Find("Bag").GetComponent<Bag>();
        for (int i = 0; i < image.Length; i++) {
            image[i] = transform.Find("item" + (i + 1).ToString()).GetComponent<Image>();
            animators[i] = transform.Find("item" + (i + 1).ToString()).GetComponent<Animator>();
            text[i] = image[i].transform.GetChild(0).GetChild(0).GetComponent<Text>();
            itemsNumber[i] = -1;
        }
    }
    
    void Update () {
        for (int i = 0; i < image.Length; i++) {
            text[i].text = itemsNumber[i] == -1 ? "" : PlayerCtrl.PD.Items[itemsNumber[i]].ToString();
        }
        if (PointStay) {
            if (afterDownTime + 1 <= Time.time) {
                if (!IsInvoking()) {
                    InvokeRepeating("UseItem", 0, 0.16f);
                }
            }
        }
	}

    private void UseItem() {
        if (!ChangingItem) {
            if (PlayerCtrl.PD.Items[itemsNumber[LongPointItemNumber - 1]] > 0) {
                Bag.selectNumber = itemsNumber[LongPointItemNumber - 1]; 
                bag._UseButton();
            }
        }
        else {
            ChangingItem = false;
            for (int i = 0; i < animators.Length; i++) {
                animators[i].SetBool("Changing", false);
            }

            itemsNumber[LongPointItemNumber - 1] = ChangItemNumber;
            image[LongPointItemNumber - 1].sprite = Bag.itemSprite[ChangItemNumber];


        }
    }

    public void _FastItemClick(int number) {
        if (!ChangingItem) {
            if (itemsNumber[number - 1] == -1) {
                return;
            }
            if (PlayerCtrl.PD.Items[itemsNumber[number - 1]] > 0) {
                Bag.selectNumber = itemsNumber[number - 1];
                bag._UseButton();
            }
        }
        else {
            ChangingItem = false;
            for (int i = 0; i < animators.Length; i++) {
                animators[i].SetBool("Changing", false);
            }

            itemsNumber[number - 1] = ChangItemNumber;
            image[number - 1].sprite = Bag.itemSprite[ChangItemNumber];
        }
    }

    float afterDownTime;
    int LongPointItemNumber;
    bool PointStay = false;
    public void _PointDown(int number) {
        LongPointItemNumber = number;
        afterDownTime = Time.time;
        PointStay = true;
    }
    public void _PointUp() {
        PointStay = false;
        CancelInvoke();
    }

    private static int ChangItemNumber;

    public static void ChangeFastItem(int ChangeNumber) {

        if (ChangingItem) {
            ChangingItem = false;
            for (int i = 0; i < animators.Length; i++) {
                animators[i].SetBool("Changing", false);
            }
            return;
        }
        
        ChangItemNumber = ChangeNumber;
        for (int i = 0; i < animators.Length; i++) {
            animators[i].SetBool("Changing", true);
        }
        
        //
        ChangingItem = true;
    }

    public static int[] GetFastItemsNumber() {
        return itemsNumber;
    }
    public static void SetFastItemsNumber(int [] vs) {
        itemsNumber = vs;
    }

    void UpdateImage() {
        for (int i = 0; i < image.Length; i++) {
            if (itemsNumber[i] > -1) {
                image[i].sprite = Bag.itemSprite[itemsNumber[i]];
            }
        }
    }
    

}
