using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapControlStatic
{
    public static Vector2 MapPos;
    public static HexagonControl[,] mapNav = new HexagonControl[9, 20];//масив содержащий все 6-ти угольники
    public static HexagonControl[] Elevation;
    public static bool /*List<HexagonControl>*/ CollisionCheck(Vector2 StartPos, Vector2 TargetPos, bool elevation)
    {
        HexagonControl controls = null;
        Vector2 currentVector = StartPos;

        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            Debug.Log(TargetPos.x - currentVector.x);
            controls = GetPositionOnTheMap(currentVector);
            Vector2 PosHex = controls.transform.position;
            if ((PosHex - currentVector).magnitude <= 1.8)
            {
                //controls.Flag();
                if (!controls.FreedomTestType(elevation))
                {
                    return false;
                }
            }
            currentVector = Vector2.MoveTowards(currentVector, TargetPos, 0.4f);
        }
        return true;
    }
    public static bool/*List<HexagonControl>*/ CollisionCheckElevation(Vector2 StartPos, Vector2 TargetPos, bool elevation)
    {
        List<HexagonControl> controls = new List<HexagonControl>();

        Vector2 currentVector = StartPos;

        while ((TargetPos - currentVector).magnitude > 0.1f)
        {

            //for (int i = 0; i < mapNav.GetLength(0); i++)
            //{
            //    for (int j = 0; j < mapNav.GetLength(1); j++)
            //    {
            //        if (mapNav[i, j].gameObject.layer==12)
            //        {
            //            continue;
            //        }
            //        Vector2 PosHex = mapNav[i, j].transform.position;
            //        if ((PosHex - currentVector).magnitude<=1.8)
            //        {
            //            //controls.Add(mapNav[i, j]);

            //            if (!mapNav[i, j].FreedomTestType(elevation))
            //            {
            //                return false;
            //            }

            //            //mapNav[i, j].Flag();
            //        }
            //    }
            //}
            currentVector = Vector2.MoveTowards(currentVector, TargetPos, 0.4f);
        }
        return true;
    }
    public static HexagonControl GetPositionOnTheMap(Vector2 Position)
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
        if ((float)System.Math.Round((XInt - X), 2) == 0.5)
        {
            mapNav[(int)Y, XInt].Flag();
            mapNav[(int)Y, XInt - 1].Flag();
        }

        return mapNav[(int)Y, XInt];
    }
}
