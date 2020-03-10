using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapControl : MonoBehaviour
{
    [SerializeField]
    private Transform[] hexagons; 

    void Start()
    {
        for (int i = 0; i < hexagons.Length; i++)
        {
            for (int j = 0; j < hexagons[i].childCount; j++)
            {
               MapControlStatic.mapNav[i, j] = hexagons[i].GetChild(j).GetComponent<HexagonControl>().TypeHexagon;
                //if (hexagons[i].GetChild(j).GetComponent<HexagonControl>().TypeHexagon==1)
                //{
                //    Debug.Log(i+ " " + j);
                //}
            }
        }
        //Debug.Log(MapControlStatic.mapNav[2, 5]);
    }

    void Update()
    {
        
    }
}
