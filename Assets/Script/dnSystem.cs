using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class dnSystem : MonoBehaviour, IStoreListener {

    [SerializeField] Image image;
    [SerializeField] Sprite E, C;
    static string kProductID_dn1 = "dn_1";
    static string kProductID_dn2 = "dn_2";
    IStoreController m_StoreController;

    private void Awake() {
        image.sprite = DataCtrl.English ? E : C;

        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(kProductID_dn1, ProductType.Consumable);
        builder.AddProduct(kProductID_dn2, ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
    }

    void Start () {
		
	}
	
	
    public void _Buy_dn1() {
        if (m_StoreController != null) {
            Product product = m_StoreController.products.WithID(kProductID_dn1);
            if (product != null && product.availableToPurchase) {
                m_StoreController.InitiatePurchase(product);
            }
        }
    }
    public void _Buy_dn2() {
        if (m_StoreController != null) {
            Product product = m_StoreController.products.WithID(kProductID_dn2);
            if (product != null && product.availableToPurchase) {
                m_StoreController.InitiatePurchase(product);
            }
        }
    }

    public void _Menu() {
        SceneManager.LoadScene("Menu");
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
        m_StoreController = controller;
    }

    public void OnInitializeFailed(InitializationFailureReason error) {
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p) {
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e) {
        transform.Find("Panel/Text").gameObject.SetActive(true);
        return PurchaseProcessingResult.Complete;
    }
    
}
