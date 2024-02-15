using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUI : MonoBehaviour
{
    const string heart = "Heart", heartFadeOut = "Heart--FadeOut";
    private static VisualElement[] hearts = new VisualElement[3];
    void Start() {
        var root = GetComponent<UIDocument>().rootVisualElement;
        for (int i = 0; i < 3; i++) {
            hearts[i] = root.Q<VisualElement>(heart + i);
        }
    }
    public static void RemoveHeart() {
        for (int i = 2; i >= 0; i--) {
            if (hearts[i].ClassListContains(heartFadeOut)) continue;
            hearts[i].AddToClassList(heartFadeOut);
            return;
        }
    }
}
