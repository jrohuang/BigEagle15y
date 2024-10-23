using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Record : MonoBehaviour {

    private GameObject allRecordPanel, panel, closeButton;
    [SerializeField] GameObject content;
    private static Text text, allRecordText;
    private static List<string> record = new List<string>();
    private static bool wait = false;
    private static float waitTime;
    private static List<string> waitingRecord = new List<string>();

    void Start() {
        text = transform.Find("Panel/Text").GetComponent<Text>();
        allRecordText = transform.Find("All/Scroll View").GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        allRecordPanel = transform.Find("All").gameObject;
        panel = transform.Find("Panel").gameObject;
        closeButton = transform.Find("All/CloseButton").gameObject;
        closeButton.SetActive(false);
            
    }

    public static void AddRecord(string s) {
        if (!wait) {
            text.text = "-" + s;
            waitTime = Time.time;
            wait = true;
        }
        else {
            waitingRecord.Add(s);
        }
        if (record.Count != 99) {
            record.Add("-" + s);
        }
        else {
            record.RemoveAt(0);
            record.Add("-" + s);
        }
        
    }
    
    private void Update() {
        
        if (waitTime + 2 < Time.time) {
            if (waitingRecord.Count > 0) {
                text.text = "-" + waitingRecord[0];
                waitingRecord.RemoveAt(0); 
                waitTime = Time.time;
            }
        }
        
    }

    TextGenerator generator;
    TextGenerationSettings settings;

    public void _Click() {
        panel.SetActive(false);
        allRecordPanel.SetActive(true);
        closeButton.SetActive(true);
        allRecordText.text = "";

        for (int i = record.Count; i > 0; i--) {
            allRecordText.text += record[i - 1] + "\n";
        }

        RectTransform rt = content.transform as RectTransform;

        generator = allRecordText.cachedTextGeneratorForLayout;
        settings = allRecordText.GetGenerationSettings(rt.rect.size);

        float height = generator.GetPreferredHeight(allRecordText.text, settings) / text.pixelsPerUnit;
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        
    }

    public void _Close() {
        panel.SetActive(true);
        allRecordPanel.SetActive(false);
        closeButton.SetActive(false);
    }

}
