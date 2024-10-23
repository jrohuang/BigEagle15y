using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.Security.Cryptography;

public class DataCtrl : MonoBehaviour {
    /////// DataCtrl.cs in Order should put on -50
    [SerializeField] Text text;
    [SerializeField] bool run;

    public static AllEQBQ eqbq;

    public static PlayerRecord playerRecord = new PlayerRecord();
    public static AIData[] aiData;
    MySqlConnection connection;
    MySqlDataAdapter adapter;
    private static LevelSaveData LSD;
    public static ItemCrafting[] ic = new ItemCrafting[18];
    public static bool LoadData = false;
    private AsyncOperation async;
    private bool isFirstScene;
    public class AllEQBQ {
        public DataTable ai_data;
        public DataTable player_data;
        public DataTable item_data;
        public DataTable wall_data;
        public DataTable level_data;
        public DataTable skill_data;
    }
    private string sceneName;
    public static bool English;
    public static float musicTime = 0;
    private bool inLevelScene = false;

    [SerializeField] AudioClip c1, c2;
    
    private AudioSource audio;

    StreamReader sr;

    private void Awake() {
        //TryGetData();
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        audio = transform.GetComponent<AudioSource>();
        
        playerRecord.SettingValue = new float[3];
        playerRecord.SettingValue[0] = 1;
        playerRecord.SettingValue[1] = 0;
        playerRecord.SettingValue[2] = 0.5f;
        
        string sceneName = SceneManager.GetSceneAt(0).name;
        if (sceneName == "First") {
            DontDestroyOnLoad(gameObject);
            text.text = English ? "Loading..." : "資料讀取中...";
        }

        try {
            if (File.Exists(Path.Combine(Application.persistentDataPath, "Peqbq"))) {
                try {
                    StreamReader file = new StreamReader(Path.Combine(Application.persistentDataPath, "Peqbq"));
                    string loadJson = file.ReadToEnd();
                    file.Close();
                    playerRecord = JsonUtility.FromJson<PlayerRecord>(ReStrEncryption(loadJson, "mo526on"));
                    English = playerRecord.English;
                }
                catch(Exception e) {
                    print(e);
                    SavePRecord();
                }
            }
            else {
                SavePRecord();
            }

            ReadJson();
            
            SortCraftingData();
            SortAIData();
            SortAIAttackData();
            sceneName = SceneManager.GetSceneAt(0).name;
            if (sceneName == "First") {
                if (File.Exists(Path.Combine(Application.persistentDataPath, "SaveLSD(DontOpen)"))) {
                    LoadLSDScene();
                }
                else {
                    SceneManager.LoadScene("Menu");
                }
            }

        }
        catch (Exception e) {
            text.text = e.ToString();
        }
    }

    public void ReadJson() {
        string path = Application.streamingAssetsPath + "/eqbq";

        WWW www = new WWW(path);
        while (!www.isDone) { }
        string json = www.text;
        string s = ReStrEncryption(json, "526moon");
        eqbq = JsonConvert.DeserializeObject<AllEQBQ>(s);
    }

    public void GetEqbqAgain() {
        File.Delete(Path.Combine(Application.persistentDataPath, "eqbq"));
        File.Copy(Path.Combine(Application.streamingAssetsPath, "eqbq"), Path.Combine(Application.persistentDataPath, "eqbq"));
        Awake();
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1) {
        string sceneName = SceneManager.GetSceneAt(0).name;
        inLevelScene = false;
        if (sceneName == "Menu" || sceneName == "GameMap") {
            CancelInvoke();
            audio.clip = c2;
            if (!audio.isPlaying) {
                audio.Play();
            }
        }
        else if(sceneName == "FeedEagle" || sceneName == "Me") {
            CancelInvoke();
            audio.clip = c1;
            if (!audio.isPlaying) {
                audio.Play();
            }
        }
        else {
            inLevelScene = true;
            audio.clip = c1;
            audio.Play();
        }
    }

