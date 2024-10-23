using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightStation : MonoBehaviour {

    private Color[] colors = new Color[5];
    private int color = 0;
    private Light light;

    private void Awake() {
        colors[0] = Color.white;
        colors[1] = new Color(0, 255f / 234, 1, 1);
        colors[2] = new Color(0, 255f / 224, 0, 1);
        colors[3] = new Color(255f / 224, 255f / 103, 255f / 52, 1);
        colors[4] = new Color(255f / 250, 0, 1, 1);

        light = transform.GetComponentInChildren<Light>();
        light.color = colors[0];
    }

    void ChangeColor() {
        if (color + 1 != colors.Length) {
            color++;
        }
        else {
            color = 0;
        }
        light.color = colors[color];
    }

}
