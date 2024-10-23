using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;

public class PlayerCtrl : MonoBehaviour {
    
    private static GameObject BagGO;
    private Text placeText;
    private GameObject Setting;
    public static bool isAndroid = false;
    public static bool InNavigation = true;
    private LayerMask layerMask, lightLayerMak;
    public static GameObject AIInfoGO;
    static GameObject uiui;
    static GameObject GetItemsText;
    private GameObject DiePanel;
    public static int MaxItemAmount;
    //PlayerData
    public static PlayerData PD  = new PlayerData();
    static AIInfo aiinfo = new AIInfo();
    class AIInfo {
        public Image BloodImage;
        public Text BloodText;
        public Text Name;
        public Text Info;
        public Text Info2;
    }
    public static int GameLevelNumber, MaxWallLevel;
    private GameObject lightStationIcon;
    
    float deltaTime;
    private GameObject lightStation;
    public static bool Lockai = false;
    private static GameObject lockAiGO;
    private float TouchTime;
    public static int speed = 25;
    public static bool LevelMaxed;
    bool Gyro = false;
    GameObject joystick;

    private void Awake() {
        Mcamera = Camera.main.gameObject;
        joystick = GameObject.Find("UI").transform.Find("joystickBG").gameObject;
        layerMask = 1 << 8;
        layerMask |= 1 << 12;
        lightLayerMak = LayerMask.GetMask("LightStation");

        BagGO = GameObject.Find("UI").transform.Find("Bag").gameObject;
        BagGO.SetActive(true);
        uiui = BagGO.transform.parent.gameObject;
        
        lightStationIcon = uiui.transform.Find("LightStationIcon").gameObject;
        DiePanel = uiui.transform.Find("DiePanel").gameObject;
        Setting = uiui.transform.Find("Setting").gameObject;
        placeText = BagGO.transform.parent.transform.Find("Level/Place/Text").GetComponent<Text>();
        EventTrigger et = placeText.transform.parent.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((eventData) => { ChangePlaceTextType(); });
        et.triggers.Add(entry);

        et = uiui.transform.Find("Gyro").transform.GetComponent<EventTrigger>();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((eventData) => { _ChangeGyro(); });
        et.triggers.Add(entry);

        GameLevelNumber = int.Parse(SceneManager.GetSceneAt(0).name.Substring(6, 1));
        MaxWallLevel = int.Parse(DataCtrl.eqbq.level_data.Rows[GameLevelNumber][2].ToString());

        rb = GetComponent<Rigidbody>();
        
        
        AIInfoGO = BagGO.transform.parent.Find("AIInfo").gameObject;
        AIInfoGO.SetActive(false);
        aiinfo.Name = AIInfoGO.transform.Find("AIName").GetComponent<Text>();
        aiinfo.Info = AIInfoGO.transform.Find("AIInfo").GetComponent<Text>();
        aiinfo.Info2 = AIInfoGO.transform.Find("AIInfo2").GetComponent<Text>();
        aiinfo.BloodImage = AIInfoGO.transform.Find("AIBloodBG/AIBlood").GetComponent<Image>();
        aiinfo.BloodText = aiinfo.BloodImage.transform.Find("Text").GetComponent<Text>();
        
        if (Application.platform == RuntimePlatform.Android) {
            isAndroid = true;
        }
        
        PD.Items = new int[Bag.itemSprite.Length];
        PD.MaxHP = int.Parse(DataCtrl.eqbq.player_data.Rows[0][3].ToString());
        PD.MaxHunger = float.Parse(DataCtrl.eqbq.player_data.Rows[0][1].ToString());
        PD.Level = 1;
        PD.Attack = 0.5f;
        PD.HP = PD.MaxHP;
        PD.Hunger = PD.MaxHunger;
        PD.skillLevel = 0;
        PD.XP = 0;
        LevelMaxed = false;
        MaxItemAmount = int.Parse(DataCtrl.eqbq.player_data.Rows[PD.Level - 1][4].ToString());
        
        GameObject AttackSkill = uiui.transform.Find("AttackSkill").gameObject;
        AttackSkill.SetActive(true);
        AttackSkill.SetActive(false);
        GameObject g = uiui.transform.Find("AttackSkill_1").gameObject;
        g.SetActive(true);
        g.SetActive(false);

        if (!DataCtrl.LoadData) {
            uiui.transform.Find("StartGift").gameObject.SetActive(true);
        }
        else {
            Time.timeScale = 1;
        }
        
    }

    RaycastHit hit;
    LayerMask floorMask = 1 << 9;
    RaycastHit floorHit;
    string floorName = "xd";
    float reduceHungerPer;
    bool B;
    GameObject Mcamera;

