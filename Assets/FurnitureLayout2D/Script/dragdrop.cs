using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class dragdrop : MonoBehaviour, 
    IBeginDragHandler, IDragHandler, IEndDragHandler, 
    IPointerClickHandler, IPointerDownHandler
{
    static Transform TargetGUI;
    public bool startSimulation;
    static public Vector3 oldPos;
    static public Vector3 newPos;
    public Slider slider;
    public GameObject selected;
    public bool directionalFurniture;

    bool onDraging;

    public bool wallFurniture;
    public bool bedFurniture;

    void Start()
    {
        startSimulation = false;
        onDraging = false;
        //slider.gameObject.SetActive(false);
        selected.SetActive(false);
    }

    void Update()
    {
        if (slider.IsActive() == true)
        {
            MoveSlider();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!onDraging)
        {
            if (this.tag == "GUI")
            {
                selected.SetActive(true);
                //this.GetComponent<Image>().color = new Color(Color.white.r, Color.white.g, Color.white.b, Color.white.a / 3.0f);
                this.tag = "Selected";
            }
            else if (this.tag == "Selected")
            {
                selected.SetActive(false);
                //this.GetComponent<Image>().color = Color.white;
                this.tag = "GUI";
            }
        }
        onDraging = false;
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
        startSimulation = true;
        onDraging = true;
        slider.gameObject.SetActive(false);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        startSimulation = false;
        slider.gameObject.SetActive(true);
        slider.transform.localPosition = this.transform.localPosition + new Vector3(0.0f, 40.0f, 0.0f);

        slider.value = 1.0f - this.transform.eulerAngles.z / 360.0f;
    }

    private Transform searchGUI(Transform P_transform)
    {
        if(P_transform.gameObject.CompareTag("Selected") || P_transform.gameObject.CompareTag("GUI"))
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
        //Debug.Log(value_slider.value);
        float value = slider.value;
        if (value > 0.1f && value < 0.14f) { value = 0.125f; }
        if (value > 0.19f && value < 0.3f) { value = 0.25f; }
        if (value > 0.32f && value < 0.40f) { value = 0.375f; }
        if (value > 0.43f && value < 0.56f) { value = 0.5f; }
        if (value > 0.59f && value < 0.65f) { value = 0.625f; }
        if (value > 0.69f && value < 0.81f) { value = 0.75f; }
        if (value > 0.84f && value < 0.90f) { value = 0.875f; }
        if (TargetGUI != null)
        {
            TargetGUI.eulerAngles = new Vector3(0.0f, 0.0f, -360.0f * value);
        }
    }

}