using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraCtrl : MonoBehaviour {

    private static GameObject button;
    private GameObject player;
    [SerializeField] int MaxDistance, MinDistane, MaxLightRange, MinLightRange;
    public static int CameraMaxDis, CameraMinDis, LightMaxRange, LightMinRange;
    public static float TouchSensitivity;
    private GameObject lightGO;
    private Light light;
    public static bool height = true;
    private GameObject AttackSkill_1;
    public static bool TouchCtrl = true;
    [SerializeField] bool StartInDown;
    public static bool LLoadData = false;
    private void Start() {

        AttackSkill_1 = GameObject.Find("UI").transform.Find("AttackSkill_1").gameObject;
        button = GameObject.Find("UI").transform.Find("CameraChagneIcon").gameObject;
        HalfScreenHeight = Screen.height / 2;
        HalfScreenWidth = Screen.width / 3;
        lightGO = transform.Find("Spotlight").gameObject;
        light = lightGO.GetComponent<Light>();
        player = GameObject.Find("Player");
        CameraMaxDis = MaxDistance;
        CameraMinDis = MinDistane;
        LightMaxRange = MaxLightRange;
        LightMinRange = MinLightRange;
        ///

        EventTrigger trigger = GameObject.Find("UI").transform.Find("CameraChagneIcon").GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => { CameraChange(false); });
        trigger.triggers.Add(entry);

        ///
        
        float a = CameraMinDis - CameraMaxDis;
        transform.position = new Vector3(Camera.main.transform.position.x,
            a * DataCtrl.playerRecord.SettingValue[1] + CameraMinDis - a, Camera.main.transform.position.z);
        light.range = DataCtrl.playerRecord.SettingValue[1] * (LightMinRange - LightMaxRange) + LightMaxRange;

        AudioListener.volume = DataCtrl.playerRecord.SettingValue[0];
        TouchSensitivity = DataCtrl.playerRecord.SettingValue[2];

        if (!LLoadData) {
            if (StartInDown) {
                ChangeHieght(true);
            }
        }
        else {
            if (player.transform.position.y < -2) {
                height = false;
                ChangeHieght(true);
            }
        }
    }

    private float HalfScreenHeight, HalfScreenWidth, x;
    private Touch t;

    void Update() {
        if (!height) {
            Vector3 v3 = new Vector3(player.transform.position.x, player.transform.position.y + 0.25f, player.transform.position.z);
            transform.position = v3 + transform.forward * -0.2f;

            for (int i = 0; i < Input.touchCount; i++) {
                if (Input.GetTouch(i).position.y > HalfScreenHeight && Input.GetTouch(i).position.x > HalfScreenWidth) {
                    t = Input.GetTouch(i);
                }
            }

            if (TouchCtrl) {
                if (t.phase == TouchPhase.Moved) {
                    transform.Rotate(new Vector3(0, t.deltaPosition.x * Time.deltaTime * 8 * TouchSensitivity, 0));
                    if (t.position.y > HalfScreenHeight || t.position.x > HalfScreenWidth) {
                        t.phase = TouchPhase.Ended;
                    }
                }
            }

        }
        else {
            Vector3 v3 = new Vector3(player.transform.position.x - 1.5f, transform.position.y, player.transform.position.z);
            transform.position = Vector3.Lerp(transform.position, v3, 0.1f);
        }
    }
    
    public void CameraChange(bool start) {
        if (height) {
            Camera.main.cullingMask &= ~(1 << 14);
            lightGO.transform.localPosition = new Vector3(0, 0, 0);
            transform.rotation = Quaternion.Euler(0, 90, 0);
            height = false;
            light.range = 18;
        }
        else {
            Camera.main.cullingMask |= 1 << 14;
            lightGO.transform.localPosition = new Vector3(0, 1, 2);
            transform.rotation = Quaternion.Euler(70, 90, 0);
            transform.position = new Vector3(player.transform.position.x - 1.5f, MaxDistance, player.transform.position.z);
            height = true;
            light.range = MaxLightRange;
        }

        if (AttackSkill_1.activeSelf) {
            AttackSkill_1.SendMessage("ChangeAttackMode");
        }

        if (!start) {
            PlayerCtrl.AIExit();
        }
    }

    void ChangeHieght(bool s) {
        if (button.activeSelf) {
            height = true;
            button.SetActive(false);
        }
        else {
            height = false;
            button.SetActive(true);
        }
        CameraChange(s);
    }

}