    Rigidbody rb;
    void Update () {
        
        if (Input.GetKeyDown(KeyCode.Escape)) {Setting.SetActive(true);}
        GetPosNameInMap();

        if (PD.Hunger - PD.MaxHunger * reduceHungerPer / 60 * Time.deltaTime > 0) {
            PD.Hunger -= PD.MaxHunger * reduceHungerPer / 60 * Time.deltaTime;
        }
        else {
            PD.Hunger = 0;
            PD.HP -= PD.MaxHP * 0.04f * Time.deltaTime;
        }

        if (PD.HP < PD.MaxHP) {
            if (PD.Hunger > PD.MaxHunger * 0.8) {
                if (PD.Hunger > PD.MaxHunger * 0.9) {
                    PD.HP += PD.MaxHP * 0.01f * Time.deltaTime;
                }
                else {
                    PD.HP += PD.MaxHP * 0.005f * Time.deltaTime;
                }
            }
        }
        else {
            PD.HP = PD.MaxHP;
        }
        
        if (PD.HP <= 0.1f) {
            DiePanel.SetActive(true);
            Time.timeScale = 0;
            if (File.Exists(Path.Combine(Application.persistentDataPath, "SaveLSD(DontOpen)"))) {
                File.Delete(Path.Combine(Application.persistentDataPath, "SaveLSD(DontOpen)"));
            }
        }
        
        //點擊牆壁
        if (isAndroid) {
            if (!CameraCtrl.height && !BagGO.activeSelf) {
                
                if (Input.GetTouch(Input.touchCount - 1).phase == TouchPhase.Began) {
                    TouchTime = Time.time;
                }
                if (Input.GetTouch(Input.touchCount - 1).phase == TouchPhase.Ended) {
                    if (Time.time < TouchTime + 0.2f) {
                        if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
                            TouchWall();
                        }
                    }
                }
            }
            else {
                if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)) {
                    if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
                        TouchWall();
                    }
                }
            }
        }
        else {
            if (Input.GetMouseButtonDown(0)) {
                if (!EventSystem.current.IsPointerOverGameObject()) {
                    TouchWall();
                }
            }
        }

        B = B ? false : true;
        if (B) {
            return;
        }

        if (Bag.EB.BulletNumber > -1) {
            Bag.EBAmount.text = PD.Items[Bag.bulletInfo[Bag.EB.BulletNumber].ItemNumber].ToString();
            Bag.EB.Amount = PD.Items[Bag.bulletInfo[Bag.EB.BulletNumber].ItemNumber];
        }
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2.5f, lightLayerMak);
        if (colliders.Length > 0) {
            if (!lightStationIcon.activeSelf) {
                lightStationIcon.SetActive(true);
                lightStation = colliders[0].gameObject;
            }
        }
        else {
            if (lightStationIcon.activeSelf) {
                lightStationIcon.SetActive(false);
            }
        }
        
    }
    


    private void FixedUpdate() {
        if (!Gyro) {
            return;
        }
        
        //if (Mathf.Abs(Input.acceleration.y - acc.y) > 0.05f || Mathf.Abs(Input.acceleration.x - acc.x) > 0.05f) {
            if (CameraCtrl.height) { 
                print(Input.acceleration.y - acc.y);
                rb.AddForce(new Vector3(Mathf.Clamp(Input.acceleration.y - acc.y * 5 * Time.deltaTime * speed, -0.4f, 0.4f)
                , 0, Mathf.Clamp(acc.x - Input.acceleration.x * 5 * Time.deltaTime * speed, -0.4f, 0.4f)));
            }
            else {
                rb.AddForce(Mcamera.transform.forward * Mathf.Clamp(Input.acceleration.y - acc.y, -0.2f, 0.2f) * Time.deltaTime * speed * 5);
                rb.AddForce(Mcamera.transform.right * Mathf.Clamp(Input.acceleration.x - acc.x, -0.2f, 0.2f) * Time.deltaTime * speed * 5);
            }
        //}
    }

    private void TouchWall() {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, 20, layerMask);
        if (hit.transform != null) {
            if (hit.transform.CompareTag("Wall")) {
                if (Vector3.Distance(transform.position, hit.transform.position) < 1) {
                    BagGO.SetActive(true);
                    BagGO.SendMessage("WallUI", hit.transform.gameObject);
                }
            }
        }
        
    }

    Vector3 acc;
    public void _ChangeGyro() {
        
        if (Gyro) {
            joystick.SetActive(true);
        }
        else {
            joystick.SetActive(false);
            acc = Input.acceleration;
        }
        
        Gyro = Gyro ? false : true;
    }

    private void GetPosNameInMap() {
        Ray ray = new Ray(transform.position, Vector3.down);
        Physics.Raycast(ray, out floorHit, 20, floorMask);

        if (floorHit.transform == null) {
            return;
        }

        if (floorHit.transform.name != floorName) {
            floorName = floorHit.transform.name;
            PlaceTextUpdate();
        }

    }

    public static void GetHurt(float hurt) {
        if (Bag.equip[1].UsingEquipNumber == -1) {
            PD.HP -= hurt;
        }
        else {
            if (Bag.equip[1].Durable >= PD.protection) {
                Bag.equip[1].Durable -= PD.protection;
                PD.HP -= hurt - PD.protection;
            }
            else {
                PD.HP -= hurt - Bag.equip[1].Durable;
                Bag.equip[1].Durable = 0;
            }
        }
    }

    public static void GetItems(int number, int amount) {
        if (PD.Items[number] + amount <= MaxItemAmount) {
            PD.Items[number] += amount;
        }
        else {
            PD.Items[number] = MaxItemAmount;
        }

        if (DataCtrl.English) {
            Record.AddRecord("Get [" + DataCtrl.eqbq.item_data.Rows[number][6].ToString() + "]*" + amount);
        }
        else {
            Record.AddRecord("獲得" + DataCtrl.eqbq.item_data.Rows[number][1].ToString() + "*" + amount);
        }
    }

    public static void GetXP(int xp) {
        if (LevelMaxed) {
            return;
        }
        if (PD.XP + xp >= int.Parse(DataCtrl.eqbq.player_data.Rows[PD.Level - 1][2].ToString())) {

            if (PD.Level == int.Parse(DataCtrl.eqbq.level_data.Rows[GameLevelNumber][1].ToString())) {
                LevelMaxed = true;
                if (DataCtrl.English) {
                    Record.AddRecord("Level Maxed");
                }
                else {
                    Record.AddRecord("等級已達到上限");
                }
                
                PD.XP = int.Parse(DataCtrl.eqbq.player_data.Rows[PD.Level - 1][2].ToString());
                Level.LevelTextUpdate();
                return;
            }

            xp -= int.Parse(DataCtrl.eqbq.player_data.Rows[PD.Level - 1][2].ToString()) - PD.XP;

            PD.MaxHunger = int.Parse(DataCtrl.eqbq.player_data.Rows[PD.Level][1].ToString());
            PD.MaxHP = int.Parse(DataCtrl.eqbq.player_data.Rows[PD.Level][3].ToString());
            PD.Hunger = PD.MaxHunger * PD.Hunger / int.Parse(DataCtrl.eqbq.player_data.Rows[PD.Level - 1][1].ToString());
            PD.HP = PD.MaxHP * PD.HP / int.Parse(DataCtrl.eqbq.player_data.Rows[PD.Level - 1][3].ToString());

            PD.Level++;
            MaxItemAmount = int.Parse(DataCtrl.eqbq.player_data.Rows[PD.Level - 1][4].ToString());
            Bag.ItemsMaxAmountText.text = DataCtrl.English ? "Weight capacity:" + MaxItemAmount.ToString() : "最高負重:" + MaxItemAmount.ToString();
            
            ////////////////Language
            PD.XP = 0;
            if (xp > 0) {
                GetXP(xp);
            }
        }
        else {
            if (!LevelMaxed) {
                PD.XP += xp;
            }

        }
        Level.LevelTextUpdate();

    }

    public static GameObject StayAI;
    public static void AIStay(int AINumber, float blood, GameObject g, int attackMode, float AddBlood, float rangeAttackR, 
        float rangeAttackH) {

        if (g == StayAI) {
            aiinfo.BloodImage.fillAmount = blood / DataCtrl.aiData[AINumber].Blood;
            aiinfo.BloodImage.fillAmount = Mathf.Lerp(aiinfo.BloodImage.fillAmount, blood / DataCtrl.aiData[AINumber].Blood, 0.1f);
            aiinfo.BloodText.text = blood > 0 ? blood.ToString("0.00") + " / " + DataCtrl.aiData[AINumber].Blood.ToString() : "0/" + DataCtrl.aiData[AINumber].Blood.ToString();
            return;
        }
        else {
            if (StayAI == null) {
                StayAI = g;
            }
            else {
                return;
            }
            AIInfoGO.SetActive(true);
            ////////////////////Langegue
            aiinfo.Name.text = DataCtrl.English ? DataCtrl.aiData[AINumber].Name_E : DataCtrl.aiData[AINumber].Name;
            aiinfo.Info.text = DataCtrl.English ? "Speed : " + DataCtrl.aiData[AINumber].Speed.ToString() +
                "\nXP : " + DataCtrl.aiData[AINumber].XP.ToString() : "移動速度 : " + DataCtrl.aiData[AINumber].Speed.ToString() +
                "\n獲得經驗 : " + DataCtrl.aiData[AINumber].XP.ToString();

            if (attackMode == 1 || attackMode == 2) {
                if (DataCtrl.aiAttack[AINumber].hpAttack) {
                    if (DataCtrl.English) {
                        aiinfo.Info2.text = attackMode == 2 ? "Bullet Hurt:" : "Attack Hurt";
                    }
                    else {
                        aiinfo.Info2.text = attackMode == 2 ? "子彈傷害:" : "攻擊傷害";
                    }
                    for (int i = 0; i < DataCtrl.aiAttack[AINumber].hpAttackTime.Length; i++) {
                        aiinfo.Info2.text += DataCtrl.aiAttack[AINumber].hpAttackHurt[i].ToString() + "/" +
                            DataCtrl.aiAttack[AINumber].hpAttackTime[i].ToString() + "s";
                        aiinfo.Info2.text += " ";
                    }
                }

                if (DataCtrl.aiAttack[AINumber].HAttack) {
                    if (DataCtrl.aiAttack[AINumber].hpAttack) {
                        if (DataCtrl.English) {
                            aiinfo.Info2.text += attackMode == 2 ? "Bullet Hunger Taken" : "Hunger Taken";
                        }
                        else {
                            aiinfo.Info2.text += attackMode == 2 ? "子彈疲倦" : "疲倦";
                        }
                    }
                    else {
                        if (DataCtrl.English) {
                            aiinfo.Info2.text = attackMode == 2 ? "Bullet Hunger Taken" : "Hunger Taken";
                        }
                        else {
                            aiinfo.Info2.text = attackMode == 2 ? "子彈疲憊" : "疲倦";
                        }
                    }

                    for (int i = 0; i < DataCtrl.aiAttack[AINumber].HAttackTime.Length; i++) {
                        aiinfo.Info2.text += DataCtrl.aiAttack[AINumber].HAttackHurt[i].ToString() + "/" +
                            DataCtrl.aiAttack[AINumber].HAttackTime[i].ToString() + "s";
                        aiinfo.Info2.text += " ";
                    }
                }
            }
            else if (attackMode == 3) {
                aiinfo.Info2.text = DataCtrl.English ? "Attack Range" : "範圍攻擊:\n";
                if (DataCtrl.English) {
                    aiinfo.Info2.text = rangeAttackR + "m/" + rangeAttackH + "Hurt/s";
                }
                else {
                    aiinfo.Info2.text = rangeAttackR + "m/" + rangeAttackH + "傷害/s";
                }
            }
            else{
                aiinfo.Info2.text = "";
            }
            if (AddBlood > 0) {
                aiinfo.Info2.text += DataCtrl.English ? "Healing:" + AddBlood + "/s" : "回血:" + AddBlood  + "/s";
            }

            aiinfo.BloodImage.fillAmount = blood / DataCtrl.aiData[AINumber].Blood;
            aiinfo.BloodImage.fillAmount = Mathf.Lerp(aiinfo.BloodImage.fillAmount, blood / DataCtrl.aiData[AINumber].Blood, 0.1f);
            
            aiinfo.BloodText.text = blood > 0 ? blood.ToString("0.00") + " / " + DataCtrl.aiData[AINumber].Blood.ToString() : "0/" + DataCtrl.aiData[AINumber].Blood.ToString();
        }
        
    }
    public static void AIExit() {
        StayAI = null;
        AIInfoGO.SetActive(false);
    }

    private bool PlaceTextIsPlaceName = true;
    private void ChangePlaceTextType() {
        PlaceTextIsPlaceName = PlaceTextIsPlaceName ? false : true;
        PlaceTextUpdate();
    }
    private void PlaceTextUpdate() {
        switch (floorName.Substring(5, 1)) {
            case "j":
                if (DataCtrl.English) {placeText.text = PlaceTextIsPlaceName ? "Peripheral Area-" + floorName.Substring(6, 1) : "(-2%/m)";}
                else {placeText.text = PlaceTextIsPlaceName ? floorName.Substring(6, 1) + "號外圍區" : "(-2%/m)";}
                reduceHungerPer = 0.02f;
                break;
            case "5":
                if (DataCtrl.English) {placeText.text = PlaceTextIsPlaceName ? "Center Area" : "(-12%/m)";}
                else {placeText.text = PlaceTextIsPlaceName ? "中心區" : "(-12%/m)";}
                reduceHungerPer = 0.12f;
                break;
            case "c":
                if (DataCtrl.English) {placeText.text = PlaceTextIsPlaceName ? "Core Area" : "(-60%/m)";}
                else {placeText.text = PlaceTextIsPlaceName ? "核心區" : "(-60%/m)";}
                reduceHungerPer = 0.6f;
                break;
        }
    }

    public void _LightStationIcon() {
        lightStation.SendMessage("ChangeColor");
    }

}