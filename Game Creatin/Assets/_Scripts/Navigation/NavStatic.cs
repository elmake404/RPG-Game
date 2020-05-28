using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NavStatic
{
    public static Graph GraphStatic;// граф в котором соеденены все ребра 

    public static bool CollisionCheck(Vector2 StartPos, Vector2 TargetPos, bool elevation)//возыращет true если на пути нет припятсвий (пол)
    {
        HexagonControl[] controls ;
        Vector2 currentVector = StartPos;

        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            controls = MapControlStatic.GetPositionOnTheMap(TargetPos, currentVector);
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
    public static bool CollisionCheckElevation(Vector2 StartPos, Vector2 TargetPos, bool elevation)//возыращет true если на пути нет припятсвий (возвышанность)
    {
        HexagonControl[] controls;

        Vector2 currentVector = StartPos;

        while ((TargetPos - currentVector).magnitude > 0.1f)
        {
            controls = MapControlStatic.GetPositionOnTheMap(TargetPos, currentVector);
            for (int i = 0; i < controls.Length; i++)
            {
                Vector2 PosHex = controls[i].transform.position;
                if ((PosHex - currentVector).magnitude <= 1.8)
                {
                    if (controls[i].Elevation != null)
                    {
                        if (!controls[i].Elevation.FreedomTestType(elevation))
                        {
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

}
