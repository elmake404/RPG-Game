using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemy;
    [SerializeField]
    private Transform[] _spawnPoint;
    private List<HeroControl> _listHero = new List<HeroControl>();
    private List<EnemyControl> _listEnemyControls = new List<EnemyControl>();
    private AlgorithmDijkstra algorithmDijkstra = new AlgorithmDijkstra();

    [SerializeField]
    private int _maxQuantityEnemy;
    private int _namberPointSpawn = 0;

    void Start()
    {
        //StartCoroutine(GoalSelection());
        if (_maxQuantityEnemy > 0)
        {
            StartCoroutine(Production());
        }
        else
        {
            Debug.LogError("Incorrect number of units(_maxQuantityEnemy)");
        }
    }

    void Update()
    {

    }

    private void GoalSelection(EnemyControl enemy)
    {
        float disteinceMin = float.PositiveInfinity;
        HeroControl hero = null;

        for (int j = 0; j < _listHero.Count; j++)
        {

            if (_listHero[j].IsFreePlace())
            {
                float disteinceThis = SearchForHero(MapControlStatic.FieldPosition(enemy.gameObject.layer, enemy.transform.position),
                    MapControlStatic.FieldPosition(_listHero[j].gameObject.layer, _listHero[j].transform.position)); 

                if (disteinceMin > disteinceThis)
                {
                    //Debug.Log(enemy.name);
                    //Debug.Log(disteinceMin);
                    //Debug.Log(disteinceThis);
                    disteinceMin = disteinceThis;
                    hero = _listHero[j];
                }
            }
        }
        if (hero != null)
            enemy.StartWay(hero.FreePlace(enemy));
    }
    private float SearchForHero(HexagonControl Start, HexagonControl Finish)//возвращает путь к герою стоимость 
    {
        float Distance;
        Graph graphMain = new Graph(MapControlStatic.GraphStatic);
        graphMain.AddNodeFirst(Start);
        graphMain.AddNode(Finish);

       algorithmDijkstra.Dijkstra(MapControlStatic.CreatingEdge(graphMain), out Distance);

        return Distance;
    }
    private IEnumerator Production()
    {
        int n =0;

        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                n++;

                if (_maxQuantityEnemy != 0)
                {
                    _maxQuantityEnemy--;
                    EnemyControl Enemy = Instantiate(_enemy, _spawnPoint[_namberPointSpawn].position, Quaternion.identity).GetComponent<EnemyControl>();
                    Enemy.gameObject.name = n.ToString() ;
                    Enemy.First();
                    GoalSelection(Enemy);

                    if (_namberPointSpawn != _spawnPoint.Length - 1)
                    {
                        _namberPointSpawn++;
                    }
                    else
                    {
                        _namberPointSpawn = 0;
                    }
                }
                //yield return new WaitForSeconds(0.00001f);

            }
            if (_maxQuantityEnemy != 0)
            {
                yield return new WaitForSeconds(1f);
            }
            else
            {
                break;
            }
        }
    }

    public void InitializationList(HeroControl[] heroes)
    {
        _listHero.AddRange(heroes);
    }
}
