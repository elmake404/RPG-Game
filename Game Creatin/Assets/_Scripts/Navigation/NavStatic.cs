using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NavStatic
{
    private static AlgorithmDijkstra _algorithmDijkstra = new AlgorithmDijkstra();


    private static List<HexagonControl> GetBending(List<HexagonControl> hexagonsBending, IMove EnemyTarget, IMove moveMain)// запускает алгоритм дейкстра и возвращает точки через которые надо пройти 
    {
        List<HexagonControl> bendingVertex = _algorithmDijkstra.Dijkstra(CreatingEdgeBending(hexagonsBending, EnemyTarget, moveMain));

        if (bendingVertex != null)
        {
            bendingVertex.Remove(bendingVertex[0]);
            //bendingVertex.Remove(bendingVertex[bendingVertex.Count - 1]);
        }

        return bendingVertex;
    }
    private static List<HexagonControl> EmploymentCheck(List<IMove> WhoBothers, IMove EnemyTarget, IMove moveMain)
    {
        List<HexagonControl> hexagons = new List<HexagonControl>();
        List<IMove> ObjAbove = new List<IMove>();
        List<IMove> ObjAboveNew = new List<IMove>();
        ObjAbove.AddRange(WhoBothers);
        ObjAboveNew.AddRange(WhoBothers);
        while (ObjAboveNew.Count > 0)
        {
            List<HexagonControl> HexAboven = new List<HexagonControl>();
            HexAboven.AddRange(ObjAboveNew[0].GetSurroundingHexes());

            for (int i = 0; i < HexAboven.Count; i++)
            {
                if (EnemyTarget != null && HexAboven[i].ObjAbove == EnemyTarget || moveMain == HexAboven[i].ObjAbove)
                {
                    hexagons.Add(HexAboven[i]);
                    continue;
                }

                if (HexAboven[i].ObjAbove != null)
                {
                    if (ObjAbove.IndexOf(HexAboven[i].ObjAbove) == -1)
                    {
                        ObjAbove.Add(HexAboven[i].ObjAbove);
                        ObjAboveNew.Add(HexAboven[i].ObjAbove);
                    }
                }
                else
                {
                    hexagons.Add(HexAboven[i]);
                }
            }
            ObjAboveNew.Remove(ObjAboveNew[0]);
        }
        return hexagons;
    }
    public static List<HexagonControl> PathCheck(List<HexagonControl> nitialPath, IMove EnemyTarget, IMove moveMain)
    {
        List<HexagonControl> ProvenPath = new List<HexagonControl>();
        List<IMove> ListMoves = new List<IMove>();
        for (int i = 0; i < nitialPath.Count - 1; i++)
        {
            HexagonControl[] controls;
            Vector2 currentVector = nitialPath[i].GetArrayElement().position;
            Vector2 TargetPos = nitialPath[i + 1].GetArrayElement().position;

            while ((TargetPos - currentVector).magnitude > 0.1f)
            {
                controls = MapControl.GetPositionOnTheMap(TargetPos, currentVector);
                for (int j = 0; j < controls.Length; j++)
                {
                    if (controls[j] == nitialPath[0] || controls[j] == nitialPath[nitialPath.Count - 1])
                    {
                        continue;
                    }

                    if (controls[j].GetHexagonMain().ObjAbove != null && !ListMoves.Contains(controls[j].GetHexagonMain().ObjAbove))
                    {
                        ListMoves.Add(controls[j].GetHexagonMain().ObjAbove);
                    }
                }
                currentVector = Vector2.MoveTowards(currentVector, TargetPos, 0.4f);
            }

        }

        if (ListMoves.Count != 0)
        {
            List<HexagonControl> hexagonControls = new List<HexagonControl>();
            hexagonControls.AddRange(nitialPath);
            hexagonControls.Remove(hexagonControls[hexagonControls.Count - 1]);
            hexagonControls.AddRange(EmploymentCheck(ListMoves, EnemyTarget, moveMain));
            hexagonControls.Add(nitialPath[nitialPath.Count - 1]);
            hexagonControls = GetBending(hexagonControls, EnemyTarget, moveMain); ;

            if (hexagonControls != null)
            {
                ProvenPath.AddRange(hexagonControls);
                return ProvenPath;
            }
            else
            {
                nitialPath.Remove(nitialPath[0]);
                return nitialPath;
            }
        }
        else
        {
            nitialPath.Remove(nitialPath[0]);
            return nitialPath;
        }
    }
    private static Graph CreatingEdgeBending(List<HexagonControl> listHexagons, IMove EnemyTarget, IMove moveMain)//выстраивает ребра в графе 
    {
        var graph = new Graph(listHexagons);

        for (int i = 0; i < graph.Length - 1; i++)
        {
            for (int j = i + 1; j < graph.Length; j++)
            {
                bool IsElevation;

                Vector2 StartPosition = graph[i].NodeHexagon.GetArrayElement().position;
                Vector2 direction = graph[j].NodeHexagon.GetArrayElement().position;

                bool NoRibs = false;
                HexagonControl node = graph[j].NodeHexagon.Elevation != null ?
                    graph[j].NodeHexagon.Elevation : graph[j].NodeHexagon;

                if (listHexagons[i].TypeHexagon <= 0 || (listHexagons[i].TypeHexagon == 3 && node.layer != 10))
                {
                    IsElevation = false;
                    if (!CollisionCheck(StartPosition, direction, IsElevation, listHexagons[0], EnemyTarget))
                    {
                        NoRibs = true;
                    }
                }
                else
                {
                    IsElevation = true;
                    if (!CollisionCheckElevation(StartPosition, direction, IsElevation, listHexagons[0], EnemyTarget))
                    {
                        NoRibs = true;
                    }
                }

                if (!NoRibs)
                {
                    float magnitude = (graph[i].NodeHexagon.position - graph[j].NodeHexagon.position).magnitude;
                    graph[i].Connect(graph[j], magnitude, null);
                }
            }
        }
        return graph;
    }
    public static bool CollisionCheck(Vector2 StartPos, Vector2 TargetPos, bool elevation, HexagonControl StartHex, IMove EnemyTarget)//возыращет true если на пути нет припятсвий (пол)
    {
        HexagonControl[] controls;
        Vector2 currentVector = StartPos;

        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            controls = MapControl.GetPositionOnTheMap(TargetPos, currentVector);
            for (int i = 0; i < controls.Length; i++)
            {
                if (controls[i] == StartHex ||
                    (EnemyTarget != null && controls[i].ObjAbove == EnemyTarget))
                {
                    continue;
                }
                if (!controls[i].FreedomTestType(elevation) || (!controls[i].IsFree) && !controls[i].ObjAbove.IsGo())
                {
                    return false;
                }

            }
            currentVector = Vector2.MoveTowards(currentVector, TargetPos, 0.4f);
        }
        return true;
    }
    public static bool CollisionCheckElevation(Vector2 StartPos, Vector2 TargetPos, bool elevation, HexagonControl StartHex, IMove EnemyTarget)//возыращет true если на пути нет припятсвий (возвышанность)
    {
        HexagonControl[] controls;

        Vector2 currentVector = StartPos;

        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            controls = MapControl.GetPositionOnTheMap(TargetPos, currentVector);
            for (int i = 0; i < controls.Length; i++)
            {
                if (controls[i].GetHexagonMain() == StartHex || (EnemyTarget != null && controls[i].GetHexagonMain().ObjAbove == EnemyTarget))
                {
                    continue;
                }

                if (!controls[i].GetHexagonMain().FreedomTestType(elevation) || (!controls[i].GetHexagonMain().IsFree) && !controls[i].GetHexagonMain().ObjAbove.IsGo())
                {
                    return false;
                }
            }
            currentVector = Vector2.MoveTowards(currentVector, TargetPos, 0.4f);
        }

        return true;
    }

    #region Fly
    private static List<HexagonControl> GetBendingFly(List<HexagonControl> hexagonsBending, List<IMove> enemyList, IMove EnemyTarget)// запускает алгоритм дейкстра и возвращает точки через которые надо пройти 
    {
        List<HexagonControl> bendingVertex = _algorithmDijkstra.Dijkstra(CreatingEdgeBendingFly(hexagonsBending, EnemyTarget, enemyList));

        if (bendingVertex != null)
        {
            bendingVertex.Remove(bendingVertex[0]);
            //bendingVertex.Remove(bendingVertex[bendingVertex.Count - 1]);
        }

        return bendingVertex;
    }
    private static List<HexagonControl> EmploymentCheckFly(List<IMove> WhoBothers, IMove EnemyTarget, out List<IMove> Above, IMove moveMain)
    {
        List<HexagonControl> hexagons = new List<HexagonControl>();
        Above = new List<IMove>();
        List<IMove> ObjAbove = new List<IMove>();
        List<IMove> ObjAboveNew = new List<IMove>();
        ObjAbove.AddRange(WhoBothers);
        ObjAboveNew.AddRange(WhoBothers);
        while (ObjAboveNew.Count > 0)
        {
            List<HexagonControl> HexAboven = new List<HexagonControl>();
            HexAboven.AddRange(ObjAboveNew[0].GetSurroundingHexes());

            for (int i = 0; i < HexAboven.Count; i++)
            {
                if (EnemyTarget != null && HexAboven[i].ObjAboveFly == EnemyTarget || moveMain == HexAboven[i].ObjAboveFly)
                {
                    if (moveMain != HexAboven[i].ObjAboveFly)
                        hexagons.Add(HexAboven[i]);
                    continue;
                }

                if (HexAboven[i].ObjAboveFly != null)
                {
                    if (ObjAbove.IndexOf(HexAboven[i].ObjAboveFly) == -1)
                    {
                        ObjAbove.Add(HexAboven[i].ObjAboveFly);
                        ObjAboveNew.Add(HexAboven[i].ObjAboveFly);
                    }
                }
                else
                {
                    hexagons.Add(HexAboven[i]);
                }
            }
            ObjAboveNew.Remove(ObjAboveNew[0]);
        }
        Above.AddRange(ObjAbove);
        return hexagons;
    }
    public static List<HexagonControl> PathCheckBypassFly(List<HexagonControl> nitialPath, IMove Collision, IMove EnemyTarget, IMove moveMain)
    {
        List<HexagonControl> ProvenPath = new List<HexagonControl>();

        if (Collision == null)
        {
            Debug.LogError("Collision==null");
        }

        List<HexagonControl> hexagonControls = new List<HexagonControl>();
        hexagonControls.AddRange(nitialPath);
        hexagonControls.InsertRange(1, EmploymentCheckFly(new List<IMove>() { Collision }, EnemyTarget, out List<IMove> ListMoves, moveMain));
        hexagonControls = GetBendingFly(hexagonControls, ListMoves, EnemyTarget);

        if (hexagonControls != null)
        {
            ProvenPath.AddRange(hexagonControls);
            return ProvenPath;
        }
        else
        {
            return ProvenPath;
        }
    }
    private static Graph CreatingEdgeBendingFly(List<HexagonControl> listHexagons, IMove EnemyTarget, List<IMove> EnemyList)//выстраивает ребра в графе 
    {
        var graph = new Graph(listHexagons);

        for (int i = 0; i < graph.Length - 1; i++)
        {
            for (int j = i + 1; j < graph.Length; j++)
            {
                bool IsElevation;

                Vector2 StartPosition = graph[i].NodeHexagon.GetArrayElement().position;
                Vector2 direction = graph[j].NodeHexagon.GetArrayElement().position;

                bool NoRibs = false;
                HexagonControl node = graph[j].NodeHexagon.GetHexagonMain();

                if (!CollisionCheckElevationFly(StartPosition, direction, listHexagons[0], EnemyTarget, EnemyList))
                {
                    NoRibs = true;
                }

                if (i == 0 && graph[j].NodeHexagon == listHexagons[listHexagons.Count - 1]&&!NoRibs)
                {
                    graph[i].NodeHexagon.Flag();
                    graph[j].NodeHexagon.Flag();
                    Debug.Log(EnemyList.Count);
                }

                if (!NoRibs)
                {
                    float magnitude = (graph[i].NodeHexagon.position - graph[j].NodeHexagon.position).magnitude;
                    graph[i].Connect(graph[j], magnitude, null);
                }
            }
        }
        //List<Node> f = graph[0].IncidentNodes();
        //foreach (var item in f)
        //{
        //    if (item.NodeHexagon == listHexagons[listHexagons.Count - 1])
        //    {
        //        item.NodeHexagon.Flag();
        //        graph[0].NodeHexagon.Flag();
        //        Debug.Log(123456);
        //    }
        //}
        return graph;
    }
    public static bool CollisionCheckFly(Vector2 StartPos, Vector2 TargetPos, bool elevation, HexagonControl StartHex, IMove EnemyTarget, List<IMove> EnemyList,bool g)//возыращет true если на пути нет припятсвий не все враги препятсвие (пол)
    {
        HexagonControl[] controls;
        Vector2 currentVector = StartPos;

        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            controls = MapControl.GetPositionOnTheMap(TargetPos, currentVector);
            for (int i = 0; i < controls.Length; i++)
            {
                if (controls[i] == StartHex || (EnemyTarget != null && controls[i].ObjAboveFly == EnemyTarget))
                {
                    continue;
                }

                if ((!controls[i].IsFreeFly) 
                    && (EnemyList.Contains(controls[i].ObjAboveFly)) && !controls[i].ObjAboveFly.IsGo())
                {
                    return false;
                }
            }
            currentVector = Vector2.MoveTowards(currentVector, TargetPos, 0.4f);
        }
        return true;
    }
    public static bool CollisionCheckElevationFly(Vector2 StartPos, Vector2 TargetPos, HexagonControl StartHex, IMove EnemyTarget, List<IMove> EnemyList)//возыращет true если на пути нет припятсвий не все враги препятсвие (возвышанность)
    {
        HexagonControl[] controls;

        Vector2 currentVector = StartPos;

        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            controls = MapControl.GetPositionOnTheMap(TargetPos, currentVector);
            for (int i = 0; i < controls.Length; i++)
            {
                if (controls[i].GetHexagonMain() == StartHex || (EnemyTarget != null && controls[i].GetHexagonMain().ObjAboveFly == EnemyTarget))
                {
                    continue;
                }

                if ((!controls[i].GetHexagonMain().IsFreeFly)
                    && (EnemyList.Contains(controls[i].GetHexagonMain().ObjAboveFly)) && !controls[i].GetHexagonMain().ObjAboveFly.IsGo())
                {
                    return false;
                }
            }
            currentVector = Vector2.MoveTowards(currentVector, TargetPos, 0.4f);
        }

        return true;
    }

    #endregion

    #region BypassCollicion
    public static List<HexagonControl> PathCheckBypass(List<HexagonControl> nitialPath, IMove Collision, IMove EnemyTarget, IMove moveMain)
    {
        List<HexagonControl> ProvenPath = new List<HexagonControl>();
        List<IMove> ListMoves;

        if (Collision == null)
        {
            Debug.LogError("Collision==null");
        }

        List<HexagonControl> hexagonControls = new List<HexagonControl>();
        hexagonControls.AddRange(nitialPath);
        hexagonControls.InsertRange(1, EmploymentCheck(new List<IMove>() { Collision }, EnemyTarget, out ListMoves, moveMain));
        hexagonControls = GetBending(hexagonControls, ListMoves, EnemyTarget);

        if (hexagonControls != null)
        {
            ProvenPath.AddRange(hexagonControls);
            return ProvenPath;
        }
        else
        {
            return ProvenPath;
        }
    }
    private static List<HexagonControl> EmploymentCheck(List<IMove> WhoBothers, IMove EnemyTarget, out List<IMove> Above, IMove moveMain)
    {
        List<HexagonControl> hexagons = new List<HexagonControl>();
        Above = new List<IMove>();
        List<IMove> ObjAbove = new List<IMove>();
        List<IMove> ObjAboveNew = new List<IMove>();
        ObjAbove.AddRange(WhoBothers);
        ObjAboveNew.AddRange(WhoBothers);
        while (ObjAboveNew.Count > 0)
        {
            List<HexagonControl> HexAboven = new List<HexagonControl>();
            HexAboven.AddRange(ObjAboveNew[0].GetSurroundingHexes());

            for (int i = 0; i < HexAboven.Count; i++)
            {
                if (EnemyTarget != null && HexAboven[i].ObjAbove == EnemyTarget || moveMain == HexAboven[i].ObjAbove)
                {
                    if (moveMain != HexAboven[i].ObjAbove)
                        hexagons.Add(HexAboven[i]);
                    continue;
                }

                if (HexAboven[i].ObjAbove != null)
                {
                    if (ObjAbove.IndexOf(HexAboven[i].ObjAbove) == -1)
                    {
                        ObjAbove.Add(HexAboven[i].ObjAbove);
                        ObjAboveNew.Add(HexAboven[i].ObjAbove);
                    }
                }
                else
                {
                    hexagons.Add(HexAboven[i]);
                }
            }
            ObjAboveNew.Remove(ObjAboveNew[0]);
        }
        Above.AddRange(ObjAbove);
        return hexagons;
    }
    private static List<HexagonControl> GetBending(List<HexagonControl> hexagonsBending, List<IMove> enemyList, IMove EnemyTarget)// запускает алгоритм дейкстра и возвращает точки через которые надо пройти 
    {
        List<HexagonControl> bendingVertex = _algorithmDijkstra.Dijkstra(CreatingEdgeBending(hexagonsBending, EnemyTarget, enemyList));

        if (bendingVertex != null)
        {
            bendingVertex.Remove(bendingVertex[0]);
            //bendingVertex.Remove(bendingVertex[bendingVertex.Count - 1]);
        }

        return bendingVertex;
    }
    private static Graph CreatingEdgeBending(List<HexagonControl> listHexagons, IMove EnemyTarget, List<IMove> EnemyList)//выстраивает ребра в графе 
    {
        var graph = new Graph(listHexagons);

        for (int i = 0; i < graph.Length - 1; i++)
        {
            for (int j = i + 1; j < graph.Length; j++)
            {
                bool IsElevation;

                Vector2 StartPosition = graph[i].NodeHexagon.GetArrayElement().position;
                Vector2 direction = graph[j].NodeHexagon.GetArrayElement().position;

                bool NoRibs = false;
                HexagonControl node = graph[j].NodeHexagon.Elevation != null ?
                    graph[j].NodeHexagon.Elevation : graph[j].NodeHexagon;

                if (listHexagons[i].TypeHexagon <= 0 || (listHexagons[i].TypeHexagon == 3 && node.layer != 10))
                {
                    IsElevation = false;
                    if (!CollisionCheck(StartPosition, direction, IsElevation, listHexagons[0], EnemyTarget, EnemyList))
                    {
                        NoRibs = true;
                    }
                }
                else
                {
                    IsElevation = true;
                    if (!CollisionCheckElevation(StartPosition, direction, IsElevation, listHexagons[0], EnemyTarget, EnemyList))
                    {
                        NoRibs = true;
                    }
                }

                if (!NoRibs)
                {
                    float magnitude = (graph[i].NodeHexagon.position - graph[j].NodeHexagon.position).magnitude;
                    graph[i].Connect(graph[j], magnitude, null);
                }
            }
        }
        return graph;
    }
    public static bool CollisionCheckElevation(Vector2 StartPos, Vector2 TargetPos, bool elevation, HexagonControl StartHex, IMove EnemyTarget, List<IMove> EnemyList)//возыращет true если на пути нет припятсвий не все враги препятсвие (возвышанность)
    {
        HexagonControl[] controls;

        Vector2 currentVector = StartPos;

        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            controls = MapControl.GetPositionOnTheMap(TargetPos, currentVector);
            for (int i = 0; i < controls.Length; i++)
            {
                if (controls[i].GetHexagonMain() == StartHex || (EnemyTarget != null && controls[i].GetHexagonMain().ObjAbove == EnemyTarget))
                {
                    continue;
                }

                if ((!controls[i].GetHexagonMain().FreedomTestType(elevation)) ||
                    (!controls[i].GetHexagonMain().IsFree) && (EnemyList.Contains(controls[i].GetHexagonMain().ObjAbove)) & !controls[i].GetHexagonMain().ObjAbove.IsGo())
                {
                    return false;
                }
            }
            currentVector = Vector2.MoveTowards(currentVector, TargetPos, 0.4f);
        }

        return true;
    }
    public static bool CollisionCheck(Vector2 StartPos, Vector2 TargetPos, bool elevation, HexagonControl StartHex, IMove EnemyTarget, List<IMove> EnemyList)//возыращет true если на пути нет припятсвий не все враги препятсвие (пол)
    {
        HexagonControl[] controls;
        Vector2 currentVector = StartPos;

        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            controls = MapControl.GetPositionOnTheMap(TargetPos, currentVector);
            for (int i = 0; i < controls.Length; i++)
            {
                if (controls[i] == StartHex || (EnemyTarget != null && controls[i].ObjAbove == EnemyTarget))
                {
                    continue;
                }

                if ((!controls[i].FreedomTestType(elevation)) ||
                    (!controls[i].IsFree) && (EnemyList.Contains(controls[i].ObjAbove)) && !controls[i].ObjAbove.IsGo())
                {
                    return false;
                }

            }
            currentVector = Vector2.MoveTowards(currentVector, TargetPos, 0.4f);
        }
        return true;
    }

    #endregion
}
