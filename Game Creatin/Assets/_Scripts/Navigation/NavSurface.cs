using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavSurface : MonoBehaviour
{
    [HideInInspector]
    public Graph GraphNav;// граф в котором соеденены все ребра 
    public MapControl Map;
    //[HideInInspector]
    public List<HexagonControl> ListHexagonControls;

    private AlgorithmDijkstra _algorithmDijkstra = new AlgorithmDijkstra();


    void Start()
    {
    }

    void Update()
    {

    }
    public void DataRecords()
    {
        ListHexagonControls = new List<HexagonControl>();
        Map.DataRecords();
        for (int i = 0; i < ListHexagonControls.Count; i++)
        {
            ListHexagonControls[i].CheckDataComponent();
        }

    }
    public void CreatingEdge()
    {
        Debug.Log("Ribs start");
        GraphNav = new Graph(ListHexagonControls);
        for (int i = 0; i < ListHexagonControls.Count; i++)
        {
            ListHexagonControls[i].Data.CreateNewList() ;
        }

        for (int i = 0; i <  GraphNav.Length - 1; i++)
        {
            for (int j = i + 1; j < GraphNav.Length; j++)
            {
                bool IsElevation;

                Vector2 StartPosition = GraphNav[i].NodeHexagon.transform.position;
                Vector2 direction = GraphNav[j].NodeHexagon.transform.position;

                bool NoRibs = false;

                if (GraphNav[i].NodeHexagon.TypeHexagon <= 0
                    || (GraphNav[i].NodeHexagon.TypeHexagon == 3 && GraphNav[j].NodeHexagon.gameObject.layer != 10))
                {
                    IsElevation = false;
                    if (!NavStatic.CollisionCheck(StartPosition, direction, IsElevation, null, null))
                    {
                        NoRibs = true;
                    }
                }
                else
                {
                    IsElevation = true;
                    if (!NavStatic.CollisionCheckElevation(StartPosition, direction, IsElevation,null,null))
                    {
                        NoRibs = true;
                    }
                }

                if (!NoRibs)
                {
                    float magnitude = (GraphNav[i].NodeHexagon.transform.position - GraphNav[j].NodeHexagon.transform.position).magnitude;
                    GraphNav[i].Connect(GraphNav[j], magnitude, null);

                    GraphNav[i].NodeHexagon.Data.SaveTheWay(GraphNav[j].NodeHexagon,new List<HexagonControl>() { GraphNav[i].NodeHexagon, GraphNav[j].NodeHexagon });
                    GraphNav[j].NodeHexagon.Data.SaveTheWay(GraphNav[i].NodeHexagon, new List<HexagonControl>() { GraphNav[j].NodeHexagon, GraphNav[i].NodeHexagon });
                }
                else
                {
                    GraphNav[i].ListUnrelated.Add(GraphNav[j]);
                }
            }
        }
        Debug.Log("Ribs completed");
    }
    public void AlgorithmDijkstra()
    {
        for (int i = 0; i < GraphNav.Length - 1; i++)
        {
            for (int j = 0; j < GraphNav[i].ListUnrelated.Count; j++)
            {
                List<HexagonControl> hexagonList = _algorithmDijkstra.Dijkstra(GraphNav, GraphNav[i], GraphNav[i].ListUnrelated[j]);
                if (hexagonList == null)
                {
                    Debug.Log("Pizdec");
                    continue;
                }

                GraphNav[i].NodeHexagon.Data.SaveTheWay(GraphNav[i].ListUnrelated[j].NodeHexagon, hexagonList);
                hexagonList.Reverse();
                GraphNav[i].ListUnrelated[j].NodeHexagon.Data.SaveTheWay(GraphNav[i].NodeHexagon, hexagonList);
            }

            GraphNav[i].ListUnrelated.Clear();
        }
        Debug.Log("Compled ");
    }
    public void chekc()
    {
        ListHexagonControls[3].Data.check() ;
    }
    //private IEnumerator CreatingEdge()
    //{
    //    GraphNav = new Graph(ListHexagonControls);
    //    Debug.Log(ListHexagonControls.Count);
    //    for (int i = 0; i < GraphNav.Length - 1; i++)
    //    {
    //        for (int j = i + 1; j < GraphNav.Length; j++)
    //        {
    //            bool IsElevation = false;

    //            Vector2 StartPosition = GraphNav[i].NodeHexagon.transform.position;
    //            Vector2 direction = GraphNav[j].NodeHexagon.transform.position;

    //            bool NoRibs = false;

    //            if (GraphNav[i].NodeHexagon.TypeHexagon <= 0
    //                || (GraphNav[i].NodeHexagon.TypeHexagon == 3 && GraphNav[j].NodeHexagon.gameObject.layer != 10))
    //            {
    //                IsElevation = false;
    //                if (!NavStatic.CollisionCheck(StartPosition, direction, IsElevation))
    //                {
    //                    NoRibs = true;
    //                }
    //            }
    //            else
    //            {
    //                IsElevation = true;
    //                if (!NavStatic.CollisionCheckElevation(StartPosition, direction, IsElevation))
    //                {
    //                    NoRibs = true;
    //                }
    //            }

    //            if (!NoRibs)
    //            {
    //                float magnitude = (GraphNav[i].NodeHexagon.transform.position - GraphNav[j].NodeHexagon.transform.position).magnitude;
    //                GraphNav[i].Connect(GraphNav[j], magnitude, null);
    //                GraphNav[i].NodeHexagon.ShortWay[GraphNav[j].NodeHexagon] = new List<HexagonControl>() { GraphNav[j].NodeHexagon };
    //            }
    //            else
    //            {
    //                GraphNav[i].ListUnrelated.Add(GraphNav[j]);
    //            }
    //        }
    //        yield return new WaitForSeconds(0.02f);
    //    }
    //    Debug.Log("Ribs completed");
    //    GraphNav[0].ListUnrelated[0].NodeHexagon.Flag();

    //    for (int i = 0; i < GraphNav.Length - 1; i++)
    //    {
    //        for (int j = 0; j < GraphNav[i].ListUnrelated.Count; j++)
    //        {
    //            List<HexagonControl> hexagonList = _algorithmDijkstra.Dijkstra(GraphNav, GraphNav[i], GraphNav[i].ListUnrelated[j]);
    //            if (hexagonList == null)
    //            {
    //                continue;
    //            }

    //            GraphNav[i].NodeHexagon.ShortWay[GraphNav[i].ListUnrelated[0].NodeHexagon] = hexagonList;
    //        }
    //        yield return new WaitForSeconds(0.02f);

    //        GraphNav[i].ListUnrelated.Clear();
    //    }
    //    Debug.Log("Compled ");
    //}
}
