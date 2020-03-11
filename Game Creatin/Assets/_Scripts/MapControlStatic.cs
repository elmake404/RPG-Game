using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapControlStatic
{
    public static HexagonControl[,] mapNav = new HexagonControl[9, 20];
    public static GameObject Flag;
    private static List<HexagonControl> _listWay = new List<HexagonControl>();

    public static void SearchForAWay(int _row, int _column)
    {
        List<HexagonControl> hexagons = new List<HexagonControl>();
        float Magnitude = (mapNav[2, 0].transform.position - mapNav[_row, _column].transform.position).magnitude;
        int _columbias = (_row % 2) == 0 ? 1 : -1;
        HexagonControl hexagonControl = null;

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

        for (int i = 0; i < hexagons.Count; i++)
        {
            if (hexagons[i].TypeHexagon != 1)
            {
                if (Magnitude > (mapNav[2, 0].transform.position - hexagons[i].transform.position).magnitude)
                {
                    Magnitude = (mapNav[2, 0].transform.position - hexagons[i].transform.position).magnitude;
                    hexagonControl = hexagons[i];
                }
            }
        }
        if (hexagonControl != null)
        {
            hexagonControl.Flag();

            _listWay.Add(hexagonControl);

            if (mapNav[hexagonControl.Row, hexagonControl.Column] != mapNav[0, 0])
            {
                SearchForAWay(hexagonControl.Row, hexagonControl.Column);
            }
        }
        else
        {
            //SearchForAWay(_listWay[_listWay.Count - 2].Row, _listWay[_listWay.Count - 2].Column);
            Debug.Log(_listWay[_listWay.Count - 1].Row + " " + _listWay[_listWay.Count - 1].Column);
        }
    }
    private static void BreakingTheDeadlock(int row, int column, int Oldrow, int Oldcolumn)
    {

    }
}
