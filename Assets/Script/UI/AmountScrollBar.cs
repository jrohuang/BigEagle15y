using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmountScrollBar : MonoBehaviour {

    private Scrollbar sBar;
    private Text text;
    private float min, max;
    private bool isUsing = false;
    
    public delegate void MyDelegate(int amount);
    public static MyDelegate whenCheck;
    private float amount;
    float a;
    int space;

    private void Awake() {
        sBar = transform.GetComponentInChildren<Scrollbar>();
        text = sBar.transform.Find("a/b/c").GetComponent<Text>();
    }
    void Update () {
        a = max - min;
        amount = sBar.value * max > min ? sBar.value * max : min;
        if (space == 1) {
            text.text = amount.ToString("0");
        }
        else{
            int a = Mathf.FloorToInt(amount);
            if(a % space == 0) {
                text.text = a.ToString();
            }
            else {
                a += a % space;
                text.text = a.ToString();
            }
        }

	}

    public void SetInfo(float Min, float Max, int SPACE, int startAmount) {
        isUsing = true;
        min = Min;
        max = Max;
        space = SPACE;
        sBar.value = 1 / max * startAmount;
    }

    public void _Exit() {
        gameObject.SetActive(false);
        sBar.value = 0;
        text.text = "0";
    }
    public void _Add() {
        sBar.value += space / max;
    }
    public void _Minus() {
        sBar.value -= space / max;
    }
    public void _Check() {
        float a = max - min;
        whenCheck(int.Parse(text.text));
        gameObject.SetActive(false);
        sBar.value = 0;
        text.text = "0";
    }

}
