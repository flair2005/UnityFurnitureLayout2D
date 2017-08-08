using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
    public List<Vector3> cornerPoint;
    public List<Vector3> normal;
    public List<int> wallID;

    public Room(List<Vector3> p)
    {
        cornerPoint = new List<Vector3>();
        normal = new List<Vector3>();
        for (int i = 0; i < p.Count; i++)
        {
            cornerPoint.Add(p[i]);
        }

        for (int i = 0; i < p.Count; i++)
        {
            int i0 = i, i1 = i+1;
            if(i1 == p.Count) { i1 = 0; }
            Vector3 d = cornerPoint[i1] - cornerPoint[i0];
            normal.Add(new Vector3(-d.y, d.x, 0.0f));
        }
    }

}
