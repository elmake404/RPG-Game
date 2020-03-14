using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapControlStatic
{
    public static HexagonControl[,] mapNav = new HexagonControl[9, 20];//масив содержащий все 6-ти угольники
    private static List<HexagonControl> _listWay = new List<HexagonControl>();    //список содержащий кротчай ший путь 
    private static List<HexagonControl> _listDeadlock = new List<HexagonControl>();    //список содержащий информацию про тупик 
    public static List<HexagonControl> ListPoint = new List<HexagonControl>();    //список вершин по которым надо проййтись 
    private static int _oldRow = 3, _oldColunm = 10;

    public static void SearchForAWay(int _row, int _column)    //метод писка пути 
    {
        if (_listWay.Count < 1)
        {
            _listWay.Add(mapNav[_row, _column]);
        }

        List<HexagonControl> hexagons = new List<HexagonControl>();// список всех соседей 6-ти угольника
        float Magnitude = (ListPoint[ListPoint.Count - 1].transform.position - mapNav[_row, _column].transform.position).magnitude;
        int _columbias = (_row % 2) == 0 ? 1 : -1;
        HexagonControl hexagonControl = null;//нужный 6-ти угольник  

        #region AddToListHeighbors
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

        //if (_listWay.Count > 0)
        //{
        //    //for (int i = 0; i < _listWay.Count; i++)
        //    //{
        //        for (int j = 0; j < hexagons.Count; j++)
        //        {
        //            if (hexagons[j].TypeHexagon != 1)
        //            {
        //                if ((hexagons[j].Row == _oldRow && hexagons[j].Column == _oldColunm)&& (Magnitude > (ListPoint[ListPoint.Count - 1].transform.position - hexagons[j].transform.position).magnitude))
        //                {

        //                    Debug.LogError("eee" + hexagons[j].transform.parent.name + " " + hexagons[j].name);
        //                    Debug.LogError("222" + _oldRow + " " + _oldColunm);
        //                    Debug.LogError("333" + _row + " " + _column);
        //                    //hexagons.Remove(hexagons[j]);
        //                }
        //            }
        //        }
        //    }
        ////}


        for (int i = 0; i < hexagons.Count; i++)
        {
            if (hexagons[i].TypeHexagon != 1)
            {
                if ((Magnitude > (ListPoint[ListPoint.Count - 1].transform.position - hexagons[i].transform.position).magnitude))
                {
                    if ((hexagons[i].Row == _oldRow && hexagons[i].Column == _oldColunm))
                    {
                      int iNew = AnotherWay(hexagons, i);
                        Magnitude = (ListPoint[ListPoint.Count - 1].transform.position - hexagons[iNew].transform.position).magnitude;
                        hexagonControl = hexagons[iNew];
                    }
                    else
                    {
                        Magnitude = (ListPoint[ListPoint.Count - 1].transform.position - hexagons[i].transform.position).magnitude;
                        hexagonControl = hexagons[i];
                    }
                }
            }

        }

        if (hexagonControl != null)
        {
            _oldRow = _listWay[_listWay.Count - 1].Row;
            _oldColunm = _listWay[_listWay.Count - 1].Column;
            _listWay.Add(hexagonControl);

            if (mapNav[hexagonControl.Row, hexagonControl.Column] != mapNav[ListPoint[ListPoint.Count - 1].Row, ListPoint[ListPoint.Count - 1].Column])
            {
                SearchForAWay(hexagonControl.Row, hexagonControl.Column);
            }
            else if (ListPoint.Count > 1)
            {
                for (int i = 0; i < _listWay.Count; i++)
                {
                    _listWay[i].Flag();
                }
                //Debug.Log("1 " + hexagonControl.Row + " " + hexagonControl.Column);
                ListPoint.Remove(ListPoint[ListPoint.Count - 1]);
                SearchForAWay(hexagonControl.Row, hexagonControl.Column);
            }
            else
            {
                for (int i = 0; i < _listWay.Count; i++)
                {
                    _listWay[i].Flag();
                }
            }
        }
        else
        {
            //Debug.Log(_listWay[_listWay.Count - 1].Row + " " + (_listWay[_listWay.Count - 1].Column - 1));

            for (int i = 0; i < hexagons.Count; i++)
            {
                if (hexagons[i].TypeHexagon == 1)
                {
                    if (Magnitude > (ListPoint[ListPoint.Count - 1].transform.position - hexagons[i].transform.position).magnitude)
                    {
                        Magnitude = (ListPoint[ListPoint.Count - 1].transform.position - hexagons[i].transform.position).magnitude;
                        //Debug.Log("new "+ _row + " " + _column);

                        hexagonControl = hexagons[i];
                    }
                }
            }

            //for (int i = 0; i < _listWay.Count; i++)
            //{
            //    _listWay[i].Flag();
            //}

            //Debug.Log("new " + hexagonControl.Row + " " + hexagonControl.Column);
            _listDeadlock.Add(MapControlStatic.mapNav[hexagonControl.Row, hexagonControl.Column]);
            BreakingTheDeadlock(hexagonControl.Row, hexagonControl.Column);
        }
    }

    private static void BreakingTheDeadlock(int row, int column)    //метод писка выхода из тупика  
    {
        List<HexagonControl> hexagonsDeadlock = new List<HexagonControl>();
        int _columbias = (row % 2) == 0 ? 1 : -1;
        float Magnitude = 0;
        HexagonControl hexagonControl = null;

        #region AddToList
        if (column < mapNav.GetLength(1) - 1)
            hexagonsDeadlock.Add(mapNav[row, column + 1]);

        if (column > 0)
            hexagonsDeadlock.Add(mapNav[row, column - 1]);

        if (row < mapNav.GetLength(0) - 1)
        {
            hexagonsDeadlock.Add(mapNav[row + 1, column]);

            if (column + _columbias > 0 && column + _columbias < mapNav.GetLength(1) - 1)
                hexagonsDeadlock.Add(mapNav[row + 1, column + _columbias]);
        }

        if (row > 0)
        {
            hexagonsDeadlock.Add(mapNav[row - 1, column]);
            if (column + _columbias > 0 && column + _columbias < mapNav.GetLength(1) - 1)
                hexagonsDeadlock.Add(mapNav[row - 1, column + _columbias]);
        }
        #endregion

        if (_listDeadlock.Count > 0)
        {
            for (int i = 0; i < _listDeadlock.Count; i++)
            {
                for (int j = 0; j < hexagonsDeadlock.Count; j++)
                {
                    if (mapNav[hexagonsDeadlock[j].Row, hexagonsDeadlock[j].Column] == mapNav[_listDeadlock[i].Row, _listDeadlock[i].Column])
                    {
                        hexagonsDeadlock.Remove(hexagonsDeadlock[j]);
                    }
                }
            }
        }


        for (int i = 0; i < hexagonsDeadlock.Count; i++)
        {
            if (hexagonsDeadlock[i].TypeHexagon == 1)
            {
                if (hexagonControl == null)
                {
                    Magnitude = (ListPoint[0].transform.position - hexagonsDeadlock[i].transform.position).magnitude;
                    hexagonControl = hexagonsDeadlock[i];
                }

                if (Magnitude > (ListPoint[0].transform.position - hexagonsDeadlock[i].transform.position).magnitude)
                {
                    Magnitude = (ListPoint[0].transform.position - hexagonsDeadlock[i].transform.position).magnitude;
                    hexagonControl = hexagonsDeadlock[i];
                }
            }
        }

        if (hexagonControl != null)
        {

            _listDeadlock.Add(hexagonControl);
            //Debug.Log(hexagonControl.Row + " " + hexagonControl.Column);
            //hexagonControl.Flag();
            BreakingTheDeadlock(hexagonControl.Row, hexagonControl.Column);
        }
        else if (hexagonControl == null && (row == 0))
        {
            mapNav[_listDeadlock[0].Row, _listDeadlock[0].Column].Flag();
            //BreakingTheDeadlock(_listDeadlock[0].Row, _listDeadlock[0].Column);
        }
        else if (hexagonControl == null && (row != 0))
        {
            int way = (row < _listDeadlock[0].Row) ? -1 : 1;
            for (int i = 0; i < hexagonsDeadlock.Count; i++)
            {
                if (hexagonsDeadlock[i].TypeHexagon != 1 && (hexagonsDeadlock[i].Row == row + way))
                {
                    //Debug.Log("row"+ hexagonsDeadlock[i].Row+"col"+hexagonsDeadlock[i].Column+"  " + (ListPoint[0].transform.position - hexagonsDeadlock[i].transform.position).magnitude);
                    if (hexagonControl == null)
                    {
                        Magnitude = (ListPoint[0].transform.position - hexagonsDeadlock[i].transform.position).magnitude;
                        hexagonControl = hexagonsDeadlock[i];
                    }

                    if (Magnitude > (ListPoint[0].transform.position - hexagonsDeadlock[i].transform.position).magnitude)
                    {
                        Magnitude = (ListPoint[0].transform.position - hexagonsDeadlock[i].transform.position).magnitude;
                        hexagonControl = hexagonsDeadlock[i];
                    }
                }
            }

            ListPoint.Add(mapNav[hexagonControl.Row, hexagonControl.Column]);
            Debug.Log("Two" + hexagonControl.Row + " " + hexagonControl.Column);
            int Row = _listWay[0].Row;
            int Column = _listWay[0].Column;

            _listWay.Clear();
            SearchForAWay(Row, Column);
        }
    }
    private static int AnotherWay(List<HexagonControl> hexagons, int I)
    {
        float Magnitude = 0;
        int index = 0;
        for (int i = 0; i < hexagons.Count; i++)
        {
            if (i != I)
            {
                if (hexagons[i].TypeHexagon != 1)
                {
                    if (Magnitude == 0)
                    {
                        Magnitude = (ListPoint[ListPoint.Count - 1].transform.position - hexagons[i].transform.position).magnitude;
                        index = i;
                    }
                    if (Magnitude > (ListPoint[ListPoint.Count - 1].transform.position - hexagons[i].transform.position).magnitude)
                    {
                        Magnitude = (ListPoint[ListPoint.Count - 1].transform.position - hexagons[i].transform.position).magnitude;
                        index = i;
                    }
                }
            }
        }
        return index;
    }
}
