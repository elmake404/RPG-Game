using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroControl : MonoBehaviour
{
    private IEnumerator MoveCorotine;

    private List<HexagonControl> _listWay = new List<HexagonControl>();    //список содержащий кротчай ший путь 
    private List<HexagonControl> _listDeadlock = new List<HexagonControl>();    //список содержащий информацию про тупик 
    public List<HexagonControl> ListPoint = new List<HexagonControl>();   //список вершин по которым надо проййтись 
    private List<HexagonControl> _listProvenHexagons = new List<HexagonControl>();
    private int _oldRow, _oldColunm;
    private int _namberPoint;


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
        }

        List<HexagonControl> hexagons = new List<HexagonControl>();// список всех соседей 6-ти угольника
        if (ListPoint.Count < 1)
        {
            return;
        }
        float Magnitude = (ListPoint[ListPoint.Count - 1].transform.position - MapControlStatic.mapNav[_row, _column].transform.position).magnitude;
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
            if (hexagons[i].TypeHexagon != 1)
            {
                if (Magnitude > (ListPoint[ListPoint.Count - 1].transform.position - hexagons[i].transform.position).magnitude)
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

            if (MapControlStatic.mapNav[hexagonControl.Row, hexagonControl.Column] != MapControlStatic.mapNav[ListPoint[ListPoint.Count - 1].Row, ListPoint[ListPoint.Count - 1].Column])
            {
                SearchForAWay(hexagonControl.Row, hexagonControl.Column);
            }
            else if (ListPoint.Count > 1)
            {
                ListPoint.Remove(ListPoint[ListPoint.Count - 1]);
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
                if (hexagons[i].TypeHexagon == 1)
                {
                    if (Magnitude > (ListPoint[ListPoint.Count - 1].transform.position - hexagons[i].transform.position).magnitude)
                    {
                        Magnitude = (ListPoint[ListPoint.Count - 1].transform.position - hexagons[i].transform.position).magnitude;
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
            if (hexagonsDeadlock[i].TypeHexagon == 1)
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
        else if (hexagonControl == null && (row == 0))
        {
            _listProvenHexagons.Clear();
            BreakingTheDeadlock(_listDeadlock[0].Row, _listDeadlock[0].Column);
        }
        else if (hexagonControl == null && (row != 0))
        {
            int RowWay;
            int ColumnWay;

            if (_listDeadlock[_listDeadlock.Count - 2].Row != row)
            {
                int _columType = (row % 2) == 0 ? 1 : -1;
                RowWay = row - _listDeadlock[_listDeadlock.Count - 2].Row;
                ColumnWay = column - _listDeadlock[_listDeadlock.Count - 2].Column + _columType;
            }
            else
            {
                RowWay = row - _listDeadlock[_listDeadlock.Count - 2].Row;
                ColumnWay = column - _listDeadlock[_listDeadlock.Count - 2].Column;
            }

            hexagonControl = MapControlStatic.mapNav[(row + RowWay), (column + ColumnWay)];

            ListPoint.Add(MapControlStatic.mapNav[hexagonControl.Row, hexagonControl.Column]);
            //ListPoint[ListPoint.Count - 1].Flag();
            int Row = _listWay[0].Row;
            int Column = _listWay[0].Column;

            _listProvenHexagons.Clear();
            _listWay.Clear();
            SearchForAWay(Row, Column);
            _listDeadlock.Clear();
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

    private void AddListPoint(HexagonControl hexagon)
    {
        _listWay.Clear();
        _listDeadlock.Clear();
        _listProvenHexagons.Clear();
        ListPoint.Clear();
        ListPoint.Add(hexagon);
    }

}
