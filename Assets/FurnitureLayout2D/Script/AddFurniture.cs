using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AddFurniture : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerClickHandler, IPointerDownHandler
{
    public GameObject selectedFrame;

    void Start () {
        selectedFrame.SetActive(false);
	}
	
    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!selectedFrame.activeSelf)
        {
            selectedFrame.SetActive(true);
        }
        else
        {
            selectedFrame.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }
}
