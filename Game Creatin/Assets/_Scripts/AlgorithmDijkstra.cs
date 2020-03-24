using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class DijkstraData
{
    public Node Previous { get; set; }
    public float Price { get; set; }
}
public class AlgorithmDijkstra
{
    public List<Node> Dijkstra(Graph graph )
    {
        var notVisited = graph.GetListNodes();
        var track = new Dictionary<Node, DijkstraData>();
        track[notVisited[0]] = new DijkstraData { Previous = null, Price = 0 };
        //int namber = 0;
        while (true)
        {
            //namber++;
            //if (namber>300)
            //{
            //    break;
            //}
            Node toOpen = null;
            float priceToOpen = float.PositiveInfinity;
            foreach (var v in notVisited)
            {
                if (track.ContainsKey(v)&&track[v].Price<priceToOpen)
                {

                    toOpen = v;
                    priceToOpen = track[v].Price;
                }
            }
            if (toOpen==null)
            {
                Debug.LogError("Disconnected graph");
            }
            if (toOpen == graph[graph.Length - 1])
            {
                break;
            }
            List<Edge> edgesList = toOpen.IncidentEdge();
            for (int i = 0; i < edgesList.Count; i++)
            {
                var currenPraise = track[toOpen].Price + edgesList[i].Price;
                var nextNode = edgesList[i].OtherNode(toOpen);
                if (!track.ContainsKey(nextNode)||track[nextNode].Price>currenPraise)
                {
                    track[nextNode] = new DijkstraData { Price = currenPraise, Previous = toOpen };
                }
            }
            notVisited.Remove(toOpen);
        }
        //if (namber > 300)
        //{
        //    Debug.LogError("Infinity");
        //    return null;
        //}
        var result = new List<Node>();
        Node end = graph[graph.Length - 1];
        while (end != null)
        {
            result.Add(end);
            end = track[end].Previous;
        }
        //result.Reverse();
        return result;

    }

}
