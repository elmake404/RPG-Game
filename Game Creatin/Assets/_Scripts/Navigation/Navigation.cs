using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour
{
    private IEnumerator MoveCorotine;
    private List<HexagonControl> ListPoints = new List<HexagonControl>();//точки через который надо пройти 
    private HeroControl _heroMain;
    private AlgorithmDijkstra _algorithmDijkstra = new AlgorithmDijkstra();

    [SerializeField]
    private float _speed;
    //private int _namberPoint;
    private List<HexagonControl> _listVertex;
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

    }
    public HexagonControl FieldPosition()//гексагон к которому принадлежит герой 
    {
        Vector3 difference = gameObject.layer == 8 ? Vector3.zero : new Vector3(MapControlStatic.X,MapControlStatic.Y,0);
        HexagonControl[] hexagonControl = MapControlStatic.GetPositionOnTheMap(0.1f,transform.position-difference);//нужный 6-ти угольник  
        return hexagonControl[0];
    } 
    private IEnumerator Movement()//коротина движения
    {
        _heroMain.Animator.SetBool("Run", true);
        List<HexagonControl> PointList = new List<HexagonControl>();
        PointList.AddRange(ListPoints);
        ListPoints.Clear();
        while (PointList.Count > 0)
        {
            HexagonControl newHex = PointList[0].GetHexagonMain();
            Vector2 positionCurrent = newHex.transform.position;
            transform.position = Vector2.MoveTowards(transform.position, positionCurrent, _speed);
            Vector2 positionMain = transform.position;
             //= PointList[0].transform.position;
            if (newHex.TypeHexagon == 2 && gameObject.layer == 8)
            {
                gameObject.layer = 11;
            }
            else if (newHex.TypeHexagon == 0 && gameObject.layer == 11)
            {
                gameObject.layer = 8;
            }
            if ((positionCurrent - positionMain).magnitude <= 0.001f)
            {
                PointList.Remove(PointList[0]);
            }
            yield return new WaitForSeconds(0.02f);
        }
        _heroMain.Animator.SetBool("Run", false);
    }
    private List<HexagonControl> SearchForAWay(HexagonControl hexagon, Transform startingPoint, bool elevation)//возврашет все вершины по которым надо пройти 
    {
        List<HexagonControl> ListOfNecessaryVertices = new List<HexagonControl>();
        List<HexagonControl> ListHexgon = new List<HexagonControl>();

        if (elevation)
        {
            if (!MapControlStatic.CollisionCheckElevation(startingPoint.position, hexagon.transform.position, elevation))
            {
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
        List<Node> nodesList = _algorithmDijkstra.Dijkstra(CreatingEdge(listHexagons, true));

        List<HexagonControl> ListVertex = new List<HexagonControl>();

        for (int i = 1; i < nodesList.Count; i++)
        {
            ListVertex.Add(nodesList[i].NodeHexagon);
        }

        return ListVertex;
    }
    private Graph CreatingEdge(List<HexagonControl> listHexagons, bool IsConsiderEnemy)//выстраивает ребра в графе 
    {
        var graph = new Graph(listHexagons);

        for (int i = 0; i < graph.Length - 1; i++)
        {
            bool IsElevation = false;

            for (int j = i + 1; j < graph.Length; j++)
            {

                Vector2 StartPosition = graph[i].NodeHexagon.transform.position;
                Vector2 direction =  graph[j].NodeHexagon.transform.position;

                //bool IsEnemyOnTheWay = false;
                bool NoRibs = false;
                HexagonControl node = graph[j].NodeHexagon.Elevation != null? graph[j].NodeHexagon.Elevation: graph[j].NodeHexagon;

                if (listHexagons[i].TypeHexagon <= 0 || (listHexagons[i].TypeHexagon == 3 && node.gameObject.layer != 10))
                {
                    IsElevation = false;
                    if (!MapControlStatic.CollisionCheck(StartPosition, direction, IsElevation))
                    {
                         NoRibs = true;
                    }
                }
                else
                {
                    IsElevation = true;
                    if (!MapControlStatic.CollisionCheckElevation(StartPosition, direction, IsElevation))
                    {
                        NoRibs = true;
                    }
                }

                if (!NoRibs)
                {
                    float magnitude = (graph[i].NodeHexagon.transform.position - graph[j].NodeHexagon.transform.position).magnitude;
                    graph[i].Connect(graph[j], magnitude);
                } 

                //if (IsEnemyOnTheWay)
                //{
                //    //ListVertexEnemy.Add(graph[i].NodeHexagon);
                //    //ListVertexEnemy.Add(graph[j].NodeHexagon);
                //    //DictionaryEnemyVertex[DictionaryEnemyVertex.Count] = ListVertexEnemy;
                //}
            }
        }

        return graph;
    }
    public void StartWay(HexagonControl hexagonFinish)
    {
        //всегда вноси новый слой
        StopCoroutine(MoveCorotine);
        ListPoints.Clear();
        HexagonControl hexagon = hexagonFinish.Floor != null ? hexagonFinish.Floor : hexagonFinish;
        ListPoints.AddRange(SearchForAWay(hexagon, transform, false));

        MoveCorotine = Movement();
        StartCoroutine(MoveCorotine);
    }
    public void StartWayElevation(HexagonControl hexagonFinish)
    {
        StopCoroutine(MoveCorotine);
        ListPoints.Clear();
        HexagonControl hexagon = hexagonFinish.Floor != null ? hexagonFinish.Floor : hexagonFinish;

        ListPoints.AddRange(SearchForAWay(hexagon, transform, true));
        MoveCorotine = Movement();
        StartCoroutine(MoveCorotine);
    }
    public void Initialization(HexagonControl[] hexagons,HeroControl hero)
    {
        _heroMain = hero;
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