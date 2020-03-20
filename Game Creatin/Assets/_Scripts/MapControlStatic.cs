using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapControlStatic
{
    public static HexagonControl[,] mapNav = new HexagonControl[9, 20];//масив содержащий все 6-ти угольники
    private static List<HexagonControl> _listWay = new List<HexagonControl>();    //список содержащий кротчай ший путь 
    private static List<HexagonControl> _listDeadlock = new List<HexagonControl>();    //список содержащий информацию про тупик 
    public static List<HexagonControl> ListPoint = new List<HexagonControl>();    //список вершин по которым надо проййтись 
    private static List<HexagonControl> _listProvenHexagons = new List<HexagonControl>();
    private static List<HexagonControl> PassedPoints = new List<HexagonControl>();//пройденные вершины 
    private static List<HexagonControl> PossibleoWrkaround = new List<HexagonControl>();
    private static int _oldRow = 3, _oldColunm = 10;
    private static int _namberPoint, _numberOfPaths, _numberOfPathsSelect;


    public static void SearchForAWay(int _row, int _column)    //метод писка пути 
    {
        if (_listWay.Count < 1)
        {
            _listWay.Add(mapNav[_row, _column]);
            _namberPoint = PointSelection(_row, _column);
        }
        List<HexagonControl> hexagons = new List<HexagonControl>();// список всех соседей 6-ти угольника
        int _columbias = (_row % 2) == 0 ? 1 : -1;
        HexagonControl hexagonControl = null;//нужный 6-ти угольник  
        float Magnitude;
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

        Magnitude = (ListPoint[_namberPoint].transform.position - mapNav[_row, _column].transform.position).magnitude;

        //Debug.Log(2);
        for (int i = 0; i < hexagons.Count; i++)
        {
            if (hexagons[i].FreedomTest())
            {
                if ((Magnitude > (ListPoint[_namberPoint].transform.position - hexagons[i].transform.position).magnitude))
                {
                    //if ((hexagons[i].Row == _oldRow && hexagons[i].Column == _oldColunm))
                    //{
                    //    Debug.Log("gavnoEbat");

                    //    int iNew = AnotherWay(hexagons, i);
                    //    Magnitude = (ListPoint[_namberPoint].transform.position - hexagons[iNew].transform.position).magnitude;
                    //    hexagonControl = hexagons[iNew];
                    //}
                    //else
                    //{
                        Magnitude = (ListPoint[_namberPoint].transform.position - hexagons[i].transform.position).magnitude;
                        hexagonControl = hexagons[i];
                    //}
                }
            }

        }


        if (hexagonControl != null)
        {
            //_oldRow = _listWay[_listWay.Count - 1].Row;
            //_oldColunm = _listWay[_listWay.Count - 1].Column;
            _listWay.Add(hexagonControl);

            if (mapNav[hexagonControl.Row, hexagonControl.Column] != mapNav[ListPoint[_namberPoint].Row, ListPoint[_namberPoint].Column])
            {
                SearchForAWay(hexagonControl.Row, hexagonControl.Column);
            }
            else if (_namberPoint > 0)
            {

                _namberPoint = PointSelection(hexagonControl.Row, hexagonControl.Column);
                SearchForAWay(hexagonControl.Row, hexagonControl.Column);
            }
            else
            {
                //Debug.Log(ListPoint.Count);
                for (int i = 0; i < ListPoint.Count; i++)
                {
                    ListPoint[i].Flag();
                }
                for (int i = 0; i < _listWay.Count; i++)
                {
                    _listWay[i].Flag();
                }
            }
        }
        else
        {
            //Debug.Log(_listWay[_listWay.Count - 1].Row + " " + (_listWay[_listWay.Count - 1].Column - 1));
            //Debug.Log("new22 " + _row + " " + _column);
            //Debug.Log("new " + hexagonControl.Row + " " + hexagonControl.Column);

            for (int i = 0; i < hexagons.Count; i++)
            {
                if (!hexagons[i].FreedomTest())
                {
                    if (Magnitude > (ListPoint[_namberPoint].transform.position - hexagons[i].transform.position).magnitude)
                    {
                        Magnitude = (ListPoint[_namberPoint].transform.position - hexagons[i].transform.position).magnitude;
                        hexagonControl = hexagons[i];
                    }
                }
            }

            if (ListPoint.Count >= 3)
            {
                //hexagonControl.Flag();

                for (int i = 0; i < _listWay.Count; i++)
                {
                    _listWay[i].Flag();
                }

                for (int i = 0; i < ListPoint.Count; i++)
                {
                    ListPoint[i].Flag();
                    Debug.Log(ListPoint[i].Row + " " + (ListPoint[i].Column));

                }
                Debug.Log("namberPoint " + ListPoint[_namberPoint].Row + " " + (ListPoint[_namberPoint].Column));
                Debug.Log("Lisr" + _listWay.Count);
                return;
            }

            if ((ListPoint.Count >= 10) || (ListPoint.Count >= 4 && _listWay.Count <= 4))
            {
                Debug.LogError(ListPoint.Count);
                return;
            }
            if (hexagonControl == null)
            {
                Debug.Log(ListPoint.Count);
            }
            _listDeadlock.Add(MapControlStatic.mapNav[hexagonControl.Row, hexagonControl.Column]);
            if (ListPoint.Count >= 3)
                MapControlStatic.mapNav[hexagonControl.Row, hexagonControl.Column].Flag();
            _numberOfPaths = NumberOfWorkarounds(hexagonControl.Row, hexagonControl.Column);
            _numberOfPathsSelect = _numberOfPaths;

            BreakingTheDeadlock(hexagonControl.Row, hexagonControl.Column);

        }
    }

    private static void BreakingTheDeadlock(int row, int column)    //метод писка выхода из тупика  
    {
        Debug.Log("-----------------------------------------------------------------------------");
        List<HexagonControl> hexagons = new List<HexagonControl>();
        hexagons.AddRange(MapControlStatic.mapNav[row, column].peaks);
        HexagonControl hexagonControl = null;//нужный 6-ти угольник  
        float Magnitude = 0;

        if (hexagons.Count > 1)
        {
            for (int i = 0; i < ListPoint.Count; i++)
            {
                for (int j = 0; j < hexagons.Count; j++)
                {
                    if (MapControlStatic.mapNav[hexagons[j].Row, hexagons[j].Column] == MapControlStatic.mapNav[ListPoint[i].Row, ListPoint[i].Column])
                    {
                        hexagons.Remove(hexagons[j]);
                    }
                }
            }
            for (int i = 0; i < hexagons.Count; i++)
            {
                //Debug.Log("namePoint " + hexagons[i].Row + " " + (hexagons[i].Column));
                //Debug.Log((MapControlStatic.mapNav[hexagons[i].Row, hexagons[i].Column].transform.position - MapControlStatic.mapNav[row, column].transform.position).magnitude);
                //Debug.Log((MapControlStatic.mapNav[hexagons[i].Row, hexagons[i].Column].transform.position - MapControlStatic.mapNav[ListPoint[0].Row, ListPoint[0].Column].transform.position).magnitude);

                if (hexagonControl == null)
                {
                    Magnitude = (MapControlStatic.mapNav[hexagons[i].Row, hexagons[i].Column].transform.position - MapControlStatic.mapNav[row, column].transform.position).magnitude;
                    hexagonControl = hexagons[i];
                }
                if (Magnitude == (MapControlStatic.mapNav[hexagons[i].Row, hexagons[i].Column].transform.position - MapControlStatic.mapNav[row, column].transform.position).magnitude)
                {
                    Debug.Log(1);
                }

                if (Magnitude > (MapControlStatic.mapNav[hexagons[i].Row, hexagons[i].Column].transform.position - MapControlStatic.mapNav[row, column].transform.position).magnitude)
                {
                    Magnitude = (MapControlStatic.mapNav[hexagons[i].Row, hexagons[i].Column].transform.position - MapControlStatic.mapNav[row, column].transform.position).magnitude;
                    hexagonControl = hexagons[i];
                    Debug.Log(" Magnitude " + Magnitude);
                }
            }
        }
        else
        {
            hexagonControl = hexagons[0];
        }
        Debug.Log("hexagonControl " + hexagonControl.Row + " " + (hexagonControl.Column));

        ListPoint.Add(hexagonControl);
        int Row = _listWay[0].Row;
        int Column = _listWay[0].Column;

        _listWay.Clear();
        SearchForAWay(Row, Column);

    }
    private static int AnotherWay(List<HexagonControl> hexagons, int I)
    {
        float Magnitude = 0;
        int index = 0;
        for (int i = 0; i < hexagons.Count; i++)
        {
            if (i != I)
            {
                if (hexagons[i].FreedomTest())
                {
                    if (Magnitude == 0)
                    {
                        Magnitude = (ListPoint[0/*_namberPoint*/].transform.position - hexagons[i].transform.position).magnitude;
                        index = i;
                    }
                    if (Magnitude > (ListPoint[0].transform.position - hexagons[i].transform.position).magnitude)
                    {
                        Magnitude = (ListPoint[0].transform.position - hexagons[i].transform.position).magnitude;
                        index = i;
                    }
                }
            }
        }
        return index;
    }
    private static void RepeatCheck(out HexagonControl hexagon, HexagonControl hexagonInstance)
    {
        for (int i = 0; i < ListPoint.Count; i++)
        {
            if (mapNav[hexagonInstance.Row, hexagonInstance.Column] == mapNav[ListPoint[i].Row, ListPoint[i].Column])
            {
                hexagon = MapControlStatic.SearchForAnotherPeak(hexagonInstance.Row, hexagonInstance.Column);
                RepeatCheck(out hexagon, hexagon);
                return;
            }
        }
        hexagon = hexagonInstance;
    }
    private static HexagonControl SearchForAnotherPeak(int _row, int _column)
    {
        List<HexagonControl> hexagonsAnother = new List<HexagonControl>();
        int _columBias = (_row % 2) == 0 ? 1 : -1;
        float Magnitude = 0;
        HexagonControl hexagonControl = null;

        #region AddToListHeighbors
        if (_column < mapNav.GetLength(1) - 1)
            hexagonsAnother.Add(mapNav[_row, _column + 1]);

        if (_column > 0)
            hexagonsAnother.Add(mapNav[_row, _column - 1]);

        if (_row < mapNav.GetLength(0) - 1)
        {
            hexagonsAnother.Add(mapNav[_row + 1, _column]);

            if (_column + _columBias > 0 && _column + _columBias < mapNav.GetLength(1) - 1)
                hexagonsAnother.Add(mapNav[_row + 1, _column + _columBias]);
        }

        if (_row > 0)
        {
            hexagonsAnother.Add(mapNav[_row - 1, _column]);
            if (_column + _columBias > 0 && _column + _columBias < mapNav.GetLength(1) - 1)
                hexagonsAnother.Add(mapNav[_row - 1, _column + _columBias]);
        }
        #endregion
        for (int i = 0; i < hexagonsAnother.Count; i++)
        {
            if (hexagonsAnother[i].FreedomTest())
            {
                if (hexagonControl == null)
                {
                    Magnitude = (_listWay[0].transform.position - hexagonsAnother[i].transform.position).magnitude;
                    hexagonControl = hexagonsAnother[i];
                }

                if (Magnitude > (_listWay[0].transform.position - hexagonsAnother[i].transform.position).magnitude)
                {
                    Magnitude = (_listWay[0].transform.position - hexagonsAnother[i].transform.position).magnitude;
                    hexagonControl = hexagonsAnother[i];
                }
            }
        }

        return hexagonControl;
    }
    private static int PointSelection(int row, int column)
    {
        float Mag = 0;
        int I = 0;
        int IPoint = 0;
        List<HexagonControl> hexagonControls = new List<HexagonControl>();
        hexagonControls.AddRange(ListPoint);

        if (PassedPoints.Count > 0)
        {
            for (int j = 0; j < PassedPoints.Count; j++)
            {
                for (int i = 0; i < hexagonControls.Count; i++)
                {
                    if (mapNav[PassedPoints[j].Row, PassedPoints[j].Column] == mapNav[hexagonControls[i].Row, hexagonControls[i].Column])
                    {
                        hexagonControls.Remove(hexagonControls[i]);
                    }
                }
            }
        }
        if (hexagonControls.Count == 1)
        {
            I = 0;
        }
        else
        {
            hexagonControls.Remove(hexagonControls[0]);
            for (int i = 0; i < hexagonControls.Count; i++)
            {
                if (Mag == 0)
                {
                    Mag = (mapNav[hexagonControls[i].Row, hexagonControls[i].Column].transform.position - mapNav[row, column].transform.position).magnitude;
                    IPoint = i;
                }
                if ((mapNav[hexagonControls[i].Row, hexagonControls[i].Column].transform.position - mapNav[row, column].transform.position).magnitude < Mag)
                {
                    Mag = (mapNav[hexagonControls[i].Row, hexagonControls[i].Column].transform.position - mapNav[row, column].transform.position).magnitude;
                    IPoint = i;
                }
            }

            for (int i = 0; i < ListPoint.Count; i++)
            {
                if (mapNav[ListPoint[i].Row, ListPoint[i].Column] == mapNav[hexagonControls[IPoint].Row, hexagonControls[IPoint].Column])
                {
                    I = i;
                }
            }
        }
        if (hexagonControls.Count == 3)
        {
            Debug.Log(I);
        }

        PassedPoints.Add(ListPoint[I]);

        return I;
    }
    private static int NumberOfWorkarounds(int row, int column)
    {
        List<HexagonControl> hexagons = new List<HexagonControl>();
        int _columBias = (row % 2) == 0 ? 1 : -1;
        int neighbors = 0;
        #region AddToList
        if (column < mapNav.GetLength(1) - 1)
            hexagons.Add(mapNav[row, column + 1]);

        if (column > 0)
            hexagons.Add(mapNav[row, column - 1]);

        if (row < mapNav.GetLength(0) - 1)
        {
            hexagons.Add(mapNav[row + 1, column]);

            if (column + _columBias > 0 && column + _columBias < mapNav.GetLength(1) - 1)
                hexagons.Add(mapNav[row + 1, column + _columBias]);
        }

        if (row > 0)
        {
            hexagons.Add(mapNav[row - 1, column]);
            if (column + _columBias > 0 && column + _columBias < mapNav.GetLength(1) - 1)
                hexagons.Add(mapNav[row - 1, column + _columBias]);
        }
        #endregion

        for (int i = 0; i < hexagons.Count; i++)
        {
            if (!hexagons[i].FreedomTest())
            {
                neighbors++;
            }
        }
        return neighbors;
    }
    private static HexagonControl FindingTheNearestPeak()
    {
        HexagonControl hexagon = null;
        float Magnitude = 0;

        for (int i = 0; i < PossibleoWrkaround.Count; i++)
        {
            if (PossibleoWrkaround[i] != null)
            {
                if (hexagon == null)
                {
                    Magnitude = (_listDeadlock[0].transform.position - PossibleoWrkaround[i].transform.position).magnitude;
                    hexagon = PossibleoWrkaround[i];
                }

                if (Magnitude > (_listDeadlock[0].transform.position - PossibleoWrkaround[i].transform.position).magnitude)
                {
                    Magnitude = (_listDeadlock[0].transform.position - PossibleoWrkaround[i].transform.position).magnitude;
                    hexagon = PossibleoWrkaround[i];
                }
            }
            else
            {
                Debug.Log("rrrr");
            }
        }
        return hexagon;
    }
}
