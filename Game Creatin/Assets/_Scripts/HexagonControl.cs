using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonControl : MonoBehaviour
{
    [SerializeField]
    private GameObject _flag;

    [System.NonSerialized]
    public int Row, Column;

    [Range(0, 1)]
    public int TypeHexagon;
    void Start()
    {
        Row = System.Convert.ToInt32
            (transform.parent.name);
        Column = System.Convert.ToInt32
            (name);
    }

    void Update()
    {

    }
    private void OnMouseDown()
    {
        if (TypeHexagon!=1)
        {
            MapControlStatic.SearchForAWay(Row, Column);
        }
    }

    public void Flag()
    {
        Instantiate(_flag,transform.position,Quaternion.identity);
    }
}
