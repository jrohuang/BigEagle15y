using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Teaching : MonoBehaviour {

    [SerializeField] GameObject menuPanel;
    private Image image, next, last;
    private Sprite[] C, E;
    int spriteNumber = 0;

    private void Awake() {
        next = transform.Find("Next").GetComponent<Image>();
        last = transform.Find("Last").GetComponent<Image>();

        C = new Sprite[5];
        E = new Sprite[5];

        for (int i = 0; i < C.Length; i++) {
            C[i] = Resources.Load<Sprite>("Teaching/C" + (i + 1).ToString());
            E[i] = Resources.Load<Sprite>("Teaching/E" + (i + 1).ToString());
        }

        image = GetComponent<Image>();
        next.gameObject.SetActive(true);
        last.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void _Open() {
        gameObject.SetActive(true);
        menuPanel.SetActive(false);
        next.gameObject.SetActive(true);
        last.gameObject.SetActive(false);
        image.sprite = DataCtrl.English ? E[0] : C[0];
        spriteNumber = 0;
    }
    public void _Close() {
        gameObject.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void _Next() {
        image.sprite = DataCtrl.English ? E[spriteNumber + 1] : C[spriteNumber + 1];
        spriteNumber++;
        if (spriteNumber == E.Length - 1) {
            next.gameObject.SetActive(false);
        }
        if (spriteNumber > 0) {
            last.gameObject.SetActive(true);
        }
    }
    public void _Last() {
        image.sprite = DataCtrl.English ? E[spriteNumber - 1] : C[spriteNumber - 1];
        spriteNumber--;
        if (spriteNumber == 0) {
            last.gameObject.SetActive(false);
        }
        if (spriteNumber < E.Length + 1) {
            next.gameObject.SetActive(true);
        }
    }
}
