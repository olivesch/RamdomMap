using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellRoom : MonoBehaviour
{
    public Rect rect;
    public List<CellRoom> resultConnectRoom;
    public List<CellRoom> connectRoom;
    public bool isSelect = false;
    public bool isConnect = false;

    private void Start()
    {
        connectRoom = new List<CellRoom>();
    }

    private void Update()
    {

        transform.position = new Vector2(rect.x, rect.y);
        if(isSelect)
            DebugX.DrawRect(rect, Color.red);
        else
            DebugX.DrawRect(rect, Color.blue);

    }
    private void FixedUpdate()
    {

        for (int i = 0; i < resultConnectRoom.Count; i++)
        {
            Debug.DrawLine(new Vector3(rect.x + rect.width / 2, rect.y + rect.height / 2, 10), 

                new Vector3(resultConnectRoom[i].rect.x + resultConnectRoom[i].rect.width / 2, 
                resultConnectRoom[i].rect.y + resultConnectRoom[i].rect.height / 2, 10),

                Color.yellow);
        }
    }

    public float GetRight()
    {
        return rect.x + rect.width;
    }

    public float GetLeft()
    {
        return rect.x;
    }

    public float GetTop()
    {
        return rect.y + rect.height;
    }

    public float GetBottom()
    {
        return rect.y;
    }
}
