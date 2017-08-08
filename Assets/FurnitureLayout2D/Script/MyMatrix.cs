using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMatrix : MonoBehaviour {
    public float[,] matA, matAInverse;
    public float[,] matB, matBInverse;


    public MyMatrix(int N)
    {
        matA = new float[N - 1, N - 1];
        for (int i = 0; i < N-1; i++)
        {
            for (int j = 0; j < N-1; j++)
            {
                if(i== j)
                {
                    matA[i, j] = N - 1;
                }
                else
                {
                    matA[i, j] = -1.0f;
                }
            }
        }
        matAInverse = new float[N - 1, N - 1];
    }

    //掃き出し法で逆行列求めた（現状ならこれぐらいでも大丈夫）
    public void getInverseMatrix(int N, float[,] a, ref float[,] aInverse)
    {
        float buf;

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                aInverse[i, j] = (i == j) ? 1.0f : 0.0f;
            }
        }

        for (int i = 0; i < N; i++)
        {
            buf = 1 / a[i, i];
            for (int j = 0; j < N; j++)
            {
                a[i, j] *= buf;
                aInverse[i, j] *= buf;
            }
            for (int j = 0; j < N; j++)
            {
                if(i!= j)
                {
                    buf = a[j, i];
                    for (int k = 0; k < N; k++)
                    {
                        a[j, k] -= a[i, k] * buf;
                        aInverse[j, k] -= aInverse[i,k] * buf;
                    }
                }
            }
        }
    }

    //LU分解で逆行列求める．（作りたいけど．．．）
    public void getLUInverseMatrix(int N, float[,] a, ref float[,] aInverse)
    {

    }


}
