using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionWall : MonoBehaviour {
    
    public void _Click() {
        Bag.MEGO.SendMessage("FunctionWallClick", int.Parse(gameObject.name));
    }
    
}
