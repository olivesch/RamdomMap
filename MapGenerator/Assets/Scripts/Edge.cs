using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{
    public Vector2 aVer = Vector2.zero;
    public Vector2 bVer = Vector2.zero;

    public Edge(Vector2 a, Vector2 b)
    {
        aVer = a;
        bVer = b;
    }

    void DrawLine()
    {
        Debug.DrawLine(aVer, bVer, Color.red, 100f);
    }
}
