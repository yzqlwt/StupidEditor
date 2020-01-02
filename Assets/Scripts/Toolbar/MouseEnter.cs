using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class MouseEnter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public void OnPointerEnter(PointerEventData eventData)
    {
        var color = gameObject.GetComponent<Image>().color;
        gameObject.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 255f);
    }

    public void OnPointerExit(PointerEventData eventData)

    {
        var color = gameObject.GetComponent<Image>().color;
        gameObject.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0f);
    }
}