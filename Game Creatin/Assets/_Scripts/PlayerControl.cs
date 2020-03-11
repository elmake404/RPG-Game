using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Camera _camera;
    private RaycastHit2D _hitHero;
    private HeroControl _heroControl;
    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D Collider = Physics2D.OverlapPoint(_camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -100)));
            if (Collider!=null)
            {
                if (Collider.gameObject.layer==8)
                {
                    Debug.Log(1);
                }
            }
        }

    }
    
}
