using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroControl : MonoBehaviour
{
    private IEnumerator MoveCorotine;
    private List<HexagonControl> ListPoints = new List<HexagonControl>();//пройденные вершины 


    [SerializeField]
    private float _speed;
    private int _namberPoint;
    private List<HexagonControl> _listVertex;

    [System.NonSerialized]
    public int HexagonRow = 0, HexagonColumn = 0;

    void Start()
    {
        MoveCorotine = Movement();
       if( _listVertex.Count<=0)
        {
            Debug.LogError("The lack of vertex in the hero");
        }
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    FieldPosition();
        //}

    }
    private HexagonControl FieldPosition(bool elevation)//гексагон к которому принадлежит герой
    {
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
                    //Debug.Log(" Magnitude " + Magnitude);
                }
            }
        }
        //Debug.Log(hexagonControl.Row + " " + hexagonControl.Column);

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
            if ((positionCurrent - positionMain).magnitude <=0.001f)
            {
                PointList.Remove(PointList[0]);
            }
            yield return new WaitForSeconds(0.02f);
        }

    }
    private List<HexagonControl> SearchForAWay(HexagonControl hexagon, LayerMask layerMask, Transform startingPoint, bool elevation)//возврашет все вершины по которым надо пройти 
    {
        List<HexagonControl> ListOfNecessaryVertices = new List<HexagonControl>();
        List<HexagonControl> ListHexgon = new List<HexagonControl>();

        bool IsStraightWay = true;
        List<RaycastHit2D> hit2Ds = new List<RaycastHit2D>();

        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.SetLayerMask(layerMask);
        contactFilter2D.useLayerMask = true;

        float distance = (startingPoint.position - hexagon.transform.position).magnitude;

        Physics2D.Raycast(startingPoint.position, -(startingPoint.position - hexagon.transform.position).normalized, contactFilter2D, hit2Ds, distance);

        if (hit2Ds.Count > 0)
        {
            for (int i = 0; i < hit2Ds.Count; i++)
            {
                var GetHit = hit2Ds[i].collider.GetComponent<HexagonControl>();

                if (GetHit.TypeHexagon==2)
                {
                    Debug.Log(1);
                }

                if (!GetHit.FreedomTestType(false))
                {
                    IsStraightWay = false;
                    ListHexgon.Add(FieldPosition(elevation));
                    ListHexgon.AddRange(_listVertex);

                    ListHexgon.Add(hexagon);
                    ListOfNecessaryVertices.AddRange(BreakingTheDeadlock(ListHexgon/*, layerMask*/));
                    break;
                }
            }
        }
        if (IsStraightWay)
        {
            ListOfNecessaryVertices.Add(hexagon);
        }
        //_listVertex.Remove(_listVertex[_listVertex.Count-1]);
        //_listVertex.Remove(_listVertex[0]);
        return ListOfNecessaryVertices;
    }
    private List<HexagonControl> BreakingTheDeadlock(List<HexagonControl> listHexagons/*, LayerMask layerMask*/)//выстравивает пути обхода
    {
        var graph = new Graph(listHexagons);
        for (int i = 0; i < graph.Length - 1; i++)
        {
            bool IsElevation = false;
            for (int j = i + 1; j < graph.Length; j++)
            {
                List<RaycastHit2D> hit2Ds = new List<RaycastHit2D>();

                ContactFilter2D contactFilter2D = new ContactFilter2D();
                LayerMask layerMask;
                if (listHexagons[i].TypeHexagon<=0)
                {
                    layerMask = LayerMask.GetMask("Hero", "Hexagon", "LowerBarrier");
                }
                else
                {
                    IsElevation = true;
                    layerMask = LayerMask.GetMask("Hero", "Hexagon", "Elevation"/*, "LowerBarrier"*/);
                }

                contactFilter2D.SetLayerMask(layerMask);
                contactFilter2D.useLayerMask = true;

                float distance = (graph[i].NodeHexagon.transform.position - graph[j].NodeHexagon.transform.position).magnitude;

                Physics2D.CircleCast(graph[i].NodeHexagon.transform.position, 1f, -(graph[i].NodeHexagon.transform.position - graph[j].NodeHexagon.transform.position).normalized, contactFilter2D, hit2Ds, distance);

                bool NoRibs = false;
                for (int v = 0; v < hit2Ds.Count; v++)
                {
                    var GetHit = hit2Ds[v].collider.GetComponent<HexagonControl>();

                    if (!GetHit.FreedomTestType(IsElevation))
                    {
                        NoRibs = true;
                    }

                }
                if (!NoRibs)
                {

                    float magnitude = (graph[i].NodeHexagon.transform.position - graph[j].NodeHexagon.transform.position).magnitude;
                    graph[i].Connect(graph[j], magnitude);
                }
            }
        }
        //List<Node> f = graph[2].IncidentNodes();
        //for (int i = 0; i < f.Count; i++)
        //{
        //    f[i].NodeHexagon.Flag();

        //}
        AlgorithmDijkstra algorithmDijkstra = new AlgorithmDijkstra();
        List<Node> nodesList = algorithmDijkstra.Dijkstra(graph);

        List<HexagonControl> ListVertex = new List<HexagonControl>();
        for (int i = 0; i < nodesList.Count; i++)
        {
            ListVertex.Add(nodesList[i].NodeHexagon);
        }
        return ListVertex;
    }

    public void StartWay(HexagonControl hexagonFinish)
    {
        //всегда вноси новый слой
        LayerMask layerMask = LayerMask.GetMask("Hero", "Hexagon", "LowerBarrier");
        ListPoints.Clear();
        ListPoints.AddRange(SearchForAWay(hexagonFinish, layerMask, transform,false));
        StopCoroutine(MoveCorotine);
        MoveCorotine = Movement();
        StartCoroutine(MoveCorotine);
    }
    public void StartWayElevation(HexagonControl hexagonFinish)
    {
        LayerMask layerMask = LayerMask.GetMask("Hero", "Hexagon", "Elevation");
        ListPoints.Clear();
        ListPoints.AddRange(SearchForAWay(hexagonFinish, layerMask, transform,true));
        StopCoroutine(MoveCorotine);
        MoveCorotine = Movement();
        StartCoroutine(MoveCorotine);
    }
    public void InitializationVertex(HexagonControl[]hexagons)
    {
        _listVertex = new List<HexagonControl>();
        _listVertex.AddRange(hexagons);
    }
}
