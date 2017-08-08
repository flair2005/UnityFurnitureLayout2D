using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture3DViewer : MonoBehaviour {
    public Main main;
    public List<GameObject> furniture;
    public UIScript uiScript;
    private Vector3[] trans;
    private float[] rotation;
    public GameObject uiCamera;
    public Camera camera;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < furniture.Count; i++)
        {
            furniture[i].SetActive(false);
        }
        trans = new Vector3[furniture.Count];
        rotation = new float[furniture.Count];
	}
	
	// Update is called once per frame
	void Update () {

        if (uiScript.view2Dmode)
        {
            for (int i = 0; i < furniture.Count; i++)
            {
                string name = furniture[i].name;
                GameObject model = GameObject.Find(name);
                furniture[i].SetActive(true);
                trans[i] = model.transform.localPosition / 15.0f;
                rotation[i] = -model.transform.localEulerAngles.z;
            }
            camera.transform.position = new Vector3(uiCamera.transform.localPosition.x, uiCamera.transform.localPosition.z + 30.0f, uiCamera.transform.localPosition.y) / 15.0f;
            camera.transform.eulerAngles = new Vector3(9.517f, 180.0f - uiCamera.transform.localEulerAngles.z, 0.0f);
        }

        for (int i = 0; i < furniture.Count; i++)
        {
            furniture[i].transform.position = new Vector3(trans[i].x, trans[i].z, trans[i].y);
            furniture[i].transform.eulerAngles = new Vector3(0.0f, rotation[i], 0.0f);
        }       
    }
}
