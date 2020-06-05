using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapControl : MonoBehaviour
{
    [SerializeField]
    private NavSurface _navSurface;
    [SerializeField]
    private EnemyManager enemyManager;
    [SerializeField]
    private Transform[] hexagons;
    [SerializeField]
    private HeroControl[] _heroControls;
    [SerializeField]
    private HexagonControl[] _arreyVertex, _elevation;

    private static Vector2 pos;

    [HideInInspector]
    public static HexagonControl[,] MapNav = new HexagonControl[9, 20];//масив содержащий все 6-ти угольники

    void Awake()//измени
    {
        //enemyManager.InitializationList(_heroControls);
        for (int i = 0; i < hexagons.Length; i++)
        {
            hexagons[i].name = i.ToString();
            for (int j = 0; j < hexagons[i].childCount; j++)
            {
                HexagonControl hexagon = hexagons[i].GetChild(j).GetComponent<HexagonControl>();
                MapNav[i, j] = hexagon;
                hexagons[i].GetChild(j).name = j.ToString();
                hexagon.NamberHex();
                if (hexagon.GetHexagonMain().TypeHexagon != 1)
                    _navSurface.ListHexagonControls.Add(hexagon.GetHexagonMain());
            }
        }

        //for (int i = 0; i < _heroControls.Length; i++)
        //{
        //    _heroControls[i].Initialization(_arreyVertex, enemyManager);
        //}

        //for (int i = 0; i < _enemyControls.Length; i++)
        //{
        //    _enemyControls[i].InitializationHero(_heroControls);
        //    _enemyControls[i].Navigation.InitializationVertex(_arreyVertex);
        //}
    }

    void Update()
    {

    }
    //private void GraphRecord()
    //{
    //    List<HexagonControl> hexagonsVartex = new List<HexagonControl>();
    //    hexagonsVartex.AddRange(_arreyVertex);
    //    MapControlStatic.GraphStatic = new Graph(hexagonsVartex);
    //    //Debug.Log(graph.Length);
    //    for (int i = 0; i < MapControlStatic.GraphStatic.Length - 1; i++)
    //    {
    //        bool IsElevation = false;

    //        for (int j = i + 1; j < MapControlStatic.GraphStatic.Length; j++)
    //        {

    //            Vector2 StartPosition = MapControlStatic.GraphStatic[i].NodeHexagon.transform.position;
    //            Vector2 direction = MapControlStatic.GraphStatic[j].NodeHexagon.transform.position;

    //            bool NoRibs = false;

    //            if (hexagonsVartex[i].TypeHexagon <= 0 || (hexagonsVartex[i].TypeHexagon == 3 && MapControlStatic.GraphStatic[j].NodeHexagon.gameObject.layer != 10))
    //            {
    //                IsElevation = false;
    //                if (!MapControlStatic.CollisionCheck(StartPosition, direction, IsElevation))
    //                {
    //                    NoRibs = true;
    //                }
    //            }
    //            else
    //            {
    //                IsElevation = true;
    //                if (!MapControlStatic.CollisionCheckElevation(StartPosition, direction, IsElevation))
    //                {
    //                    NoRibs = true;
    //                }
    //            }
    //            if (!NoRibs)
    //            {
    //                float magnitude = (MapControlStatic.GraphStatic[i].NodeHexagon.transform.position - MapControlStatic.GraphStatic[j].NodeHexagon.transform.position).magnitude;
    //                MapControlStatic.GraphStatic[i].Connect(MapControlStatic.GraphStatic[j], magnitude,null);
    //            }
    //        }
    //    }
    //}    
    private static void OwnershipCheck(int _row, int _column, out float Row, out int Column, Vector2 pos)
    {
        if (_row > MapNav.GetLength(0) - 1)
        {
            _row = MapNav.GetLength(0) - 1;
        }
        else if (_row < 0)
        {
            _row = 0;
        }

        if (_column > MapNav.GetLength(1) - 1)
        {
            _column = MapNav.GetLength(1) - 1;
        }
        List<HexagonControl> hexagons = new List<HexagonControl>();
        int _columbias = (_row % 2) == 0 ? 1 : -1;
        if (((Vector2)MapNav[_row, _column].transform.position - pos).magnitude <= 1.8f)
        {
            Row = _row;
            Column = _column;
            return;
        }

        #region AddToList
        hexagons.Add(MapNav[_row, _column]);
        if (_column < MapNav.GetLength(1) - 1)
            hexagons.Add(MapNav[_row, _column + 1]);

        if (_column > 0)
            hexagons.Add(MapNav[_row, _column - 1]);

        if (_row < MapNav.GetLength(0) - 1)
        {
            hexagons.Add(MapNav[_row + 1, _column]);

            if (_column + _columbias > 0 && _column + _columbias < MapNav.GetLength(1) - 1)
                hexagons.Add(MapNav[_row + 1, _column + _columbias]);
        }

        if (_row > 0)
        {
            hexagons.Add(MapNav[_row - 1, _column]);
            if (_column + _columbias > 0 && _column + _columbias < MapNav.GetLength(1) - 1)
                hexagons.Add(MapNav[_row - 1, _column + _columbias]);
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
    private static HexagonControl OwnershipCheckadditional(int _row, int _column, Vector2 pos)
    {
        List<HexagonControl> hexagons = new List<HexagonControl>();
        int _columbias = (_row % 2) == 0 ? 1 : -1;

        #region AddToList
        if (_column < MapNav.GetLength(1) - 1)
            hexagons.Add(MapNav[_row, _column + 1]);

        if (_column > 0)
            hexagons.Add(MapNav[_row, _column - 1]);

        if (_row < MapNav.GetLength(0) - 1)
        {
            hexagons.Add(MapNav[_row + 1, _column]);

            if (_column + _columbias > 0 && _column + _columbias < MapNav.GetLength(1) - 1)
                hexagons.Add(MapNav[_row + 1, _column + _columbias]);
        }

        if (_row > 0)
        {
            hexagons.Add(MapNav[_row - 1, _column]);
            if (_column + _columbias > 0 && _column + _columbias < MapNav.GetLength(1) - 1)
                hexagons.Add(MapNav[_row - 1, _column + _columbias]);
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
    public void DataRecords()
    {
        pos = transform.position;
        for (int i = 0; i < hexagons.Length; i++)
        {
            hexagons[i].name = i.ToString();
            for (int j = 0; j < hexagons[i].childCount; j++)
            {
                HexagonControl hexagon = hexagons[i].GetChild(j).GetComponent<HexagonControl>();
                MapNav[i, j] = hexagon;
                hexagons[i].GetChild(j).name = j.ToString();
                hexagon.NamberHex();
                if (hexagon.GetHexagonMain().TypeHexagon != 1)
                    _navSurface.ListHexagonControls.Add(hexagon.GetHexagonMain());
            }
        }
    }

    public static HexagonControl[] GetPositionOnTheMap(Vector2 Target, Vector2 Position) // возвращает гексагон через позицию (может определить два блока)
    {

        Vector2 Normalized = (Target - Position).normalized;
        float XTarget = (float)System.Math.Round(Normalized.x, 1);
        float YTarget = (float)System.Math.Round(Normalized.y, 1);
        float Y = (Position.y - pos.y) / -3f;
        float YOld = (Position.y - pos.y) / -3f;

        int YMax = Mathf.Abs(Mathf.RoundToInt(Y));
        float Difference = 0;
        float f = ((Position.x - (pos.x - 3.46f)) / 1.73f);
        float R = Mathf.Floor(f) * 1.73f - (Position.x - (pos.x - 3.46f));
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

        float factor = (MapNav[0, 1].transform.position.x - MapNav[0, 0].transform.position.x);
        float X = ((Position.x - pos.x) / factor) + Difference;

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
        if (Mathf.Abs((float)System.Math.Round((XInt - X), 2)) == 0.5f && XTarget == 0)
        {
            if (XInt <= 19)
            {
                hexagonsList.Add(MapNav[(int)Y, XInt]);
            }
            if (XInt > 0)
            {
                hexagonsList.Add(MapNav[(int)Y, XInt - 1]);
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
            hexagonsList.Add(OwnershipCheckadditional((int)Y, XInt, Position));
            hexagonsList.Add(MapNav[(int)Y, XInt]);

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
            hexagons[0] = MapNav[(int)Y, XInt];

            return hexagons;
        }
    }
}
