using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tset : MonoBehaviour {

    TextGenerator generator;
    TextGenerationSettings settings;
    RectTransform rt, ChildRT;
    Text text;

    void Start () {
        text = transform.GetChild(0).GetComponent<Text>();
        rt = transform.GetComponent<RectTransform>();

        generator = text.cachedTextGeneratorForLayout;
        settings = text.GetGenerationSettings(rt.rect.size);
    }
	
	void Update () {
        float height = generator.GetPreferredHeight(text.text, settings) / text.pixelsPerUnit;
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }
}
