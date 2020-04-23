using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapControlStatic
{
    public static Vector2 MapPos;
    private static AlgorithmDijkstra _algorithmDijkstra = new AlgorithmDijkstra();

    public static HexagonControl[,] mapNav = new HexagonControl[9, 20];//масив содержащий все 6-ти угольники
    //public static HexagonControl[] Elevation;
    public static Graph GraphStatic;
    public static float X, Y;

    private static List<HexagonControl> GetBending(List<HexagonControl> hexagonsBending, IMove iMove , out float Mag)
    {
        List<HexagonControl> bendingVertex = _algorithmDijkstra.Dijkstra(CreatingEdgeBending(hexagonsBending, iMove),out Mag);
        if (bendingVertex != null)
        {
            //Debug.Log(1);
            //for (int i = 0; i < bendingVertex.Count; i++)
            //{
            //    bendingVertex[i].Flag();
            //}
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
            currentVector = Vector2.MoveTowards(currentVector, TargetPos, 0.4f);
        }
        disteins = (StartPos - TargetPos).magnitude;

        return true;
    }
    public static void CollisionCheck(out float disteins, Node from, Node to, bool elevation, IMove iMove)//возыращет true если на пути нет припятсвий , обход врагов и героев (пол) 
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
                    if (!controls[i].FreedomTestType(elevation))
                    {
                        disteins = float.PositiveInfinity;
                        return;
                    }

                    if (iMove.GetEnemy() == null)
                    {
                        if ((!controls[i].IsFree) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetHero() != iMove.GetHero()))
                        {
                            hexagonsBending.AddRange(controls[i].ObjAbove.GetHero().GetSurroundingHexes());
                        }
                    }
                    else
                    {
                        if ((!controls[i].IsFree) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetEnemy() != iMove.GetEnemy()))
                        {
                            hexagonsBending.AddRange(controls[i].ObjAbove.GetHero().GetSurroundingHexes());
                        }
                    }
                }
            }
            currentVector = Vector2.MoveTowards(currentVector, TargetPos, 0.4f);
        }

        Vector2 StartPos = from.NodeHexagon.transform.position;
        disteins = (StartPos - TargetPos).magnitude;

        if (hexagonsBending.Count != 0)
        {
            hexagonsBending.Insert(0, from.NodeHexagon);
            hexagonsBending.Add(to.NodeHexagon);
            List<HexagonControl> Vertex = GetBending(hexagonsBending, iMove, out float Magnitude);
            from.Connect(to,Magnitude, Vertex);
        }
        else
        {
            from.Connect(to, disteins, null);
        }

        //return true;
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
            currentVector = Vector2.MoveTowards(currentVector, TargetPos, 0.4f);
        }
        disteins = (StartPos - TargetPos).magnitude;
        return true;
    }
    //public static bool CollisionCheckElevation(out float disteins ,Vector2 StartPos, Vector2 TargetPos, bool elevation, IMove iMove)//возыращет true если на пути нет припятсвий, обход врагов и героев (возвышанность)
    //{
    //    HexagonControl[] controls;

    //    Vector2 currentVector = StartPos;

    //    while ((TargetPos - currentVector).magnitude > 0.1f)
    //    {
    //        controls = GetPositionOnTheMap(TargetPos.x - currentVector.x, currentVector);

    //        for (int i = 0; i < controls.Length; i++)
    //        {
    //            Vector2 PosHex = controls[i].transform.position;
    //            if ((PosHex - currentVector).magnitude <= 1.8)
    //            {
    //                if (controls[i].Elevation != null)
    //                {
    //                    if (!controls[i].Elevation.FreedomTestType(elevation))
    //                    {
    //                        disteins = float.PositiveInfinity;
    //                        return false;
    //                    }

    //                    if (iMove.GetEnemy() == null)
    //                    {
    //                        if ((!controls[i].Elevation.IsFree) && (!controls[i].Elevation.ObjAbove.IsGo() && controls[i].Elevation.ObjAbove.GetHero() != iMove.GetHero()))
    //                        {
    //                            disteins = float.PositiveInfinity;
    //                            return false;
    //                        }
    //                    }
    //                    else
    //                    {
    //                        if ((!controls[i].Elevation.IsFree) && (!controls[i].Elevation.ObjAbove.IsGo() && controls[i].Elevation.ObjAbove.GetEnemy() != iMove.GetEnemy()))
    //                        {
    //                            disteins = float.PositiveInfinity;
    //                            return false;
    //                        }
    //                    }

    //                }
    //                else
    //                {
    //                    if (!controls[i].FreedomTestType(elevation))
    //                    {
    //                        disteins = float.PositiveInfinity;
    //                        return false;
    //                    }

    //                    if (iMove.GetEnemy() == null)
    //                    {
    //                        if ((!controls[i].IsFree) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetHero() != iMove.GetHero()))
    //                        {
    //                            disteins = float.PositiveInfinity;
    //                            return false;
    //                        }
    //                    }
    //                    else
    //                    {
    //                        if ((!controls[i].IsFree) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetEnemy() != iMove.GetEnemy()))
    //                        {
    //                            disteins = float.PositiveInfinity;
    //                            return false;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        currentVector = Vector2.MoveTowards(currentVector, TargetPos, 0.4f);
    //    }
    //    disteins = (StartPos-TargetPos).magnitude;
    //    return true;
    //}
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
                float magnitude;

                Vector2 StartPosition = graph[i].NodeHexagon.transform.position;
                Vector2 direction = graph[j].NodeHexagon.transform.position;

                bool NoRibs = false;
                HexagonControl node = graph[j].NodeHexagon.Elevation != null ? graph[j].NodeHexagon.Elevation : graph[j].NodeHexagon;

                if (listHexagons[i].TypeHexagon <= 0 || (listHexagons[i].TypeHexagon == 3 && node.gameObject.layer != 10))
                {
                    IsElevation = false;
                    /*if (!*/
                    CollisionCheck(out magnitude, graph[i], graph[j], IsElevation, move);/*)*/
                    //{
                    //    NoRibs = true;
                    //}
                }
                else
                {
                    IsElevation = true;
                    if (!CollisionCheckElevation(out magnitude, StartPosition, direction, IsElevation, move))
                    {
                        NoRibs = true;
                    }
                }

                //if (!NoRibs)
                //{
                //    graph[i].Connect(graph[j], magnitude, null);
                //}
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
                    if (!MapControlStatic.CollisionCheck(StartPosition, direction, IsElevation))
                    {
                        NoRibs = true;
                    }
                }
                else
                {
                    IsElevation = true;
                    if (!MapControlStatic.CollisionCheckElevation(StartPosition, direction, IsElevation))
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


}
