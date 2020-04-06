﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour
{
    private IEnumerator MoveCorotine;
    private List<HexagonControl> ListPoints = new List<HexagonControl>();//пройденные вершины 


    [SerializeField]
    private float _speed;
    //private int _namberPoint;
    private List<HexagonControl> _listVertex;
    bool GG;
    [System.NonSerialized]
    public int HexagonRow = 0, HexagonColumn = 0;

    void Start()
    {
        MoveCorotine = Movement();
        if (_listVertex.Count <= 0)
        {
            Debug.LogError("The lack of vertex in the hero");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GG = !GG;
        }

    }
    public HexagonControl FieldPosition()//гексагон к которому принадлежит герой
    {
        bool elevation = gameObject.layer != 8;
        List<RaycastHit2D> hit2Ds = new List<RaycastHit2D>();
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        Physics2D.CircleCast(transform.position, 1.7f, transform.position - transform.position, contactFilter2D, hit2Ds);
        HexagonControl hexagonControl = null;//нужный 6-ти угольник  
        float Magnitude = 0;

        for (int i = 0; i < hit2Ds.Count; i++)
        {
            var getHex = hit2Ds[i].collider.GetComponent<HexagonControl>();
            if (getHex.FreedomTestType(elevation))
            {
                if (hexagonControl == null)
                {
                    Magnitude = (new Vector2(hit2Ds[i].transform.position.x, hit2Ds[i].transform.position.y) - new Vector2(transform.position.x, transform.position.y)).magnitude;
                    hexagonControl = getHex;
                }

                if (Magnitude > (new Vector2(hit2Ds[i].transform.position.x, hit2Ds[i].transform.position.y) - new Vector2(transform.position.x, transform.position.y)).magnitude)
                {
                    Magnitude = (new Vector2(hit2Ds[i].transform.position.x, hit2Ds[i].transform.position.y) - new Vector2(transform.position.x, transform.position.y)).magnitude;
                    hexagonControl = getHex;
                }
            }
        }
        return hexagonControl;
    }
    private IEnumerator Movement()//коротина движения
    {
        List<HexagonControl> PointList = new List<HexagonControl>();
        PointList.AddRange(ListPoints);
        ListPoints.Clear();
        while (PointList.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, PointList[0].transform.position, _speed);
            Vector2 positionMain = transform.position;
            Vector2 positionCurrent = PointList[0].transform.position;
            if (PointList[0].TypeHexagon == 2 && gameObject.layer == 8)
            {
                gameObject.layer = 11;
            }
            else if (PointList[0].TypeHexagon == 0 && gameObject.layer == 11)
            {
                gameObject.layer = 8;
            }
            if ((positionCurrent - positionMain).magnitude <= 0.001f)
            {
                PointList.Remove(PointList[0]);
            }
            yield return new WaitForSeconds(0.02f);
        }

    }
    private List<HexagonControl> SearchForAWay(HexagonControl hexagon, Transform startingPoint, bool elevation)//возврашет все вершины по которым надо пройти 
    {
        List<HexagonControl> ListOfNecessaryVertices = new List<HexagonControl>();
        List<HexagonControl> ListHexgon = new List<HexagonControl>();

        List<HexagonControl> CollisionHexagon = new List<HexagonControl>();
        if (elevation)
        {
            if (!MapControlStatic.CollisionCheckElevation(startingPoint.position, hexagon.transform.position, elevation))
            {
                Debug.Log(23);
                ListHexgon.Add(FieldPosition());
                ListHexgon.AddRange(_listVertex);

                ListHexgon.Add(hexagon);
                ListOfNecessaryVertices.AddRange(BreakingTheDeadlock(ListHexgon));
            }
            else
            {
                ListOfNecessaryVertices.Add(hexagon);
            }
        }
        else
        {
            if (!MapControlStatic.CollisionCheck(startingPoint.position, hexagon.transform.position, elevation))
            {
                Debug.Log(1);
                //IsStraightWay = false;
                ListHexgon.Add(FieldPosition());
                ListHexgon.AddRange(_listVertex);

                ListHexgon.Add(hexagon);
                ListOfNecessaryVertices.AddRange(BreakingTheDeadlock(ListHexgon));
            }
            else
            {
                ListOfNecessaryVertices.Add(hexagon);
            }
        }

        return ListOfNecessaryVertices;
    }
    private List<HexagonControl> BreakingTheDeadlock(List<HexagonControl> listHexagons)//выстравивает пути обхода
    {
        AlgorithmDijkstra algorithmDijkstra = new AlgorithmDijkstra();
        List<Node> nodesList = algorithmDijkstra.Dijkstra(CreatingEdge(listHexagons, true));

        List<HexagonControl> ListVertex = new List<HexagonControl>();

        for (int i = 1; i < nodesList.Count; i++)
        {
            ListVertex.Add(nodesList[i].NodeHexagon);
        }

        return ListVertex;
    }
    private Graph CreatingEdge(List<HexagonControl> listHexagons, bool IsConsiderEnemy)
    {
        var graph = new Graph(listHexagons);
        var DictionaryEnemyVertex = new Dictionary<int, List<HexagonControl>>();

        //Debug.Log(graph.Length);
        for (int i = 0; i < graph.Length - 1; i++)
        {
            bool IsElevation = false;

            for (int j = i + 1; j < graph.Length; j++)
            {

                Vector2 StartPosition = graph[i].NodeHexagon.transform.position;
                Vector2 direction =  graph[j].NodeHexagon.transform.position;

                bool IsEnemyOnTheWay = false;
                bool NoRibs = false;

                if (listHexagons[i].TypeHexagon <= 0 || (listHexagons[i].TypeHexagon == 3 && graph[j].NodeHexagon.gameObject.layer != 10))
                {
                    //Debug.Log(1);
                    if (!MapControlStatic.CollisionCheck(StartPosition, direction, IsElevation))
                    {
                         NoRibs = true;
                    }
                }
                else
                {
                    //Debug.Log(2);

                    IsElevation = true;
                    if (!MapControlStatic.CollisionCheckElevation(StartPosition, direction, IsElevation))
                    {
                        NoRibs = true;
                    }
                }


                float distance = (graph[i].NodeHexagon.transform.position - graph[j].NodeHexagon.transform.position).magnitude;

                //Vector2 StartPosition = graph[i].NodeHexagon.transform.position;
                //Vector2 direction = -(graph[i].NodeHexagon.transform.position - graph[j].NodeHexagon.transform.position).normalized;
                //Physics2D.CircleCast(StartPosition, 0.1f, direction, contactFilter2D, hit2Ds, distance);

                if (!NoRibs)
                {
                    //Debug.Log(2222);
                    float magnitude = (graph[i].NodeHexagon.transform.position - graph[j].NodeHexagon.transform.position).magnitude;
                    graph[i].Connect(graph[j], magnitude);
                }
                if (IsEnemyOnTheWay)
                {
                    //ListVertexEnemy.Add(graph[i].NodeHexagon);
                    //ListVertexEnemy.Add(graph[j].NodeHexagon);
                    //DictionaryEnemyVertex[DictionaryEnemyVertex.Count] = ListVertexEnemy;
                }
            }
        }
        if (DictionaryEnemyVertex.Count > 0)
        {
            for (int i = 0; i < DictionaryEnemyVertex.Count; i++)
            {

            }
        }
        //Debug.Log(DictionaryEnemyVertex.Count);
        return graph;
    }
    public void StartWay(HexagonControl hexagonFinish)
    {
        //всегда вноси новый слой
        StopCoroutine(MoveCorotine);
        ListPoints.Clear();

        //LayerMask layerMask = LayerMask.GetMask("Hero", "Hexagon", "LowerBarrier");
        ListPoints.AddRange(SearchForAWay(hexagonFinish, transform, false));

        MoveCorotine = Movement();
        StartCoroutine(MoveCorotine);
    }
    public void StartWayElevation(HexagonControl hexagonFinish)
    {
        StopCoroutine(MoveCorotine);
        ListPoints.Clear();
        //LayerMask layerMask = LayerMask.GetMask("Hero", "Hexagon", "HeroElevation", "Elevation");
        ListPoints.AddRange(SearchForAWay(hexagonFinish, transform, true));
        MoveCorotine = Movement();
        StartCoroutine(MoveCorotine);
    }
    public void InitializationVertex(HexagonControl[] hexagons)
    {
        _listVertex = new List<HexagonControl>();
        _listVertex.AddRange(hexagons);
    }

    public void StopMove()
    {
        StopCoroutine(MoveCorotine);
    }
    public void ContinueMove()
    {
        StartCoroutine(MoveCorotine);
    }
}