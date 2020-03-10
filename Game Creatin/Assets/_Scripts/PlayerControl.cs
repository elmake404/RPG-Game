using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Camera _camera;
    private RaycastHit2D _hitHero;
    private HeroControl heroControl;
    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray2D ray = _camera.(Input.mousePosition);
        //    Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

        //    RaycastHit2D hit2D = Physics2D.Raycast(_camera.transform.position, _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -100))/*, out hit2D*/);
        //    //Debug.DrawRay(, Color.yellow);
        //    Debug.Log(hit2D.collider.gameObject.name+" "+ hit2D.collider.transform.parent.name);
        //}

    }
    
}
