using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class dragdrop_Emphasis : MonoBehaviour, 
    IBeginDragHandler, IDragHandler, IEndDragHandler, 
    IPointerClickHandler, IPointerDownHandler
{
    static Transform TargetGUI;
    static public Vector3 oldPos;
    static public Vector3 newPos;


    void Start()
    {
    }

    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        oldPos = Input.mousePosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        TargetGUI = searchGUI(eventData.pointerPressRaycast.gameObject.transform);
        oldPos = Input.mousePosition;   
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (TargetGUI != null)
        {
            TargetGUI.position = eventData.position;
        }
     

    }

    public void OnEndDrag(PointerEventData eventData)
    {


    }

    private Transform searchGUI(Transform P_transform)
    {
        if(P_transform.gameObject.CompareTag("Camera"))
        {
            return P_transform;
        }
        if (P_transform == P_transform.root)
        {
            return null;
        }
        return searchGUI(P_transform.parent);
    }

    public void MoveSlider()
    {

    }

}