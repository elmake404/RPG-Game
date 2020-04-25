using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapControlStatic
{
    private static AlgorithmDijkstra _algorithmDijkstra = new AlgorithmDijkstra();

    public static Vector2 MapPos;
    public static HexagonControl[,] mapNav = new HexagonControl[9, 20];//масив содержащий все 6-ти угольники
    public static Graph GraphStatic;// граф в котором соеденены все ребра 
    public static float X, Y;

    private static List<HexagonControl> GetBending(List<HexagonControl> hexagonsBending, IMove iMove, out float Mag)// запускает алгоритм дейкстра и возвращает точки через которые надо пройти 
    {
        List<HexagonControl> bendingVertex = _algorithmDijkstra.Dijkstra(CreatingEdgeBending(hexagonsBending, iMove), out Mag);

        if (bendingVertex != null)
        {
            bendingVertex.Remove(bendingVertex[0]);
            bendingVertex.Remove(bendingVertex[bendingVertex.Count - 1]);
        }

        return bendingVertex;
    }
    public static bool CollisionCheck(Vector2 StartPos, Vector2 TargetPos, bool elevation)//возыращет true если на пути нет припятсвий (пол)
    {
        HexagonControl[] controls = null;
        Vector2 currentVector = StartPos;

        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            controls = GetPositionOnTheMap(TargetPos.x - currentVector.x, currentVector);
            for (int i = 0; i < controls.Length; i++)
            {
                Vector2 PosHex = controls[i].transform.position;
                if ((PosHex - currentVector).magnitude <= 1.8)
                {
                    //controls.Flag();
                    if (!controls[i].FreedomTestType(elevation))
                    {
                        return false;
                    }
                }
            }
            currentVector = Vector2.MoveTowards(currentVector, TargetPos, 0.4f);
        }
        return true;
    }
    public static bool CollisionCheck(out float disteins, Vector2 StartPos, Vector2 TargetPos, bool elevation, IMove iMove)//возыращет true если на пути нет припятсвий , враги и герои тоже препятствие(пол) 
    {
        HexagonControl[] controls;
        Vector2 currentVector = StartPos;

        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            controls = GetPositionOnTheMap(TargetPos.x - currentVector.x, currentVector);
            for (int i = 0; i < controls.Length; i++)
            {
                Vector2 PosHex = controls[i].transform.position;
                if ((PosHex - currentVector).magnitude <= 1.8)
                {
                    if (!controls[i].FreedomTestType(elevation))
                    {
                        disteins = float.PositiveInfinity;
                        return false;
                    }

                    if (iMove.GetEnemy() == null)
                    {
                        if ((!controls[i].IsFree) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetHero() != iMove.GetHero()))
                        {
                            disteins = float.PositiveInfinity;
                            return false;
                        }
                    }
                    else
                    {
                        if ((!controls[i].IsFree) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetEnemy() != iMove.GetEnemy()))
                        {
                            disteins = float.PositiveInfinity;
                            return false;
                        }
                    }
                }
            }
            currentVector = Vector2.MoveTowards(currentVector, TargetPos, 1f);
        }
        disteins = (StartPos - TargetPos).magnitude;

        return true;
    }
    public static void CollisionCheck(out float disteins, Node from, Node to, bool elevation, IMove iMove)//соеденяет ребра если на пути нет припятсвий , обход врагов и героев (пол) 
    {
        Vector2 TargetPos = to.NodeHexagon.transform.position;
        HexagonControl[] controls;
        List<HexagonControl> hexagonsBending = new List<HexagonControl>();
        Vector2 currentVector = from.NodeHexagon.transform.position;
        List<IMove> ListOblMove = new List<IMove>();
        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            controls = GetPositionOnTheMap(TargetPos.x - currentVector.x, currentVector);
            for (int i = 0; i < controls.Length; i++)
            {
                Vector2 PosHex = controls[i].transform.position;
                if ((PosHex - currentVector).magnitude <= 1.8)
                {
                    if (!controls[i].FreedomTestType(elevation))
                    {
                        disteins = float.PositiveInfinity;
                        return;
                    }

                    if (iMove.GetEnemy() == null)
                    {
                        if ((!controls[i].IsFree) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetHero() != iMove.GetHero() && ListOblMove.IndexOf(iMove) == -1))
                        {
                            hexagonsBending.AddRange(controls[i].ObjAbove.GetSurroundingHexes());
                            ListOblMove.Add(iMove);
                        }
                    }
                    else
                    {
                        if ((!controls[i].IsFree) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetEnemy() != iMove.GetEnemy() && ListOblMove.IndexOf(iMove) == -1))
                        {
                            hexagonsBending.AddRange(controls[i].ObjAbove.GetSurroundingHexes());
                        }
                    }
                }
            }
            currentVector = Vector2.MoveTowards(currentVector, TargetPos, 1f);
        }

        Vector2 StartPos = from.NodeHexagon.transform.position;
        disteins = (StartPos - TargetPos).magnitude;

        if (hexagonsBending.Count != 0)
        {
            hexagonsBending.Insert(0, from.NodeHexagon);
            hexagonsBending.Add(to.NodeHexagon);
            List<HexagonControl> Vertex = GetBending(hexagonsBending, iMove, out float Magnitude);

            if (Vertex != null)
            {
                from.Connect(to, Magnitude, Vertex);
            }
        }
        else
        {
            from.Connect(to, disteins, null);
        }
    }
    public static float CollisionCheckRepeated(Node from, Node to, bool elevation, IMove iMove)//возыращет disteins если на пути нет припятсвий или float.PositiveInfinity , обход врагов и героев (пол) 
    {
        float disteins;
        Vector2 TargetPos = to.NodeHexagon.transform.position;
        HexagonControl[] controls;
        List<HexagonControl> hexagonsBending = new List<HexagonControl>();
        Vector2 currentVector = from.NodeHexagon.transform.position;
        List<IMove> ListOblMove = new List<IMove>();
        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            controls = GetPositionOnTheMap(TargetPos.x - currentVector.x, currentVector);
            for (int i = 0; i < controls.Length; i++)
            {
                Vector2 PosHex = controls[i].transform.position;
                if ((PosHex - currentVector).magnitude <= 1.8)
                {
                    if (!controls[i].FreedomTestType(elevation))
                    {
                        return float.PositiveInfinity;
                    }

                    if (iMove.GetEnemy() == null)
                    {
                        if ((!controls[i].IsFree) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetHero() != iMove.GetHero() && ListOblMove.IndexOf(iMove) == -1))
                        {
                            hexagonsBending.AddRange(controls[i].ObjAbove.GetSurroundingHexes());
                            ListOblMove.Add(iMove);
                        }
                    }
                    else
                    {
                        if ((!controls[i].IsFree) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetEnemy() != iMove.GetEnemy() && ListOblMove.IndexOf(iMove) == -1))
                        {
                            hexagonsBending.AddRange(controls[i].ObjAbove.GetSurroundingHexes());
                        }
                    }
                }
            }
            currentVector = Vector2.MoveTowards(currentVector, TargetPos, 1f);
        }

        Vector2 StartPos = from.NodeHexagon.transform.position;
        disteins = (StartPos - TargetPos).magnitude;

        if (hexagonsBending.Count != 0)
        {
            hexagonsBending.Insert(0, from.NodeHexagon);
            hexagonsBending.Add(to.NodeHexagon);
            List<HexagonControl> Vertex = GetBending(hexagonsBending, iMove, out float Magnitude);

            if (Vertex == null)
            {
                return float.PositiveInfinity;
            }
            else
            {
                return Magnitude;
            }
        }
        else
        {
            return disteins;
        }
    }
    public static bool CollisionCheckElevation(Vector2 StartPos, Vector2 TargetPos, bool elevation)//возыращет true если на пути нет припятсвий (возвышанность)
    {
        HexagonControl[] controls = null;

        Vector2 currentVector = StartPos;

        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            controls = GetPositionOnTheMap(TargetPos.x - currentVector.x, currentVector);
            for (int i = 0; i < controls.Length; i++)
            {
                Vector2 PosHex = controls[i].transform.position;
                if ((PosHex - currentVector).magnitude <= 1.8)
                {
                    if (controls[i].Elevation != null)
                    {
                        if (!controls[i].Elevation.FreedomTestType(elevation))
                        {
                            Debug.Log(elevation);

                            return false;
                        }
                    }
                    else
                    {
                        if (!controls[i].FreedomTestType(elevation))
                        {
                            return false;
                        }
                    }
                }
            }
            currentVector = Vector2.MoveTowards(currentVector, TargetPos, 0.4f);
        }

        return true;
    }
    public static bool CollisionCheckElevation(out float disteins, Vector2 StartPos, Vector2 TargetPos, bool elevation, IMove iMove)//возыращет true если на пути нет припятсвий, враги и герои тоже препятствие (возвышанность)
    {
        HexagonControl[] controls;

        Vector2 currentVector = StartPos;

        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            controls = GetPositionOnTheMap(TargetPos.x - currentVector.x, currentVector);

            for (int i = 0; i < controls.Length; i++)
            {
                Vector2 PosHex = controls[i].transform.position;
                if ((PosHex - currentVector).magnitude <= 1.8)
                {
                    if (controls[i].Elevation != null)
                    {
                        if (!controls[i].Elevation.FreedomTestType(elevation))
                        {
                            disteins = float.PositiveInfinity;
                            return false;
                        }

                        if (iMove.GetEnemy() == null)
                        {
                            if ((!controls[i].Elevation.IsFree) && (!controls[i].Elevation.ObjAbove.IsGo() && controls[i].Elevation.ObjAbove.GetHero() != iMove.GetHero()))
                            {
                                disteins = float.PositiveInfinity;
                                return false;
                            }
                        }
                        else
                        {
                            if ((!controls[i].Elevation.IsFree) && (!controls[i].Elevation.ObjAbove.IsGo() && controls[i].Elevation.ObjAbove.GetEnemy() != iMove.GetEnemy()))
                            {
                                disteins = float.PositiveInfinity;
                                return false;
                            }
                        }

                    }
                    else
                    {
                        if (!controls[i].FreedomTestType(elevation))
                        {
                            disteins = float.PositiveInfinity;
                            return false;
                        }

                        if (iMove.GetEnemy() == null)
                        {
                            if ((!controls[i].IsFree) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetHero() != iMove.GetHero()))
                            {
                                disteins = float.PositiveInfinity;
                                return false;
                            }
                        }
                        else
                        {
                            if ((!controls[i].IsFree) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetEnemy() != iMove.GetEnemy()))
                            {
                                disteins = float.PositiveInfinity;
                                return false;
                            }
                        }
                    }
                }
            }
            currentVector = Vector2.MoveTowards(currentVector, TargetPos, 1f);
        }
        disteins = (StartPos - TargetPos).magnitude;
        return true;
    }
    public static void CollisionCheckElevation(out float disteins, Node from, Node to, bool elevation, IMove iMove)//соеденяет ребра  если на пути нет припятсвий, обход врагов и героев (возвышанность)
    {
        Vector2 TargetPos = to.NodeHexagon.transform.position;
        HexagonControl[] controls;
        List<HexagonControl> hexagonsBending = new List<HexagonControl>();
        Vector2 currentVector = from.NodeHexagon.transform.position;

        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            controls = GetPositionOnTheMap(TargetPos.x - currentVector.x, currentVector);

            for (int i = 0; i < controls.Length; i++)
            {
                Vector2 PosHex = controls[i].transform.position;
                if ((PosHex - currentVector).magnitude <= 1.8)
                {
                    if (controls[i].Elevation != null)
                    {
                        if (!controls[i].Elevation.FreedomTestType(elevation))
                        {
                            disteins = float.PositiveInfinity;
                            return;
                        }

                        if (iMove.GetEnemy() == null)
                        {
                            if ((!controls[i].Elevation.IsFree) && (!controls[i].Elevation.ObjAbove.IsGo() && controls[i].Elevation.ObjAbove.GetHero() != iMove.GetHero()))
                            {
                                hexagonsBending.AddRange(controls[i].Elevation.ObjAbove.GetSurroundingHexes());
                            }
                        }
                        else
                        {
                            if ((!controls[i].Elevation.IsFree) && (!controls[i].Elevation.ObjAbove.IsGo() && controls[i].Elevation.ObjAbove.GetEnemy() != iMove.GetEnemy()))
                            {
                                hexagonsBending.AddRange(controls[i].Elevation.ObjAbove.GetSurroundingHexes());
                            }
                        }

                    }
                    else
                    {
                        if (!controls[i].FreedomTestType(elevation))
                        {
                            disteins = float.PositiveInfinity;
                            return;
                        }

                        if (iMove.GetEnemy() == null)
                        {
                            if ((!controls[i].IsFree) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetHero() != iMove.GetHero()))
                            {
                                hexagonsBending.AddRange(controls[i].ObjAbove.GetSurroundingHexes());
                            }
                        }
                        else
                        {
                            if ((!controls[i].IsFree) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetEnemy() != iMove.GetEnemy()))
                            {
                                hexagonsBending.AddRange(controls[i].ObjAbove.GetSurroundingHexes());
                            }
                        }
                    }
                }
            }
            currentVector = Vector2.MoveTowards(currentVector, TargetPos,1f);
        }
        Vector2 StartPos = from.NodeHexagon.transform.position;
        disteins = (StartPos - TargetPos).magnitude;

        if (hexagonsBending.Count != 0)
        {
            List<HexagonControl> newHex = new List<HexagonControl>();
            for (int i = 0; i < hexagonsBending.Count; i++)
            {
                if (hexagonsBending[i].Floor != null)
                {
                    newHex.Add(hexagonsBending[i].Floor);
                }
                else
                    newHex.Add(hexagonsBending[i]);
            }
            newHex.Insert(0, from.NodeHexagon);
            newHex.Add(to.NodeHexagon);
            List<HexagonControl> Vertex = GetBending(newHex, iMove, out float Magnitude);
            if (Vertex != null)
            {
                from.Connect(to, Magnitude, Vertex);
            }
        }
        else
        {
            from.Connect(to, disteins, null);
        }
    }
    public static float CollisionCheckElevationRepeated( Node from, Node to, bool elevation, IMove iMove)//возыращет disteins если на пути нет припятсвий или float.PositiveInfinity , обход врагов и героев (возвышанность)
    {
        float disteins;
        Vector2 TargetPos = to.NodeHexagon.transform.position;
        HexagonControl[] controls;
        List<HexagonControl> hexagonsBending = new List<HexagonControl>();
        Vector2 currentVector = from.NodeHexagon.transform.position;

        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            controls = GetPositionOnTheMap(TargetPos.x - currentVector.x, currentVector);

            for (int i = 0; i < controls.Length; i++)
            {
                Vector2 PosHex = controls[i].transform.position;
                if ((PosHex - currentVector).magnitude <= 1.8)
                {
                    if (controls[i].Elevation != null)
                    {
                        if (!controls[i].Elevation.FreedomTestType(elevation))
                        {
                            return float.PositiveInfinity;
                        }

                        if (iMove.GetEnemy() == null)
                        {
                            if ((!controls[i].Elevation.IsFree) && (!controls[i].Elevation.ObjAbove.IsGo() && controls[i].Elevation.ObjAbove.GetHero() != iMove.GetHero()))
                            {
                                hexagonsBending.AddRange(controls[i].Elevation.ObjAbove.GetSurroundingHexes());
                            }
                        }
                        else
                        {
                            if ((!controls[i].Elevation.IsFree) && (!controls[i].Elevation.ObjAbove.IsGo() && controls[i].Elevation.ObjAbove.GetEnemy() != iMove.GetEnemy()))
                            {
                                hexagonsBending.AddRange(controls[i].Elevation.ObjAbove.GetSurroundingHexes());
                            }
                        }

                    }
                    else
                    {
                        if (!controls[i].FreedomTestType(elevation))
                        {
                            return float.PositiveInfinity;
                        }

                        if (iMove.GetEnemy() == null)
                        {
                            if ((!controls[i].IsFree) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetHero() != iMove.GetHero()))
                            {
                                hexagonsBending.AddRange(controls[i].ObjAbove.GetSurroundingHexes());
                            }
                        }
                        else
                        {
                            if ((!controls[i].IsFree) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetEnemy() != iMove.GetEnemy()))
                            {
                                hexagonsBending.AddRange(controls[i].ObjAbove.GetSurroundingHexes());
                            }
                        }
                    }
                }
            }
            currentVector = Vector2.MoveTowards(currentVector, TargetPos, 1f);
        }
        Vector2 StartPos = from.NodeHexagon.transform.position;
        disteins = (StartPos - TargetPos).magnitude;

        if (hexagonsBending.Count != 0)
        {
            List<HexagonControl> newHex = new List<HexagonControl>();
            for (int i = 0; i < hexagonsBending.Count; i++)
            {
                if (hexagonsBending[i].Floor != null)
                {
                    newHex.Add(hexagonsBending[i].Floor);
                }
                else
                    newHex.Add(hexagonsBending[i]);
            }
            newHex.Insert(0, from.NodeHexagon);
            newHex.Add(to.NodeHexagon);
            List<HexagonControl> Vertex = GetBending(newHex, iMove, out float Magnitude);
            if (Vertex == null)
            {
                return float.PositiveInfinity;
            }
            else
            {
                return Magnitude;

            }
        }
        else
        {
            return disteins;
        }
    }
    public static HexagonControl[] GetPositionOnTheMap(float XTarget, Vector2 Position) // возвращает гексагон через позицию (может определить два блока)
    {
        float Y = (Position.y - MapPos.y) / 3f;
        int YMax = Mathf.Abs(Mathf.RoundToInt(Y));
        float Difference = 0;
        float f = ((Position.x - (MapPos.x - 3.46f)) / 1.73f);
        float R = Mathf.Floor(f) * 1.73f - (Position.x - (MapPos.x - 3.46f));
        int G = R % 2 == 0 ? -1 : 0;

        if (Mathf.Abs(YMax - Mathf.Abs(Y)) < 1 - (G + (0.3f + ((0.3 / 1.73) * Mathf.Abs(R)))))
        {
            Y = Mathf.Round(Mathf.Abs(Y));
        }
        else
        {
            if (YMax - Mathf.Abs(Y) > 0)
            {
                Y = Mathf.Floor(Mathf.Abs(Y));
            }
            else
            {
                Y = Mathf.Ceil(Mathf.Abs(Y));
            }
        }

        if ((Y % 2) != 0)
        {
            Difference = 0.5f;
        }
        float factor = (mapNav[0, 1].transform.position.x - mapNav[0, 0].transform.position.x);
        float X = ((Position.x - MapPos.x) / factor) + Difference;

        X = X > 0 ? X : 0;
        int XInt = Mathf.RoundToInt(X);
        if ((float)System.Math.Round((XInt - X), 2) == 0.5 && XTarget == 0)
        {
            HexagonControl[] hexagons = new HexagonControl[2];
            int namber = 0;
            if (XInt <= 19)
            {
                hexagons[namber] = mapNav[(int)Y, XInt];
                namber++;
            }
            if (XInt > 0)
            {
                if (namber == 0)
                    hexagons = new HexagonControl[1];

                hexagons[namber] = mapNav[(int)Y, XInt - 1];
            }
            return hexagons;
        }
        else
        {
            HexagonControl[] hexagons = new HexagonControl[1];
            hexagons[0] = mapNav[(int)Y, XInt];
            return hexagons;
        }
    }
    public static HexagonControl GetPositionOnTheMap(Vector2 Position)// возвращает гексагон через позицию
    {
        float Y = (Position.y - MapPos.y) / 3f;
        int YMax = Mathf.Abs(Mathf.RoundToInt(Y));
        float Difference = 0;
        float f = ((Position.x - (MapPos.x - 3.46f)) / 1.73f);
        float R = Mathf.Floor(f) * 1.73f - (Position.x - (MapPos.x - 3.46f));
        int G = R % 2 == 0 ? -1 : 0;

        if (Mathf.Abs(YMax - Mathf.Abs(Y)) < 1 - (G + (0.3f + ((0.3 / 1.73) * Mathf.Abs(R)))))
        {
            Y = Mathf.Round(Mathf.Abs(Y));
        }
        else
        {
            if (YMax - Mathf.Abs(Y) > 0)
            {
                Y = Mathf.Floor(Mathf.Abs(Y));
            }
            else
            {
                Y = Mathf.Ceil(Mathf.Abs(Y));
            }
        }

        if ((Y % 2) != 0)
        {
            Difference = 0.5f;
        }
        float factor = (mapNav[0, 1].transform.position.x - mapNav[0, 0].transform.position.x);
        float X = ((Position.x - MapPos.x) / factor) + Difference;

        X = X > 0 ? X : 0;
        int XInt = Mathf.RoundToInt(X);
        HexagonControl hexagons = mapNav[(int)Y, XInt];
        return hexagons;
    }
    public static HexagonControl FieldPosition(int layer, Vector2 position)//гексагон к которому принадлежит герой
    {
        Vector2 difference = layer == 8 ? Vector2.zero : new Vector2(X, Y);

        return GetPositionOnTheMap(position - difference).Elevation != null ?
           GetPositionOnTheMap(position - difference).Elevation : GetPositionOnTheMap(position - difference);//нужный 6-ти угольник  
    }
    public static Graph CreatingEdge(List<HexagonControl> listHexagons, IMove move)//выстраивает ребра в графе 
    {
        var graph = new Graph(listHexagons);

        for (int i = 0; i < graph.Length - 1; i++)
        {
            bool IsElevation = false;
            for (int j = i + 1; j < graph.Length; j++)
            {
                HexagonControl node = graph[j].NodeHexagon.Elevation != null ? graph[j].NodeHexagon.Elevation : graph[j].NodeHexagon;

                if (listHexagons[i].TypeHexagon <= 0 || (listHexagons[i].TypeHexagon == 3 && node.gameObject.layer != 10))
                {
                    IsElevation = false;
                    CollisionCheck(out float magnitude, graph[i], graph[j], IsElevation, move);
                }
                else
                {
                    IsElevation = true;
                    CollisionCheckElevation(out float magnitude, graph[i], graph[j], IsElevation, move);
                }
            }
        }
        return graph;
    }
    public static Graph CreatingEdgeBending(List<HexagonControl> listHexagons, IMove move)//выстраивает ребра в графе 
    {
        var graph = new Graph(listHexagons);

        for (int i = 0; i < graph.Length - 1; i++)
        {
            bool IsElevation = false;
            for (int j = i + 1; j < graph.Length; j++)
            {
                float magnitude;

                Vector2 StartPosition = graph[i].NodeHexagon.transform.position;
                Vector2 direction = graph[j].NodeHexagon.transform.position;

                bool NoRibs = false;
                HexagonControl node = graph[j].NodeHexagon.Elevation != null ? graph[j].NodeHexagon.Elevation : graph[j].NodeHexagon;

                if (listHexagons[i].TypeHexagon <= 0 || (listHexagons[i].TypeHexagon == 3 && node.gameObject.layer != 10))
                {
                    IsElevation = false;
                    if (!CollisionCheck(out magnitude, StartPosition, direction, IsElevation, move))
                    {
                        NoRibs = true;
                    }
                }
                else
                {
                    IsElevation = true;
                    if (!CollisionCheckElevation(out magnitude, StartPosition, direction, IsElevation, move))
                    {
                        NoRibs = true;
                    }
                }

                if (!NoRibs)
                {
                    graph[i].Connect(graph[j], magnitude, null);
                }
            }
        }

        return graph;
    }
    public static Graph CreatingEdge(Graph graph)//выстраивает 2 ребра в готовом графе(первое и последнее)
    {
        int NamberElement = -(graph.Length - 1);

        for (int i = 0; i < 2; i++)
        {
            NamberElement += (graph.Length - 1);
            bool IsElevation = false;

            for (int j = 0; j < graph.Length; j++)
            {
                if (graph[NamberElement] == graph[j])
                {
                    continue;
                }

                Vector2 StartPosition = graph[NamberElement].NodeHexagon.transform.position;
                Vector2 direction = graph[j].NodeHexagon.transform.position;

                bool NoRibs = false;

                if (graph[NamberElement].NodeHexagon.TypeHexagon <= 0 || (graph[NamberElement].NodeHexagon.TypeHexagon == 3 && graph[j].NodeHexagon.gameObject.layer != 10))
                {
                    IsElevation = false;
                    if (!CollisionCheck(StartPosition, direction, IsElevation))
                    {
                        NoRibs = true;
                    }
                }
                else
                {
                    IsElevation = true;
                    if (!CollisionCheckElevation(StartPosition, direction, IsElevation))
                    {
                        NoRibs = true;
                    }
                }

                if (!NoRibs)
                {
                    float magnitude = (graph[NamberElement].NodeHexagon.transform.position - graph[j].NodeHexagon.transform.position).magnitude;
                    graph[NamberElement].Connect(graph[j], magnitude, null);
                }
            }
        }
        return graph;
    }
    public static Graph CreatingEdge(Graph graph, IMove move)//выстраивает 2 ребра в готовом графе(первое и последнее)
    {
        int NamberElement = -(graph.Length - 1);

        for (int i = 0; i < 2; i++)
        {
            NamberElement += (graph.Length - 1);
            bool IsElevation = false;
            //graph[NamberElement].NodeHexagon.Flag();
            for (int j = 0; j < graph.Length; j++)
            {
                if (graph[NamberElement] == graph[j])
                {
                    continue;
                }
                if (graph[NamberElement].NodeHexagon.TypeHexagon <= 0 || (graph[NamberElement].NodeHexagon.TypeHexagon == 3 && graph[j].NodeHexagon.gameObject.layer != 10))
                {
                    IsElevation = false;
                    CollisionCheck(out float magnitude, graph[NamberElement], graph[j], IsElevation, move);
                }
                else
                {
                    IsElevation = true;
                    CollisionCheckElevation(out float magnitude, graph[NamberElement], graph[j], IsElevation, move);

                }
            }
        }
        return graph;
    }
    public static Graph EdgeCheck(Graph OrgGraph, IMove move)//переназначает цену уже сушесвующим ребрам с учетом стоящих врагов
    {
        Graph graph = new Graph(OrgGraph);
        for (int i = 0; i < graph.Length; i++)
        {
            bool IsElevation = false;

            List<Node> ListNodes = graph[i].IncidentNodes();
            for (int j = 0; j < ListNodes.Count; j++)
            {
                float disteins;
                HexagonControl node = ListNodes[j].NodeHexagon.Elevation != null ? ListNodes[j].NodeHexagon.Elevation : ListNodes[j].NodeHexagon;

                if (graph[i].NodeHexagon.TypeHexagon <= 0 || (graph[i].NodeHexagon.TypeHexagon == 3 && node.gameObject.layer != 10))
                {
                    IsElevation = false;
                    disteins = CollisionCheckRepeated(graph[i], ListNodes[j], IsElevation, move);
                }
                else
                {
                    IsElevation = true;
                    disteins = CollisionCheckElevationRepeated(graph[i], ListNodes[j], IsElevation, move);
                }
                graph[i].RevalueEdge(j, disteins);
            }
        }
        return graph;
    }
}
