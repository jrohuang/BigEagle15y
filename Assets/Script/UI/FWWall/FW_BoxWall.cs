using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FW_BoxWall : MonoBehaviour {

    [SerializeField] GameObject prefab, content;
    public static BoxWall boxWall;
    private Text name, info;
    private RectTransform contentRT;

	void Start () {
        Bag.OpenBox = true;
        name = transform.Find("name").GetComponent<Text>();
        info = transform.Find("info").GetComponent<Text>();
        boxWall = Bag.WallSir.GetComponent<BoxWall>();
        name.text = DataCtrl.English ? DataCtrl.eqbq.wall_data.Rows[boxWall.WallNumber - 1][6].ToString() : DataCtrl.eqbq.wall_data.Rows[boxWall.WallNumber -1 ][1].ToString();
        info.text = DataCtrl.English ? "Capacity:" + DataCtrl.eqbq.wall_data.Rows[boxWall.WallNumber - 1][5].ToString() : "容量:" + DataCtrl.eqbq.wall_data.Rows[boxWall.WallNumber - 1][5].ToString();
        contentRT = content.transform as RectTransform;

        for (int i = 0; i < boxWall.Items.Length; i++) {
            if (boxWall.Items[i] > 0) {
                GameObject g = Instantiate(prefab, content.transform);
                g.SendMessage("StartSetting", i);
                boxWall.alreadyInsPrefab[i] = true;
            }
        }

        UpdateContentSize();
    }

    public void PutItem(int number, int amount) {
        if (!boxWall.alreadyInsPrefab[number]) {
            GameObject g = Instantiate(prefab, content.transform);
            g.SendMessage("StartSetting", number);
            boxWall.alreadyInsPrefab[number] = true;
            UpdateContentSize();
        }
        boxWall.Items[number] += amount;
    }
    
	private void UpdateContentSize() {
        if (content.transform.childCount > 0) {
            GridLayoutGroup glg = content.GetComponent<GridLayoutGroup>();
            glg.constraintCount = Mathf.FloorToInt(contentRT.rect.height / glg.cellSize.x);
            float x = glg.cellSize.x;
            
            contentRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 
                (x + 5) * Mathf.CeilToInt((float)content.transform.childCount / glg.constraintCount));
        }
    }

}
