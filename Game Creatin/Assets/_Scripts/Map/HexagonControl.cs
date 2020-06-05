﻿using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class HexagonControl : MonoBehaviour
{
    [SerializeField]
    private GameObject _flag;
    public DataHexNav Data;
    private Dictionary<HexagonControl, List<HexagonControl>> ShortWay = new Dictionary<HexagonControl, List<HexagonControl>>();

    public HexagonControl Elevation, Floor;
    //[System.NonSerialized]
    public int Row, Column;

    public IMove ObjAbove;// интерфейс стоящего

    [Range(0, 3)]
    public int TypeHexagon;
    //[SerializeField]
    public bool IsFree = true;

    private void Awake()
    {
        if (Elevation != null)
        {
            Elevation.Floor = this;
            if (MapControlStatic.X == 0 && MapControlStatic.Y == 0)
            {
                MapControlStatic.X = Elevation.transform.position.x - transform.position.x;
                MapControlStatic.Y = Elevation.transform.position.y - transform.position.y;
            }
        }
        if (TypeHexagon!=1)
            DataRecords();
    }
    private void DataRecords()
    {
        var Data = GetComponent<DataHexNav>();

        if (Data == null)
        {
            Debug.LogError("Dissing data file");
        }
        else
        {
            int j = 0;
            this.Data = Data;
            List<HexagonControl> ListWay = new List<HexagonControl>();

            for (int i = 0; i < this.Data.Way.Count; i++)
            {
                if (this.Data.Way[i] !=null)
                {
                    ListWay.Add(this.Data.Way[i]);
                }
                else
                {
                    ShortWay[this.Data.EndWay[j]] = new List<HexagonControl>();
                    ShortWay[this.Data.EndWay[j]].AddRange(ListWay);
                    ListWay.Clear();
                    j++;
                }
            }
        }
        //Debug.Log(ShortWay.Count);
        //if (Row==0&&Column==0)
        //{
        //    for (int i = 0; i < ShortWay[data._endWay[74]].Count; i++)
        //    {
        //        ShortWay[data._endWay[74]][i].Flag();
        //    }
        //}
    }
    public bool FreedomTestType(bool Elevtion)
    {
        if (!Elevtion)
        {
            if (TypeHexagon == 1 || (gameObject.layer == 10))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            if (TypeHexagon == 3)
            {
                return true;
            }
            else if (gameObject.layer != 10)
            {
                return false;
            }
            else if (TypeHexagon == 1)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
    }
    public void Flag()
    {
        Instantiate(_flag, transform);
    }
    public HexagonControl GetHexagonMain(bool elevation)
    {
        if (Elevation != null)
        {
            if (!Elevation.FreedomTestType(elevation))
            {
                return null;
            }
            else
            {
                return Elevation;
            }
        }
        else
        {
            if (!FreedomTestType(elevation))
            {
                return null;
            }
            else
            {
                return this;
            }
        }
    }
    public HexagonControl GetHexagonMain()
    {
        if (Elevation != null)
        {
            return Elevation;
        }
        else
        {
            return this;
        }
    }
    public void NamberHex()
    {
        if (TypeHexagon != 2)
        {
            Row = System.Convert.ToInt32
                (transform.parent.name);
            Column = System.Convert.ToInt32
                (name);
        }
    }
    public void Contact(IMove move)
    {
        IsFree = false;

        ObjAbove = move;
    }
    public void Gap()
    {
        IsFree = true;

        ObjAbove = null;
    }

    public void CheckDataComponent()
    {
        var Data = GetComponent<DataHexNav>();
        if (Data == null)
        {
            gameObject.AddComponent<DataHexNav>();
            this.Data = GetComponent<DataHexNav>();
        }
        else
        {
            this.Data = Data;
        }
    }
    public List<HexagonControl> GetWay(HexagonControl where)
    {
        if (ShortWay.ContainsKey(where))
        {
            List<HexagonControl> hexagonControls = new List<HexagonControl>();
            hexagonControls.AddRange(ShortWay[where]);
            return hexagonControls;
        }
        else
        {
            Debug.Log("Unknown hexagon");
            return null;
        }
    }
}
