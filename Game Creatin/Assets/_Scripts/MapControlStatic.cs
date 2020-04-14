using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapControlStatic
{
    public static Vector2 MapPos;
    public static HexagonControl[,] mapNav = new HexagonControl[9, 20];//масив содержащий все 6-ти угольники
    //public static HexagonControl[] Elevation;
    public static Graph GraphStatic;
    public static bool CollisionCheck(Vector2 StartPos, Vector2 TargetPos, bool elevation)
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
    public static bool CollisionCheckElevation(Vector2 StartPos, Vector2 TargetPos, bool elevation)
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
                    if (controls[i].Elevstion!=null)
                    {                    
                        if (!controls[i].Elevstion.FreedomTestType(elevation))
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
    public static HexagonControl[] GetPositionOnTheMap(float XTarget, Vector2 Position)
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
            hexagons[0] = mapNav[(int)Y, XInt];
            hexagons[1] = mapNav[(int)Y, XInt - 1];
            return hexagons;
        }
        else
        {
            HexagonControl[] hexagons = new HexagonControl[1];
            hexagons[0] = mapNav[(int)Y, XInt];
            return hexagons;
        }
    }
}
