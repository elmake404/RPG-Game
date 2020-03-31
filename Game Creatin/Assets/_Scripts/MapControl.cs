using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapControl : MonoBehaviour
{
    [SerializeField]
    private Transform[] hexagons;
    [SerializeField]
    private HeroControl[] _heroControls;
    [SerializeField]
    private EnemyControl[] _enemyControls;
    [SerializeField]
    private HexagonControl[] _arreyVertex;

    void Awake()
    {
        for (int i = 0; i < hexagons.Length; i++)
        {
            hexagons[i].name = i.ToString();
            for (int j = 0; j < hexagons[i].childCount; j++)
            {
               //MapControlStatic.mapNav[i, j] = hexagons[i].GetChild(j).GetComponent<HexagonControl>();
                hexagons[i].GetChild(j).name = j.ToString();
            }
        }
        for (int i = 0; i < _heroControls.Length; i++)
        {
            _heroControls[i].NavigationHero.InitializationVertex(_arreyVertex);
        }
        for (int i = 0; i < _enemyControls.Length; i++)
        {
            _enemyControls[i].InitializationHero(_heroControls);
            _enemyControls[i].Navigation.InitializationVertex(_arreyVertex);
        }
    }

    void Update()
    {
        
    }
}
