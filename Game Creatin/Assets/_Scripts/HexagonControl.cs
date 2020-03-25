using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonControl : MonoBehaviour
{
    [SerializeField]
    private GameObject _flag;

    [System.NonSerialized]
    public int Row, Column;

    [Range(0, 3)]
    public int TypeHexagon;
    //[SerializeField]
    //float mag,  mag2;
    void Start()
    {
        if (TypeHexagon != 2)
        {
            Row = System.Convert.ToInt32
                (transform.parent.name);
            Column = System.Convert.ToInt32
                (name);
        }
    }

    void Update()
    {

    }
    public bool FreedomTestType( bool Elevtion)
    {
        if (!Elevtion)
        {
            if (TypeHexagon == 1)
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
    public bool FreedomTestLayer()
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
    public void Flag()
    {
        Instantiate(_flag, transform);
    }
}
