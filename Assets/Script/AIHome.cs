using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIHome : MonoBehaviour {

    [SerializeField] bool DontInsAI;
    private static GameObject[] AI = new GameObject[21];
    public GameObject[] AIAim1, AIAim2, AIAim3; // 1:外圍區, 2:中心區, 3:核心區
    public GameObject[] AIHomes;
    public int MaxAmount; //最大AI數量
    public int[] AI1, AI2, AI3; //三個部分AI各要哪一些AI
    public int[] InsAIProb, AIAimProb;
    
    private void Awake() {
        if (!DontInsAI) {
            //抓取AI prefab
            for (int i = 0; i < AI.Length; i++) {
                AI[i] = Resources.Load<GameObject>("AI/AI_" + (i + 1).ToString());
            }

            InvokeRepeating("InstantiateAI", 0, 1);
        }
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).transform.SendMessage("StartSetting");
        }
    }
    
    void Update () {
        if (!DontInsAI) {
            if (transform.childCount < MaxAmount) {
                if (!IsInvoking()) {
                    InstantiateAI();
                }
            }
            else {
                CancelInvoke();
            }
        }
        
	}
    
    void InstantiateAI() {

        if (transform.childCount >= MaxAmount) {
            return;
        }

        int a = Random.Range(0, 101);
        int b = Random.Range(0, AIHomes.Length);

        GameObject ai;
        if (a <= InsAIProb[0]) {
            ai = Instantiate(AI[AI1[Random.Range(0, AI1.Length)] - 1], AIHomes[b].transform.position, Quaternion.identity);
        }
        else if (a >= 100 - InsAIProb[1]) {
            ai = Instantiate(AI[AI2[Random.Range(0, AI2.Length)] - 1], AIHomes[b].transform.position, Quaternion.identity);
        }
        else {
            ai = Instantiate(AI[AI3[Random.Range(0, AI3.Length)] - 1], AIHomes[b].transform.position, Quaternion.identity);
        }
        ai.transform.SetParent(transform);
        ai.transform.SendMessage("StartSetting");

    }
    
    void GetAim(GameObject g) {
        if (!DontInsAI) {
            int a = Random.Range(0, 101);
            Vector3 v;
            if (a <= AIAimProb[0]) {
                v = AIAim1[Random.Range(0, AIAim1.Length)].transform.position;
            }
            else if (a >= 100 - AIAimProb[1]) {
                v = AIAim2[Random.Range(0, AIAim2.Length)].transform.position;
            }
            else {
                v = AIAim3[Random.Range(0, AIAim3.Length)].transform.position;
            }
            g.SendMessage("SetAim", v);
        }
    }
    
    public static void LoadAIsData(AIInfo[] aIInfos) {
        GameObject[] AllAIHomes = new GameObject[GameObject.FindGameObjectsWithTag("AIHome").Length];

        //print(AllAIHomes.Length);
        for (int i = 0; i < AllAIHomes.Length; i++) {
            AllAIHomes = GameObject.FindGameObjectsWithTag("AIHome");
        }
        for (int i = 0; i < AllAIHomes.Length; i++) {
            AllAIHomes[i] = GameObject.Find("AIHome_" + (i + 1).ToString());
        }

        for (int i = 0; i < AllAIHomes.Length; i++) {
            for (int j = 0; j < AllAIHomes[i].transform.childCount; j++) {
                Destroy(AllAIHomes[i].transform.GetChild(j).gameObject);
            }
        }
        
        for (int i = 0; i < aIInfos.Length; i++) {
            try {
                GameObject g = Instantiate(AI[aIInfos[i].number - 1], aIInfos[i].pos, Quaternion.identity);
                g.transform.SetParent(AllAIHomes[aIInfos[i].HomeNumber - 1].transform);
                g.SendMessage("LoadAIInfo", aIInfos[i]);
            }
            catch {}
            finally {}
        }
    }

    public static void LoadBossesData(AIInfo[] aIInfos) {
        GameObject bosses = GameObject.Find("Bosses");
        for (int i = 0; i < bosses.transform.childCount; i++) {
            Destroy(bosses.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < aIInfos.Length; i++) {
            GameObject g = Instantiate(AI[aIInfos[i].number - 1], aIInfos[i].pos, aIInfos[i].rotation);
            g.transform.SetParent(bosses.transform);
            g.name = aIInfos[i].Name;
            g.SendMessage("LoadAIInfo", aIInfos[i]);
        }
    }

}
