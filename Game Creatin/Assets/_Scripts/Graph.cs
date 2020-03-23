using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Edge
{
    public readonly Node From;
    public readonly Node To;
    public readonly float Price;
    public Edge(Node first, Node second, float price)
    {
        From = first;
        To = second;
        Price = price;
    }
    public bool IsIncident(Node node)
    {
        return From == node || To == node;
    }
    public Node OtherNode(Node node)
    {
        if (!IsIncident(node)) 
            throw new ArgumentException();
        if (From == node) 
            return To;
        return From;
    }
}
public class Node
{
    private readonly List<Edge> incidentEdge = new List<Edge>();
    public readonly int NodeNumber;
    public readonly HexagonControl NodeHexagon;

    public Node(int number,HexagonControl main)
    {
        NodeNumber = number;
        NodeHexagon = main;
    }

    public List<Node> IncidentNodes()
    {
        List<Node> nodes = new List<Node>();
        for (int i = 0; i < incidentEdge.Count; i++)
        {
            nodes.Add(incidentEdge[i].OtherNode(this));
        }
        return nodes;
    }
    public List<Edge> IncidentEdge()
    {
        return incidentEdge;
    }

    public void Connect(Node node,float magnitude)
    {
        Edge edge = new Edge(this,node,magnitude);
        incidentEdge.Add(edge);
        node.incidentEdge.Add(edge);
    }
}

public class Graph
{
    private Node[] nodes;
    public Graph(List<HexagonControl>ListHexagon)
    {
        nodes = new Node[ListHexagon.Count];

        for (int i = 0; i < ListHexagon.Count; i++)
        {
            nodes[i] = new Node(i, ListHexagon[i]);
        }
    }

    public int Length
    {
        get { return nodes.Length; }
    }

    public Node this[int index]
    {
        get
        { return nodes[index]; }
    }
    public List<Node> GetListNodes()
    {
        List<Node> nodesList = new List<Node>();
        nodesList.AddRange(nodes);
        return nodesList;
    }
    public IEnumerable<Node> Nodes
    {
        get
        {
            foreach (var node in nodes) yield return node;
        }
    }
}
