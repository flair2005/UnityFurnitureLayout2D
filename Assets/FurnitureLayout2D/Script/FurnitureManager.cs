using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureManager : MonoBehaviour {
    //家具のオブジェクト
    public List<GameObject> furniture;
    //家具の状態(壁に張り付く＝１)
    public List<int> state;
    public static int userConstraint;

    //位置
    public List<Vector3> beforeGlobalTrans;
    public List<Vector3> beforeLocalTrans;

    //回転
    public List<float> beforeGlobalOrientaion;
    public List<float> beforeLocalOrientation;

    //最適化した後の各位置と回転
    public List<Vector3> userTrans, conversationTrans, alignmentTrans, emphasisTrans;
    public List<float> userOrientation, conversationOrientation, alignmentOrientation, emphasisOrientation;

    public FurnitureManager()
    {
        furniture = new List<GameObject>(); state = new List<int>();
        beforeGlobalTrans = new List<Vector3>(); beforeGlobalOrientaion = new List<float>();
        //GameObject[] tagobjs = GameObject.FindGameObjectsWithTag("GUI");
        GameObject[] tagobjs = GameObject.FindGameObjectsWithTag("Selected");
        for (int i = 0; i < tagobjs.Length; i++) { furniture.Add(tagobjs[i]); }
        Debug.Log("Num of Furniture = " + furniture.Count);

        for (int i = 0; i < furniture.Count; i++)
        {
            beforeGlobalTrans.Add(furniture[i].transform.localPosition);
            beforeGlobalOrientaion.Add(furniture[i].transform.localEulerAngles.z);
            state.Add(0);
        }

        beforeLocalTrans = new List<Vector3>();
        beforeLocalOrientation = new List<float>();

        //最適化した後の各位置と回転
        userTrans = new List<Vector3>();
        conversationTrans = new List<Vector3>();
        emphasisTrans = new List<Vector3>();
        alignmentTrans = new List<Vector3>();

        userOrientation = new List<float>();
        conversationOrientation = new List<float>();
        emphasisOrientation = new List<float>();
        alignmentOrientation = new List<float>();

    }

    public void findUserConstraint()
    {
        for (int i = 0; i < furniture.Count; i++)
        {
            if (furniture[i].GetComponent<dragdrop>().startSimulation )
            {
                Debug.Log("Constraint = No." + i);
                userConstraint = i;
            }
        }
    }

    public void transformGlobalToLocalTranslationAndOrientation()
    {
        //「引越し前」の指定した家具のローカル座標を定義する
        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, beforeGlobalOrientaion[userConstraint]);
        Matrix4x4 m = Matrix4x4.identity;
        m.SetTRS(new Vector3(0.0f, 0.0f, 0.0f), rotation, new Vector3(1.0f, 1.0f, 1.0f));
        Vector3 localU = m.MultiplyPoint3x4(new Vector3(1.0f, 0.0f, 0.0f));
        Vector3 localV = m.MultiplyPoint3x4(new Vector3(0.0f, 1.0f, 0.0f));
        //引越し前のローカルな位置
        beforeLocalTrans = new List<Vector3>();
        for (int i = 0; i < furniture.Count; i++)
        {
            beforeLocalTrans.Add(new Vector3(Vector3.Dot(localU, (beforeGlobalTrans[i] - beforeGlobalTrans[userConstraint])), Vector3.Dot(localV, (beforeGlobalTrans[i] - beforeGlobalTrans[userConstraint])), 0.0f));
        }

        List<Vector3> localFrontVector = new List<Vector3>();
        beforeLocalOrientation = new List<float>();
        //「引越し前」の家具の正面ベクトル
        for (int i = 0; i < furniture.Count; i++)
        {
            rotation = Quaternion.Euler(0.0f, 0.0f, beforeGlobalOrientaion[i]);
            m = Matrix4x4.identity;
            m.SetTRS(new Vector3(0.0f, 0.0f, 0.0f), rotation, new Vector3(1.0f, 1.0f, 1.0f));
            Vector3 globalFrontVector = m.MultiplyPoint3x4(new Vector3(0.0f, 1.0f, 0.0f));
            localFrontVector.Add(new Vector3(Vector3.Dot(localU, globalFrontVector), Vector3.Dot(localV, globalFrontVector), 0.0f));
            float dot = Vector3.Dot(localFrontVector[i], new Vector3(0.0f, 1.0f, 0.0f));
            if (dot <= -1.0f) { dot = -1.0f; }
            else if (dot >= 1.0f) { dot = 1.0f; }
            //float theta = Mathf.Acos(Vector3.Dot(localFrontVector[i], new Vector3(0.0f, 1.0f, 0.0f))) * 180.0f / Mathf.PI;
            float theta = Mathf.Acos(dot) * 180.0f / Mathf.PI;
            if (localFrontVector[i].x > 0)
            {
                theta = 360.0f - theta;
            }
            beforeLocalOrientation.Add(theta);

        }
    }

    public void transformGlobalToLocalTranslationAndOrientation(ref List<Vector3> localTrans, ref List<float> localOrientation)
    {
        //「引越し前」の指定した家具のローカル座標を定義する
        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, furniture[userConstraint].transform.localEulerAngles.z);
        Matrix4x4 m = Matrix4x4.identity;
        m.SetTRS(new Vector3(0.0f, 0.0f, 0.0f), rotation, new Vector3(1.0f, 1.0f, 1.0f));
        Vector3 localU = m.MultiplyPoint3x4(new Vector3(1.0f, 0.0f, 0.0f));
        Vector3 localV = m.MultiplyPoint3x4(new Vector3(0.0f, 1.0f, 0.0f));
        //引越し前のローカルな位置
        localTrans = new List<Vector3>();
        for (int i = 0; i < furniture.Count; i++)
        {
            localTrans.Add(new Vector3(Vector3.Dot(localU, (furniture[i].transform.localPosition - furniture[userConstraint].transform.localPosition)), Vector3.Dot(localV, (furniture[i].transform.localPosition - furniture[userConstraint].transform.localPosition)), 0.0f));
            //localTrans.Add(new Vector3(Vector3.Dot(localU, (globalTrans[i] - globalTrans[userConstraint])), Vector3.Dot(localV, (globalTrans[i] - globalTrans[userConstraint])), 0.0f));
        }

        List<Vector3> localFrontVector = new List<Vector3>();
        //ここメモリ危ない．．．苦肉の策　下でif文で切った．．．
        localOrientation = new List<float>();
        //「引越し前」の家具の正面ベクトル
        for (int i = 0; i < furniture.Count; i++)
        {
            rotation = Quaternion.Euler(0.0f, 0.0f, furniture[i].transform.localEulerAngles.z);
            m = Matrix4x4.identity;
            m.SetTRS(new Vector3(0.0f, 0.0f, 0.0f), rotation, new Vector3(1.0f, 1.0f, 1.0f));
            Vector3 globalFrontVector = m.MultiplyPoint3x4(new Vector3(0.0f, 1.0f, 0.0f));
            localFrontVector.Add(new Vector3(Vector3.Dot(localU, globalFrontVector), Vector3.Dot(localV, globalFrontVector), 0.0f));
            float dot = Vector3.Dot(localFrontVector[i], new Vector3(0.0f, 1.0f, 0.0f));
            if(dot <= -1.0f) { dot = -1.0f; }
            else if(dot >= 1.0f) { dot = 1.0f; }
            //float theta = Mathf.Acos(Vector3.Dot(localFrontVector[i], new Vector3(0.0f, 1.0f, 0.0f))) * 180.0f / Mathf.PI;
            float theta = Mathf.Acos(dot) * 180.0f / Mathf.PI;
            if (localFrontVector[i].x > 0)
            {
                theta = 360.0f - theta;
            }
            localOrientation.Add(theta);
        }
    }

    public void transformLocalToGlobalTranslationAndOrientation(List<Vector3> transCalculatedByMatrix, List<float> OrientationCaluculatedByMatrix)
    {
        //座標戻す
        //「引越し後」の指定した家具のローカル座標
        Quaternion rotation = Quaternion.Euler(furniture[userConstraint].transform.localEulerAngles.x, furniture[userConstraint].transform.localEulerAngles.y, furniture[userConstraint].transform.localEulerAngles.z);
        Matrix4x4 m = Matrix4x4.identity;
        m.SetTRS(new Vector3(0.0f, 0.0f, 0.0f), rotation, new Vector3(1.0f, 1.0f, 1.0f));
        Vector3 afterLocalU = m.MultiplyPoint3x4(new Vector3(1.0f, 0.0f, 0.0f));
        Vector3 afterLocalV = m.MultiplyPoint3x4(new Vector3(0.0f, 1.0f, 0.0f));

        int ai = 0;
        for (int i = 0; i < furniture.Count; i++)
        {
            if (i==userConstraint /*|| state[i] != 0*/)
            {
                furniture[i].transform.localPosition = furniture[i].transform.localPosition;
                furniture[i].transform.localEulerAngles = furniture[i].transform.localEulerAngles; 
            }
            else
            {
                furniture[i].transform.localPosition = 
                    furniture[userConstraint].transform.localPosition + transCalculatedByMatrix[ai].x * afterLocalU + transCalculatedByMatrix[ai].y * afterLocalV;
                furniture[i].transform.localEulerAngles = new Vector3(0.0f, 0.0f, OrientationCaluculatedByMatrix[ai] + furniture[userConstraint].transform.localEulerAngles.z);
                ai++;
            }
        }
    }

    public void transformLocalToGlobalTranslationAndOrientation(List<int> nearWallID, List<Vector3> transCalculatedByMatrix, List<float> OrientationCaluculatedByMatrix)
    {
        //座標戻す
        //「引越し後」の指定した家具のローカル座標
        Quaternion rotation = Quaternion.Euler(furniture[userConstraint].transform.localEulerAngles.x, furniture[userConstraint].transform.localEulerAngles.y, furniture[userConstraint].transform.localEulerAngles.z);
        Matrix4x4 m = Matrix4x4.identity;
        m.SetTRS(new Vector3(0.0f, 0.0f, 0.0f), rotation, new Vector3(1.0f, 1.0f, 1.0f));
        Vector3 afterLocalU = m.MultiplyPoint3x4(new Vector3(1.0f, 0.0f, 0.0f));
        Vector3 afterLocalV = m.MultiplyPoint3x4(new Vector3(0.0f, 1.0f, 0.0f));

        int ai = 0;
        for (int i = 0; i < furniture.Count; i++)
        {
            if (i == userConstraint || nearWallID[i] != -1)
            {
                furniture[i].transform.localPosition = furniture[i].transform.localPosition;
                furniture[i].transform.localEulerAngles = furniture[i].transform.localEulerAngles;
            }
            else
            {
                furniture[i].transform.localPosition =
                    furniture[userConstraint].transform.localPosition + transCalculatedByMatrix[ai].x * afterLocalU + transCalculatedByMatrix[ai].y * afterLocalV;
                furniture[i].transform.localEulerAngles = new Vector3(0.0f, 0.0f, OrientationCaluculatedByMatrix[ai] + furniture[userConstraint].transform.localEulerAngles.z);
                ai++;
            }
        }
    }

    //行列計算など最適化提案計算
    public void calcAfterGlobalTranslationAndOrientation(MyMatrix matrixManager, List<Room> roomManager)
    {
        //部屋判定
        List<Vector3> p = new List<Vector3>();
        Room room = new Room(p);
        Vector3 center = new Vector3();
        for (int i = 0; i < furniture.Count; i++)
        {
            center += furniture[i].transform.localPosition;
        }
        center /= furniture.Count;

        int roomIndex = -1;
        for (int i = 0; i < roomManager.Count; i++)
        {
            float deg = 0.0f;
            for (int j = 0; j < roomManager[i].cornerPoint.Count; j++)
            {
                Vector3 p1, p2;
                p1 = (roomManager[i].cornerPoint[j] - center).normalized;
                if (j + 1 >= roomManager[i].cornerPoint.Count) { p2 = (roomManager[i].cornerPoint[0] - center).normalized; }
                else { p2 = (roomManager[i].cornerPoint[j + 1] - center).normalized; }
                float dot = Vector3.Dot(p1, p2);
                if (dot <= -1.0f) { dot = -1.0f; }
                else if (dot >= 1.0f) { dot = 1.0f; }
                float cos = dot;
                deg += Mathf.Acos(cos) * 180.0f / Mathf.PI;
            }
            if (359.0f < deg && deg < 361.0f)
            {
                for (int j = 0; j < roomManager[i].cornerPoint.Count; j++)
                {
                    p.Add(roomManager[i].cornerPoint[j]);
                }
                room = new Room(p);
                roomIndex = i;
            }
        }
        Debug.Log("roomIndex = " + roomIndex);
        
        //行列の右辺
        List<Vector3> vectorBeforeLocalF = new List<Vector3>();
        List<float> vectorBeforeLocalTheta = new List<float>();
        Vector3 temp; float tempTheta;
        for (int i = 0; i < furniture.Count; i++)
        {
            temp = new Vector3();
            tempTheta = 0.0f;
            for (int j = 0; j < furniture.Count; j++)
            {
                temp += (beforeLocalTrans[i] - beforeLocalTrans[j]);
                tempTheta += (beforeLocalOrientation[i] - beforeLocalOrientation[j]);
            }
            vectorBeforeLocalF.Add(temp);
            vectorBeforeLocalTheta.Add(tempTheta);
        }

        //行列計算
        List<Vector3> a = new List<Vector3>(), b = new List<Vector3>();
        List<float> atheta = new List<float>(), btheta = new List<float>();
        for (int i = 0; i < furniture.Count; i++)
        {
            if (i != userConstraint)
            {
                b.Add(vectorBeforeLocalF[i]);
                btheta.Add(vectorBeforeLocalTheta[i]);
            }
        }
        for (int i = 0; i < b.Count; i++)
        {
            //自作逆行列を使って実装
            Vector3 aTemp = new Vector3();
            float athetaTemp = 0.0f;
            for (int j = 0; j < b.Count; j++)
            {
                aTemp += matrixManager.matAInverse[i, j] * b[j];
                athetaTemp += matrixManager.matAInverse[i, j] * btheta[j];
            }
            a.Add(aTemp);
            atheta.Add(athetaTemp);
        }

        //一度グローバルに戻す
        transformLocalToGlobalTranslationAndOrientation(a,atheta);

        //壁にはみ出した家具があるかチェックする
        int numOfOutFurniture = detectionInsideorNot(room);

        //もしはみ出した家具があればそれを壁に修正して再行列計算
        int N = furniture.Count - numOfOutFurniture - 1;
        if (numOfOutFurniture > 0 && N > -1)
        {
            matrixManager.matB = new float[N, N]; matrixManager.matBInverse = new float[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    matrixManager.matB[i, j] = -1.0f;
                    if (i == j)
                    {
                        matrixManager.matB[i, j] = furniture.Count - 1.0f;
                    }
                }
            }
            matrixManager.getInverseMatrix(N, matrixManager.matB, ref matrixManager.matBInverse);

            //はみ出した家具を修正したglobal座標をローカルに変えて
            List<int> nearWallID = new List<int>();
            int min_id = new int();
            float mindistance = float.MaxValue, distance;
            for (int i = 0; i < furniture.Count; i++)
            {
                if (state[i] != 0)
                {
                    for (int j = 0; j < room.cornerPoint.Count; j++)
                    {
                        float d, pij, pij1; Vector3 AB, AP;
                        AP = furniture[i].transform.localPosition - room.cornerPoint[j];
                        if (j + 1 >= room.cornerPoint.Count) { AB = room.cornerPoint[0] - room.cornerPoint[j]; }
                        else { AB = room.cornerPoint[j + 1] - room.cornerPoint[j]; }
                        d = Vector3.Cross(AP, AB).magnitude / AB.magnitude;
                        pij = Vector3.Distance(room.cornerPoint[j], furniture[i].transform.localPosition);
                        if (j + 1 >= room.cornerPoint.Count) { pij1 = Vector3.Distance(room.cornerPoint[0], furniture[i].transform.localPosition); }
                        else { pij1 = Vector3.Distance(room.cornerPoint[j + 1], furniture[i].transform.localPosition); }

                        float t = Vector3.Dot(AP, AB.normalized) / AB.magnitude;
                        if (0 < t && t < 1) { distance = d; }
                        else
                        {
                            if (pij < pij1) { distance = pij; }
                            else { distance = pij1; }
                        }

                        if (distance < mindistance)
                        {
                            mindistance = distance;
                            min_id = j;
                        }
                    }
                    nearWallID.Add(min_id);
                }
                else
                {
                    nearWallID.Add(-1);
                }
            }

            for (int i = 0; i < furniture.Count; i++)
            {
                if (nearWallID[i] == 0)
                {
                    furniture[i].transform.localPosition = new Vector3(room.cornerPoint[0].x, furniture[i].transform.localPosition.y, 0.0f);
                    if (!furniture[i].GetComponent<dragdrop>().bedFurniture)
                    {
                        furniture[i].transform.localEulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
                    }
                    //furniture[i].transform.localEulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
                }
                if (nearWallID[i] == 1)
                {
                    furniture[i].transform.localPosition = new Vector3(furniture[i].transform.localPosition.x, room.cornerPoint[1].y, 0.0f);
                    if (!furniture[i].GetComponent<dragdrop>().bedFurniture)
                    {
                        furniture[i].transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                    }
                    //furniture[i].transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                }
                if (nearWallID[i] == 2)
                {
                    furniture[i].transform.localPosition = new Vector3(room.cornerPoint[2].x, furniture[i].transform.localPosition.y, 0.0f);
                    if (!furniture[i].GetComponent<dragdrop>().bedFurniture)
                    {
                        furniture[i].transform.localEulerAngles = new Vector3(0.0f, 0.0f, 270.0f);
                    }
                    //furniture[i].transform.localEulerAngles = new Vector3(0.0f, 0.0f, 270.0f);
                }
                if (nearWallID[i] == 3)
                {
                    furniture[i].transform.localPosition = new Vector3(furniture[i].transform.localPosition.x, room.cornerPoint[3].y, 0.0f);
                    if (!furniture[i].GetComponent<dragdrop>().bedFurniture)
                    {
                        furniture[i].transform.localEulerAngles = new Vector3(0.0f, 0.0f, 180.0f);
                    }
                    //furniture[i].transform.localEulerAngles = new Vector3(0.0f, 0.0f, 180.0f);
                }
            }

            //if (nearWallID[1] == 0)
            //{
            //    furniture[1].transform.localPosition = new Vector3(room.cornerPoint[0].x, furniture[1].transform.localPosition.y, 0.0f);
            //    furniture[1].transform.localEulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
            //}
            //if (nearWallID[1] == 1)
            //{
            //    furniture[1].transform.localPosition = new Vector3(furniture[1].transform.localPosition.x, room.cornerPoint[1].y, 0.0f);
            //    furniture[1].transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
            //}
            //if (nearWallID[1] == 2)
            //{
            //    furniture[1].transform.localPosition = new Vector3(room.cornerPoint[2].x, furniture[1].transform.localPosition.y, 0.0f);
            //    furniture[1].transform.localEulerAngles = new Vector3(0.0f, 0.0f, 270.0f);
            //}
            //if (nearWallID[1] == 3)
            //{
            //    furniture[1].transform.localPosition = new Vector3(furniture[1].transform.localPosition.x, room.cornerPoint[3].y, 0.0f);
            //    furniture[1].transform.localEulerAngles = new Vector3(0.0f, 0.0f, 180.0f);
            //}


            List<Vector3> localTrans = new List<Vector3>();
            List<float> localOrientation = new List<float>();
            transformGlobalToLocalTranslationAndOrientation(ref localTrans, ref localOrientation);
            Vector3 tempLocalTrans = new Vector3();
            float tempLocalOrientation = new float();
            for (int i = 0; i < furniture.Count; i++)
            {
                if (nearWallID[i] != -1)
                {
                    tempLocalTrans += localTrans[i];
                    tempLocalOrientation += localOrientation[i];
                }
            }

            //行列計算
            a = new List<Vector3>(); b = new List<Vector3>();
            atheta = new List<float>(); btheta = new List<float>();
            for (int i = 0; i < furniture.Count; i++)
            {
                if (i != userConstraint && nearWallID[i] == -1)
                {
                    b.Add(vectorBeforeLocalF[i] + tempLocalTrans);
                    btheta.Add(vectorBeforeLocalTheta[i] + tempLocalOrientation);
                }
            }

            if (N == b.Count)
            {
                for (int i = 0; i < b.Count; i++)
                {
                    Vector3 aTemp = new Vector3();
                    float athetaTemp = 0.0f;
                    for (int j = 0; j < b.Count; j++)
                    {
                        aTemp += matrixManager.matBInverse[i, j] * b[j];
                        athetaTemp += matrixManager.matBInverse[i, j] * btheta[j];
                    }
                    a.Add(aTemp);
                    atheta.Add(athetaTemp);
                }
                transformLocalToGlobalTranslationAndOrientation(nearWallID, a, atheta);
            }

        }

        //最適化した後の各位置と回転を保存する！
        userTrans = new List<Vector3>();
        userOrientation = new List<float>();
        for (int i = 0; i < furniture.Count; i++)
        {
            userTrans.Add(furniture[i].transform.localPosition);
            userOrientation.Add(furniture[i].transform.localEulerAngles.z);
        }
    }

    int detectionInsideorNot(Room room)
    {
        int num = 0;
        for (int i = 0; i < furniture.Count; i++)
        {
            float deg = 0.0f;
            for (int j = 0; j < room.cornerPoint.Count; j++)
            {
                Vector3 p1, p2;
                p1 = room.cornerPoint[j] - furniture[i].transform.localPosition;
                if (j + 1 >= room.cornerPoint.Count) { p2 = room.cornerPoint[0] - furniture[i].transform.localPosition; }
                else { p2 = room.cornerPoint[j + 1] - furniture[i].transform.localPosition; }

                float cos = Vector3.Dot(p1, p2) / (p1.magnitude*p2.magnitude);
                deg += Mathf.Acos(cos) * 180.0f / Mathf.PI;
            }
            //内側判定
            if (359 < deg && deg < 361){ state[i] = 0; }
            //外側判定
            else{ state[i] = 1; num++; }
        }
        return num;
    }

    //Conversationの最急降下法のプログラム
    public void calcConversationGradientSolver()
    {
        int loop = 1000, N = furniture.Count;
        float alphaTrans = 0.01f, alphaOrientation = 0.1f;
        float[] p, gradE;
        p = new float[3 * N];
        gradE = new float[3 * N];

        //pに初期位置，初期状態を代入
        for (int i = 0; i < N; i++)
        {
            p[3 * i] = furniture[i].transform.localPosition.x;
            p[3 * i + 1] = furniture[i].transform.localPosition.y;
            p[3 * i + 2] = furniture[i].transform.localEulerAngles.z;
        }

        for (int i = 0; i < loop; i++)
        {
            gradE = new float[3 * N];
            //微分の計算
            for (int k = 0; k < N; k++)
            {
                Vector3 df = dEdfk_conv(k, N, p);
                float dtheta = dEdthetak_conv(k, N, p);
                gradE[3 * k] = df.x;
                gradE[3 * k + 1] = df.y;
                gradE[3 * k + 2] = dtheta;
            }

            //pの更新
            for (int k = 0; k < N; k++)
            {
                if (k == userConstraint)
                {

                }
                else if (!furniture[k].GetComponent<dragdrop>().directionalFurniture)
                {

                }
                else
                {
                    p[3 * k] = p[3 * k] - alphaTrans * gradE[3 * k];
                    p[3 * k + 1] = p[3 * k + 1] - alphaTrans * gradE[3 * k + 1];
                    p[3 * k + 2] = p[3 * k + 2] - alphaOrientation * gradE[3 * k + 2];
                }
            }
        }

        ////最終結果を家具の情報に代入
        //for (int i = 0; i < N; i++)
        //{
        //    furniture[i].transform.localPosition = new Vector3(p[3 * i], p[3 * i + 1], 0.0f);
        //    furniture[i].transform.localEulerAngles = new Vector3(0.0f, 0.0f, p[3 * i + 2]);
        //}

        //最適化した後の各位置と回転を保存する
        conversationTrans = new List<Vector3>();
        conversationOrientation = new List<float>();
        for (int i = 0; i < furniture.Count; i++)
        {
            //conversationTrans.Add(furniture[i].transform.localPosition);
            //conversationOrientation.Add(furniture[i].transform.localEulerAngles.z);
            conversationTrans.Add(new Vector3(p[3 * i], p[3 * i + 1], 0.0f));
            conversationOrientation.Add(p[3 * i + 2]);
        }
    }

    float cosPhi_ij(int i, int j, float[] p)
    {
        Vector3 fj, fi;
        fj = new Vector3(p[3 * j], p[3 * j + 1], 0.0f);
        fi = new Vector3(p[3 * i], p[3 * i + 1], 0.0f);
        //Vector3 f = furniture[j].transform.localPosition - furniture[i].transform.localPosition;
        Vector3 f = fj - fi;
        //float angle = (furniture[i].transform.localEulerAngles.z + 90.0f) * Mathf.PI / 180.0f;
        float angle = (p[3 * i + 2] + 90.0f) * Mathf.PI / 180.0f;
        Vector3 orientation = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0.0f);
        float distance = f.magnitude;
        float cos = Vector3.Dot(f, orientation) / distance;
        return cos;
    }

    Vector3 dEdfk_conv(int k, int N, float[] p)
    {
        Vector3 sum = new Vector3(0.0f, 0.0f, 0.0f);
        for (int i = 0; i < N; i++)
        {
            float angle_i, angle_k, distance_ik, distance_ki;
            Vector3 fi, fk, fik, fki, temp = new Vector3(0.0f, 0.0f, 0.0f), tempA = new Vector3(), tempB = new Vector3();
            fi = new Vector3(p[3 * i], p[3 * i + 1], 0.0f);
            fk = new Vector3(p[3 * k], p[3 * k + 1], 0.0f);
            fik = fk - fi;
            fki = fi - fk;
            distance_ik = fik.magnitude;
            distance_ki = fki.magnitude;
            angle_i = (p[3 * i + 2] + 90.0f) * Mathf.PI / 180.0f;
            angle_k = (p[3 * k + 2] + 90.0f) * Mathf.PI / 180.0f;
            if(i == k)
            {
                temp = new Vector3(0.0f, 0.0f, 0.0f);
            }
            else
            {
                tempA = (new Vector3(Mathf.Cos(angle_i), Mathf.Sin(angle_i), 0.0f) + fik) / distance_ik;
                tempB = (new Vector3((-1.0f)*Mathf.Cos(angle_k), (-1.0f)*Mathf.Sin(angle_k), 0.0f) + fki) / distance_ki;
                temp = tempA * (cosPhi_ij(k, i, p) + 1.0f) + (cosPhi_ij(i, k, p) + 1.0f) * tempB;
            }
            sum += temp;
        }
        return -2.0f * sum;
    }

    float dEdthetak_conv(int k, int N, float[] p)
    {
        float sum = 0.0f;
        for (int i = 0; i < N; i++)
        {
            float angle_i, angle_k, distance_ik, distance_ki, temp = 0.0f, tempA = 0.0f;
            Vector3 fi, fk, fik, fki;
            fi = new Vector3(p[3 * i], p[3 * i + 1], 0.0f);
            fk = new Vector3(p[3 * k], p[3 * k + 1], 0.0f);
            fik = fk - fi;
            fki = fi - fk;
            distance_ik = fik.magnitude;
            distance_ki = fki.magnitude;
            angle_i = (p[3 * i + 2] + 90.0f) * Mathf.PI / 180.0f;
            angle_k = (p[3 * k + 2] + 90.0f) * Mathf.PI / 180.0f;
            if(i == k)
            {
                temp = 0.0f;
            }
            else
            {
                Vector3 a = new Vector3((-1.0f) * Mathf.Sin(angle_k), Mathf.Cos(angle_k), 0.0f);
                tempA = Vector3.Dot(fki, a) / distance_ki;
                temp = (cosPhi_ij(i, k, p) + 1.0f) * tempA;
            }
            sum += temp;
        }
        return -2.0f * sum;
    }

    //Alignmentの最急降下法
    public void calcAlignmentGradientSolver()
    {
        int loop = 1000, N = furniture.Count;
        float alphaOrientation = 0.01f;
        float[] p, gradE;
        p = new float[3 * N];
        gradE = new float[3 * N];

        //pに初期位置，初期状態を代入
        for (int i = 0; i < N; i++)
        {
            p[3 * i] = furniture[i].transform.localPosition.x;
            p[3 * i + 1] = furniture[i].transform.localPosition.y;
            p[3 * i + 2] = furniture[i].transform.localEulerAngles.z;
        }

        for (int i = 0; i < loop; i++)
        {
            gradE = new float[3 * N];
            //微分の計算
            for (int k = 0; k < N; k++)
            {
                float dtheta = dEdthetak_align(k, N, p);
                gradE[3 * k] = 0.0f;
                gradE[3 * k + 1] = 0.0f;
                gradE[3 * k + 2] = dtheta;
            }

            //pの更新
            for (int k = 0; k < N; k++)
            {
                if (k == userConstraint)
                {

                }
                else
                {
                    //p[3 * k] = p[3 * k] - gradE[3 * k];
                    //p[3 * k + 1] = p[3 * k + 1] - gradE[3 * k + 1];
                    p[3 * k + 2] = p[3 * k + 2] - alphaOrientation * gradE[3 * k + 2];
                }
            }
        }

        ////最終結果を家具の情報に代入
        //for (int i = 0; i < N; i++)
        //{
        //    furniture[i].transform.localPosition = new Vector3(p[3 * i], p[3 * i + 1], 0.0f);
        //    furniture[i].transform.localEulerAngles = new Vector3(0.0f, 0.0f, p[3 * i + 2]);
        //}

        //最適化した後の各位置と回転を保存する
        alignmentTrans = new List<Vector3>();
        alignmentOrientation = new List<float>();
        for (int i = 0; i < furniture.Count; i++)
        {
            //alignmentTrans.Add(furniture[i].transform.localPosition);
            //alignmentOrientation.Add(furniture[i].transform.localEulerAngles.z);
            alignmentTrans.Add(new Vector3(p[3 * i], p[3 * i + 1], 0.0f));
            alignmentOrientation.Add(p[3 * i + 2]);
        }

    }

    float dEdthetak_align(int k, int N, float[] p)
    {
        float sum = 0.0f;
        for (int i = 0; i < N; i++)
        {
            float angle_i = 0.0f, angle_k = 0.0f, angle = 0.0f;
            angle_i = (p[3 * i + 2] + 90.0f) * Mathf.PI / 180.0f;
            angle_k = (p[3 * k + 2] + 90.0f) * Mathf.PI / 180.0f;
            angle = angle_i - angle_k;
            sum += Mathf.Sin(4.0f * angle);
        }
        return -8.0f * sum;
    }

    //Emphasisの最急降下法
    public void calcEmphasisGradientSolver(Vector3 emphasisPoint)
    {
        int loop = 1000, N = furniture.Count;
        float alphaTrans = 0.01f;
        float alphaOrientation = 0.1f;
        float[] p, gradE;
        p = new float[3 * N];
        gradE = new float[3 * N];

        //pに初期位置，初期状態を代入
        for (int i = 0; i < N; i++)
        {
            p[3 * i] = furniture[i].transform.localPosition.x;
            p[3 * i + 1] = furniture[i].transform.localPosition.y;
            p[3 * i + 2] = furniture[i].transform.localEulerAngles.z;
        }

        for (int i = 0; i < loop; i++)
        {
            gradE = new float[3 * N];
            //微分の計算
            for (int k = 0; k < N; k++)
            {
                Vector3 df = dEdfk_Emphasis(k, p);
                float dtheta = dEdthetak_Emphasis(k, emphasisPoint, p);
                gradE[3 * k] = df.x;
                gradE[3 * k + 1] = df.y;
                gradE[3 * k + 2] = dtheta;
            }

            //pの更新
            for (int k = 0; k < N; k++)
            {
                if (k == userConstraint)
                {
                    p[3 * k + 2] = p[3 * k + 2] - alphaOrientation * gradE[3 * k + 2];
                }
                else if (!furniture[k].GetComponent<dragdrop>().directionalFurniture)
                {

                }
                else
                {
                    //p[3 * k] = p[3 * k] - alphaTrans * gradE[3 * k];
                    //p[3 * k + 1] = p[3 * k + 1] - alphaTrans * gradE[3 * k + 1];
                    p[3 * k + 2] = p[3 * k + 2] - alphaOrientation * gradE[3 * k + 2];
                }
            }
        }

        ////最終結果を家具の情報に代入
        //for (int i = 0; i < N; i++)
        //{
        //    furniture[i].transform.localPosition = new Vector3(p[3 * i], p[3 * i + 1], 0.0f);
        //    furniture[i].transform.localEulerAngles = new Vector3(0.0f, 0.0f, p[3 * i + 2]);
        //}

        //最適化した後の各位置と回転を保存する
        emphasisTrans = new List<Vector3>();
        emphasisOrientation = new List<float>();
        for (int i = 0; i < furniture.Count; i++)
        {
            //emphasisTrans.Add(furniture[i].transform.localPosition);
            //emphasisOrientation.Add(furniture[i].transform.localEulerAngles.z);
            emphasisTrans.Add(new Vector3(p[3 * i], p[3 * i + 1], 0.0f));
            emphasisOrientation.Add(p[3 * i + 2]);
        }

    }

    float dEdthetak_Emphasis(int k, Vector3 emphasisPoint, float[] p)
    {
        float result = 0.0f;
        Vector3 d = emphasisPoint - new Vector3(p[3 * k], p[3 * k + 1], 0.0f);
        d = d.normalized;
        float angle_k = (p[3 * k + 2] + 90.0f) * Mathf.PI / 180.0f;
        result = d.x * Mathf.Sin(angle_k) - d.y * Mathf.Cos(angle_k);
        return result;
    }

    Vector3 dEdfk_Emphasis(int k, float[] p)
    {
        Vector3 result = new Vector3();
        float angle_k = (p[3 * k + 2] + 90.0f) * Mathf.PI / 180.0f;
        result = new Vector3(Mathf.Cos(angle_k), Mathf.Sin(angle_k), 0.0f);
        return result;
    }

    public void blendOptimizedPlacement(float conversationSliderValue, float alignmentSliderValue, float emphasisSliderValue)
    {
        for (int i = 0; i < furniture.Count; i++)
        {
            Vector3 resultTrans = new Vector3(0.0f, 0.0f, 0.0f); float resultOrientation = 0.0f;

            resultTrans = (1 - conversationSliderValue) * userTrans[i] + conversationSliderValue * conversationTrans[i];
            resultOrientation = (1 - conversationSliderValue) * userOrientation[i] + conversationSliderValue * conversationOrientation[i];

            resultTrans = (1 - alignmentSliderValue) * resultTrans + alignmentSliderValue * alignmentTrans[i];
            resultOrientation = (1 - alignmentSliderValue) * resultOrientation + alignmentSliderValue * alignmentOrientation[i];

            resultTrans = (1 - emphasisSliderValue) * resultTrans + emphasisSliderValue * emphasisTrans[i];
            resultOrientation = (1 - emphasisSliderValue) * resultOrientation + emphasisSliderValue * emphasisOrientation[i];

            furniture[i].transform.localPosition = new Vector3(resultTrans.x, resultTrans.y, resultTrans.z);
            furniture[i].transform.localEulerAngles = new Vector3(0.0f, 0.0f, resultOrientation);
        }
    }


}
