using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonControl : MonoBehaviour
{
    [SerializeField]
    private GameObject _flag;

    public HexagonControl[] peaks;

    [System.NonSerialized]
    public int Row, Column;


    [Range(0, 3)]
    public int TypeHexagon;
    [SerializeField]
    float mag,  mag2;
    void Start()
    {
        Row = System.Convert.ToInt32
            (transform.parent.name);
        Column = System.Convert.ToInt32
            (name);
        mag = (MapControlStatic.mapNav[3, 17].transform.position - transform.position).magnitude;
        mag2 = (MapControlStatic.mapNav[5, 11].transform.position - transform.position).magnitude;
    }

    void Update()
    {

    }
    //private void OnMouseDown()
    //{
    //    if (TypeHexagon != 1)
    //    {
    //        MapControlStatic.SearchForAWay(Row, Column);
    //    }
    //}
    public bool FreedomTest()
    {
        if (TypeHexagon==1||TypeHexagon==2)
        {
            return false;
        }
        else /*if (true)*/
        {
            return true;
        }
    }
    public bool FreedomTest(bool map)
    {
        if (TypeHexagon==1||TypeHexagon==2)
        {
            return false;
        }
        else /*if (true)*/
        {
            return true;
        }
    }
    public void Flag()
    {
        Instantiate(_flag,transform);
    }
}