    private void NextSong() {
        audio.clip = audio.clip == c1 ? c2 : c1;
        audio.Play();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.P)) { ScreenCapture.CaptureScreenshot("screen.png", 2); }
        if (inLevelScene) {
            if (!audio.isPlaying) {
                NextSong();
            }
        }
    }

    public void LoadLSDScene() {
        if (SceneManager.GetSceneAt(0).name == "Menu") {
            text = GameObject.Find("Canvas").transform.Find("LoadingPanel").GetComponentInChildren<Text>();
            text.text = "Loading.....";
            GameObject.Find("Canvas").transform.Find("LoadingPanel").gameObject.SetActive(true);
        }
        LoadData = true;
        LSD = new LevelSaveData();
        StreamReader file = new StreamReader(Path.Combine(Application.persistentDataPath, "SaveLSD(DontOpen)"));
        string loadJson = file.ReadToEnd();
        file.Close();
        LSD = JsonUtility.FromJson<LevelSaveData>(ReStrEncryption(loadJson, "526moon"));
        async = SceneManager.LoadSceneAsync("Level_" + LSD.Level);
    }

    public static AIAttack[] aiAttack;
    void SortAIAttackData() {
        aiAttack = new AIAttack[eqbq.ai_data.Rows.Count];

        for (int ai = 0; ai < aiAttack.Length; ai++) {
            aiAttack[ai] = new AIAttack();
            aiAttack[ai].HAttack = false;
            aiAttack[ai].hpAttack = false;

            string s = eqbq.ai_data.Rows[ai][6].ToString();

            if (aiData[ai].AttackMode != 0) {
                
                if (aiData[ai].AttackMode == 1 || aiData[ai].AttackMode == 2) {
                    string[] array = s.Split(',');
                    
                    aiAttack[ai].hpAttack = false;
                    int h = int.Parse(array[0]);
                    if (h > 0) {
                        aiAttack[ai].hpAttack = true;
                        aiAttack[ai].hpAttackHurt = new float[h];
                        aiAttack[ai].hpAttackTime = new float[h];

                        for (int i = 0; i < aiAttack[ai].hpAttackTime.Length; i++) {
                            string[] vs = array[i + 2].Split('/');
                            aiAttack[ai].hpAttackHurt[i] = float.Parse(vs[0].Substring(1, vs[0].Length - 1));
                            aiAttack[ai].hpAttackTime[i] = float.Parse(vs[1]);
                        }
                    }

                    aiAttack[ai].HAttack = false;
                    int H = int.Parse(array[1]);
                    if (H > 0) {
                        aiAttack[ai].HAttack = true;
                        aiAttack[ai].HAttackHurt = new float[H];
                        aiAttack[ai].HAttackTime = new float[H];

                        for (int i = 0; i < aiAttack[ai].HAttackHurt.Length; i++) {
                            string[] vs = array[i + 2 + h].Split('/');
                            aiAttack[ai].HAttackHurt[i] = float.Parse(vs[0].Substring(1, vs[0].Length - 1));
                            aiAttack[ai].HAttackTime[i] = float.Parse(vs[1]);
                        }
                    }
                    if (aiData[ai].AttackMode == 2) {
                        aiAttack[ai].AttackDistance = float.Parse(array[array.Length - 1].ToString());
                    }
                        
                }

                if (aiData[ai].AttackMode == 3) {
                    string[] array = s.Split(',');
                    aiAttack[ai].RangeAttackHurt = float.Parse(array[0]);
                    aiAttack[ai].AttackDistance = float.Parse(array[1]);
                }
                
                
            }
        }
        


    }
    void SortCraftingData() {
        
        for (int i = 0; i < ic.Length; i++) {
            ic[i] = new ItemCrafting();
        }

        int c = 0;
        for (int i = 0; i < eqbq.item_data.Rows.Count; i++) {
            if ((string)eqbq.item_data.Rows[i][5] == "x") {}
            else {
                string s = eqbq.item_data.Rows[i][5].ToString();
                int dl = s.IndexOf("_");
                int ldl = s.LastIndexOf("_");
                ic[c].wall = int.Parse(s.Substring(0, dl));

                ic[c].level = int.Parse(s.Substring(dl + 1, ldl - dl - 1));
                ic[c].name = (string)eqbq.item_data.Rows[i][1];
                ic[c].name_E = (string)eqbq.item_data.Rows[i][6];
                ic[c].number = int.Parse(eqbq.item_data.Rows[i][0].ToString());
                int j = s.IndexOf(">");
                ic[c].Amount = int.Parse(s.Substring(j + 1, s.Length - j - 1));
                ic[c].need = s.Substring(ldl + 1, s.Length - ldl - 1 - (s.Length - j));
                c++;
            }
        }
    }
    void SortAIData() {
        
        aiData = new AIData[eqbq.ai_data.Rows.Count];

        for (int i = 0; i < aiData.Length; i++) {
            aiData[i] = new AIData();
            aiData[i].Name = eqbq.ai_data.Rows[i][1].ToString();
            aiData[i].Name_E = eqbq.ai_data.Rows[i][7].ToString();
            aiData[i].Blood = float.Parse(eqbq.ai_data.Rows[i][2].ToString());
            aiData[i].Speed = float.Parse(eqbq.ai_data.Rows[i][3].ToString());
            aiData[i].XP = int.Parse(eqbq.ai_data.Rows[i][5].ToString());
            aiData[i].AttackMode = int.Parse(eqbq.ai_data.Rows[i][8].ToString());
        }
    }
    
    public static void SavePRecord() {
        if (File.Exists(Path.Combine(Application.persistentDataPath, "Peqbq"))) {
            File.Delete(Path.Combine(Application.persistentDataPath, "Peqbq"));
        }
        string s = StrEncryption(JsonConvert.SerializeObject(playerRecord), "mo526on");
        StreamWriter file = new StreamWriter(Path.Combine(Application.persistentDataPath, "Peqbq"));
        file.Write(s);
        file.Close();
    }

    public void LoadLSD() {

        //存取儲存檔案
        if (!LoadData) {
            return;
        }
        try {

            GameObject.Find("Player").transform.position = LSD.playerPos;
            PlayerCtrl.PD = LSD.pd;
            FastItems.SetFastItemsNumber(LSD.fastItemsNumber);
            Camera.main.transform.position = new Vector3(LSD.playerPos.x- 1.5f, CameraCtrl.CameraMaxDis, LSD.playerPos.z);
            GameObject.Find("UI/FastItems").SendMessage("UpdateImage");
            PlayerCtrl.LevelMaxed = LSD.LevelMaxed;

            //AIs
            AIHome.LoadAIsData(LSD.AIs);

            //Boss
            AIHome.LoadBossesData(LSD.bosses);

            //equip
            for (int i = 0; i < LSD.EquipDurable.Length; i++) {
                Bag.equipInfo[i].DurableRecord = LSD.EquipDurable[i];
            }
            for (int i = 0; i < LSD.equip.Length; i++) {
                Bag.equip[i].UsingEquipNumber = LSD.equip[i];
            }
            Bag.LoadEquipData();
            if (LSD.EBNumber != -1) {
                Bag.LoadEBNumber(LSD.EBNumber);
            }

            //skill
            AttackSkill.usingSkillNumber = LSD.UsingAttackSkillNumber;
            AttackSkill.IfLoadData();

            //FW
            GameObject AllFW = GameObject.Find("AllFW");
            for (int i = 0; i < LSD.FW.Length; i++) {
                GameObject g = Instantiate(Bag.functionWall[LSD.FW[i].Number]);
                g.name = "FW_" + LSD.FW[i].Number;
                g.transform.position = LSD.FW[i].Pos;
                g.transform.SetParent(AllFW.transform);

                Collider[] colliders;
                if ((colliders = Physics.OverlapSphere(g.transform.position, 0.1f)).Length > 1) {
                    foreach (var collider in colliders) {
                        var go = collider.gameObject;
                        if (go.CompareTag("Wall")) {
                            if (go != g) {
                                Destroy(go);
                            }
                        }
                    }
                }

                if (LSD.FW[i].Number == 3 || LSD.FW[i].Number == 8) {
                    AttackWall aw = g.GetComponent<AttackWall>();
                    aw.SetInfo(LSD.FW[i].f3.AttackInfo);
                    aw.SetSaveItmes(LSD.FW[i].f3.SaveItems);
                }
                else if(LSD.FW[i].Number == 4 || LSD.FW[i].Number == 9) {
                    BoxWall bw = g.GetComponent<BoxWall>();
                    bw.SetItems(LSD.FW[i].FBox.Item);
                }
            }

            CameraCtrl.LLoadData = true;
        }
        catch (Exception e) {
            print(e);
            SceneManager.LoadScene("Menu");
            return;
        }
        finally {
            LoadData = false;
        }
    }

    private void OnLevelWasLoaded(int level) {
        sceneName = SceneManager.GetSceneAt(0).name;
    }

    //////////////////////
    public static string StrEncryption(string conten, string ckey) {
        //H E L L O 
        char[] arrContent = conten.ToCharArray();
        char[] arrKey = ckey.ToCharArray();
        for (int i = 0; i < arrContent.Length; i++) {
            arrContent[i] ^= arrKey[i % arrKey.Length];
        }
        return new string(arrContent);
    }
    public static string ReStrEncryption(string conten, string ckey) {
        //H E L L O 
        char[] arrContent = conten.ToCharArray();
        char[] arrkey = ckey.ToCharArray();
        for (int i = 0; i < arrContent.Length; i++) {
            arrContent[i] ^= arrkey[i % arrkey.Length];
        }

        return new string(arrContent);
    }

    

}
