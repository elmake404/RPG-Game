using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavSurface : MonoBehaviour
{
    [HideInInspector]
    public List<HexagonControl> ListHexagonControls = new List<HexagonControl>();
    private AlgorithmDijkstra _algorithmDijkstra = new AlgorithmDijkstra();


    void Start()
    {
        StartCoroutine(CreatingEdge());
    }

    void Update()
    {

    }
    private IEnumerator CreatingEdge()
    {
        NavStatic.GraphStatic = new Graph(ListHexagonControls);
        for (int i = 0; i < NavStatic.GraphStatic.Length - 1; i++)
        {
            for (int j = i + 1; j < NavStatic.GraphStatic.Length; j++)
            {
                bool IsElevation = false;

                Vector2 StartPosition = NavStatic.GraphStatic[i].NodeHexagon.transform.position;
                Vector2 direction = NavStatic.GraphStatic[j].NodeHexagon.transform.position;

                bool NoRibs = false;

                if (NavStatic.GraphStatic[i].NodeHexagon.TypeHexagon <= 0
                    || (NavStatic.GraphStatic[i].NodeHexagon.TypeHexagon == 3 && NavStatic.GraphStatic[j].NodeHexagon.gameObject.layer != 10))
                {
                    IsElevation = false;
                    if (!NavStatic.CollisionCheck(StartPosition, direction, IsElevation))
                    {
                        NoRibs = true;
                    }
                }
                else
                {
                    IsElevation = true;
                    if (!NavStatic.CollisionCheckElevation(StartPosition, direction, IsElevation))
                    {
                        NoRibs = true;
                    }
                }

                if (!NoRibs)
                {
                    float magnitude = (NavStatic.GraphStatic[i].NodeHexagon.transform.position - NavStatic.GraphStatic[j].NodeHexagon.transform.position).magnitude;
                    NavStatic.GraphStatic[i].Connect(NavStatic.GraphStatic[j], magnitude, null);
                    NavStatic.GraphStatic[i].NodeHexagon.ShortWay[NavStatic.GraphStatic[j].NodeHexagon] = new List<HexagonControl>() { NavStatic.GraphStatic[j].NodeHexagon };
                }
                else
                {
                    NavStatic.GraphStatic[i].ListUnrelated.Add(NavStatic.GraphStatic[j]);
                }
            }
            yield return new WaitForSeconds(0.02f);
        }
        Debug.Log("Ribs completed");
        //NavStatic.GraphStatic[0].ListUnrelated[0].NodeHexagon.Flag();

        for (int i = 0; i < NavStatic.GraphStatic.Length - 1; i++)
        {
            for (int j = 0; j < NavStatic.GraphStatic[i].ListUnrelated.Count; j++)
            {
                List<HexagonControl> hexagonList = _algorithmDijkstra.Dijkstra(NavStatic.GraphStatic, NavStatic.GraphStatic[i], NavStatic.GraphStatic[i].ListUnrelated[j]);
                if (hexagonList == null)
                {
                    //NavStatic.GraphStatic[i].ListUnrelated.Remove(NavStatic.GraphStatic[i].ListUnrelated[0]);
                    continue;
                }

                NavStatic.GraphStatic[i].NodeHexagon.ShortWay[NavStatic.GraphStatic[i].ListUnrelated[0].NodeHexagon] = hexagonList;
                yield return new WaitForSeconds(0.02f);
            }
            NavStatic.GraphStatic[i].ListUnrelated.Clear();
        }
        Debug.Log("Compled ");
    }
}
