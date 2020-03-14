using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Camera _camera;
    private HeroControl _heroControl;
    void Start()
    {
        _camera = Camera.main;
        MapControlStatic.ListPoint.Add(MapControlStatic.mapNav[8, 11]);
    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Collider2D Collider = Physics2D.OverlapPoint(_camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -100)));
        //    if (Collider != null)
        //    {
        //        if (Collider.gameObject.layer == 8)
        //        {
        //            _heroControl = Collider.gameObject.GetComponent<HeroControl>();
        //            _heroControl.ListPoint.Add(MapControlStatic.mapNav[_heroControl.HexagonRow, _heroControl.HexagonColumn]);
        //        }
        //    }
        //}
        //else if (Input.GetMouseButtonUp(0))
        //{
        //    Collider2D Collider = Physics2D.OverlapPoint(_camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -100)));

        //    if (_heroControl != null)
        //    {
        //        if (Collider != null)
        //        {
        //            var Hexagon = Collider.GetComponent<HexagonControl>();

        //            if (Collider.gameObject.layer == 9 && Hexagon.TypeHexagon != 1)
        //            {
        //                _heroControl.SearchForAWay(Hexagon.Row, Hexagon.Column);
        //            }
        //            else
        //            {
        //                _heroControl.ListPoint.Clear();
        //            }
        //        }
        //        else
        //        {
        //            _heroControl.ListPoint.Clear();
        //        }
        //    }
        //}

    }

}
