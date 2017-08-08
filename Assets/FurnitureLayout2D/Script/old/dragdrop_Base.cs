using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class dragdrop_Base : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler 
{
    Transform TargetGUI;
    public GameObject slider;
    public Slider value_slider;
    float preAngle;
    static GameObject latestFurniture;

    void Start()
    {
        //preAngle = this.transform.eulerAngles.z;
        value_slider.value = 1.0f - this.transform.eulerAngles.z / 360.0f;
    }

    void Update()
    {
        if (latestFurniture != this.gameObject)
        {
            slider.SetActive(false);
        }
        if (Input.GetMouseButtonDown(1))
        {
            slider.SetActive(false);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        TargetGUI = searchGUI(eventData.pointerPressRaycast.gameObject.transform);
        slider.SetActive(false);
       // preAngle = TargetGUI.eulerAngles.z;
        latestFurniture = this.gameObject;    
    }

    public void OnDrag(PointerEventData eventData)
    {
        //eventData.lastPress.transform.position = eventData.position;
        if (TargetGUI != null)
        {
            TargetGUI.position = eventData.position;
            value_slider.transform.position = eventData.position + Screen.height*new Vector2(0.0f, 0.1f);
        }
        slider.SetActive(false);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        slider.SetActive(true);
    }

    public void MoveSlider()
    {
        //Debug.Log(value_slider.value);
        float value = value_slider.value;
        if (value > 0.05 && value < 0.14) { value = 0.125f; }
        if (value > 0.17 && value < 0.33) { value = 0.25f;  }
        if (value > 0.30 && value < 0.40) { value = 0.375f; }
        if (value > 0.42 && value < 0.58) { value = 0.5f;   }
        if (value > 0.59 && value < 0.66) { value = 0.625f; }
        if (value > 0.67 && value < 0.83) { value = 0.75f;  }
        if (value > 0.84 && value < 0.90) { value = 0.875f; }
        if(TargetGUI != null)
        {
            TargetGUI.eulerAngles = new Vector3(0.0f, 0.0f, - 360.0f * value);
        }
    }

    private Transform searchGUI(Transform P_transform)
    {
        if (P_transform.gameObject.CompareTag("GUI"))
        {
            
            return P_transform;
        }
        if (P_transform == P_transform.root)
        {
            slider.SetActive(false);
            return null;
        }
        return searchGUI(P_transform.parent);
    }

}