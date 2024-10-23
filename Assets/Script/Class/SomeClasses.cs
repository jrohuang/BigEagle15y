using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data;

[System.Serializable]
public class PlayerData {
    public float MaxHunger;
    public float Hunger;
    public float MaxHP;
    public float HP;
    public float Attack;
    public int[] Items;
    public int Level;
    public int XP;
    public float protection;
    public int skillLevel;
}

[System.Serializable]
public class ItemCrafting {
    public int wall;
    public int level;
    public string name;
    public string need;
    public int number;
    public int Amount;
    public string name_E;
}

[System.Serializable]
public class PlayerRecord{
    public int Level;
    public bool English;
    public float[] SettingValue;// music / camera / touch
}

[System.Serializable]
public class AIInfo {
    public int number;
    public Vector3 pos;
    public float helath;
    public int HomeNumber;
    public string Name;
    public Quaternion rotation;
}

[System.Serializable]
public class AIAttack {
    public bool hpAttack;
    public float[] hpAttackTime;
    public float[] hpAttackHurt;
    public bool HAttack;
    public float[] HAttackTime;
    public float[] HAttackHurt;
    public float AttackDistance;
    public float RangeAttackHurt;
}

[System.Serializable]
public class LevelSaveData {
    public int Level;
    public Vector3 playerPos;
    public PlayerData pd;
    public int[] fastItemsNumber = new int[3];
    public AIInfo[] AIs;
    public AIInfo[] bosses;
    public float[] EquipDurable;
    public int[] equip;
    public int EBNumber;
    public FWWallInfo[] FW;
    public int[] UsingAttackSkillNumber;
    public bool LevelMaxed;
}

[System.Serializable]
public class Equip {
    public Image image;
    public Image imageDurable;
    public Text EquipText;
    public int UsingEquipNumber;
    public float MaxDurable;
    public float Durable;
}

[System.Serializable]
public class FWWallInfo {
    public int Number;
    public Vector3 Pos;
    public F3 f3;
    public FBox FBox;
}

[System.Serializable]
public class AIData {
    public string Name;
    public float Blood;
    public float Speed;
    public int XP;
    public int AttackMode;
    public string Name_E;
}

[System.Serializable]
public class F3 {
    public int[] AttackInfo;
    public int[] SaveItems;
}

[System.Serializable]
public class FBox {
    public int[] Item;
}

