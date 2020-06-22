using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Camera _camera;
    private HeroControl _heroControl;
    private void Awake()
    {
        StaticLevelManager.IsGameFlove = true;
    }
    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D Collider = Physics2D.OverlapPoint(_camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -100)));
            if (Collider != null)
            {
                if (Collider.gameObject.tag == "Hero")
                {
                    _heroControl = Collider.gameObject.GetComponentInParent<HeroControl>();
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Collider2D Collider = Physics2D.OverlapPoint(_camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -100)));

            if (_heroControl != null)
            {
                if (Collider != null)
                {
                    var Hexagon = Collider.GetComponent<HexagonControl>();

                    if ((Collider.gameObject.layer == 9 || Collider.gameObject.layer == 10) && Hexagon.TypeHexagon != 1)
                    {
                        _heroControl.DisConectEnemy();
                        _heroControl.StartWay(Hexagon);

                    }
                    else if (Collider.tag == "Enemy")
                    {
                        var Enemy = Collider.GetComponent<EnemyControl>();

                        _heroControl.StartWayEnemy( Enemy);
                    }
                }
            }
            _heroControl = null;
        }

    }

}
