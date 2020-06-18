using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemy;
    [SerializeField]
    private Transform[] _spawnPoint;
    [SerializeField]
    private List<HeroControl> _listHero = new List<HeroControl>();
    private List<EnemyControl> _listEnemyControls = new List<EnemyControl>();

    [SerializeField]
    private int _maxQuantityEnemy;
    private int _namberPointSpawn = 0;

    void Start()
    {
        foreach (var item in _listHero)
        {
            item.Initialization(this);
        }

        if (_maxQuantityEnemy > 0)
        {
            StartCoroutine(Production());
        }
        else
        {
            Debug.LogError("Incorrect number of units(_maxQuantityEnemy)");
        }
    }
    private IEnumerator Production()
    {
        yield return new WaitForSeconds(0.1f);
        int n = 0;

        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                n++;

                if (_maxQuantityEnemy != 0)
                {
                    _maxQuantityEnemy--;
                    EnemyControl Enemy = Instantiate(_enemy, _spawnPoint[_namberPointSpawn].position, Quaternion.identity).GetComponent<EnemyControl>();
                    Enemy.gameObject.name = n.ToString();
                    Enemy.First(this);
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

                yield return new WaitForSeconds(0.5f);
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

    private HeroControl GetNearestHero(HexagonControl hexagon)
    {
        HeroControl heroControl = null;
        float Magnitude = float.PositiveInfinity;

        for (int i = 0; i < _listHero.Count; i++)
        {
            List<HexagonControl> listHex = new List<HexagonControl>();
            listHex.AddRange(hexagon.GetWay(_listHero[i].HexagonMain()));
            float magnitude = 0;

            for (int j = 0; j < listHex.Count - 1; j++)
            {
                magnitude += (listHex[j].position - listHex[j+1].position).magnitude;
            }

            if (Magnitude>magnitude)
            {
                Magnitude = magnitude;
                heroControl = _listHero[i];
            }

            //Debug.Log(_listHero[i].name);
            //Debug.Log(magnitude);
        }

        return heroControl;
    }

    public void GoalSelection(EnemyControl enemy)
    {
        if (_listHero.Count <= 0)
        {
            StaticLevelManager.IsGameFlove = false;
            return;
        }

        HeroControl hero = GetNearestHero(enemy.HexagonMain());

        if (hero == null)
        {
            Debug.LogError("No free hero");
            return;
        }

        hero.AddNewEnemy(enemy);
        enemy.HeroTarget = hero;
        enemy.StartWay(hero);
    }
    public void RemoveHero(HeroControl heroControl)
    {
        _listHero.Remove(heroControl);
    }
    public void InitializationList(HeroControl[] heroes)
    {
        _listHero.AddRange(heroes);
    }
}
