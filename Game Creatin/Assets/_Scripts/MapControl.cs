using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapControl : MonoBehaviour
{
    [SerializeField]
    private Transform[] hexagons;
    [SerializeField]
    private HeroControl[] heroControls;
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
        for (int i = 0; i < heroControls.Length; i++)
        {
            heroControls[i].InitializationVertex(_arreyVertex);
        }
    }

    void Update()
    {
        
    }
}
