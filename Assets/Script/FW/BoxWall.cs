using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxWall : MonoBehaviour {

    public int WallNumber;
    public int[] Items;
    public bool[] alreadyInsPrefab;
    public int MaxAmount;

    private void Awake() {
        Items = new int[35];
    }

    void Start () {
        alreadyInsPrefab = new bool[PlayerCtrl.PD.Items.Length];
        MaxAmount = int.Parse(DataCtrl.eqbq.wall_data.Rows[WallNumber - 1][5].ToString());
	}
    
    public void SetItems(int[] items) {
        Items = items;
    }
    
}
