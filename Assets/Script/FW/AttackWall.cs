using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackWall : MonoBehaviour {

    [SerializeField] GameObject bullet;
    private static float[] info = new float[4]; //number, speed, hurt, maxAmount
    private bool[] canAttack = new bool[4]; //四個方向是否為空格
    private LayerMask wall, monster;
    Vector3[] dir4 = new Vector3[4];
    private int[] AttackInfo = new int[3]; // saveXp, killAmount, allHurt;
    private int[] saveItems = new int[35];

    void Start() {
        
        wall = 1 << 8;
        monster = 1 << 10;
        monster |= 1 << 8;
        monster |= 1 << 12;

        info[0] = int.Parse(gameObject.name.Substring(3, gameObject.name.Length - 3));

        string[] array = DataCtrl.eqbq.wall_data.Rows[(int)info[0] - 1][5].ToString().Split('.');

        for (int i = 0; i < array.Length; i++) {
            info[i + 1] = int.Parse(array[i].ToString());
        }

        dir4[0] = Vector3.forward;
        dir4[1] = Vector3.right;
        dir4[2] = Vector3.back;
        dir4[3] = Vector3.left;
        for (int i = 0; i < 4; i++) {
            r4[i] = new Ray(transform.position, dir4[i]);
        }

        for (int i = 0; i < 4; i++) {
            Physics.Raycast(r4[i], out hit, 1, wall);
            if (hit.transform) {
                canAttack[i] = false;
            }
            else {
                canAttack[i] = true;
            }
        }

    }

    Ray[] r4 = new Ray[4];
    RaycastHit hit;

    void Update() {
        for (int i = 0; i < 4; i++) {
            if (canAttack[i]) {
                Physics.Raycast(r4[i], out hit, 5, monster);
                if (hit.transform != null) {
                    if (hit.transform.gameObject.CompareTag("AI")) {
                        if (!IsInvoking()) {
                            Attack(i);
                        }
                    }
                }
            }
        }
    }

    void AllHurt(int hurt) {
        AttackInfo[2] += hurt;
    }
    void GetXp(int xp) {
        AttackInfo[1]++;
        if (!PlayerCtrl.LevelMaxed) {
            AttackInfo[0] += xp;
        }
    }
    void GetItems(int[] items) {
        for (int i = 0; i < items.Length; i++) {
            if (saveItems[i] + items[i] <= info[3]) {
                saveItems[i] += items[i];
            }
            else {
                saveItems[i] = (int)info[3];
            }
        }
    }

    private void Attack(int d) {
        //生成子彈攻擊
        GameObject g = Instantiate(bullet, transform.position, Quaternion.Euler(new Vector3(0, 90 * (d - 1), 0)));
        g.transform.SetParent(transform);
        Rigidbody rb = g.GetComponent<Rigidbody>();
        rb.velocity = g.transform.right * 5;
        g.SendMessage("SetHurt", info[2]);

        Invoke("CancelInvoke_", info[1]);
    }
    private void CancelInvoke_() {
        CancelInvoke();
    }
    void SendInfo(GameObject g) {
        g.SendMessage("GetInfo", info);
    }

    public int[] GetInfo() {
        return AttackInfo;
    }
    public int[] GetSaveItems() {
        return saveItems;
    }
    public void SetInfo(int[] vs) {
        AttackInfo = vs;
    }
    public void SetSaveItmes(int[] vs) {
        saveItems = vs;
    }


}
