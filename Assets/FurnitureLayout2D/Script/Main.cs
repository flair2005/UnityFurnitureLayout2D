using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour {
    //クラス化　家具をすべて扱う
    public FurnitureManager furnitureManager;

    //行列演算のための変数
    MyMatrix matrixManager; 

    //部屋情報
    Room room, room2;
    float roomMaxX = 350.0f; float roomMinX = 60.0f;
    float roomMaxY = 180.0f; float roomMinY = -180.0f;
    List<Room> roomManager;

    //UIの情報取得
    public UIScript uiScript;

    //最適化の調整用のスライダー
    public Slider conversationSlider;
    public Slider emphasisSlider;
    public Slider alignmentSlider;


    //Emphasis用のポイント
    public GameObject emphasisPoint;


    bool isCalled = false;

    // Use this for initialization
    void Start () {
        //家具情報
        furnitureManager = new FurnitureManager();

        //部屋情報格納
        roomManager = new List<Room>();
        //部屋1
        List<Vector3> p = new List<Vector3>();
        p.Add(new Vector3(172.3f, 182.0f, 0.0f)); p.Add(new Vector3(172.3f, 114.6f, 0.0f));
        p.Add(new Vector3(105.9f, 114.6f, 0.0f)); p.Add(new Vector3(105.9f, 182.0f, 0.0f));
        room = new Room(p);
        roomManager.Add(room);
        //部屋2
        p = new List<Vector3>();
        p.Add(new Vector3(172.3f, -5.77f, 0.0f)); p.Add(new Vector3(172.3f, -156.44f, 0.0f));
        p.Add(new Vector3(92.75f, -156.44f, 0.0f)); p.Add(new Vector3(92.75f, -5.77f, 0.0f));
        room = new Room(p);
        roomManager.Add(room);
        //部屋3
        p = new List<Vector3>();
        p.Add(new Vector3(63.97f, 178.81f, 0.0f)); p.Add(new Vector3(63.97f, 60.84f, 0.0f));
        p.Add(new Vector3(9.35f, 60.84f, 0.0f)); p.Add(new Vector3(9.35f, 178.81f, 0.0f));
        room = new Room(p);
        roomManager.Add(room);
        //部屋4
        p = new List<Vector3>();
        p.Add(new Vector3(72.08f, -66.79f, 0.0f)); p.Add(new Vector3(72.08f, -157.09f, 0.0f));
        p.Add(new Vector3(9.45f, -157.09f, 0.0f)); p.Add(new Vector3(9.45f, -66.79f, 0.0f));
        room = new Room(p);
        roomManager.Add(room);
    }
	
    // Update is called once per frame
    void Update () {
        if (uiScript.setPosition){
            furnitureManager = new FurnitureManager();
            uiScript.setPosition = false;
        }
        furnitureManager.findUserConstraint();
        if(uiScript.multipleModeFlag == 1)
        {
            //一回だけ呼ばれるプログラム
            if (Time.time > 1.5f)
            {
                if (!isCalled)
                {
                    isCalled = true;
                    furnitureManager = new FurnitureManager();


                }

                if (uiScript.reSelected)
                {
                    isCalled = false;
                    uiScript.reSelected = false;
                }

            }

            if (furnitureManager.furniture.Count > 0)
            {
                //行列
                matrixManager = new MyMatrix(furnitureManager.furniture.Count);
                matrixManager.getInverseMatrix(furnitureManager.furniture.Count - 1, matrixManager.matA, ref matrixManager.matAInverse);

                //行列変換
                furnitureManager.transformGlobalToLocalTranslationAndOrientation();
                furnitureManager.calcAfterGlobalTranslationAndOrientation(matrixManager, roomManager);

                //Conversationの最急降下法のプログラム(OK)
                furnitureManager.calcConversationGradientSolver();

                //Alignmentの最急降下法のプログラム(OK)
                furnitureManager.calcAlignmentGradientSolver();

                //Emphasisの最急降下法のプログラム(OK)
                furnitureManager.calcEmphasisGradientSolver(emphasisPoint.transform.localPosition);    

                //最適化の結果をブレンドする
                furnitureManager.blendOptimizedPlacement(conversationSlider.value, alignmentSlider.value, emphasisSlider.value);

            }
        }
        else
        {
            isCalled = false;
        }
    }
}
