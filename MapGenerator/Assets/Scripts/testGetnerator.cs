using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class testGetnerator : MonoBehaviour
{
    public GameObject room;
    public int roomCount;
    public int selectRoomCount;
    public GameObject infTriangle;

    List<GameObject> roomList;
    public List<GameObject> triangleList;
    List<Vector2> vertexList;
    List<int> selectRoomList;

    private void Start()
    {
        Time.timeScale = 0.01f;


        roomList = new List<GameObject>();
        triangleList = new List<GameObject>();
        vertexList = new List<Vector2>();
        selectRoomList = new List<int>();

        GameObject a = Instantiate(room);

        a.GetComponent<CellRoom>().rect = new Rect(-1, -1, 1, 1);
        roomList.Add(a);

        for(int i = roomCount; i > 0; i--)
        {
            GameObject emptyRoom = Instantiate(room);
            emptyRoom.GetComponent<CellRoom>().rect = new Rect((Mathf.Cos(Random.Range(0, 360)) * Random.Range(1, 3)),
               (Mathf.Sin(Random.Range(0, 360)) * Random.Range(1, 3)),
                ((int)Random.Range(1, 3)),
                ((int)Random.Range(1, 3)));
            
            roomList.Add(emptyRoom);
        }
        StartCoroutine(SeparateRoom());
    }

    public static CellRoom FindRoom(List<GameObject> room, Vector2 pos)
    {
        for (int i = 0; i < room.Count; i++)
        {
            CellRoom cellRoom = room[i].GetComponent<CellRoom>();
            if (new Vector2(cellRoom.rect.x + cellRoom.rect.width / 2, cellRoom.rect.y + cellRoom.rect.height / 2) == pos)
                return cellRoom;
        }
        return null;
    }

    public void ConnectRoom(Triangle tri, List<GameObject> room)
    {
        for (int i = 0; i < room.Count; i++)
        {
            CellRoom cellRoom = room[i].GetComponent<CellRoom>();

            if ((Vector2)tri.pos[0] == new Vector2(cellRoom.rect.x + cellRoom.rect.width / 2, cellRoom.rect.y + cellRoom.rect.height / 2))
            {
                    cellRoom.connectRoom.Add(FindRoom(room, tri.pos[1]));
                    cellRoom.connectRoom.Add(FindRoom(room, tri.pos[2]));
            }

            if ((Vector2)tri.pos[1] == new Vector2(cellRoom.rect.x + cellRoom.rect.width / 2, cellRoom.rect.y + cellRoom.rect.height / 2))
            {
                    cellRoom.connectRoom.Add(FindRoom(room, tri.pos[0]));
                    cellRoom.connectRoom.Add(FindRoom(room, tri.pos[2]));
            }

            if ((Vector2)tri.pos[2] == new Vector2(cellRoom.rect.x + cellRoom.rect.width / 2, cellRoom.rect.y + cellRoom.rect.height / 2))
            {
                    cellRoom.connectRoom.Add(FindRoom(room, tri.pos[1]));
                    cellRoom.connectRoom.Add(FindRoom(room, tri.pos[0]));
            }
        }

    }
    void MSTPrim()
    {
        //선택된 방 외의 방들을 제거
        for(int i = 0; i < roomList.Count; i++)
        {
            if(roomList[i].GetComponent<CellRoom>().isSelect == false)
            {
                Destroy(roomList[i]);
                roomList.Remove(roomList[i]);
                i--;
            }
        }

        //방들 마다 이어줌
        for (int i = 0; i < triangleList.Count; i++)
        {
            ConnectRoom(triangleList[i].GetComponent<Triangle>(), roomList);
        }

        //이어진 방들 중 중복 제거
        for(int i = 0; i< roomList.Count; i++)
        {
            roomList[i].GetComponent<CellRoom>().connectRoom = roomList[i].GetComponent<CellRoom>().connectRoom.Distinct().ToList();
        }

        //Prim 알고리즘
        int roomNum = Random.Range(0, roomList.Count - 1);
        List<CellRoom> primRoom = new List<CellRoom>();
        primRoom.Add(roomList[roomNum].GetComponent<CellRoom>());
        while(true)
        {
            if (primRoom.Count == roomList.Count)
                break;

            float minCost = 1000000;
            int fRoom = 0;
            int sRoom = 0;
            for (int i = 0; i < primRoom.Count; i++)
            {
                for (int j = 0; j < primRoom[i].connectRoom.Count; j++)
                {
                    if (Mathf.Min(minCost, Vector2.Distance(primRoom[i].rect.position, primRoom[i].connectRoom[j].rect.position)) == minCost) ;
                    else
                    {
                        if (primRoom[i].connectRoom[j].isConnect == true)
                            continue;
                        minCost = Vector2.Distance(primRoom[i].rect.position, primRoom[i].connectRoom[j].rect.position);
                        
                        fRoom = i;
                        sRoom = j;
                    }
                }
            }

            primRoom.Add(primRoom[fRoom].connectRoom[sRoom]);
            primRoom[fRoom].isConnect = true;
            primRoom[fRoom].connectRoom[sRoom].isConnect = true;

            //서로 결과 간선에 연결
            primRoom[fRoom].resultConnectRoom.Add(primRoom[fRoom].connectRoom[sRoom]);
            primRoom[fRoom].connectRoom[sRoom].resultConnectRoom.Add(primRoom[fRoom]);
            
            //연결된 간선은 삭제한다
            primRoom[fRoom].connectRoom.Remove(primRoom[fRoom].connectRoom[sRoom]);

            for (int i = 0; i < primRoom[fRoom].connectRoom.Count; i++)
            {
                if((primRoom[fRoom].connectRoom[i].rect.position) == primRoom[fRoom].rect.position)
                {
                    primRoom[fRoom].connectRoom.Remove((primRoom[fRoom].connectRoom[i]));
                    break;
                }
            
            }

        }
        

    }

    void DelaunayTri()
    {
        for (int i = 0; i < vertexList.Count - 3; i++)
        {

            List<Edge> edgeSet = new List<Edge>();
            for (int j = 0; j < triangleList.Count; j++)
            {

                Triangle tri = triangleList[j].GetComponent<Triangle>(); //infTriangle.GetComponent<Triangle>();

                if (DebugX.isCircumCircleColVert(tri, vertexList[i]))
                {
                    bool isABEdge = false, isBCEdge = false, isCAEdge = false;
                    for (int k = 0; k < edgeSet.Count; k++)
                    {

                        if (edgeSet[k].aVer == (Vector2)tri.pos[0] && edgeSet[k].bVer == (Vector2)tri.pos[1]
                            || (edgeSet[k].bVer == (Vector2)tri.pos[0] && edgeSet[k].aVer == (Vector2)tri.pos[1]))
                        {
                            isABEdge = true;
                            edgeSet.Remove(edgeSet[k]);
                            k--;
                            continue;
                        }

                        if (edgeSet[k].aVer == (Vector2)tri.pos[1] && edgeSet[k].bVer == (Vector2)tri.pos[2]
                            || (edgeSet[k].bVer == (Vector2)tri.pos[1] && edgeSet[k].aVer == (Vector2)tri.pos[2]))
                        {
                            isBCEdge = true;
                            edgeSet.Remove(edgeSet[k]);
                            k--;
                            continue;
                        }

                        if ((edgeSet[k].aVer == (Vector2)tri.pos[2] && edgeSet[k].bVer == (Vector2)tri.pos[0])
                            || (edgeSet[k].bVer == (Vector2)tri.pos[2] && edgeSet[k].aVer == (Vector2)tri.pos[0]))
                        {
                            isCAEdge = true;
                            edgeSet.Remove(edgeSet[k]);
                            k--;
                            continue;
                        }
                    }
                    if (!isABEdge)
                        edgeSet.Add(new Edge(tri.pos[0], tri.pos[1]));
                    if (!isBCEdge)
                        edgeSet.Add(new Edge(tri.pos[1], tri.pos[2]));
                    if (!isCAEdge)
                        edgeSet.Add(new Edge(tri.pos[2], tri.pos[0]));

                    Destroy(triangleList[j]);
                    triangleList.Remove(triangleList[j]);

                    j--;
                    continue;
                }

            }
            foreach (Edge edge in edgeSet)
            {
                if (DebugX.Vec2Cross((edge.bVer - edge.aVer), (vertexList[i] - edge.aVer)) == 0) continue;
                GameObject tempTri = new GameObject();
                tempTri.AddComponent<Triangle>();
                tempTri.GetComponent<Triangle>().SetTriangle(edge.aVer, edge.bVer, vertexList[i]);
                triangleList.Add(tempTri);
            }
        }

        for (int i = 0; i < triangleList.Count; i++)
        {
            Triangle cur = triangleList[i].GetComponent<Triangle>();

            if (vertexList.IndexOf(cur.pos[0]) >= vertexList.Count - 3 ||
            vertexList.IndexOf(cur.pos[1]) >= vertexList.Count - 3 ||
            vertexList.IndexOf(cur.pos[2]) >= vertexList.Count - 3)
            { // n ~ n+2 의 정점을 사용하는 삼각형은 모두 제거.

                GameObject objTri = triangleList[i];
                triangleList[i] = triangleList[triangleList.Count - 1];
                triangleList[triangleList.Count - 1] = objTri;


                Destroy(triangleList[triangleList.Count - 1]);
                triangleList.Remove(triangleList[triangleList.Count - 1]);


                i--;
                continue;
            }
        }

        MSTPrim();


    }

    IEnumerator SelectRedRoom()
    {
        while(true)
        {
            for (int i = 0; i < selectRoomCount; i++)
            {
                bool isHave = false;
                int selectRoomNum = Random.Range(0, roomList.Count - 1);
                if(roomList[selectRoomNum].GetComponent<CellRoom>().isSelect)
                {
                    i--;
                    continue;
                }
                //bool isContinue = false; 
                //for(int j = 0; j < selectRoomList.Count; j++)
                //{
                //    if(Vector2.Distance(new Vector2(roomList[selectRoomList[j]].GetComponent<CellRoom>().rect.x,
                //        roomList[selectRoomList[j]].GetComponent<CellRoom>().rect.y),
                //        new Vector2(roomList[selectRoomNum].GetComponent<CellRoom>().rect.x, 
                //        roomList[selectRoomNum].GetComponent<CellRoom>().rect.y)) <= 10f)
                //    {
                //        isContinue = true;
                //    }
                //}
                //if(isContinue)
                //{
                //    i--;
                //    continue;
                //}


                selectRoomList.Add(selectRoomNum);
                vertexList.Add(new Vector2(roomList[selectRoomNum].GetComponent<CellRoom>().rect.x + roomList[selectRoomNum].GetComponent<CellRoom>().rect.width / 2
                    , roomList[selectRoomNum].GetComponent<CellRoom>().rect.y + roomList[selectRoomNum].GetComponent<CellRoom>().rect.height / 2));
                roomList[selectRoomNum].GetComponent<CellRoom>().isSelect = true;
                roomList[selectRoomNum].name = "SelectRoom";
                
            }
            yield return null;
            break;

        }
        GameObject tri = Instantiate(infTriangle);
        float angle = Random.Range(0, 360);
        tri.GetComponent<Triangle>().SetTriangle(
            new Vector3( 0 + Mathf.Cos(angle) * 40, 0 + Mathf.Sin(angle) * 40, 0),
            new Vector3(0 + Mathf.Cos(angle + 90) * 40, 0 + Mathf.Sin(angle + 90) * 40, 0),
            new Vector3(0 + Mathf.Cos(angle + 180) * 40, 0 + Mathf.Sin(angle + 180) * 40, 0)
            );
        
        vertexList.Add((Vector2)tri.GetComponent<Triangle>().pos[0]);
        vertexList.Add((Vector2)tri.GetComponent<Triangle>().pos[1]);
        vertexList.Add((Vector2)tri.GetComponent<Triangle>().pos[2]);
        triangleList.Add(tri);
        infTriangle = tri;
        DelaunayTri();
    }

    IEnumerator SeparateRoom()
    {
        bool isCollision;
        //방을 나누는 기준은 최소한의 이동으로 충돌없이 분리하는것
        do
        {
            isCollision = false;
            for (int i = 0; i < roomList.Count; i++)
            {
                CellRoom aR = roomList[i].GetComponent<CellRoom>();
                for (int j = i + 1; j < roomList.Count; j++) // <- 이거만 이해 못하겠음
                {
                    //if (roomList[i] == roomList[j])
                    //    continue;

                    CellRoom bR = roomList[j].GetComponent<CellRoom>();
                    if (aR.rect.Overlaps(bR.rect))
                    {
                        //aR.rect.x = (int)aR.rect.x;
                        //aR.rect.y = (int)aR.rect.y;
                        //bR.rect.x = (int)bR.rect.x;
                        //bR.rect.y = (int)bR.rect.y;


                        isCollision = true;
                        //X, Y의 최소 충돌값을 찾는다.
                        float minDirX = Mathf.Min(aR.GetRight() - bR.GetLeft(), aR.GetLeft() - bR.GetRight());
                        float minDirY = Mathf.Min(aR.GetBottom() - bR.GetTop(), aR.GetTop() - bR.GetBottom());

                        minDirX = Mathf.Floor(minDirX);
                        minDirY = Mathf.Floor(minDirY);


                        if (Mathf.Abs(minDirX) < Mathf.Abs(minDirY))
                        {
                            //최소 X가 Y보다 더 적으면 X값을 이동
                            aR.rect.x += (-minDirX / 2);
                            bR.rect.x += minDirX - (minDirX / 2);
                        }
                        else
                        {
                            //최소 Y가 X보다 더 적으면 Y값을 이동
                            aR.rect.y += (-minDirY / 2);
                            bR.rect.y += minDirY - (minDirY / 2);
                        }
                    }

                }
            }
            yield return new WaitForSeconds(0.001f);
        } while (isCollision);

        
        StartCoroutine(SelectRedRoom());
    }


}
