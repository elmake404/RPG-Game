using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroControl : MonoBehaviour
{
    private IEnumerator MoveCorotine;

    #region 
    private List<HexagonControl> _listWay = new List<HexagonControl>();    //список содержащий кротчай ший путь 
    private List<HexagonControl> _listDeadlock = new List<HexagonControl>();    //список содержащий информацию про тупик 
    private List<HexagonControl> _listProvenHexagons = new List<HexagonControl>();
    private List<HexagonControl> PassedPoints = new List<HexagonControl>();
    public List<HexagonControl> ListPoint = new List<HexagonControl>();   //список вершин по которым надо проййтись 

    private int _oldRow, _oldColunm;
    private int _namberPoint;
    #endregion

    [SerializeField]
    private float _speed;

    [System.NonSerialized]
    public int HexagonRow = 0, HexagonColumn = 0;

    void Start()
    {
        MoveCorotine = Movement();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(1);
        }

    }
    private IEnumerator Movement()
    {

        List<HexagonControl> ListPos = new List<HexagonControl>();
        ListPos.AddRange(_listWay);
        _listWay.Clear();

        while (ListPos.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, ListPos[0].transform.position, _speed);
            if ((ListPos[0].transform.position - transform.position).magnitude <= 3.7f)
            {
                HexagonRow = ListPos[0].Row;
                HexagonColumn = ListPos[0].Column;

                ListPos.Remove(ListPos[0]);
            }
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void SearchForAWay(int _row, int _column)    //метод писка пути 
    {
        if (_listWay.Count < 1)
        {
            _listWay.Add(MapControlStatic.mapNav[_row, _column]);
            _namberPoint = PointSelection(_row, _column);
        }

        List<HexagonControl> hexagons = new List<HexagonControl>();// список всех соседей 6-ти угольника
        if (ListPoint.Count < 1)
        {
            return;
        }
        float Magnitude = (ListPoint[_namberPoint].transform.position - MapControlStatic.mapNav[_row, _column].transform.position).magnitude;
        int _columBias = (_row % 2) == 0 ? 1 : -1;
        HexagonControl hexagonControl = null;//нужный 6-ти угольник  

        #region AddToListHeighbors
        if (_column < MapControlStatic.mapNav.GetLength(1) - 1)
            hexagons.Add(MapControlStatic.mapNav[_row, _column + 1]);

        if (_column > 0)
            hexagons.Add(MapControlStatic.mapNav[_row, _column - 1]);

        if (_row < MapControlStatic.mapNav.GetLength(0) - 1)
        {
            hexagons.Add(MapControlStatic.mapNav[_row + 1, _column]);

            if (_column + _columBias > 0 && _column + _columBias < MapControlStatic.mapNav.GetLength(1) - 1)
                hexagons.Add(MapControlStatic.mapNav[_row + 1, _column + _columBias]);
        }

        if (_row > 0)
        {
            hexagons.Add(MapControlStatic.mapNav[_row - 1, _column]);
            if (_column + _columBias > 0 && _column + _columBias < MapControlStatic.mapNav.GetLength(1) - 1)
                hexagons.Add(MapControlStatic.mapNav[_row - 1, _column + _columBias]);
        }
        #endregion

        for (int i = 0; i < hexagons.Count; i++)
        {
            if (hexagons[i].FreedomTest())
            {
                if (Magnitude > (ListPoint[_namberPoint].transform.position - hexagons[i].transform.position).magnitude)
                {
                    if ((hexagons[i].Row == _oldRow && hexagons[i].Column == _oldColunm))
                    {
                        int iNew = AnotherWay(hexagons, i);
                        Magnitude = (ListPoint[_namberPoint].transform.position - hexagons[iNew].transform.position).magnitude;
                        hexagonControl = hexagons[iNew];
                    }
                    else
                    {
                        Magnitude = (ListPoint[_namberPoint].transform.position - hexagons[i].transform.position).magnitude;
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

            if (MapControlStatic.mapNav[hexagonControl.Row, hexagonControl.Column] != MapControlStatic.mapNav[ListPoint[_namberPoint].Row, ListPoint[_namberPoint].Column])
            {
                SearchForAWay(hexagonControl.Row, hexagonControl.Column);
            }
            else if (_namberPoint > 0)
            {
                _namberPoint = _namberPoint = PointSelection(hexagonControl.Row, hexagonControl.Column);
                SearchForAWay(hexagonControl.Row, hexagonControl.Column);
            }
            else
            {
                StopCoroutine(MoveCorotine);

                _listWay.Reverse();
                MoveCorotine = Movement();
                StartCoroutine(MoveCorotine);

                ListPoint.Clear();
            }
        }
        else
        {
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

            _listDeadlock.Add(MapControlStatic.mapNav[hexagonControl.Row, hexagonControl.Column]);
            BreakingTheDeadlock(hexagonControl.Row, hexagonControl.Column);
        }
    }

    private void BreakingTheDeadlock(int row, int column)    //метод писка выхода из тупика  
    {
        List<HexagonControl> hexagonsDeadlock = new List<HexagonControl>();
        int _columBias = (row % 2) == 0 ? 1 : -1;
        float Magnitude = 0;
        HexagonControl hexagonControl = null;

        #region AddToList
        if (column < MapControlStatic.mapNav.GetLength(1) - 1)
            hexagonsDeadlock.Add(MapControlStatic.mapNav[row, column + 1]);

        if (column > 0)
            hexagonsDeadlock.Add(MapControlStatic.mapNav[row, column - 1]);

        if (row < MapControlStatic.mapNav.GetLength(0) - 1)
        {
            hexagonsDeadlock.Add(MapControlStatic.mapNav[row + 1, column]);

            if (column + _columBias > 0 && column + _columBias < MapControlStatic.mapNav.GetLength(1) - 1)
                hexagonsDeadlock.Add(MapControlStatic.mapNav[row + 1, column + _columBias]);
        }

        if (row > 0)
        {
            hexagonsDeadlock.Add(MapControlStatic.mapNav[row - 1, column]);
            if (column + _columBias > 0 && column + _columBias < MapControlStatic.mapNav.GetLength(1) - 1)
                hexagonsDeadlock.Add(MapControlStatic.mapNav[row - 1, column + _columBias]);
        }
        #endregion

        if (_listDeadlock.Count > 0)
        {
            for (int i = 0; i < _listDeadlock.Count; i++)
            {
                for (int j = 0; j < hexagonsDeadlock.Count; j++)
                {
                    if (MapControlStatic.mapNav[hexagonsDeadlock[j].Row, hexagonsDeadlock[j].Column] == MapControlStatic.mapNav[_listDeadlock[i].Row, _listDeadlock[i].Column])
                    {
                        hexagonsDeadlock.Remove(hexagonsDeadlock[j]);
                    }
                }
            }
        }

        if (_listProvenHexagons.Count > 0)
        {
            for (int i = 0; i < _listProvenHexagons.Count; i++)
            {
                for (int j = 0; j < hexagonsDeadlock.Count; j++)
                {
                    if (MapControlStatic.mapNav[hexagonsDeadlock[j].Row, hexagonsDeadlock[j].Column] == MapControlStatic.mapNav[_listProvenHexagons[i].Row, _listProvenHexagons[i].Column])
                    {

                        hexagonsDeadlock.Remove(hexagonsDeadlock[j]);
                    }
                }
            }
        }
        _listProvenHexagons.AddRange(hexagonsDeadlock);

        for (int i = 0; i < hexagonsDeadlock.Count; i++)
        {
            if (!hexagonsDeadlock[i].FreedomTest())
            {
                if (hexagonControl == null)
                {
                    Magnitude = (_listWay[0].transform.position - hexagonsDeadlock[i].transform.position).magnitude;
                    hexagonControl = hexagonsDeadlock[i];
                }

                if (Magnitude > (_listWay[0].transform.position - hexagonsDeadlock[i].transform.position).magnitude)
                {
                    Magnitude = (_listWay[0].transform.position - hexagonsDeadlock[i].transform.position).magnitude;
                    hexagonControl = hexagonsDeadlock[i];
                }
            }
        }

        if (hexagonControl != null)
        {

            _listDeadlock.Add(hexagonControl);
            BreakingTheDeadlock(hexagonControl.Row, hexagonControl.Column);
        }
        else if (hexagonControl == null && (row == 0 || column == 0))
        {
            _listProvenHexagons.Clear();
            BreakingTheDeadlock(_listDeadlock[0].Row, _listDeadlock[0].Column);
        }
        else if (hexagonControl == null && (row != 0 || column != 0))
        {
            int RowWay;
            int ColumnWay;

            if (_listDeadlock[_listDeadlock.Count - 2].Row != row)
            {
                int _columType = (row % 2) == 0 ? 1 : -1;
                RowWay = (row - _listDeadlock[_listDeadlock.Count - 2].Row) > 0 ? 1 : -1;
                if (_listDeadlock[_listDeadlock.Count - 2].Column != column)
                {
                    ColumnWay = ((column - _listDeadlock[_listDeadlock.Count - 2].Column) > 0 ? 1 : -1) + _columType;
                }
                else
                {
                    ColumnWay = _columType;
                }
            }
            else
            {
                RowWay = row - _listDeadlock[_listDeadlock.Count - 2].Row;
                ColumnWay = column - _listDeadlock[_listDeadlock.Count - 2].Column;
            }

            hexagonControl = MapControlStatic.mapNav[(row + RowWay), (column + ColumnWay)];

            RepeatCheck(out hexagonControl, hexagonControl);

            ListPoint.Add(MapControlStatic.mapNav[hexagonControl.Row, hexagonControl.Column]);
            int Row = _listWay[0].Row;
            int Column = _listWay[0].Column;

            _listProvenHexagons.Clear();
            _listWay.Clear();
            _listDeadlock.Clear();
            SearchForAWay(Row, Column);
        }
    }
    private int AnotherWay(List<HexagonControl> hexagons, int I)
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
                        Magnitude = (ListPoint[_namberPoint].transform.position - hexagons[i].transform.position).magnitude;
                        index = i;
                    }
                    if (Magnitude > (ListPoint[_namberPoint].transform.position - hexagons[i].transform.position).magnitude)
                    {
                        Magnitude = (ListPoint[_namberPoint].transform.position - hexagons[i].transform.position).magnitude;
                        index = i;
                    }
                }
            }
        }
        return index;
    }
    private void RepeatCheck(out HexagonControl hexagon, HexagonControl hexagonInstance)
    {
        for (int i = 0; i < ListPoint.Count; i++)
        {
            if (MapControlStatic.mapNav[hexagonInstance.Row, hexagonInstance.Column] == MapControlStatic.mapNav[ListPoint[i].Row, ListPoint[i].Column])
            {
                hexagon = SearchForAnotherPeak(hexagonInstance.Row, hexagonInstance.Column);
                RepeatCheck(out hexagon, hexagon);
                return;
            }
        }
        hexagon = hexagonInstance;
    }
    private HexagonControl SearchForAnotherPeak(int _row, int _column)
    {
        List<HexagonControl> hexagonsAnother = new List<HexagonControl>();
        int _columBias = (_row % 2) == 0 ? 1 : -1;
        float Magnitude = 0;
        HexagonControl hexagonControl = null;

        #region AddToListHeighbors
        if (_column < MapControlStatic.mapNav.GetLength(1) - 1)
            hexagonsAnother.Add(MapControlStatic.mapNav[_row, _column + 1]);

        if (_column > 0)
            hexagonsAnother.Add(MapControlStatic.mapNav[_row, _column - 1]);

        if (_row < MapControlStatic.mapNav.GetLength(0) - 1)
        {
            hexagonsAnother.Add(MapControlStatic.mapNav[_row + 1, _column]);

            if (_column + _columBias > 0 && _column + _columBias < MapControlStatic.mapNav.GetLength(1) - 1)
                hexagonsAnother.Add(MapControlStatic.mapNav[_row + 1, _column + _columBias]);
        }

        if (_row > 0)
        {
            hexagonsAnother.Add(MapControlStatic.mapNav[_row - 1, _column]);
            if (_column + _columBias > 0 && _column + _columBias < MapControlStatic.mapNav.GetLength(1) - 1)
                hexagonsAnother.Add(MapControlStatic.mapNav[_row - 1, _column + _columBias]);
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
    private int PointSelection(int row, int column)
    {
        float Mag = 0;
        int I = 0;
        if (PassedPoints.Count <= 0)
        {
            Debug.Log(1);
            for (int i = 0; i < ListPoint.Count; i++)
            {
                if (Mag == 0)
                {
                    Mag = (MapControlStatic.mapNav[ListPoint[i].Row, ListPoint[i].Column].transform.position - MapControlStatic.mapNav[row, column].transform.position).magnitude;
                    I = i;
                }
                if ((MapControlStatic.mapNav[ListPoint[i].Row, ListPoint[i].Column].transform.position - MapControlStatic.mapNav[row, column].transform.position).magnitude < Mag)
                {
                    Mag = (MapControlStatic.mapNav[ListPoint[i].Row, ListPoint[i].Column].transform.position - MapControlStatic.mapNav[row, column].transform.position).magnitude;
                    I = i;
                }
            }
        }
        else
        {
            Debug.Log(2);

            for (int i = 0; i < ListPoint.Count; i++)
            {
                for (int j = 0; j < PassedPoints.Count; j++)
                {
                    if (MapControlStatic.mapNav[PassedPoints[j].Row, PassedPoints[j].Column] != MapControlStatic.mapNav[ListPoint[i].Row, ListPoint[i].Column])
                    {
                        if (Mag == 0)
                        {
                            Mag = (MapControlStatic.mapNav[ListPoint[i].Row, ListPoint[i].Column].transform.position - MapControlStatic.mapNav[row, column].transform.position).magnitude;
                            I = i;
                        }
                        if ((MapControlStatic.mapNav[ListPoint[i].Row, ListPoint[i].Column].transform.position - MapControlStatic.mapNav[row, column].transform.position).magnitude != 0
                            && ((MapControlStatic.mapNav[ListPoint[i].Row, ListPoint[i].Column].transform.position - MapControlStatic.mapNav[row, column].transform.position).magnitude < Mag))
                        {
                            Mag = (MapControlStatic.mapNav[ListPoint[i].Row, ListPoint[i].Column].transform.position - MapControlStatic.mapNav[row, column].transform.position).magnitude;
                            I = i;
                        }

                    }
                }
            }
        }
        Debug.Log("Two" + ListPoint[I].Row + " " + ListPoint[I].Column);

        PassedPoints.Add(ListPoint[I]);

        return I;
    }

    public void AddListPoint(HexagonControl hexagon)
    {
        _listWay.Clear();
        _listDeadlock.Clear();
        _listProvenHexagons.Clear();
        ListPoint.Clear();
        ListPoint.Add(hexagon);
        _namberPoint = ListPoint.Count - 1;
    }

}
