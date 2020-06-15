using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHexNav : MonoBehaviour
{
    public List<HexagonControl> EndWay ;
    public List<HexagonControl> Way;
    public void CreateNewList()
    {
        Way = new List<HexagonControl>() ;
        EndWay = new List<HexagonControl>();
    }

    public void SaveTheWay(HexagonControl endWay, List<HexagonControl> way)
    {
        List<HexagonControl> NewWayList = new List<HexagonControl>();
        NewWayList.AddRange(way);
        EndWay.Add(endWay);
        Way.AddRange(NewWayList);
        Way.Add(null);
    }
    public void check()
    {
        Debug.Log(EndWay.Count);
        Debug.Log(Way.Count);
    }

}
