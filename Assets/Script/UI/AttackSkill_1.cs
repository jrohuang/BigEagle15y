using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AttackSkill_1 : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler {

    [SerializeField] LayerMask layerMask;
    private Image bgImg, joystickImg;
    private Vector3 inputVector;
    private Vector2 pos;
    private LineRenderer lineRenderer;
    private bool usingJoy;
    private GameObject playerGO, centerPanel, cameraGO, sight;
    private GameObject[] bullet = new GameObject[Bag.bulletInfo.Length];
    public static Text BulletAmount;
    
    private void Awake() {
        for (int i = 0; i < bullet.Length; i++) {
            bullet[i] = Resources.Load<GameObject>("Bullet/PlayerBullet_" + i.ToString());
        }

        BulletAmount = transform.Find("BulletAmount").GetComponent<Text>();
        sight = transform.parent.Find("Sight").gameObject;
        cameraGO = Camera.main.gameObject;
        bgImg = GetComponent<Image>();
        lineRenderer = GetComponent<LineRenderer>();
        centerPanel = transform.GetChild(1).gameObject;
        centerPanel.SetActive(false);
        playerGO = GameObject.Find("Player");
        joystickImg = transform.Find("joy").GetComponent<Image>();
        gameObject.SetActive(false);
    }
    
    public virtual void OnDrag(PointerEventData ped) {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg.rectTransform, ped.position, ped.pressEventCamera, out pos)) {
            pos.x = (pos.x / bgImg.rectTransform.sizeDelta.x);
            pos.y = (pos.y / bgImg.rectTransform.sizeDelta.y);
            inputVector = new Vector3(pos.x * 2 + 1, 0, pos.y * 2 - 1);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;
            joystickImg.rectTransform.anchoredPosition = new Vector3(inputVector.x * (bgImg.rectTransform.sizeDelta.x / 4), inputVector.z * (bgImg.rectTransform.sizeDelta.y / 4));
        }

    }
    private GameObject AIofInfo;
    RaycastHit hit;
    
    private void Update() {
        
        if (bgImg.fillAmount < 1) {
            bgImg.fillAmount = Mathf.MoveTowards(bgImg.fillAmount, 1, 0.7f * Time.deltaTime);
        }

        if (usingJoy && CameraCtrl.height) {
            if (Mathf.Abs(inputVector.x) < 0.2f && Mathf.Abs(inputVector.z) < 0.2f) {
                lineRenderer.positionCount = 0;
                return;
            }
        }
        if (!CameraCtrl.height) {
            Ray ray = new Ray(cameraGO.transform.position, cameraGO.transform.forward);
            Physics.Raycast(ray, out hit, 100, layerMask);


            if (hit.transform) {
                if (hit.transform.gameObject.CompareTag("AI")) {
                    if (AIofInfo == hit.transform.gameObject) {
                        AIofInfo = hit.transform.gameObject;
                    }
                    else {
                        PlayerCtrl.AIExit();
                    }
                    AIofInfo = hit.transform.gameObject;
                    hit.transform.gameObject.SendMessage("GetRay");
                }
                else { PlayerCtrl.AIExit(); }
            }
            else { PlayerCtrl.AIExit(); }
        }

        if (usingJoy && Bag.EB.Amount > 0 && bgImg.fillAmount == 1 && Bag.EB.BulletNumber != -1) {
            if (CameraCtrl.height) {
                Ray ray = new Ray(playerGO.transform.position, new Vector3(inputVector.z, 0, -inputVector.x));
                Physics.Raycast(ray, out hit, 100, layerMask);
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, playerGO.transform.position);
                lineRenderer.SetPosition(1, playerGO.transform.position + new Vector3(inputVector.z, 0, -inputVector.x) * Vector3.Distance(playerGO.transform.position, hit.transform.gameObject.transform.position));
            }

            if (hit.transform) {
                if (hit.transform.gameObject.CompareTag("AI")) {
                    if (AIofInfo == hit.transform.gameObject) {
                        AIofInfo = hit.transform.gameObject;
                    }
                    else {
                        PlayerCtrl.AIExit();
                    }
                    AIofInfo = hit.transform.gameObject;
                    hit.transform.gameObject.SendMessage("GetRay");
                }
                else { PlayerCtrl.AIExit(); }
            }
            else { PlayerCtrl.AIExit(); }
        }

        
    }

    public virtual void OnPointerDown(PointerEventData ped) {
        if (CameraCtrl.height) {
            OnDrag(ped);
            centerPanel.SetActive(true);
            lineRenderer.positionCount = 2;
        }
        usingJoy = true;

    }

    public virtual void OnPointerUp(PointerEventData ped) {
        if (usingJoy && Bag.EB.Amount > 0 && bgImg.fillAmount == 1) {
            if (CameraCtrl.height) {
                if (CameraCtrl.height && Mathf.Abs(inputVector.x) > 0.2f || Mathf.Abs(inputVector.z) > 0.2f) {
                    AS_1Attack();
                }
            }
            else {
                AS_1Attack();
            }
        }
        PlayerCtrl.AIExit();
        centerPanel.SetActive(false);
        inputVector = Vector3.zero;
        joystickImg.rectTransform.anchoredPosition = Vector3.zero;
        lineRenderer.positionCount = 0;
        usingJoy = false;
    }

    public void _EnterCenterPanel() {
        lineRenderer.positionCount = 0;
        usingJoy = false;
    }
    public void _ExitCenterPanel() {
        lineRenderer.positionCount = 2;
        usingJoy = true;
    }

    private void AS_1Attack() {
        GameObject g;
        Rigidbody rb;
        if (CameraCtrl.height) {
            g = Instantiate(bullet[Bag.EB.BulletNumber], playerGO.transform.position, Quaternion.identity);
            rb = g.GetComponent<Rigidbody>();
            g.transform.forward = lineRenderer.GetPosition(1) - lineRenderer.GetPosition(0);
        }
        else {
            g = Instantiate(bullet[Bag.EB.BulletNumber], cameraGO.transform.position, Quaternion.identity);
            rb = g.GetComponent<Rigidbody>();
            g.transform.forward = cameraGO.transform.forward;
        }

        g.SendMessage("SetHurt", Bag.EB.Hurt);
        rb.velocity = g.transform.forward * Bag.EB.Speed;

        PlayerCtrl.PD.Items[Bag.bulletInfo[Bag.EB.BulletNumber].ItemNumber]--;
        bgImg.fillAmount = 0;
        BulletAmount.text = PlayerCtrl.PD.Items[Bag.bulletInfo[Bag.EB.BulletNumber].ItemNumber].ToString();
        
        if (AttackSkill.usingSkillNumber[0] == 3 || AttackSkill.usingSkillNumber[1] == 3) {
            if (CameraCtrl.height) {
                g = Instantiate(bullet[Bag.EB.BulletNumber], playerGO.transform.position, Quaternion.identity);
                rb = g.GetComponent<Rigidbody>();
                g.transform.forward = lineRenderer.GetPosition(1) - lineRenderer.GetPosition(0);
            }
            else {
                g = Instantiate(bullet[Bag.EB.BulletNumber], cameraGO.transform.position, Quaternion.identity);
                rb = g.GetComponent<Rigidbody>();
                g.transform.forward = cameraGO.transform.forward;
            }

            g.SendMessage("SetHurt", Bag.EB.Hurt);
            rb.velocity = g.transform.forward * Bag.EB.Speed;
            g.SendMessage("CloneBullet");
            
        }

    }

    void ChangeAttackMode() {
        if (CameraCtrl.height) {
            sight.SetActive(false);
            joystickImg.gameObject.SetActive(true);
        }
        else {
            sight.SetActive(true);
            joystickImg.gameObject.SetActive(false);
        }
        
    }

}
