using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : MonoBehaviour
{
    public Vector3[] pos = new Vector3[3];

    public void SetTriangle(Vector3 posF, Vector3 posS, Vector3 posT)
    {
        pos[0] = posF;
        pos[1] = posS;
        pos[2] = posT;
    }
    
    private void Update()
    {
        DebugX.DrawTriangle(transform.position + pos[0], transform.position + pos[1], transform.position + pos[2], Color.green);
    }


}
