using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class joystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler {

    [SerializeField] GameObject BagGo;
    private Image bgImg, joystickImg;
    private Vector3 inputVector;
    private GameObject player;
    private Vector2 pos;
    Rigidbody rb;
    GameObject Mcamera;

    private void Start() {
        Mcamera = Camera.main.gameObject;
        player = GameObject.Find("Player");
        rb = player.GetComponent<Rigidbody>();
        bgImg = GetComponent<Image>();
        joystickImg = transform.Find("joy").GetComponent<Image>();
    }

    private void FixedUpdate() {
        if (CameraCtrl.height) {
            rb.AddForce(new Vector3(inputVector.z, 0, -inputVector.x) * Time.deltaTime * PlayerCtrl.speed);
        }
        else {
            if (inputVector.x > 0.4f) {
                rb.AddForce(Camera.main.transform.right * Time.deltaTime * PlayerCtrl.speed);
            }
            if (inputVector.x < -0.4f) {
                rb.AddForce(-Camera.main.transform.right * Time.deltaTime * PlayerCtrl.speed);
            }
            if (inputVector.z > 0.4f) {
                rb.AddForce(Camera.main.transform.forward * Time.deltaTime * PlayerCtrl.speed);
            }
            if (inputVector.z < -0.4f) {
                rb.AddForce(-Camera.main.transform.forward * Time.deltaTime * PlayerCtrl.speed);
            }
        }
    }
    
    public virtual void OnDrag(PointerEventData ped) {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg.rectTransform, ped.position, ped.pressEventCamera, out pos)) {
            pos.x = (pos.x / bgImg.rectTransform.sizeDelta.x);
            pos.y = (pos.y / bgImg.rectTransform.sizeDelta.y);
            inputVector = new Vector3(pos.x * 2 + 1, 0, pos.y * 2 - 1);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;
            joystickImg.rectTransform.anchoredPosition = new Vector3(inputVector.x * (bgImg.rectTransform.sizeDelta.x / 4),inputVector.z * (bgImg.rectTransform.sizeDelta.y / 4));
        }

    }

    public virtual void OnPointerDown(PointerEventData ped) {
        OnDrag(ped);
        if (!Bag.IsOpenBag) {
            if (BagGo.activeSelf) {
                BagGo.SendMessage("_BagClose");
            }
        }
    }

    public virtual void OnPointerUp(PointerEventData ped) {
        inputVector = Vector3.zero;
        joystickImg.rectTransform.anchoredPosition = Vector3.zero;
    }
}