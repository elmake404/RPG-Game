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



    [System.NonSerialized]
    public int HexagonRow = 0, HexagonColumn = 0;

    void Start()
    {
        MoveCorotine = Movement();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FieldPosition();
        }

    }
    private HexagonControl FieldPosition()
    {
        List<RaycastHit2D> hit2Ds = new List<RaycastHit2D>();
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        Physics2D.CircleCast(transform.position, 1.7f, transform.position - transform.position, contactFilter2D, hit2Ds);
        HexagonControl hexagonControl = null;//нужный 6-ти угольник  
        float Magnitude = 0;

        for (int i = 0; i < hit2Ds.Count; i++)
        {
            var getHex = hit2Ds[i].collider.GetComponent<HexagonControl>();
            if (getHex.FreedomTest())
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
    private IEnumerator Movement()
    {
        List<HexagonControl> PointList = new List<HexagonControl>();
        ListPoints.Reverse();
        PointList.AddRange(ListPoints);
        Debug.Log(ListPoints.Count);
        ListPoints.Clear();
        while (PointList.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, PointList[0].transform.position, _speed);
            if ((PointList[0].transform.position - transform.position).magnitude <= 3.65f)
            {
                PointList.Remove(PointList[0]);
            }
            yield return new WaitForSeconds(0.02f);
        }

    }
    private void SearchForAWay(HexagonControl hexagon)
    {
        List<RaycastHit2D> hit2Ds = new List<RaycastHit2D>();
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        float distance = (transform.position - hexagon.transform.position).magnitude;
        Physics2D.Raycast(transform.position, -(transform.position - hexagon.transform.position).normalized, contactFilter2D, hit2Ds, distance);
        if (hit2Ds.Count > 0)
        {
            for (int i = 0; i < hit2Ds.Count; i++)
            {
                var GetHit = hit2Ds[i].collider.GetComponent<HexagonControl>();

                if (!GetHit.FreedomTest())
                {
                    List<HexagonControl> listHexagonsPeaks = new List<HexagonControl>();
                    listHexagonsPeaks.Add(FieldPosition());
                    listHexagonsPeaks.AddRange(GetHit.peaks);
                    listHexagonsPeaks.Add(hexagon);
                    //hit2Ds[i].collider.GetComponent<HexagonControl>().Flag();
                    BreakingTheDeadlock(listHexagonsPeaks);
                    //здесь чистим у нас есть проблемы 
                    return;
                }
            }
        }
            StopCoroutine(MoveCorotine);
            ListPoints.Clear();

            ListPoints.Add(hexagon);
            MoveCorotine = Movement();
            StartCoroutine(MoveCorotine);
    }
    private void BreakingTheDeadlock(List<HexagonControl> listHexagons)
    {
        var graph = new Graph(listHexagons);
        for (int i = 0; i < graph.Length - 1; i++)
        {
            //graph[i].NodeHexagon.Flag();
            for (int j = i + 1; j < graph.Length; j++)
            {
                List<RaycastHit2D> hit2Ds = new List<RaycastHit2D>();
                ContactFilter2D contactFilter2D = new ContactFilter2D();
                float distance = (graph[i].NodeHexagon.transform.position - graph[j].NodeHexagon.transform.position).magnitude;
                Physics2D.Raycast(graph[i].NodeHexagon.transform.position, -(graph[i].NodeHexagon.transform.position - graph[j].NodeHexagon.transform.position).normalized, contactFilter2D, hit2Ds, distance);
                bool NoRibs = false;
                for (int v = 0; v < hit2Ds.Count; v++)
                {
                    var GetHit = hit2Ds[v].collider.GetComponent<HexagonControl>();
                    if (!GetHit.FreedomTest())
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
        AlgorithmDijkstra algorithmDijkstra = new AlgorithmDijkstra();
        List<Node> nodesList = algorithmDijkstra.Dijkstra(graph);

        StopCoroutine(MoveCorotine);
        ListPoints.Clear();

        for (int i = 0; i < nodesList.Count; i++)
        {
            ListPoints.Add(nodesList[i].NodeHexagon);
        }

        MoveCorotine = Movement();
        StartCoroutine(MoveCorotine);

    }

    public void SatrtWay(HexagonControl hexagon)
    {
        SearchForAWay(hexagon);
    }

}
