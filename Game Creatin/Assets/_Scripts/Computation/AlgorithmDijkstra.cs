using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

class DijkstraData
{
    public Node Previous { get; set; }
    public float Price { get; set; }
}
public class AlgorithmDijkstra
{
    public List<HexagonControl> Dijkstra(Graph _graph, Node start, Node finish)
    {
        Graph graph = new Graph(_graph);
        var notVisited = graph.GetListNodes();
        var track = new Dictionary<Node, DijkstraData>();
        track[start] = new DijkstraData { Previous = null, Price = 0 };
        while (true)
        {
            Node toOpen = null;
            float priceToOpen = float.PositiveInfinity;
            foreach (var v in notVisited)
            {
                if (track.ContainsKey(v) && track[v].Price < priceToOpen)
                {
                    toOpen = v;
                    priceToOpen = track[v].Price;
                }
            }
            if (toOpen == null)
            {
                //Debug.Log("Disconnected graph");
                return null;
            }
            if (toOpen == finish)
            {
                break;
            }
            List<Edge> edgesList = toOpen.IncidentEdge();
            for (int i = 0; i < edgesList.Count; i++)
            {
                var currenPraise = track[toOpen].Price + edgesList[i].Price;
                var nextNode = edgesList[i].OtherNode(toOpen);
                if (!track.ContainsKey(nextNode) || track[nextNode].Price > currenPraise)
                {
                    track[nextNode] = new DijkstraData { Price = currenPraise, Previous = toOpen };
                }
            }
            notVisited.Remove(toOpen);
        }
        var result = new List<HexagonControl>();
        Node end = finish;
        while (end != null)
        {
            result.Add(end.NodeHexagon);
            end = track[end].Previous;
        }
        result.Reverse();
        return result;
    }
    public List<Node> Dijkstra(Graph graph, Node finish)
    {
        var notVisited = graph.GetListNodes();
        var track = new Dictionary<Node, DijkstraData>();
        track[notVisited[0]] = new DijkstraData { Previous = null, Price = 0 };
        while (true)
        {
            Node toOpen = null;
            float priceToOpen = float.PositiveInfinity;
            foreach (var v in notVisited)
            {
                if (track.ContainsKey(v) && track[v].Price < priceToOpen)
                {
                    toOpen = v;
                    priceToOpen = track[v].Price;
                }
            }
            if (toOpen == null)
            {
                //Debug.Log("Disconnected graph");
                return null;
            }
            if (toOpen == finish)
            {
                break;
            }
            List<Edge> edgesList = toOpen.IncidentEdge();
            for (int i = 0; i < edgesList.Count; i++)
            {
                var currenPraise = track[toOpen].Price + edgesList[i].Price;
                var nextNode = edgesList[i].OtherNode(toOpen);
                if (!track.ContainsKey(nextNode) || track[nextNode].Price > currenPraise)
                {
                    track[nextNode] = new DijkstraData { Price = currenPraise, Previous = toOpen };
                }
            }
            notVisited.Remove(toOpen);
        }
        var result = new List<Node>();
        Node end = graph[graph.Length - 1];
        while (end != null)
        {
            result.Add(end);
            end = track[end].Previous;
        }
        result.Reverse();
        return result;
    }
    public List<HexagonControl> Dijkstra(Graph graph)
    {        
        var notVisited = graph.GetListNodes();

        var track = new Dictionary<Node, DijkstraData>();
        track[notVisited[0]] = new DijkstraData { Previous = null, Price = 0 };
        while (true)
        {
            Node toOpen = null;
            float priceToOpen = float.PositiveInfinity;
            foreach (var v in notVisited)
            {
                if (track.ContainsKey(v) && track[v].Price < priceToOpen)
                {
                    toOpen = v;
                    priceToOpen = track[v].Price;
                }
            }

            if (toOpen == null)
            {
                //Debug.Log("Disconnected graph");

                return null;
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
                if (!track.ContainsKey(nextNode) || track[nextNode].Price > currenPraise)
                {
                    track[nextNode] = new DijkstraData { Price = currenPraise, Previous = toOpen };
                }
            }
            notVisited.Remove(toOpen);
        }

        var result = new List<HexagonControl>();
        Node end = graph[graph.Length - 1];
        while (end != null)
        {
            result.Add(end.NodeHexagon);
            end = track[end].Previous;
        }
        result.Reverse();
        //Debug.Log(result.Count);
        return result;
    }
}
