using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class Bag : MonoBehaviour {
    
    [SerializeField] GameObject P_Info_Item, P_Info_Wall, P_WallCreate;
    [SerializeField] GameObject[] FWUI;
    [SerializeField] GameObject FunctionWallPrefab, DestroyFWButton;
    [SerializeField] GameObject content;
    GameObject sceneGameObject;
    public static GameObject MEGO;

    public static bool IsOpenBag = false;
    private GameObject functionWallPanel, functionWallPanelabc, equipPanel, RecordPanel;
    public static GameObject[] functionWall = new GameObject[10];
    int[] ItemNumber = new int[35];
    private Image[] itemsImage = new Image[35];
    private Text[] itemsText = new Text[35];
    public static Sprite[] itemSprite = new Sprite[35];
    Sprite[] wallSprite = new Sprite[9];
    public static int selectNumber = -1;
    
    private static ItemInfo info = new ItemInfo();
    private FWCreate wallCreate = new FWCreate();
    private bool CreatingWall = false;
    public static GameObject WallSir { get; private set; }
    public static Text ItemsMaxAmountText;
    private GameObject bagContent;
    private GameObject amountScrollBar;
    //Equip
    public static Equip[] equip = new Equip[2];
    public static EquipInfo[] equipInfo = new EquipInfo[6]; //[equipAmount]
    public static EquipInfo[] bulletInfo = new EquipInfo[3]; //[bulletAmount]
    public static Bullet EB = new Bullet();

    //equip-bullet(EB)
    private static Image EBImage;
    public static Text EBAmount, EBText;

    public class Bullet {
        public int BulletNumber;
        public int Amount;
        public float Hurt;
        public float Speed;
    }
    public class EquipInfo {
        public int ItemNumber;
        public float Attack;
        public float MaxDurable;
        public float DurableRecord;
        public float Protect;
        public float Speed;
        public float hurt;
    }
    class FWCreate {
        public Text name;
        public Text need;
        public Button button;
    }
    class ItemInfo {
        public Image image;
        public Text  name;
        public Text kind;
        public Text use;
        public Text amount;
        public Button unloadButton;
        public Button useButton;
        public Button toFastButton;
    }
    
    private void GetGet() {
        EB.BulletNumber = -1;
        bagContent = transform.Find("Bag/Viewport/Content").gameObject;
        MEGO = gameObject;
        sceneGameObject = GameObject.Find("Scene");
        amountScrollBar = transform.parent.Find("AmountScrollBar").gameObject;
        functionWallPanel = transform.Find("DownPanel/Panel").gameObject;
        functionWallPanelabc = functionWallPanel.transform.Find("a/b/c").gameObject;
        equipPanel = transform.Find("DownPanel/equipPanel").gameObject;
        EBAmount = equipPanel.transform.Find("EB/Amount/Text").GetComponent<Text>();
        EBImage = equipPanel.transform.Find("EB/Image").GetComponent<Image>();
        EBText = equipPanel.transform.Find("EBText").GetComponent<Text>();
        RecordPanel = transform.parent.Find("Record").gameObject;
        info.image = transform.Find("Info/ItemImage").GetComponent<Image>();
        info.name = transform.Find("Info/ItemName").GetComponent<Text>();
        info.kind = transform.Find("Info/ItemKind").GetComponent<Text>();
        info.use = transform.Find("Info/ItemUse/a/b/c").GetComponent<Text>();
        info.amount = transform.Find("Info/ItemAmount").GetComponent<Text>();
        info.useButton = transform.Find("Info/UseButton").GetComponent<Button>();
        info.unloadButton = transform.Find("Info/UnloadButton").GetComponent<Button>();
        info.toFastButton = transform.Find("Info/ToFastUse").GetComponent<Button>();
        wallCreate.name = transform.Find("FWinfo/WallCreate/Name").GetComponent<Text>();
        wallCreate.need = transform.Find("FWinfo/WallCreate/Need").GetComponent<Text>();
        wallCreate.button = transform.Find("FWinfo/WallCreate/Button").GetComponent<Button>();
        ItemsMaxAmountText = transform.Find("Space/Text").GetComponent<Text>();

        for (int i = 1; i < itemSprite.Length + 1; i++) {
            itemSprite[i - 1] = Resources.Load<Sprite>("Items/Item" + (i - 1).ToString());
        }
        for (int i = 1; i < wallSprite.Length + 1; i++) {
            wallSprite[i - 1] = Resources.Load<Sprite>("FunctionWall/F" + i);
        }
        for (int i = 0; i < itemsImage.Length; i++) {
            itemsImage[i] = content.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>();
            itemsText[i] = content.transform.GetChild(i).transform.Find("Amount/Text").GetComponent<Text>();
        }
        functionWall[0] = Resources.Load<GameObject>("FunctionWall/Wall");
        for (int i = 1; i < functionWall.Length; i++) {
            functionWall[i] = Resources.Load<GameObject>("FunctionWall/FunctionWall_" + (i).ToString());
        }
        
        for (int i = 0; i < equip.Length; i++) {
            equip[i] = new Equip();
            equip[i].image = equipPanel.transform.Find("E" + i + "/Image").GetComponent<Image>();
            equip[i].imageDurable = equipPanel.transform.Find("Durable" + i).GetComponent<Image>();
            equip[i].EquipText = equipPanel.transform.Find("E" + i + "Text").GetComponent<Text>();
            equip[i].UsingEquipNumber = -1;
        }


        string[] array;
        int a;
        for (int i = 0; i < DataCtrl.eqbq.item_data.Rows.Count; i++) {
            string s = DataCtrl.eqbq.item_data.Rows[i][4].ToString();
            if (s.Substring(0, 1) == "o") {

                switch (s.Substring(1, 1)) {
                    case "A":
                        array = s.Substring(2, s.Length - 2).Split('.');
                        a = int.Parse(array[0]) - 1;
                        equipInfo[a] = new EquipInfo();
                        equipInfo[a].ItemNumber = i;
                        equipInfo[a].Attack = float.Parse(array[1]);
                        equipInfo[a].MaxDurable = float.Parse(array[2]);
                        break;
                    case "P":
                        array = s.Substring(2, s.Length - 2).Split('.');
                        a = int.Parse(array[0]) - 1;
                        equipInfo[a] = new EquipInfo();
                        equipInfo[a].ItemNumber = i;
                        equipInfo[a].Protect = float.Parse(array[1]);
                        equipInfo[a].MaxDurable = float.Parse(array[2]);
                        break;
                    case "B":
                        array = s.Substring(2, s.Length - 2).Split('.');
                        a = int.Parse(array[0]) - 1;
                        bulletInfo[a] = new EquipInfo();
                        bulletInfo[a].ItemNumber = i;
                        bulletInfo[a].hurt = float.Parse(array[1]);
                        bulletInfo[a].Speed = float.Parse(array[2]);
                        break;
                }
            }
        }
        
        for (int i = 0; i < ItemNumber.Length; i++) {
            ItemNumber[i] = -1;
        }

        GridLayoutGroup glg = bagContent.GetComponent<GridLayoutGroup>();
        RectTransform rt = bagContent.transform as RectTransform;
        
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
            glg.cellSize.y * Mathf.CeilToInt((float)bagContent.transform.childCount / Mathf.FloorToInt(rt.rect.width / glg.cellSize.x)));
        gameObject.SetActive(false);
    }

    private void Awake() {
        P_Info_Wall.SetActive(false);
        P_Info_Item.SetActive(true);
        
        GetGet();
    }
    bool B;
    bool B2;
    void Update() {

        B = B ? false : true;
        if (B) {
            return;
        }
        B2 = B2 ? false : true;
        if (B2) {
            return;
        }

        if (selectNumber != -1) {
            if (PlayerCtrl.PD.Items[selectNumber] < 1) {
                if (IsInvoking()) {
                    CancelInvoke();
                }
            }
        }

        UpdateBag();
        UpdateItemInfo();
        EquipUpdate();
        if (CreatingWall) {
            FunctionWallClick(FunctionWallClickNumber);
        }
        if (Clicking) {
            if (Time.time - 0.6f > time) {
                Clicking = false;
                if (OpenBox && selectNumber != -1) {
                    usingASB = true;
                    GameObject g = GameObject.Find("UI").transform.Find("AmountScrollBar").gameObject;
                    g.SetActive(true);
                    g.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y + 100, Input.mousePosition.z);
                    AmountScrollBar asb = g.GetComponent<AmountScrollBar>();
                    BoxWall bw = WallSir.GetComponent<BoxWall>();
                    int k = bw.MaxAmount - bw.Items[selectNumber];
                    asb.SetInfo(0, k > PlayerCtrl.PD.Items[selectNumber] ? PlayerCtrl.PD.Items[selectNumber] : k, 1, 1);
                    AmountScrollBar.whenCheck = PutItemToBoxWall;
                }
                else {
                    if (info.useButton.interactable) {
                        InvokeRepeating("_UseButton", 0, 0.25f);
                    }
                }
            }
        }
        

    }

    private void FunctionWallUI() {
        functionWallPanel.SetActive(false);
        P_WallCreate.SetActive(false);
        int w = WallSir.name.IndexOf("_");
        int a = int.Parse(WallSir.name.Substring(w + 1, WallSir.name.Length - w - 1));
        GameObject g = Instantiate(FWUI[a - 1], transform, false);
        if (a == 3 || a == 8) {
            OpenBox = true;
        }
        DestroyFWUI(g);
    }
    
    void WallUI(GameObject wallSir) {
        CameraCtrl.TouchCtrl = false;
        RecordPanel.SetActive(false);
        DestroyFWUI(gameObject);
        P_Info_Item.SetActive(false);
        P_Info_Wall.SetActive(true);
        functionWallPanel.SetActive(true);
        WallSir = wallSir;
        equipPanel.SetActive(false);

        if (wallSir.name.Substring(0, 4) == "Wall") {
            P_WallCreate.SetActive(true);
            DestroyFWButton.SetActive(false);
        }
        else {
            FunctionWallUI();
            DestroyFWButton.SetActive(true);
            equipPanel.SetActive(true);
        }
        
        Image image;
        Text name, use;
        for (int i = 0; i < DataCtrl.eqbq.wall_data.Rows.Count; i++) {
            if (functionWallPanelabc.transform.Find(int.Parse(DataCtrl.eqbq.wall_data.Rows[i][0].ToString()).ToString())) {}
            else {
                if (PlayerCtrl.PD.Level >= int.Parse(DataCtrl.eqbq.wall_data.Rows[i][4].ToString())) {
                    if (int.Parse(DataCtrl.eqbq.wall_data.Rows[i][0].ToString()) <= PlayerCtrl.MaxWallLevel) {
                        GameObject g = Instantiate(FunctionWallPrefab, functionWallPanelabc.transform, false);
                        g.name = int.Parse(DataCtrl.eqbq.wall_data.Rows[i][0].ToString()).ToString();
                        image = g.transform.Find("Image").GetComponent<Image>();
                        use = g.transform.Find("UseText").GetComponent<Text>();
                        name = g.transform.Find("NameText").GetComponent<Text>();
                        image.sprite = wallSprite[i];
                        if (DataCtrl.English) {
                            name.text = (string)DataCtrl.eqbq.wall_data.Rows[i][6];
                            use.text = (string)DataCtrl.eqbq.wall_data.Rows[i][7];
                        }
                        else {
                            name.text = (string)DataCtrl.eqbq.wall_data.Rows[i][1];
                            use.text = (string)DataCtrl.eqbq.wall_data.Rows[i][2];
                        }
                        
                        RectTransform rt = functionWallPanelabc.transform as RectTransform;
                        float x = (FunctionWallPrefab.transform as RectTransform).sizeDelta.x;
                        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (x + 3) * functionWallPanelabc.transform.childCount);
                    }
                }
            }
        }

        FunctionWallClick(1);
    }

    private int[, ] need;
    private int FunctionWallClickNumber;
    
    private int FunctionWallNumber;
    string[] array;
    private bool canBuildFunctionWall = true;
    void FunctionWallClick(int number) {
        CreatingWall = true;
        FunctionWallClickNumber = number;
        array = Regex.Split((string)DataCtrl.eqbq.wall_data.Rows[number - 1][3], "/", RegexOptions.IgnoreCase);
        need = new int[array.Length, 2];
        
        wallCreate.name.text = DataCtrl.English ? (string)DataCtrl.eqbq.wall_data.Rows[number - 1][6] : (string)DataCtrl.eqbq.wall_data.Rows[number - 1][1];
        FunctionWallNumber = number;
        
        canBuildFunctionWall = true;
        string s = DataCtrl.English ? "Wall building requirement : \n" : "建造需求 : \n";
        for (int i = 0; i < array.Length; i++) {
            int m = array[i].IndexOf("*");
            need[i, 0] = int.Parse(array[i].Substring(0, m));
            need[i, 1] = int.Parse(array[i].Substring(m + 1, array[i].Length - m - 1));
            if (PlayerCtrl.PD.Items[need[i, 0]] >= need[i, 1]) {
                s += "<color=#000000>";
            }
            else {
                s += "<color=#FF0000>";
                canBuildFunctionWall = false;
            }

            s += DataCtrl.English ? (string)DataCtrl.eqbq.item_data.Rows[int.Parse(array[i].Substring(0, m))][6] : (string)DataCtrl.eqbq.item_data.Rows[int.Parse(array[i].Substring(0, m))][1];
            s += "(" + PlayerCtrl.PD.Items[need[i, 0]] + "/" + array[i].Substring(m + 1, array[i].Length - m - 1) + ")";
            s += "</color>\n";
        }
        wallCreate.need.text = s;
        
        wallCreate.button.interactable = canBuildFunctionWall ? true : false;
    }
    
    public void _CreateFunctionWall() {

        for (int i = 0; i < array.Length; i++) {
            PlayerCtrl.PD.Items[need[i, 0]] -= need[i, 1];
        }
        
        Transform wallTransform = WallSir.transform;
        Destroy(WallSir);
        //刪除原本牆壁, 換上新改造的功能牆壁
        GameObject g = Instantiate(functionWall[FunctionWallNumber], sceneGameObject.transform, false);
        g.transform.position = wallTransform.position;
        g.transform.localScale = wallTransform.localScale;
        g.transform.rotation = wallTransform.rotation;
        g.transform.SetParent(GameObject.Find("AllFW").transform);
        WallSir = g;
        g.name = "FW_" + FunctionWallNumber;

        P_WallCreate.SetActive(false);
        FunctionWallUI();
        DestroyFWButton.SetActive(true);
        equipPanel.SetActive(true);
    }

    public void UpdateItemInfo() {
        
        if (selectNumber >= 0) {
            info.image.color = new Color(255, 255, 255, 255);
            info.image.sprite = itemSprite[selectNumber];

            string[] array = DataCtrl.English ? DataCtrl.eqbq.item_data.Rows[selectNumber][8].ToString().Split('%') : DataCtrl.eqbq.item_data.Rows[selectNumber][3].ToString().Split('%');
            
            for (int i = 0; i < array.Length; i++) {
                if (i == 0) {
                    info.use.text = "";
                }
                info.use.text += array[i] + "\n";
            }
            info.name.text = DataCtrl.English ? (string)DataCtrl.eqbq.item_data.Rows[selectNumber][6] : (string)DataCtrl.eqbq.item_data.Rows[selectNumber][1];
            info.kind.text = DataCtrl.English ? "Type:" + (string)DataCtrl.eqbq.item_data.Rows[selectNumber][7] : "種類:" + (string)DataCtrl.eqbq.item_data.Rows[selectNumber][2];
            info.amount.text = DataCtrl.English ? "Amount:" + PlayerCtrl.PD.Items[selectNumber].ToString() : "數量:" + PlayerCtrl.PD.Items[selectNumber].ToString();
            
            if (PlayerCtrl.PD.Items[selectNumber] < 1 && CliclEquipKind == -1 && !Clicking) {
                _SelectItem(0);
            }
        }
        else {
            info.name.text = "";
            info.kind.text = "";
            info.amount.text = "";
            info.use.text = "";
            info.image.color = new Color(0, 0, 0, 0);
            info.useButton.interactable = false;
            info.toFastButton.interactable = false;
        }
        
    }
    
    //使用物品
    public void _UseButton(){
        PlayerCtrl.PD.Items[selectNumber]--;

        string s = (string)DataCtrl.eqbq.item_data.Rows[selectNumber][4];
        s = s.Substring(1, s.Length - 1);

        switch (s.Substring(0, 1)) {
            case "h":
                if (PlayerCtrl.PD.Hunger + int.Parse(s.Substring(2, s.Length - 2)) <= PlayerCtrl.PD.MaxHunger) {
                    PlayerCtrl.PD.Hunger += int.Parse(s.Substring(2, s.Length - 2));
                }
                else {
                    PlayerCtrl.PD.Hunger = PlayerCtrl.PD.MaxHunger;
                }
                break;
            case "H":
                if (PlayerCtrl.PD.HP + int.Parse(s.Substring(2, s.Length - 2)) <= PlayerCtrl.PD.MaxHP) {
                    PlayerCtrl.PD.HP += int.Parse(s.Substring(2, s.Length - 2));
                }
                else {
                    PlayerCtrl.PD.HP = PlayerCtrl.PD.MaxHP;
                }
                PlayerCtrl.speed = 40;
                if (IsInvoking()) {
                    CancelInvoke("ResetSpeed");
                }
                Invoke("ResetSpeed", 15);
                break;
            case "A":
                ChangeEquip(0, int.Parse(s.Substring(1, s.IndexOf(".") - 1)) - 1, false);
                break;
            case "P":
                ChangeEquip(1, int.Parse(s.Substring(1, s.IndexOf(".") - 1)) - 1, false);
                break;
            case "B":
                EB.BulletNumber = int.Parse(s.Substring(1, s.IndexOf(".") - 1)) - 1;
                EB.Hurt = bulletInfo[EB.BulletNumber].hurt;
                EB.Speed = bulletInfo[EB.BulletNumber].Speed;
                EB.Amount = PlayerCtrl.PD.Items[selectNumber];
                AttackSkill_1.BulletAmount.text = (EB.Amount + 1).ToString();

                EBImage.sprite = itemSprite[bulletInfo[EB.BulletNumber].ItemNumber];
                EBImage.color = Color.white;
                EBText.text = DataCtrl.English ? "Hurt:" + EB.Hurt + "/Speed:" + EB.Speed : "傷害:" + EB.Hurt + "/速度:" + EB.Speed; ;
                PlayerCtrl.PD.Items[selectNumber]++;
                EBAmount.text = PlayerCtrl.PD.Items[bulletInfo[EB.BulletNumber].ItemNumber].ToString();
                if (DataCtrl.English) {
                    Record.AddRecord("Use " + DataCtrl.eqbq.item_data.Rows[selectNumber][6].ToString());
                }
                else {
                    Record.AddRecord("裝備" + DataCtrl.eqbq.item_data.Rows[selectNumber][1].ToString());
                }
                break;
            case "p":
                PlayerCtrl.PD.Hunger = PlayerCtrl.PD.MaxHunger;
                PlayerCtrl.PD.HP = PlayerCtrl.PD.MaxHP;
                break;
        }

        if (PlayerCtrl.PD.Items[selectNumber] == 0) {
            UpdateBag();
            UpdateItemInfo();
            EquipUpdate();

            Clicking = false;
            if (IsInvoking()) {
                CancelInvoke();
                return;
            }

            selectNumber = ItemNumber[0];
            time = Time.time;
            CliclEquipKind = -1;

            ////////
            info.unloadButton.gameObject.SetActive(false);
            info.useButton.gameObject.SetActive(true);
            info.useButton.interactable = false;
            info.toFastButton.interactable = false;

            if (selectNumber != -1) {
                if ((string)DataCtrl.eqbq.item_data.Rows[selectNumber][4] == "x") {
                    info.useButton.interactable = false;
                    info.toFastButton.interactable = false;
                }
                else {
                    info.useButton.interactable = true;
                    info.toFastButton.interactable = true;
                }
            }
        }

        
    }

    //變成快捷物品
    public void _ToFastUse() {
        FastItems.ChangeFastItem(selectNumber);
    }
    
    private void UpdateBag() {
        int j = 0; 
        for (int i = 0; i < PlayerCtrl.PD.Items.Length; i++) {
            if (PlayerCtrl.PD.Items[i] > 0) {
                itemsImage[j].color = new Color(255, 255, 255, 255);
                itemsImage[j].sprite = itemSprite[i];
                itemsText[j].text = PlayerCtrl.PD.Items[i].ToString();
                ItemNumber[j] = i;
                j++;
            }
        }
        
        for (int i = 0; i < itemsImage.Length - j; i++) {
            ItemNumber[i + j] = -1;
            itemsImage[i + j].color = new Color(0, 0, 0, 0);
            itemsText[i + j].text = "";
        }
    }

    public static bool OpenBox = false;
    private FW_BoxWall boxWall;
    private bool usingASB;
    void PutItemToBoxWall(int amount) {
        boxWall = transform.GetComponentInChildren<FW_BoxWall>();
        PlayerCtrl.PD.Items[selectNumber] -= amount;
        boxWall.PutItem(selectNumber, amount);
        
        //prevent bug
        if (PlayerCtrl.PD.Items[selectNumber] == 0) {
            selectNumber = -1;
        }
        usingASB = false;
    }
    public void _SelectItem(int number) {
        if (itemsText[number].text.Length > 0) {
            selectNumber = ItemNumber[number];
        }
        else {
            selectNumber = -1;
            return;
        }
        
        if (OpenBox) {
            if (!usingASB) {
                BoxWall bw = WallSir.GetComponent<BoxWall>();
                if (bw.Items[selectNumber] < bw.MaxAmount) {
                    boxWall = transform.GetComponentInChildren<FW_BoxWall>();

                    PlayerCtrl.PD.Items[selectNumber]--;
                    boxWall.PutItem(selectNumber, 1);

                    //prevent bug
                    if (PlayerCtrl.PD.Items[selectNumber] == 0) {
                        selectNumber = -1;
                    }
                    return;
                }
            }
        }
    }
    private bool Clicking;
    private float time;
    
    public void _ClickDown(int number) {
        ////////////////

        if (number != -2) {
            selectNumber = ItemNumber[number];
            time = Time.time;
            Clicking = true;
            CliclEquipKind = -1;
        }
        
        ////////
        info.unloadButton.gameObject.SetActive(false);
        info.useButton.gameObject.SetActive(true);
        info.useButton.interactable = false;
        info.toFastButton.interactable = false;
        
        if (selectNumber != -1) {
            if ((string)DataCtrl.eqbq.item_data.Rows[selectNumber][4] == "x") {
                info.useButton.interactable = false;
                info.toFastButton.interactable = false;
            }
            else {
                info.useButton.interactable = true;
                info.toFastButton.interactable = true;
            }
        }
        
    }
    public void _ClickFalse() {
        Clicking = false;
        if (IsInvoking()) {
            CancelInvoke();
        }
    }


    public void _BagOpen() {
        IsOpenBag = true;
        P_Info_Wall.SetActive(false);
        P_Info_Item.SetActive(true);
        gameObject.SetActive(true);
        functionWallPanel.SetActive(false);
        equipPanel.SetActive(true);
        DestroyFWButton.SetActive(false);
        RecordPanel.SetActive(false);

        info.unloadButton.gameObject.SetActive(false);
        info.useButton.gameObject.SetActive(true);
        info.useButton.interactable = false;
        info.toFastButton.interactable = false;
        
        UpdateItemInfo();
        UpdateBag();
        _SelectItem(0);
        _ClickDown(-2);

        for (int i = 0; i < equip.Length; i++) {
            equip[i].imageDurable.fillAmount = equip[i].Durable / equip[i].MaxDurable;
        }
        

    }
    public void _BagClose() {
        if (gameObject.activeSelf) {
            CameraCtrl.TouchCtrl = true;
            IsOpenBag = false;
            CreatingWall = false;
            P_Info_Wall.SetActive(false);
            P_Info_Item.SetActive(false);
            functionWallPanel.SetActive(false);
            gameObject.SetActive(false);
            PlayerCtrl.InNavigation = true;
            RecordPanel.SetActive(true);
            DestroyFWUI(gameObject);
            OpenBox = false;
            if (amountScrollBar.activeSelf) {
                amountScrollBar.SetActive(false);
            }
        }
        
        if (EB.BulletNumber != -1 && EB.Amount > 0) {
            AttackSkill_1.BulletAmount.text = PlayerCtrl.PD.Items[bulletInfo[EB.BulletNumber].ItemNumber].ToString();
        }
    }
    
    GameObject fwui;
    private void DestroyFWUI(GameObject g) {    
        if (g == gameObject) {
            if (fwui) {
                Destroy(fwui);
            }
        }
        else{
            fwui = g;
        }
    }

    public void _DestroyFW() {
        DestroyFWButton.SetActive(false);
        OpenBox = false;

        Transform wallTransform = WallSir.transform;
        Destroy(WallSir);
        //刪除原本牆壁, 換上新改造的功能牆壁
        
        GameObject g = Instantiate(functionWall[0], sceneGameObject.transform, false);
        g.transform.position = wallTransform.position;
        g.transform.localScale = wallTransform.localScale;
        g.transform.rotation = wallTransform.rotation;
        WallSir = g;
        g.name = "Wall";
        
        
        functionWallPanel.SetActive(true);
        equipPanel.SetActive(false);
        P_WallCreate.SetActive(true);
        DestroyFWUI(gameObject);
    }
    
    static void ChangeEquip(int equipKind, int EquipNumber, bool loadRecord) {
        if (!DataCtrl.LoadData) {
            if (DataCtrl.English) {
                Record.AddRecord("Use " + DataCtrl.eqbq.item_data.Rows[equipInfo[EquipNumber].ItemNumber][6].ToString());
            }
            else {
                Record.AddRecord("裝備" + DataCtrl.eqbq.item_data.Rows[equipInfo[EquipNumber].ItemNumber][1].ToString());
            }
        }

        switch (equipKind) {
            case 0:
                PlayerCtrl.PD.Attack = equipInfo[EquipNumber].Attack;
                equip[equipKind].EquipText.text = DataCtrl.English ? "Attack " + PlayerCtrl.PD.Attack.ToString() + "t" : "攻擊力 " + PlayerCtrl.PD.Attack.ToString() + "t";
                break;
            case 1:
                PlayerCtrl.PD.protection = equipInfo[EquipNumber].Protect;
                equip[equipKind].EquipText.text = DataCtrl.English ? "Protection " + PlayerCtrl.PD.protection.ToString() : "防護力 " + PlayerCtrl.PD.protection.ToString();
                break;
        }

        equip[equipKind].MaxDurable = equipInfo[EquipNumber].MaxDurable;

        if (!loadRecord) {
            if (equip[equipKind].UsingEquipNumber == EquipNumber) {
                PlayerCtrl.PD.Items[equipInfo[equip[equipKind].UsingEquipNumber].ItemNumber]++;
                return;
            }
            //if 有武器的狀態
            if (equip[equipKind].UsingEquipNumber != -1) {
                //把換下的武器的耐久度填入EquipDurble統一耐久度紀錄陣列
                equipInfo[equip[equipKind].UsingEquipNumber].DurableRecord = equip[equipKind].Durable;
                PlayerCtrl.PD.Items[equipInfo[equip[equipKind].UsingEquipNumber].ItemNumber]++;
            }
        }

        //if 有用過的同樣武器
        if (equipInfo[EquipNumber].DurableRecord > 0) {
            equip[equipKind].Durable = equipInfo[EquipNumber].DurableRecord;
        }
        else {
            equip[equipKind].Durable = equip[equipKind].MaxDurable;
        }

        equip[equipKind].image.sprite = itemSprite[equipInfo[EquipNumber].ItemNumber];
        equip[equipKind].image.color = Color.white;
        equip[equipKind].UsingEquipNumber = EquipNumber;
        
        equip[equipKind].imageDurable.fillAmount = equip[equipKind].Durable / equip[equipKind].MaxDurable;
    }
    
    public static void EquipUpdate() {
        for (int i = 0; i < equip.Length; i++) {
            if (equip[i].UsingEquipNumber >= 0) {
                if (equip[i].Durable <= 0) {
                    equip[i].imageDurable.fillAmount = 1;
                    EquipBreak(i);
                }
                else {
                    equip[i].imageDurable.fillAmount = Mathf.Lerp(equip[i].imageDurable.fillAmount, equip[i].Durable / equip[i].MaxDurable, 0.1f);
                }
            }
            else {
                equip[i].imageDurable.fillAmount = 1;
            }

        }
        
    }
    
    private static void EquipBreak(int equipKind) {
        equipInfo[equip[equipKind].UsingEquipNumber].DurableRecord = equip[equipKind].Durable;

        if (DataCtrl.English) {
            Record.AddRecord("<color=#FF0000>" + DataCtrl.eqbq.item_data.Rows[equipInfo[equip[equipKind].UsingEquipNumber].ItemNumber][6].ToString() + " Broken</color>");
        }
        else {
            Record.AddRecord("<color=#FF0000>" + DataCtrl.eqbq.item_data.Rows[equipInfo[equip[equipKind].UsingEquipNumber].ItemNumber][1].ToString() + "已毀損</color>");
        }

        equipInfo[equip[equipKind].UsingEquipNumber].DurableRecord = 0;
        equip[equipKind].image.color = new Color(1, 1, 1, 0);
        equip[equipKind].UsingEquipNumber = -1;
        switch (equipKind) {
            case 0:
                PlayerCtrl.PD.Attack = 0.5f;
                equip[equipKind].EquipText.text = DataCtrl.English ? "Attack 0.5t" : "攻擊力 0.5t";
                break;
            case 1:
                PlayerCtrl.PD.protection = 0;
                equip[equipKind].EquipText.text = DataCtrl.English ? "No Protction" : "無防護";
                break;
        }

        info.unloadButton.gameObject.SetActive(false);
        info.useButton.gameObject.SetActive(true);
        selectNumber = 0;
    }

    int UnLoadEquipKind;
    private int CliclEquipKind = -1;
    public void _ClickEquip(int equipKind) {
        UnLoadEquipKind = equipKind;

        if (equipKind < 2) {
            if (equip[equipKind].Durable > 0) {
                CliclEquipKind = equipKind;
                info.useButton.gameObject.SetActive(false);
                info.unloadButton.gameObject.SetActive(true);
                info.unloadButton.interactable = true;
                selectNumber = equipInfo[equip[CliclEquipKind].UsingEquipNumber].ItemNumber;
            }
        }
        else {
            if (EB.BulletNumber > -1) {
                info.useButton.gameObject.SetActive(false);
                info.unloadButton.gameObject.SetActive(true);
                info.unloadButton.interactable = true;
                selectNumber = bulletInfo[EB.BulletNumber].ItemNumber;
            }
        }

        
    }
    public void _UnLoadButton() {
        if (UnLoadEquipKind < 2) {
            if (DataCtrl.English) {
                Record.AddRecord("Unload " + DataCtrl.eqbq.item_data.Rows[equipInfo[equip[UnLoadEquipKind].UsingEquipNumber].ItemNumber][6].ToString());
            }
            else {
                Record.AddRecord("卸下" + DataCtrl.eqbq.item_data.Rows[equipInfo[equip[UnLoadEquipKind].UsingEquipNumber].ItemNumber][1].ToString());
            }
            equipInfo[equip[UnLoadEquipKind].UsingEquipNumber].DurableRecord = equip[UnLoadEquipKind].Durable;
            PlayerCtrl.PD.Items[equipInfo[equip[CliclEquipKind].UsingEquipNumber].ItemNumber]++;
            equip[UnLoadEquipKind].image.color = new Color(1, 1, 1, 0);
            equip[UnLoadEquipKind].UsingEquipNumber = -1;
            if (UnLoadEquipKind == 0) {
                PlayerCtrl.PD.Attack = 0.5f;
                equip[UnLoadEquipKind].EquipText.text = DataCtrl.English ? "Attack 0.5t" : "攻擊力 0.5t";
            }
            if (UnLoadEquipKind == 1) {
                PlayerCtrl.PD.protection = 0;
                equip[UnLoadEquipKind].EquipText.text = DataCtrl.English ? "No Protction" : "無防護";
            }
        }
        else {
            if (DataCtrl.English) {
                Record.AddRecord("Unload " + DataCtrl.eqbq.item_data.Rows[bulletInfo[EB.BulletNumber].ItemNumber][6].ToString());
            }
            else {
                Record.AddRecord("卸下" + DataCtrl.eqbq.item_data.Rows[bulletInfo[EB.BulletNumber].ItemNumber][1].ToString());
            }
            EB.BulletNumber = -1;
            EB.Hurt = 0;
            EB.Speed = 0;
            EB.Amount = 0;
            
            EBImage.color = new Color(0, 0, 0, 0);
            EBText.text = "No Bullet";
            EBAmount.text = "0";
            AttackSkill_1.BulletAmount.text = "0";

        }

        info.unloadButton.gameObject.SetActive(false);
        info.useButton.gameObject.SetActive(true);
    }

    public static void LoadEquipData() {
        for (int i = 0; i < equip.Length;i++) {
            if (equip[i].UsingEquipNumber != -1) {
                ChangeEquip(i, equip[i].UsingEquipNumber, true);
            }
        }
    }
    public static void LoadEBNumber(int number) {
        EB.BulletNumber = number;
        EB.Hurt = bulletInfo[EB.BulletNumber].hurt;
        EB.Speed = bulletInfo[EB.BulletNumber].Speed;
        EB.Amount = PlayerCtrl.PD.Items[bulletInfo[number].ItemNumber];
        AttackSkill_1.BulletAmount.text = (EB.Amount + 1).ToString();
        EBImage.sprite = itemSprite[bulletInfo[EB.BulletNumber].ItemNumber];
        EBImage.color = Color.white;
        EBText.text = DataCtrl.English ? "Hurt:" + EB.Hurt + "/Speed:" + EB.Speed : "攻擊力:" + EB.Hurt + "/速度:" + EB.Speed;
        
        EBAmount.text = PlayerCtrl.PD.Items[bulletInfo[EB.BulletNumber].ItemNumber].ToString();
    }
    private void ResetSpeed() {
        PlayerCtrl.speed = 25;
    }
    
    public void _BackToMenu() {
        SceneManager.LoadScene("Menu");
    }
    
}
