using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class Edge
{
    public readonly Node From;
    public readonly Node To;
    public readonly List<HexagonControl> BendingPoints;
    public float Price { get; private set; }

    public Edge(Node first, Node second, float price, List<HexagonControl> bendingPoints)
    {
        From = first;
        To = second;
        Price = price;
        BendingPoints = bendingPoints;
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
    public void Revalue (float NewPrice)
    {
        Price = NewPrice;
    }
}
public class Node
{
    private readonly List<Edge> incidentEdge = new List<Edge>();

    public List<Node> ListUnrelated = new List<Node>();
    public int NodeNumber;
    public readonly HexagonControl NodeHexagon;

    public Node(int number,HexagonControl main)
    {
        NodeNumber = number;
        NodeHexagon = main;
    }
    public List<HexagonControl> GetHexagonsBending(Node nodeNext)
    {
        List<HexagonControl> getList = null;
        for (int i = 0; i < incidentEdge.Count; i++)
        {
            if (incidentEdge[i].To == nodeNext)
            {
                getList = incidentEdge[i].BendingPoints;
            } 
        }
        return getList;
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
    public int incidentEdgeCount()
    {
        return incidentEdge.Count;
    }
    public void Connect(Node node,float magnitude,List<HexagonControl> bendingPoints)
    {
        Edge edge = new Edge(this,node,magnitude,bendingPoints);
        incidentEdge.Add(edge);
        node.incidentEdge.Add(edge);
    }
    public void RevalueEdge(int index,float priace)
    {
        incidentEdge[index].Revalue(priace);
    }
}

public class Graph
{
    private List<Node> nodes;
    public Graph(List<HexagonControl>ListHexagon)
    {
        nodes = new List<Node>();

        for (int i = 0; i < ListHexagon.Count; i++)
        {
            nodes.Add(new Node(i, ListHexagon[i]));
        }
    }
    public Graph(Graph graph)
    {
        nodes = new List<Node>();

        for (int i = 0; i < graph.Length; i++)
        {
            nodes.Add(graph[i]);
        }
    }
    public void AddNode(HexagonControl newNode)
    {
        nodes.Add(new Node(nodes.Count-1, newNode));
    }
    public void AddNodeFirst(HexagonControl newNode)
    {
        nodes.Insert(0, new Node(0, newNode)); 
        for (int i = 1; i < nodes.Count; i++)
        {
            nodes[i].NodeNumber += 1;
        }
    }

    public int Length
    {
        get { return nodes.Count; }
    }

    public Node this[int index]
    {
        get
        { return nodes[index]; }
    }
    public List<Node> GetListNodes()
    {
        return nodes;
    }
    public IEnumerable<Node> Nodes
    {
        get
        {
            foreach (var node in nodes) yield return node;
        }
    }
}
