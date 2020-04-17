using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<HeroControl> _listHero = new List<HeroControl>();
    private List<EnemyControl> _listEnemyControls = new List<EnemyControl>();
    private AlgorithmDijkstra algorithmDijkstra = new AlgorithmDijkstra();

    void Start()
    {
        StartCoroutine(GoalSelection());
    }

    void Update()
    {
        
    }

    private IEnumerator GoalSelection()
    {
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < _listEnemyControls.Count; i++)
        {
            float disteinceMin = float.PositiveInfinity;
            HeroControl hero = null;
            for (int j = 0; j < _listHero.Count; j++)
            {
                if (_listHero[j].IsFreePlace())
                {
                    float disteinceThis = float.PositiveInfinity;

                    SearchForHero(MapControlStatic.FieldPosition(_listEnemyControls[i].gameObject.layer, _listEnemyControls[i].transform.position),
                        MapControlStatic.FieldPosition(_listHero[j].gameObject.layer, _listHero[j].transform.position), out disteinceThis);

                    if (disteinceMin> disteinceThis)
                    {
                        disteinceMin = disteinceThis;
                        hero = _listHero[j];
                    }
                    yield return new WaitForSeconds(0.000001f);
                }
            }
            if(hero!=null)
            _listEnemyControls[i].StartWay(hero.FreePlace(_listEnemyControls[i]));
            //Debug.Log(disteinceMin+" "+ _listEnemyControls[i].gameObject.name);
        }
    }
    private List<HexagonControl> SearchForHero(HexagonControl Start,HexagonControl Finish,out float Distance)//возвращает путь к герою стоимость 
    {
        Graph graphMain = new Graph(MapControlStatic.GraphStatic);
        graphMain.AddNodeFirst(Start);
        graphMain.AddNode(Finish);

        List<HexagonControl> ListVertex = algorithmDijkstra.Dijkstra(CreatingEdge(graphMain),out Distance);

        return ListVertex;
    }

    private Graph CreatingEdge(Graph graph)
    {
        int NamberElement = -(graph.Length - 1);

        for (int i = 0; i < 2; i++)
        {
            NamberElement += (graph.Length - 1);
            bool IsElevation = false;

            for (int j = 0; j < graph.Length; j++)
            {
                if (graph[NamberElement] == graph[j])
                {
                    continue;
                }

                Vector2 StartPosition = graph[NamberElement].NodeHexagon.transform.position;
                Vector2 direction = graph[j].NodeHexagon.transform.position;

                bool NoRibs = false;

                if (graph[NamberElement].NodeHexagon.TypeHexagon <= 0 || (graph[NamberElement].NodeHexagon.TypeHexagon == 3 && graph[j].NodeHexagon.gameObject.layer != 10))
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


                float distance = (graph[NamberElement].NodeHexagon.transform.position - graph[j].NodeHexagon.transform.position).magnitude;

                if (!NoRibs)
                {
                    float magnitude = (graph[NamberElement].NodeHexagon.transform.position - graph[j].NodeHexagon.transform.position).magnitude;
                    graph[NamberElement].Connect(graph[j], magnitude);
                }
            }
        }
        return graph;
    }

    public void InitializationList(HeroControl[] heroes, EnemyControl[] enemies )
    {
        _listHero.AddRange(heroes);
        _listEnemyControls.AddRange(enemies);
    }
}
