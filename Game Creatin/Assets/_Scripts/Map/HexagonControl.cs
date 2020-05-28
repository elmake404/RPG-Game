using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonControl : MonoBehaviour
{
    [SerializeField]
    private GameObject _flag;

    [HideInInspector]
    public Dictionary<HexagonControl, List<HexagonControl>> ShortWay = new Dictionary<HexagonControl, List<HexagonControl>>();
    public HexagonControl Elevation, Floor;
    [System.NonSerialized]
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
}
