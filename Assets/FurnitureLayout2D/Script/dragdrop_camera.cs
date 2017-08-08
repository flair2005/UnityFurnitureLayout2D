using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class dragdrop_camera : MonoBehaviour, 
    IBeginDragHandler, IDragHandler, IEndDragHandler, 
    IPointerClickHandler, IPointerDownHandler
{
    static Transform TargetGUI;
    //public bool startSimulation;
    static public Vector3 oldPos;
    static public Vector3 newPos;
    public Slider slider;
    //public GameObject selected;

    //bool onDraging;

    //public bool wallFurniture;
    //public bool bedFurniture;

    void Start()
    {
    }

    void Update()
    {
        if (slider != null && slider.IsActive() == true)
        {
            MoveSlider();
        }
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
        if(slider != null)
        {
            slider.gameObject.SetActive(false);
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(slider != null)
        {
            slider.gameObject.SetActive(true);
            slider.transform.localPosition = this.transform.localPosition + new Vector3(0.0f, 30.0f, 0.0f);
            slider.value = 1.0f - this.transform.eulerAngles.z / 360.0f;
        }

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
        float value = 0.0f;
        if(slider != null)
        {
            value = slider.value;
        }
        //if (value > 0.05 && value < 0.14) { value = 0.125f; }
        //if (value > 0.17 && value < 0.33) { value = 0.25f; }
        //if (value > 0.30 && value < 0.40) { value = 0.375f; }
        //if (value > 0.42 && value < 0.58) { value = 0.5f; }
        //if (value > 0.59 && value < 0.66) { value = 0.625f; }
        //if (value > 0.67 && value < 0.83) { value = 0.75f; }
        //if (value > 0.84 && value < 0.90) { value = 0.875f; }
        if (TargetGUI != null)
        {
            TargetGUI.eulerAngles = new Vector3(0.0f, 0.0f, -360.0f * value);
        }
    }

}