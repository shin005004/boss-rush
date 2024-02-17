using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackArrow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject guide;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = new Color(169 / 255f, 169 / 255f, 169 / 255f, 1f);
        guide.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = Color.white;
        guide.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SceneLoader.Instance.LoadMainStoreScene();
    }
}
