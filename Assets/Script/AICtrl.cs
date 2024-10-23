using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class AICtrl : MonoBehaviour {
    
    NavMeshAgent agent;
    
    private float speed, maxHealth, health, AttackDistance;
    public int AINumber;
    private string name;
    private int xp;
    public GameObject bulletGO;
    public bool IsBoss;
    public int AttackMode; //0:x, 1:CloseRange, 2:distance, 3:range
    private GameObject Bullet;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float AddBlood;
    
    private bool IsLoad = false;
    private Vector3 aim;
    private float hpAttack;
    private bool AttackPlayer = false;
    private bool SetAimPlayerAgain = true;

    private bool noBug = true;
    private float RangeAttack_R, RangeAttack_H; //for attackMode3, R=range, H=Hurt
    private GameObject pos; //LongDisAttack

    /////attack
    private AIAttack aiAttack;
    private bool[] AttackH, Attackhp;
    private bool LongDistanceAttacking= false;
    private GameObject playerGO;

    //destroy with boss 
    List<GameObject> DestoryWithMe = new List<GameObject>();

    //ins ai
    private int InsAIAmount;
    private int[] InsAIInfo = new int[3];//ins ai number, ins ai time, ins ai max amount
    private GameObject InsAIgo;
    private GameObject insAIPS;

    void Awake() {
        playerGO = GameObject.Find("Player");
        if (IsBoss) {
            if (!IsLoad) {
                StartSetting();
            }
        }
        AttackPlayer = AttackMode != 0 ? true : false;

        if (AttackMode == 2) {
            
            AttackDistance = DataCtrl.aiAttack[AINumber - 1].AttackDistance;
            pos = transform.GetChild(0).gameObject;
            Bullet = Resources.Load<GameObject>("Bullet/AIBullet_" + AINumber);
        }
        if (AttackMode == 3) {
            string s = DataCtrl.eqbq.ai_data.Rows[AINumber - 1][6].ToString();
            string[] f = s.Split(',');
            RangeAttack_H = float.Parse(f[0]);
            RangeAttack_R = float.Parse(f[1]);
        }
        if (AttackMode == 5) {
            string[] st = DataCtrl.eqbq.ai_data.Rows[AINumber - 1][6].ToString().Split(',');
            InsAIInfo[0] = int.Parse(st[0]);
            InsAIInfo[1] = int.Parse(st[1]);
            InsAIInfo[2] = int.Parse(st[2]);
            InsAIgo = Resources.Load<GameObject>("AI/AI_" + InsAIInfo[0]);
            insAIPS = Resources.Load<GameObject>("AIInsAI");
        }
    }

    void StartSetting() {
        
        //基本數據設定
        xp = DataCtrl.aiData[AINumber - 1].XP;
        speed = DataCtrl.aiData[AINumber - 1].Speed;
        maxHealth = DataCtrl.aiData[AINumber - 1].Blood;
        name = DataCtrl.aiData[AINumber - 1].Name;
        health = maxHealth;
        if (AttackMode == 1 || AttackMode == 2) {
            aiAttack = DataCtrl.aiAttack[AINumber - 1];
            
            if (aiAttack.HAttack) {
                AttackH = new bool[aiAttack.HAttackHurt.Length];
            }
            if (aiAttack.hpAttack) {
                Attackhp = new bool[aiAttack.hpAttackTime.Length];
            }
        }

        DropItems();

        if (!IsBoss && AINumber != 17) {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = speed;
            //抓取導航component, 設定速度
            if (IsLoad) {
                if (!IsBoss) {
                    agent.enabled = false;
                    agent.enabled = true;
                }
            }
            transform.parent.SendMessage("GetAim", gameObject);
        }

        if (IsLoad) {
            health = LoadInfo.helath;
        }
    }

    void SetAim(Vector3 v3) {
        aim = v3;
        agent.SetDestination(v3);
    }

    bool B;
    void Update() {

        B = B ? false : true;
        if (B) {
            return;
        }

        if (health + AddBlood * Time.deltaTime < maxHealth) {
            health += AddBlood * Time.deltaTime;
        }

        if (AttackMode == 2) {
            
            if (LongDistanceAttacking) {
                Vector3 ap = playerGO.transform.position - transform.position;
                transform.rotation = Quaternion.LookRotation(new Vector3(ap.x, 0, ap.z));

                Ray ray = new Ray(transform.position, ap);
                RaycastHit hit;
                Physics.Raycast(ray, out hit, AttackDistance, layerMask);
                if (hit.transform) {
                    if (hit.transform.gameObject.CompareTag("Player")) {
                        if (aiAttack.HAttack) {
                            for (int i = 0; i < AttackH.Length; i++) {
                                if (AttackH[i]) {
                                    AttackH[i] = false;
                                    Attack(false, i);
                                }
                            }
                        }
                        if (aiAttack.hpAttack) {
                            for (int i = 0; i < Attackhp.Length; i++) {
                                if (Attackhp[i]) {
                                    Attackhp[i] = false;
                                    Attack(true, i);
                                }
                            }
                        }
                    }
                }


                if (Vector3.Distance(playerGO.transform.position, gameObject.transform.position) > AttackDistance) {
                    LongDistanceAttacking = false;
                    StopAllCoroutines();
                    if (AINumber != 17) {
                        agent.speed = speed;
                    }
                }
            }
            else {
                if (Vector3.Distance(playerGO.transform.position, gameObject.transform.position) < AttackDistance) {
                    if (AINumber != 17) {
                        agent.speed = 0;
                    }

                    LongDistanceAttacking = true;
                    if (aiAttack.HAttack) {
                        for (int i = 0; i < AttackH.Length; i++) {
                            AttackH[i] = true;
                        }
                    }
                    if (aiAttack.hpAttack) {
                        for (int i = 0; i < Attackhp.Length; i++) {
                            Attackhp[i] = true;
                        }
                    }
                }
            }
        }
        if (AttackMode == 3) {
            if (Vector3.Distance(playerGO.transform.position, transform.position) < RangeAttack_R) {
                PlayerCtrl.PD.HP -= RangeAttack_H * Time.deltaTime;
            }
        }
        if (AttackMode == 4 && SetAimPlayerAgain) {
            agent.SetDestination(playerGO.transform.position);
        }
        if (AttackMode == 5) {
            if (transform.childCount - 2 < InsAIInfo[1] && !IsInvoking()) {
                InvokeRepeating("InsAI", 0, InsAIInfo[1]);
            }
        }

        if (health <= 0) {
            AIDead(true);
        }
        else {
            if (IsBoss || AINumber == 17) {
                return;
            }
            else {
                if (agent.remainingDistance < 1 && agent.remainingDistance > 0.1f) {
                    transform.parent.SendMessage("GetAim", gameObject);
                    if (AttackMode == 4 && agent.remainingDistance > 0.1f) {
                        SetAimPlayerAgain = SetAimPlayerAgain ? false : true;
                    }
                }
            }
        }
    }

    //10改成物品總數
    private int[] dropItems = new int[Bag.itemSprite.Length];
    //計算掉落物品
    private void DropItems() {
        
        if (DataCtrl.eqbq.ai_data.Rows[AINumber - 1][4].ToString().Length == 0) {
            return;
        }

        string[] array = Regex.Split(DataCtrl.eqbq.ai_data.Rows[AINumber - 1][4].ToString(), "/", RegexOptions.IgnoreCase);
        for (int i = 0; i < array.Length; i++) {
            string[] s = Regex.Split(array[i], ",", RegexOptions.IgnoreCase);
            for (int j = 0; j < int.Parse(s[2]); j++) {
                if (Random.Range(1, 101) <= int.Parse(s[1])) {
                    dropItems[int.Parse(s[0])]++;
                }
            }

        }

    }

    void GetBulletHurt(int hurt) {
        if (health - hurt > 0) {
            health -= hurt;
            bulletGO.SendMessage("AllHurt", hurt);
        }
        else {
            AIDead(false);
            bulletGO.SendMessage("AllHurt", hurt);
        }
    }
    void GetHurt(int hurt) {
        if (health - hurt > 0) {
            health -= hurt;
        }
        else {
            AIDead(true);
        }
    }
    
    private void AIDead(bool killFromPlayer) {
        if (killFromPlayer) {
            PlayerCtrl.GetXP(xp);

            if (DataCtrl.English) {
                Record.AddRecord("<color=#800080>Kill [" + DataCtrl.eqbq.ai_data.Rows[AINumber - 1][7].ToString() + "]</color>");
                Record.AddRecord("+" + xp.ToString() + "xp");
            }
            else {
                Record.AddRecord("<color=#800080>擊殺" + DataCtrl.eqbq.ai_data.Rows[AINumber - 1][1].ToString() + "</color>");
                Record.AddRecord("獲得經驗" + xp.ToString());
            }
            
            for (int i = 0; i < dropItems.Length; i++) {
                if (dropItems[i] > 0) {
                    if (PlayerCtrl.PD.Items[i] + dropItems[i] <= PlayerCtrl.MaxItemAmount) {
                        PlayerCtrl.GetItems(i, dropItems[i]);
                    }
                    else {
                        PlayerCtrl.PD.Items[i] = PlayerCtrl.MaxItemAmount;
                    }
                }
            }
        }
        else {
            PlayerCtrl.GetXP(xp);
            bulletGO.SendMessage("GetXp", xp);
            bulletGO.SendMessage("GetItems", dropItems);
        }

        if (DestoryWithMe.Count > 0) {
            for (int i = 0; i < DestoryWithMe.Count; i++) {
                Destroy(DestoryWithMe[i]);
            }
        }
        

        if (PlayerCtrl.StayAI == gameObject) {
            PlayerCtrl.AIInfoGO.SetActive(false);
        }

        if (AINumber == 12) {
            GameObject boom = Resources.Load<GameObject>("Boom");
            Instantiate(boom, transform.position, Quaternion.identity);
        }

        if (AINumber > 19) {
            gameObject.SendMessage("YoBigEagle");
        }

        PlayerCtrl.AIExit();
        Destroy(gameObject);
        noBug = false;
    }
    

    private void OnTriggerEnter(Collider other) {
        if (AttackMode == 1) {
            if (other.CompareTag("Player") && AttackPlayer) {
                if (aiAttack.HAttack) {
                    for (int i = 0; i < AttackH.Length; i++) {
                        AttackH[i] = true;
                    }
                }
                if (aiAttack.hpAttack) {
                    for (int i = 0; i < Attackhp.Length; i++) {
                        Attackhp[i] = true;
                    }
                }
            }
        }
    }
    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {

            health -= Time.deltaTime * PlayerCtrl.PD.Attack;
            Bag.equip[0].Durable -= Time.deltaTime * PlayerCtrl.PD.Attack;
            Bag.EquipUpdate();

            if (noBug) {
                PlayerCtrl.AIStay(AINumber - 1, health, gameObject, AttackMode, AddBlood, RangeAttack_R, RangeAttack_H);
            }

            if (AttackMode == 1) {
                if (aiAttack.HAttack) {
                    for (int i = 0; i < AttackH.Length; i++) {
                        if (AttackH[i]) {
                            Attack(false, i);
                        }
                    }
                }
                if (aiAttack.hpAttack) {
                    for (int i = 0; i < Attackhp.Length; i++) {
                        if (Attackhp[i]) {
                            Attack(true, i);
                        }
                    }
                }
            }
            
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            PlayerCtrl.AIExit();
        }
    }

    private void Attack(bool HPHurt, int number) {
        if (HPHurt) {
            Attackhp[number] = false;
            StartCoroutine(AttackReset(HPHurt, number, aiAttack.hpAttackTime[number]));
        }
        else {
            AttackH[number] = false;
            StartCoroutine(AttackReset(HPHurt, number, aiAttack.HAttackTime[number]));
        }
    }
    IEnumerator AttackReset(bool HPHurt, int number, float time) {
        
        if (AttackMode == 1) {
            if (HPHurt) {
                PlayerCtrl.GetHurt(aiAttack.hpAttackHurt[number]);
            }
            else {
                PlayerCtrl.PD.Hunger -= aiAttack.HAttackHurt[number];
            }
        }
        else if(AttackMode == 2) {
            GameObject g = Instantiate(Bullet, pos.transform.position, Quaternion.identity);
            if (HPHurt) {                
                g.SendMessage("AIBulletStartSetting", aiAttack.hpAttackHurt[number]);
                g.SendMessage("SetHurtKind", true);
            }
            else {
                g.SendMessage("AIBulletStartSetting", aiAttack.HAttackHurt[number]);
                g.SendMessage("SetHurtKind", false);
            }
        }

        yield return new WaitForSeconds(time);

        if (HPHurt) { Attackhp[number] = true; }
        else { AttackH[number] = true; }
        
    }

    void GetRay() {
        PlayerCtrl.AIStay(AINumber - 1, health, gameObject, AttackMode, AddBlood, RangeAttack_R, RangeAttack_H);
    }
    private AIInfo LoadInfo;

    //載入用
    void LoadAIInfo(AIInfo aIInfo) {
        IsLoad = true;
        LoadInfo = aIInfo;
        transform.rotation = aIInfo.rotation;
        StartSetting();
    }
    //save use
    public AIInfo GetAiInfo() {
        AIInfo aIInfo = new AIInfo();
        aIInfo.number = AINumber;
        aIInfo.pos = transform.position;
        aIInfo.helath = health;
        aIInfo.Name = gameObject.name;
        aIInfo.rotation = transform.rotation;
        if (!IsBoss) {
            aIInfo.HomeNumber = int.Parse(transform.parent.name.Substring(7, 1));
        }
        return aIInfo;
    }
    void AddDWM(GameObject g) {
        DestoryWithMe.Add(g);
    }
    
    private void InsAI() {
        if (gameObject.activeSelf) {
            GameObject g = Instantiate(InsAIgo, transform);
            g.SendMessage("StartSetting");
            g.transform.position = transform.position + new Vector3(Random.Range(-5, 5), 2, Random.Range(-5, 5));
            Instantiate(insAIPS, transform);

            if (transform.childCount - 2 == InsAIInfo[2]) {
                CancelInvoke();
            }
        }
    }

}
