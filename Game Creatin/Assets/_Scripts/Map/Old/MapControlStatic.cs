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

    private static List<HexagonControl> EmploymentCheck(List<IMove> WhoBothers)
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
    //private static List<HexagonControl> GetBending(List<HexagonControl> hexagonsBending, IMove iMoveMaim, IMove iMoveTarget, out float Mag)// запускает алгоритм дейкстра и возвращает точки через которые надо пройти 
    //{
    //    List<HexagonControl> bendingVertex = _algorithmDijkstra.Dijkstra(CreatingEdgeBending(hexagonsBending, iMoveMaim, iMoveTarget), out Mag);

    //    if (bendingVertex != null)
    //    {
    //        bendingVertex.Remove(bendingVertex[0]);
    //        bendingVertex.Remove(bendingVertex[bendingVertex.Count - 1]);
    //    }

    //    return bendingVertex;
    //}
    public static bool CollisionCheck(Vector2 StartPos, Vector2 TargetPos, bool elevation)//возыращет true если на пути нет припятсвий (пол)
    {
        HexagonControl[] controls = null;
        Vector2 currentVector = StartPos;

        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            controls = GetPositionOnTheMap(TargetPos, currentVector);
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
    public static bool CollisionCheck(out float disteins, Vector2 StartPos, Vector2 TargetPos, bool elevation, IMove iMove, IMove iMoveTarget)//возыращет true если на пути нет припятсвий , враги и герои тоже препятствие(пол) 
    {
        HexagonControl[] controls;
        Vector2 currentVector = StartPos;

        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            controls = GetPositionOnTheMap(TargetPos, currentVector);
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
                        if ((!controls[i].IsFree) && (controls[i].ObjAbove != iMoveTarget) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetHero() != iMove.GetHero()))
                        {
                            disteins = float.PositiveInfinity;

                            return false;
                        }
                    }
                    else
                    {
                        if ((!controls[i].IsFree) && (controls[i].ObjAbove != iMoveTarget) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetEnemy() != iMove.GetEnemy()))
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
    public static void CollisionCheck(out float disteins, Node from, Node to, bool elevation, IMove iMoveMain)//соеденяет ребра если на пути нет припятсвий , обход врагов и героев (пол) 
    {
        Vector2 TargetPos = to.NodeHexagon.transform.position;
        HexagonControl[] controls;
        List<HexagonControl> hexagonsBending = new List<HexagonControl>();
        Vector2 currentVector = from.NodeHexagon.transform.position;
        List<IMove> ListOblMove = new List<IMove>();
        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            controls = GetPositionOnTheMap(TargetPos, currentVector);

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

                    if (iMoveMain.GetEnemy() == null)
                    {
                        if ((!controls[i].IsFree) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetHero() != iMoveMain.GetHero()
                            && ListOblMove.IndexOf(controls[i].ObjAbove) == -1))
                        {
                            ListOblMove.Add(controls[i].ObjAbove);
                        }
                    }
                    else
                    {
                        if ((!controls[i].IsFree) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetEnemy() != iMoveMain.GetEnemy()
                            && ListOblMove.IndexOf(controls[i].ObjAbove) == -1))
                        {
                            ListOblMove.Add(controls[i].ObjAbove);
                        }
                    }
                }
            }
            currentVector = Vector2.MoveTowards(currentVector, TargetPos, 1f);
        }

        Vector2 StartPos = from.NodeHexagon.transform.position;
        disteins = (StartPos - TargetPos).magnitude;

        if (ListOblMove.Count != 0)
        {
            hexagonsBending.AddRange(EmploymentCheck(ListOblMove));
            hexagonsBending.Insert(0, from.NodeHexagon);
            hexagonsBending.Add(to.NodeHexagon);
            List<HexagonControl> Vertex = GetBending(hexagonsBending, iMoveMain, out float Magnitude);

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
    //public static void CollisionCheck(out float disteins, Node from, Node to, bool elevation, IMove iMoveMain, IMove imoveTarget)//соеденяет ребра если на пути нет припятсвий , обход врагов и героев (пол) 
    //{
    //    Vector2 TargetPos = to.NodeHexagon.transform.position;
    //    HexagonControl[] controls;
    //    List<HexagonControl> hexagonsBending = new List<HexagonControl>();
    //    Vector2 currentVector = from.NodeHexagon.transform.position;
    //    List<IMove> ListOblMove = new List<IMove>();
    //    while ((TargetPos - currentVector).magnitude > 0.1f)
    //    {
    //        controls = GetPositionOnTheMap(TargetPos, currentVector);

    //        for (int i = 0; i < controls.Length; i++)
    //        {
    //            Vector2 PosHex = controls[i].transform.position;
    //            if ((PosHex - currentVector).magnitude <= 1.8)
    //            {
    //                if (!controls[i].FreedomTestType(elevation))
    //                {
    //                    disteins = float.PositiveInfinity;
    //                    return;
    //                }

    //                if (iMoveMain.GetEnemy() == null)
    //                {
    //                    if ((!controls[i].IsFree) && (controls[i].ObjAbove != imoveTarget) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetHero() != iMoveMain.GetHero()
    //                        && ListOblMove.IndexOf(controls[i].ObjAbove) == -1))
    //                    {
    //                        hexagonsBending.AddRange(controls[i].ObjAbove.GetSurroundingHexes());
    //                        ListOblMove.Add(controls[i].ObjAbove);
    //                    }
    //                }
    //                else
    //                {
    //                    if ((!controls[i].IsFree) && (controls[i].ObjAbove != imoveTarget) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetEnemy() != iMoveMain.GetEnemy()
    //                        && ListOblMove.IndexOf(controls[i].ObjAbove) == -1))
    //                    {
    //                        hexagonsBending.AddRange(controls[i].ObjAbove.GetSurroundingHexes());
    //                        ListOblMove.Add(controls[i].ObjAbove);
    //                    }
    //                }
    //            }
    //        }
    //        currentVector = Vector2.MoveTowards(currentVector, TargetPos, 1f);
    //    }

    //    Vector2 StartPos = from.NodeHexagon.transform.position;
    //    disteins = (StartPos - TargetPos).magnitude;

    //    if (hexagonsBending.Count != 0)
    //    {
    //        hexagonsBending.Insert(0, from.NodeHexagon);
    //        hexagonsBending.Add(to.NodeHexagon);
    //        List<HexagonControl> Vertex;
    //        float Magnitude;
    //        if (imoveTarget == null)
    //        {
    //            Vertex = GetBending(hexagonsBending, iMoveMain, out Magnitude);
    //        }
    //        else
    //        {
    //            Vertex = GetBending(hexagonsBending, iMoveMain, imoveTarget, out Magnitude);

    //        }

    //        if (Vertex != null)
    //        {
    //            from.Connect(to, Magnitude, Vertex);
    //        }
    //    }
    //    else
    //    {
    //        from.Connect(to, disteins, null);
    //    }
    //}
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
            controls = GetPositionOnTheMap(TargetPos, currentVector);
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
                            //hexagonsBending.AddRange(controls[i].ObjAbove.GetSurroundingHexes());
                            ListOblMove.Add(controls[i].ObjAbove);
                        }
                    }
                    else
                    {
                        if ((!controls[i].IsFree) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetEnemy() != iMove.GetEnemy() && ListOblMove.IndexOf(iMove) == -1))
                        {
                            //hexagonsBending.AddRange(controls[i].ObjAbove.GetSurroundingHexes());
                            ListOblMove.Add(controls[i].ObjAbove);
                        }
                    }
                }
            }
            currentVector = Vector2.MoveTowards(currentVector, TargetPos, 1f);
        }

        Vector2 StartPos = from.NodeHexagon.transform.position;
        disteins = (StartPos - TargetPos).magnitude;

        if (ListOblMove.Count != 0)
        {
            hexagonsBending.AddRange(EmploymentCheck(ListOblMove));
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
            controls = GetPositionOnTheMap(TargetPos, currentVector);
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
    public static bool CollisionCheckElevation(out float disteins, Vector2 StartPos, Vector2 TargetPos, bool elevation, IMove iMove, IMove iMoveTarget)//возыращет true если на пути нет припятсвий, враги и герои тоже препятствие (возвышанность)
    {
        HexagonControl[] controls;

        Vector2 currentVector = StartPos;

        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            controls = GetPositionOnTheMap(TargetPos, currentVector);

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
                            if ((!controls[i].Elevation.IsFree) && (controls[i].Elevation.ObjAbove != iMoveTarget) && (!controls[i].Elevation.ObjAbove.IsGo() && controls[i].Elevation.ObjAbove.GetHero() != iMove.GetHero()))
                            {
                                disteins = float.PositiveInfinity;
                                return false;
                            }
                        }
                        else
                        {
                            if ((!controls[i].Elevation.IsFree) && (controls[i].Elevation.ObjAbove != iMoveTarget) && (!controls[i].Elevation.ObjAbove.IsGo() && controls[i].Elevation.ObjAbove.GetEnemy() != iMove.GetEnemy()))
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
                            if ((!controls[i].IsFree) && (controls[i].ObjAbove != iMoveTarget) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetHero() != iMove.GetHero()))
                            {
                                disteins = float.PositiveInfinity;
                                return false;
                            }
                        }
                        else
                        {
                            if ((!controls[i].IsFree) && (controls[i].ObjAbove != iMoveTarget) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetEnemy() != iMove.GetEnemy()))
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

        List<IMove> ListOblMove = new List<IMove>();

        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            controls = GetPositionOnTheMap(TargetPos, currentVector);

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
                            if ((!controls[i].Elevation.IsFree) && (!controls[i].Elevation.ObjAbove.IsGo() && controls[i].Elevation.ObjAbove.GetHero() != iMove.GetHero()) && ListOblMove.IndexOf(controls[i].Elevation.ObjAbove) == -1)
                            {
                                //hexagonsBending.AddRange(controls[i].Elevation.ObjAbove.GetSurroundingHexes());
                                ListOblMove.Add(controls[i].Elevation.ObjAbove);
                            }
                        }
                        else
                        {
                            if ((!controls[i].Elevation.IsFree) && (!controls[i].Elevation.ObjAbove.IsGo() && controls[i].Elevation.ObjAbove.GetEnemy() != iMove.GetEnemy()) && ListOblMove.IndexOf(controls[i].Elevation.ObjAbove) == -1)
                            {
                                //hexagonsBending.AddRange(controls[i].Elevation.ObjAbove.GetSurroundingHexes());
                                ListOblMove.Add(controls[i].Elevation.ObjAbove);
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
                            if ((!controls[i].IsFree) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetHero() != iMove.GetHero()) && ListOblMove.IndexOf(controls[i].ObjAbove) == -1)
                            {
                                //hexagonsBending.AddRange(controls[i].ObjAbove.GetSurroundingHexes());
                                ListOblMove.Add(controls[i].ObjAbove);
                            }
                        }
                        else
                        {
                            if ((!controls[i].IsFree) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetEnemy() != iMove.GetEnemy()) && ListOblMove.IndexOf(controls[i].ObjAbove) == -1)
                            {
                                //hexagonsBending.AddRange(controls[i].ObjAbove.GetSurroundingHexes());
                                ListOblMove.Add(controls[i].ObjAbove);
                            }
                        }
                    }
                }
            }
            currentVector = Vector2.MoveTowards(currentVector, TargetPos, 1f);
        }
        Vector2 StartPos = from.NodeHexagon.transform.position;
        disteins = (StartPos - TargetPos).magnitude;

        if (ListOblMove.Count != 0)
        {
            List<HexagonControl> newHex = new List<HexagonControl>();
            hexagonsBending.AddRange(EmploymentCheck(ListOblMove));
            for (int i = 0; i < hexagonsBending.Count; i++)
            {
                if (hexagonsBending[i].Floor != null)
                {
                    //hexagonsBending[i].Floor.Flag();
                    newHex.Add(hexagonsBending[i].Floor);
                }
                else
                {
                    newHex.Add(hexagonsBending[i]);

                    //hexagonsBending[i].Flag();
                }
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
    //public static void CollisionCheckElevation(out float disteins, Node from, Node to, bool elevation, IMove iMoveMain, IMove imoveTarget)//соеденяет ребра  если на пути нет припятсвий, обход врагов и героев (возвышанность)
    //{
    //    Vector2 TargetPos = to.NodeHexagon.transform.position;
    //    HexagonControl[] controls;
    //    List<HexagonControl> hexagonsBending = new List<HexagonControl>();
    //    Vector2 currentVector = from.NodeHexagon.transform.position;
    //    List<IMove> ListOblMove = new List<IMove>();

    //    while ((TargetPos - currentVector).magnitude > 0.1f)
    //    {
    //        controls = GetPositionOnTheMap(TargetPos, currentVector);

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
    //                        return;
    //                    }

    //                    if (iMoveMain.GetEnemy() == null)
    //                    {
    //                        if ((!controls[i].Elevation.IsFree) && (controls[i].Elevation.ObjAbove != imoveTarget) && (!controls[i].Elevation.ObjAbove.IsGo() && controls[i].Elevation.ObjAbove.GetHero() != iMoveMain.GetHero())
    //                            && ListOblMove.IndexOf(controls[i].Elevation.ObjAbove) == -1)
    //                        {
    //                            //hexagonsBending.AddRange(controls[i].Elevation.ObjAbove.GetSurroundingHexes());
    //                            ListOblMove.Add(controls[i].Elevation.ObjAbove);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        if ((!controls[i].Elevation.IsFree) && (controls[i].Elevation.ObjAbove != imoveTarget) && (!controls[i].Elevation.ObjAbove.IsGo() && controls[i].Elevation.ObjAbove.GetEnemy() != iMoveMain.GetEnemy())
    //                            && ListOblMove.IndexOf(controls[i].Elevation.ObjAbove) == -1)
    //                        {
    //                            //hexagonsBending.AddRange(controls[i].Elevation.ObjAbove.GetSurroundingHexes());
    //                            ListOblMove.Add(controls[i].Elevation.ObjAbove);
    //                        }
    //                    }

    //                }
    //                else
    //                {
    //                    if (!controls[i].FreedomTestType(elevation))
    //                    {
    //                        disteins = float.PositiveInfinity;
    //                        return;
    //                    }

    //                    if (iMoveMain.GetEnemy() == null)
    //                    {
    //                        if ((!controls[i].IsFree) && (controls[i].ObjAbove != imoveTarget) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetHero() != iMoveMain.GetHero()) && ListOblMove.IndexOf(controls[i].ObjAbove) == -1)
    //                        {
    //                            //hexagonsBending.AddRange(controls[i].ObjAbove.GetSurroundingHexes());
    //                            ListOblMove.Add(controls[i].ObjAbove);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        if ((!controls[i].IsFree) && (controls[i].ObjAbove != imoveTarget) && (!controls[i].ObjAbove.IsGo() && controls[i].ObjAbove.GetEnemy() != iMoveMain.GetEnemy())
    //                            && ListOblMove.IndexOf(controls[i].ObjAbove) == -1)
    //                        {
    //                            //hexagonsBending.AddRange(controls[i].ObjAbove.GetSurroundingHexes());
    //                            ListOblMove.Add(controls[i].ObjAbove);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        currentVector = Vector2.MoveTowards(currentVector, TargetPos, 1f);
    //    }
    //    Vector2 StartPos = from.NodeHexagon.transform.position;
    //    disteins = (StartPos - TargetPos).magnitude;

    //    if (ListOblMove.Count != 0)
    //    {
    //        List<HexagonControl> newHex = new List<HexagonControl>();
    //        hexagonsBending.AddRange(EmploymentCheck(ListOblMove));

    //        for (int i = 0; i < hexagonsBending.Count; i++)
    //        {
    //            hexagonsBending[i].Flag();
    //            if (hexagonsBending[i].Floor != null)
    //            {
    //                newHex.Add(hexagonsBending[i].Floor);
    //            }
    //            else
    //                newHex.Add(hexagonsBending[i]);
    //        }
    //        newHex.Insert(0, from.NodeHexagon);
    //        newHex.Add(to.NodeHexagon);
    //        List<HexagonControl> Vertex = GetBending(newHex, iMoveMain, out float Magnitude);
    //        if (Vertex != null)
    //        {
    //            from.Connect(to, Magnitude, Vertex);
    //        }
    //    }
    //    else
    //    {
    //        from.Connect(to, disteins, null);
    //    }
    //}
    public static float CollisionCheckElevationRepeated(Node from, Node to, bool elevation, IMove iMove)//возыращет disteins если на пути нет припятсвий или float.PositiveInfinity , обход врагов и героев (возвышанность)
    {
        float disteins;
        Vector2 TargetPos = to.NodeHexagon.transform.position;
        HexagonControl[] controls;
        List<HexagonControl> hexagonsBending = new List<HexagonControl>();
        Vector2 currentVector = from.NodeHexagon.transform.position;

        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            controls = GetPositionOnTheMap(TargetPos, currentVector);

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
    public static HexagonControl FieldPosition(int layer, Vector2 position)//гексагон к которому принадлежит герой
    {
        Vector2 difference = layer == 8 ? Vector2.zero : new Vector2(X, Y);
        int layerHex = layer == 8 ? 9 : 12;
        HexagonControl hexagon = GetPositionOnTheMap(position - difference, layerHex);
        if (hexagon == null)
        {
            Debug.Log(layerHex);
        }
        return hexagon.Elevation != null ?
           hexagon.Elevation : hexagon;//нужный 6-ти угольник  
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
    //public static Graph CreatingEdgeBending(List<HexagonControl> listHexagons, IMove moveMain, IMove iMoveTarget)//выстраивает ребра в графе 
    //{
    //    var graph = new Graph(listHexagons);

    //    for (int i = 0; i < graph.Length - 1; i++)
    //    {
    //        bool IsElevation = false;
    //        for (int j = i + 1; j < graph.Length; j++)
    //        {
    //            float magnitude;

    //            Vector2 StartPosition = graph[i].NodeHexagon.transform.position;
    //            Vector2 direction = graph[j].NodeHexagon.transform.position;

    //            bool NoRibs = false;
    //            HexagonControl node = graph[j].NodeHexagon.Elevation != null ? graph[j].NodeHexagon.Elevation : graph[j].NodeHexagon;

    //            if (listHexagons[i].TypeHexagon <= 0 || (listHexagons[i].TypeHexagon == 3 && node.gameObject.layer != 10))
    //            {
    //                IsElevation = false;
    //                if (!CollisionCheck(out magnitude, StartPosition, direction, IsElevation, moveMain, iMoveTarget))
    //                {
    //                    NoRibs = true;
    //                }
    //            }
    //            else
    //            {
    //                IsElevation = true;
    //                if (!CollisionCheckElevation(out magnitude, StartPosition, direction, IsElevation, moveMain, iMoveTarget))
    //                {
    //                    NoRibs = true;
    //                }
    //            }

    //            if (!NoRibs)
    //            {
    //                graph[i].Connect(graph[j], magnitude, null);
    //            }
    //        }
    //    }

    //    return graph;
    //}
    public static Graph CreatingEdgeBending(List<HexagonControl> listHexagons, IMove move)//выстраивает ребра в графе 
    {
        //Debug.Log(2222);
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
                    if (!CollisionCheck(out magnitude, StartPosition, direction, IsElevation, move, null))
                    {
                        NoRibs = true;
                    }
                }
                else
                {
                    IsElevation = true;
                    if (!CollisionCheckElevation(out magnitude, StartPosition, direction, IsElevation, move, null))
                    {
                        NoRibs = true;
                    }
                }

                if (!NoRibs)
                {
                    graph[i].Connect(graph[j], magnitude, null);
                }
                else
                {
                    //if (graph[j] == graph[graph.Length - 1])
                    //{
                    //    graph[i].NodeHexagon.Flag();
                    //}
                }
            }
        }
        //List<Node> d = graph[graph.Length - 1].IncidentNodes();

        //Debug.Log(d.Count);

        //for (int i = 0; i < d.Count; i++)
        //{
        //    d[i].NodeHexagon.Flag();
        //}
        //graph[graph.Length - 1].NodeHexagon.Flag() ;
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
    //public static Graph CreatingEdge(Graph graph, IMove move, IMove iMoveTarget)//выстраивает 2 ребра в готовом графе(первое и последнее)
    //{
    //    int NamberElement = -(graph.Length - 1);

    //    for (int i = 0; i < 2; i++)
    //    {
    //        NamberElement += (graph.Length - 1);
    //        bool IsElevation = false;
    //        //graph[NamberElement].NodeHexagon.Flag();
    //        for (int j = 0; j < graph.Length; j++)
    //        {
    //            if (graph[NamberElement] == graph[j])
    //            {
    //                continue;
    //            }
    //            if (graph[NamberElement].NodeHexagon.TypeHexagon <= 0 || (graph[NamberElement].NodeHexagon.TypeHexagon == 3 && graph[j].NodeHexagon.gameObject.layer != 10))
    //            {
    //                IsElevation = false;
    //                CollisionCheck(out float magnitude, graph[NamberElement], graph[j], IsElevation, move, iMoveTarget);
    //            }
    //            else
    //            {
    //                IsElevation = true;
    //                CollisionCheckElevation(out float magnitude, graph[NamberElement], graph[j], IsElevation, move, iMoveTarget);
    //            }
    //        }
    //    }
    //    return graph;
    //}
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
    public static HexagonControl[] GetPositionOnTheMap(Vector2 Target, Vector2 Position) // возвращает гексагон через позицию (может определить два блока)
    {

        Vector2 Normalized = (Target - Position).normalized;
        float XTarget = (float)System.Math.Round(Normalized.x, 1);
        float YTarget = (float)System.Math.Round(Normalized.y, 1);
        float Y = (Position.y - MapPos.y) / -3f;
        float YOld = (Position.y - MapPos.y) / -3f;

        int YMax = Mathf.Abs(Mathf.RoundToInt(Y));
        float Difference = 0;
        float f = ((Position.x - (MapPos.x - 3.46f)) / 1.73f);
        float R = Mathf.Floor(f) * 1.73f - (Position.x - (MapPos.x - 3.46f));
        int G = (int)f % 2 == 0 ? 0 : 1;
        //int D = (int)Y % 2 == 0 ? 0 : 1;
        //int D = 
        //Debug.Log(Mathf.Abs(YMax - Mathf.Abs(Y)));
        //Debug.Log(Mathf.Abs(((0.333f + ((0.333f / 1.73f) * Mathf.Abs(R))))));
        //Debug.Log(Y);

        if (Mathf.Abs(YMax - Mathf.Abs(Y)) < Mathf.Abs(G - ((0.333f + ((0.333f / 1.73f) * Mathf.Abs(R))))))
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
        int XInt;

        if (X == 0.5)
        {
            XInt = 1;
        }
        else
        {
            XInt = Mathf.RoundToInt(X);
        }

        OwnershipCheck((int)Y, XInt, out Y, out XInt, Position);

        List<HexagonControl> hexagonsList = new List<HexagonControl>();
        //if (Mathf.Abs((float)System.Math.Round((XInt - X), 2)) == 0.5f && (float)System.Math.Round((YMax - YOld), 1) == -0.3f)
        //{
        //    Debug.Log(Mathf.Abs((float)System.Math.Round((XInt - X), 2)));
        //    Debug.Log((float)System.Math.Round((YMax - YOld), 1));

        //    if (Y < 8)
        //    {
        //        int NewY = (int)Y + 1;
        //        int deviationEven;
        //        int deviationOdd;
        //        if (YTarget > 0)
        //        {
        //            deviationEven = XTarget > 0 ? 0 : 1;
        //            deviationOdd = XTarget > 0 ? -1 : 0;
        //        }
        //        else
        //        {
        //            deviationEven = XTarget > 0 ? 1 : 0;
        //            deviationOdd = XTarget > 0 ? 0 : -1;
        //        }

        //        int DifferenceX = NewY % 2 == 0 ? deviationOdd : deviationEven;
        //        if ((XInt + DifferenceX) <= 19 && (XInt + DifferenceX) >= 0)
        //            hexagonsList.Add(mapNav[NewY, XInt + DifferenceX]);
        //        //Debug.Log(YTarget);
        //    }
        //    if (XInt <= 19)
        //    {
        //        hexagonsList.Add(mapNav[(int)Y, XInt]);
        //    }

        //    if (XInt > 0)
        //    {
        //        int deviation = XTarget > 0 ? -1 : 1;

        //        if ((XInt + deviation) <= 19 && (XInt + deviation) >= 0)
        //            hexagonsList.Add(mapNav[(int)Y, XInt + deviation]);
        //    }

        //    HexagonControl[] hexagons = new HexagonControl[hexagonsList.Count];
        //    for (int i = 0; i < hexagonsList.Count; i++)
        //    {
        //        hexagons[i] = hexagonsList[i];
        //    }

        //    return hexagons;
        //}
        //else if (Mathf.Abs((float)System.Math.Round((XInt - X), 2)) == 1f && (float)System.Math.Round((YMax - YOld), 1) == 0.3f)
        //{
        //    //Debug.Log(Mathf.Abs((float)System.Math.Round((XInt - X), 2)));
        //    //Debug.Log((float)System.Math.Round((YMax - YOld), 1));

        //    if (Y > 0)
        //    {
        //        int NewY = (int)Y - 1;
        //        int deviationEven;
        //        int deviationOdd;
        //        if (YTarget > 0)
        //        {
        //            deviationEven = XTarget > 0 ? -1 : -1;
        //            deviationOdd = XTarget > 0 ? 1 : 0;
        //        }
        //        else
        //        {
        //            deviationEven = XTarget > 0 ? -1 : 0;
        //            deviationOdd = XTarget > 0 ? 1 : 0;
        //        }

        //        int DifferenceX = NewY % 2 == 0 ? deviationEven : deviationOdd;
        //        if ((XInt + DifferenceX) <= 19 && (XInt + DifferenceX) >= 0)
        //        {
        //            hexagonsList.Add(mapNav[NewY, XInt + DifferenceX]);
        //        }
        //    }

        //    if (XInt <= 19)
        //    {
        //        hexagonsList.Add(mapNav[(int)Y, XInt]);
        //    }
        //    if (XInt > 0)
        //    {
        //        int deviation;
        //        if (YTarget > 0)
        //        {
        //            //deviation = XTarget > 0 ? 1 :-1 ;
        //            deviation = -1;
        //        }
        //        else
        //        {
        //            deviation = XTarget > 0 ? -1 : 1;
        //        }
        //        if ((XInt + deviation) <= 19 && (XInt + deviation) >= 0)
        //            hexagonsList.Add(mapNav[(int)Y, XInt + deviation]);
        //        Debug.Log(deviation);
        //    }

        //    HexagonControl[] hexagons = new HexagonControl[hexagonsList.Count];
        //    for (int i = 0; i < hexagonsList.Count; i++)
        //    {
        //        hexagons[i] = hexagonsList[i];
        //    }

        //    return hexagons;
        //}
        if (Mathf.Abs((float)System.Math.Round((XInt - X), 2)) == 0.5f && XTarget == 0)
        {
            if (XInt <= 19)
            {
                hexagonsList.Add(mapNav[(int)Y, XInt]);
            }
            if (XInt > 0)
            {
                hexagonsList.Add(mapNav[(int)Y, XInt - 1]);
            }
            HexagonControl[] hexagons = new HexagonControl[hexagonsList.Count];
            for (int i = 0; i < hexagonsList.Count; i++)
            {
                hexagons[i] = hexagonsList[i];
            }

            return hexagons;
        }
        else if (Mathf.Abs((float)System.Math.Round((XInt - X), 3)) < 0.5f && (Mathf.Abs((float)System.Math.Round((YMax - YOld), 3)) < 0.51 && Mathf.Abs((float)System.Math.Round((YMax - YOld), 3)) > 0.33) && ((Mathf.Abs(XTarget) == 0.9f) && (Mathf.Abs(YTarget) == 0.5f)))
        {
            //Debug.Log((Target - Position).normalized);

            //if (YMax - YOld > 0)
            //{
            //    if (Y > 0)
            //    {
            //        int deviationEven;
            //        int deviationOdd;

            //        if (YTarget > 0)
            //        {

            //            deviationEven = XTarget > 0 ? 1 : 1;
            //            deviationOdd = XTarget > 0 ? -1 : 0;

            //        }
            //        else
            //        {
            //            deviationEven = XTarget > 0 ? 1 : 0;
            //            deviationOdd = XTarget > 0 ? 0 : -1;
            //        }

            //        int NewY = (int)Y + 1;
            //        int DifferenceX = NewY % 2 == 0 ? deviationOdd : deviationEven;
            //        if ((XInt + DifferenceX) <= 19 && (XInt + DifferenceX) >= 0)
            //        {
            //            hexagonsList.Add(mapNav[NewY, XInt + DifferenceX]);
            //        }
            //    }

            //    hexagonsList.Add(mapNav[(int)Y, XInt]);
            //    HexagonControl[] hexagons = new HexagonControl[hexagonsList.Count];
            //    for (int i = 0; i < hexagonsList.Count; i++)
            //    {
            //        hexagons[i] = hexagonsList[i];
            //    }

            //    return hexagons;
            //}
            //else
            //{
            //    if (Y < 8)
            //    {
            //        int deviationEven;
            //        int deviationOdd;

            //        if (YTarget > 0)
            //        {
            //            deviationEven = XTarget > 0 ? 0 : -1;
            //            deviationOdd = XTarget > 0 ? 1 : 0;
            //        }
            //        else
            //        {
            //            deviationEven = XTarget > 0 ? -1 : -1;
            //            deviationOdd = XTarget > 0 ? 0 : 0;
            //        }
            //        int NewY = (int)Y + 1;
            //        int DifferenceX = NewY % 2 == 0 ? deviationEven : deviationOdd;
            //        if ((XInt + DifferenceX) <= 19 && (XInt + DifferenceX) >= 0)
            //        {
            //            hexagonsList.Add(mapNav[NewY, XInt + DifferenceX]);
            //        }
            //        //Debug.Log(DifferenceX);
            //    }

            //}
            hexagonsList.Add(OwnershipCheckadditional((int)Y, XInt, Position));
            hexagonsList.Add(mapNav[(int)Y, XInt]);

            HexagonControl[] hexagons = new HexagonControl[hexagonsList.Count];
            for (int i = 0; i < hexagonsList.Count; i++)
            {
                hexagons[i] = hexagonsList[i];
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
    public static HexagonControl GetPositionOnTheMap(Vector2 Position, int Layer)// возвращает гексагон через позицию
    {
        float Y = (Position.y - MapPos.y) / 3f;
        int YMax = Mathf.Abs(Mathf.RoundToInt(Y));
        float Difference = 0;
        float f = ((Position.x - (MapPos.x - 3.46f)) / 1.73f);
        float R = Mathf.Floor(f) * 1.73f - (Position.x - (MapPos.x - 3.46f));
        int G = (int)f % 2 == 0 ? 0 : 1;

        if (Mathf.Abs(YMax - Mathf.Abs(Y)) < Mathf.Abs(G - ((0.333f + ((0.333f / 1.73f) * Mathf.Abs(R))))))
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
        return OwnershipCheck((int)Y, XInt, Position, Layer);
    }
    private static HexagonControl OwnershipCheck(int _row, int _column, Vector2 pos, int Layer)
    {
        List<HexagonControl> hexagons = new List<HexagonControl>();
        int _columbias = (_row % 2) == 0 ? 1 : -1;
        if (((Vector2)mapNav[_row, _column].transform.position - pos).magnitude <= 1.8f)
        {
            if (mapNav[_row, _column].gameObject.layer == Layer && (mapNav[_row, _column].TypeHexagon != 1 || Layer == 12) || (Layer == 12 && mapNav[_row, _column].TypeHexagon == 3))
                return mapNav[_row, _column];
        }

        #region AddToList
        hexagons.Add(mapNav[_row, _column]);
        if (_column < mapNav.GetLength(1) - 1)
            hexagons.Add(mapNav[_row, _column + 1]);

        if (_column > 0)
            hexagons.Add(mapNav[_row, _column - 1]);

        if (_row < mapNav.GetLength(0) - 1)
        {
            hexagons.Add(mapNav[_row + 1, _column]);

            if (_column + _columbias > 0 && _column + _columbias < mapNav.GetLength(1) - 1)
                hexagons.Add(mapNav[_row + 1, _column + _columbias]);
        }

        if (_row > 0)
        {
            hexagons.Add(mapNav[_row - 1, _column]);
            if (_column + _columbias > 0 && _column + _columbias < mapNav.GetLength(1) - 1)
                hexagons.Add(mapNav[_row - 1, _column + _columbias]);
        }
        #endregion

        float mag = float.PositiveInfinity;
        HexagonControl hexagon = null;
        for (int i = 0; i < hexagons.Count; i++)
        {

            if (hexagons[i].gameObject.layer == Layer && (hexagons[i].TypeHexagon != 1 || Layer == 12) || (Layer == 12 && hexagons[i].TypeHexagon == 3))
            {

                if (mag > ((Vector2)hexagons[i].transform.position - pos).magnitude)
                {
                    mag = ((Vector2)hexagons[i].transform.position - pos).magnitude;
                    hexagon = hexagons[i];
                    if (mag <= 1.8f)
                    {
                        return hexagons[i];
                    }
                }
            }
            else
            {
                if (Layer == 12 && hexagons[i].gameObject.layer == 12)
                {
                    hexagons[i].Flag();
                }
            }
        }

        return hexagon;
    }
    private static HexagonControl OwnershipCheckadditional(int _row, int _column, Vector2 pos)
    {
        List<HexagonControl> hexagons = new List<HexagonControl>();
        int _columbias = (_row % 2) == 0 ? 1 : -1;

        #region AddToList
        if (_column < mapNav.GetLength(1) - 1)
            hexagons.Add(mapNav[_row, _column + 1]);

        if (_column > 0)
            hexagons.Add(mapNav[_row, _column - 1]);

        if (_row < mapNav.GetLength(0) - 1)
        {
            hexagons.Add(mapNav[_row + 1, _column]);

            if (_column + _columbias > 0 && _column + _columbias < mapNav.GetLength(1) - 1)
                hexagons.Add(mapNav[_row + 1, _column + _columbias]);
        }

        if (_row > 0)
        {
            hexagons.Add(mapNav[_row - 1, _column]);
            if (_column + _columbias > 0 && _column + _columbias < mapNav.GetLength(1) - 1)
                hexagons.Add(mapNav[_row - 1, _column + _columbias]);
        }
        #endregion

        float mag = float.PositiveInfinity;
        HexagonControl hexagon = null;
        for (int i = 0; i < hexagons.Count; i++)
        {
            if (mag > ((Vector2)hexagons[i].transform.position - pos).magnitude)
            {
                mag = ((Vector2)hexagons[i].transform.position - pos).magnitude;
                hexagon = hexagons[i];
                if (mag <= 1.8f)
                {
                    return hexagons[i];
                }

            }
        }
        return hexagon;
    }
    private static void OwnershipCheck(int _row, int _column, out float Row, out int Column, Vector2 pos)
    {
        if (_row > mapNav.GetLength(0) - 1)
        {
            _row = mapNav.GetLength(0) - 1;
        }
        else if (_row < 0)
        {
            _row = 0;
        }

        if (_column > mapNav.GetLength(1) - 1)
        {
            _column = mapNav.GetLength(1) - 1;
        }
        List<HexagonControl> hexagons = new List<HexagonControl>();
        int _columbias = (_row % 2) == 0 ? 1 : -1;
        if (((Vector2)mapNav[_row, _column].transform.position - pos).magnitude <= 1.8f)
        {
            Row = _row;
            Column = _column;
            return;
        }

        #region AddToList
        hexagons.Add(mapNav[_row, _column]);
        if (_column < mapNav.GetLength(1) - 1)
            hexagons.Add(mapNav[_row, _column + 1]);

        if (_column > 0)
            hexagons.Add(mapNav[_row, _column - 1]);

        if (_row < mapNav.GetLength(0) - 1)
        {
            hexagons.Add(mapNav[_row + 1, _column]);

            if (_column + _columbias > 0 && _column + _columbias < mapNav.GetLength(1) - 1)
                hexagons.Add(mapNav[_row + 1, _column + _columbias]);
        }

        if (_row > 0)
        {
            hexagons.Add(mapNav[_row - 1, _column]);
            if (_column + _columbias > 0 && _column + _columbias < mapNav.GetLength(1) - 1)
                hexagons.Add(mapNav[_row - 1, _column + _columbias]);
        }
        #endregion

        float mag = float.PositiveInfinity;
        HexagonControl hexagon = null;
        for (int i = 0; i < hexagons.Count; i++)
        {
            if (mag > ((Vector2)hexagons[i].transform.position - pos).magnitude)
            {
                mag = ((Vector2)hexagons[i].transform.position - pos).magnitude;
                hexagon = hexagons[i];
                if (mag <= 1.8f)
                {
                    Row = hexagons[i].Row;
                    Column = hexagons[i].Column;
                    return;
                }

            }
        }
        Row = hexagon.Row;
        Column = hexagon.Column;
        return;
    }
}
