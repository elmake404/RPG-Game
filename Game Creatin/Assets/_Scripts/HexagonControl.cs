using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonControl : MonoBehaviour
{
    [SerializeField]
    private GameObject _flag;

    public HexagonControl Elevstion, Floor;
    [System.NonSerialized]
    public int Row, Column;

    [Range(0, 3)]
    public int TypeHexagon;
    //[SerializeField]
    //float mag,  mag2;
    private void Awake()
    {
        if (Elevstion != null)
        {
            Elevstion.Floor = this;
        }
    }
    void Start()
    {
        if (TypeHexagon != 2)
        {
            Row = System.Convert.ToInt32
                (transform.parent.name);
            Column = System.Convert.ToInt32
                (name);
        }
        //if (Floor != null)
        //{
        //    Floor.Flag();
        //}
    }

    void Update()
    {

    }
    public List<HexagonControl> SurroundingPeak()//вершины вогрук героя 
    {
        List<HexagonControl> hexagonControls = new List<HexagonControl>();
        bool elevation = gameObject.layer != 9;
        List<RaycastHit2D> hit2Ds = new List<RaycastHit2D>();
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        Physics2D.CircleCast(transform.position, 2f, transform.position - transform.position, contactFilter2D, hit2Ds);

        for (int i = 0; i < hit2Ds.Count; i++)
        {
            if (hit2Ds[i].collider.gameObject == gameObject)
            {
                continue;
            }
            var getHex = hit2Ds[i].collider.GetComponent<HexagonControl>();
            if (getHex.FreedomTestType(elevation))
            {
                hexagonControls.Add(getHex);
            }
        }
        return hexagonControls;
    }

    public HexagonControl FieldPosition(Transform customer)//гексагон к которому надо идти 
    {
        bool elevation = gameObject.layer != 9;
        List<RaycastHit2D> hit2Ds = new List<RaycastHit2D>();
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        Physics2D.CircleCast(transform.position, 2f, transform.position - transform.position, contactFilter2D, hit2Ds);
        HexagonControl hexagonControl = null;//нужный 6-ти угольник  
        float Magnitude = 0;

        for (int i = 0; i < hit2Ds.Count; i++)
        {
            var getHex = hit2Ds[i].collider.GetComponent<HexagonControl>();
            if (getHex.FreedomTestType(elevation))
            {
                if (getHex == this)
                {
                    continue;
                }
                if (hexagonControl == null)
                {
                    Magnitude = (new Vector2(hit2Ds[i].transform.position.x, hit2Ds[i].transform.position.y) - new Vector2(customer.position.x, customer.position.y)).magnitude;
                    hexagonControl = getHex;
                }

                if (Magnitude > (new Vector2(hit2Ds[i].transform.position.x, hit2Ds[i].transform.position.y) - new Vector2(customer.position.x, customer.position.y)).magnitude)
                {
                    Magnitude = (new Vector2(hit2Ds[i].transform.position.x, hit2Ds[i].transform.position.y) - new Vector2(customer.position.x, customer.position.y)).magnitude;
                    hexagonControl = getHex;
                }
            }
        }
        return hexagonControl;
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
}
