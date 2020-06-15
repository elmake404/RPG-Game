using System.Collections;
using System.Collections.Generic;
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

    public void GoalSelection(EnemyControl enemy)
    {
        if (_listHero.Count != 0)
        {
            int namber = Random.Range(0, _listHero.Count);
            HeroControl hero = _listHero[namber];
            List<int> NextNamber = new List<int>();
            NextNamber.Add(namber);
            //Debug.Log(NextNamber.IndexOf(10));
            while (hero.CountEnemy() == 0)
            {
                if (NextNamber.Count >= 3)
                {
                    hero = null;
                    break;
                }

                namber = Random.Range(0, _listHero.Count);

                if (NextNamber.IndexOf(namber) != -1)
                {
                    continue;
                }

                hero = _listHero[namber];
                NextNamber.Add(namber);
            }

            if (hero == null)
            {
                Debug.LogError("No free hero");
                return;
            }
            hero.AddNewEnemy(enemy);
            enemy.HeroTarget = hero;
            enemy.StartWay(hero);
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

    public void RemoveHero(HeroControl heroControl)
    {
        _listHero.Remove(heroControl);
    }
    public void InitializationList(HeroControl[] heroes)
    {
        _listHero.AddRange(heroes);
    }
}
