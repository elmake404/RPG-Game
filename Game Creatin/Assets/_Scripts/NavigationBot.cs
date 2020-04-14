using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationBot : MonoBehaviour
{
    private IEnumerator MoveCorotine;
    private List<HexagonControl> ListPoints = new List<HexagonControl>();//точки через который надо пройти 

    [SerializeField]
    private float _speed;
    void Start()
    {
        MoveCorotine = Movement();
    }

    void Update()
    {
        
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

    private List<HexagonControl> SearchForAWay(HexagonControl hexagon)//возврашет все вершины по которым надо пройти 
    {
        Graph graphMain = MapControlStatic.GraphStatic;
        graphMain.AddNode(hexagon);
        graphMain.AddNode(FieldPosition());
        CreatingEdge(graphMain);
        AlgorithmDijkstra algorithmDijkstra = new AlgorithmDijkstra();

        List<Node> nodesList = algorithmDijkstra.Dijkstra(CreatingEdge(graphMain));

        List<HexagonControl> ListVertex = new List<HexagonControl>();

        for (int i = 1; i < nodesList.Count; i++)
        {
            ListVertex.Add(nodesList[i].NodeHexagon);
        }

        return ListVertex;
    }
    private Graph CreatingEdge(Graph graph)
    {
        int NamberElement = graph.Length;

        for (int i = 0; i < 2; i++)
        {
            NamberElement -=1;
            bool IsElevation = false;

            for (int j = 0; j < graph.Length; j++)
            {
                if (graph[NamberElement]== graph[j])
                {
                    Debug.Log("continue");
                    continue;
                }

                Vector2 StartPosition = graph[NamberElement].NodeHexagon.transform.position;
                Vector2 direction = graph[j].NodeHexagon.transform.position;

                bool NoRibs = false;

                if (graph[NamberElement].NodeHexagon.TypeHexagon <= 0 || (graph[NamberElement].NodeHexagon.TypeHexagon == 3 && graph[j].NodeHexagon.gameObject.layer != 10))
                {
                    //Debug.Log(1);
                    IsElevation = false;
                    if (!MapControlStatic.CollisionCheck(StartPosition, direction, IsElevation))
                    {
                        NoRibs = true;
                    }
                }
                else
                {
                    //Debug.Log(2);
                    //graph[i].NodeHexagon.Flag();
                    IsElevation = true;
                    if (!MapControlStatic.CollisionCheckElevation(StartPosition, direction, IsElevation))
                    {
                        NoRibs = true;
                    }
                }


                float distance = (graph[NamberElement].NodeHexagon.transform.position - graph[j].NodeHexagon.transform.position).magnitude;

                if (!NoRibs)
                {
                    //Debug.Log(2222);
                    float magnitude = (graph[NamberElement].NodeHexagon.transform.position - graph[j].NodeHexagon.transform.position).magnitude;
                    graph[NamberElement].Connect(graph[j], magnitude);
                }
            }
        }
        return graph;
    }
    public HexagonControl FieldPosition()//гексагон к которому принадлежит герой (надо переделать)
    {
        HexagonControl[] hexagonControl = MapControlStatic.GetPositionOnTheMap(0.1f, transform.position);//нужный 6-ти угольник  
        return hexagonControl[0];
    }

    public void StartWay(HexagonControl hexagonFinish)
    {
        StopCoroutine(MoveCorotine);
        ListPoints.Clear();
        //LayerMask layerMask = LayerMask.GetMask("Hero", "Hexagon", "HeroElevation", "Elevation");
        ListPoints.AddRange(SearchForAWay(hexagonFinish));
        MoveCorotine = Movement();
        StartCoroutine(MoveCorotine);
    }
}
