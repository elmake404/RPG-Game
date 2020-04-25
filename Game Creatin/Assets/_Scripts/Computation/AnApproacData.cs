using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnApproacData
{
    public HexagonControl hexagon;
    public bool busy;
    public List<EnemyControl> enemy = new List<EnemyControl>();

    public void Ban()
    {
        hexagon = null;
        enemy = null;
        busy = false;
    }
    public bool Suitability()
    {
        if (hexagon != null && !busy)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}