using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIScript : MonoBehaviour {
    public int multipleModeFlag;
    public Canvas canvas2DView;
    public Camera mainCamera;

    public Canvas canvas3DView;
    public Camera view3DCamera;

    public bool view3Dmode;
    public bool view2Dmode;

    public bool setPosition;

    public bool reSelected;

    public Slider slider2D;
    public Slider slider3D;

    void Start()
    {
        view2Dmode = true;
        view3Dmode = false;
        setPosition = false;
        reSelected = false;

        canvas3DView.gameObject.SetActive(false);
        view3DCamera.depth = 0;
        view3DCamera.rect = new Rect(0.805f, 0.8f, 0.185f, 0.185f);
    }

    public UIScript() { }

    //終了ボタン(ファイル出力時のみ有効)
    public void OnQuiteButton(){ Application.Quit(); }

    //最初からスタート
    public void OnReloadButton()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);

    }

    //移動モード選択　Single or Multiple
    public void ChangeMoveMode(Dropdown dropdown)
    {
        switch (dropdown.value)
        {
            case 0:
                Debug.Log("singleMode");
                multipleModeFlag = 0;
                break;
            case 1:
                Debug.Log("multipleMode");
                multipleModeFlag = 1;
                break;
            default:
                multipleModeFlag = -1;
                break;
        }
    }

    public void OnView3DCamera()
    {
        //2Dカメラの設定
        //canvas2DView.enabled = false;
        canvas2DView.gameObject.SetActive(false);
        mainCamera.enabled = true;
        view2Dmode = false;


        //3Dカメラの設定
        //canvas3DView.enabled = true;
        canvas3DView.gameObject.SetActive(true);
        view3DCamera.enabled = true;
        view3Dmode = true;
        view3DCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

        slider3D.value = slider2D.value;
    }

    public void OnView2DCamere()
    {
        //2Dカメラの設定
        //canvas2DView.enabled = true;
        canvas2DView.gameObject.SetActive(true);
        mainCamera.enabled = true;
        view2Dmode = true;

        //3Dカメラの設定
        //canvas3DView.enabled = false;
        canvas3DView.gameObject.SetActive(false);
        view3DCamera.enabled = true;
        view3Dmode = false;
        view3DCamera.depth = 0;
        view3DCamera.rect = new Rect(0.805f, 0.8f, 0.185f, 0.185f);

        slider2D.value = slider3D.value;

    }

    public void OnSetPositionButton()
    {
       setPosition = true;
    }

    public void reSelectFurnitureItemsButton()
    {
        reSelected = true;
    }

    public void OnChangeScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        if(scene.name == "Main")
        {
            SceneManager.LoadScene("Demo");
        }
        else if(scene.name == "Demo")
        {
            SceneManager.LoadScene("Main");
        }
    }

}
