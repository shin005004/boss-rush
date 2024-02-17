using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SkipButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject tutorialManager;
    public void OnPointerClick(PointerEventData eventData)
    {
        Time.timeScale = 1f;
        Destroy(tutorialManager);
        SceneLoader.Instance.LoadMainStoreScene();
    }
}
