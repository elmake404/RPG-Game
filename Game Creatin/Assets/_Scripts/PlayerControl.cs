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
                    _heroControl = Collider.gameObject.GetComponent<HeroControl>();
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
                        if (_heroControl.gameObject.layer == 8)
                        {
                            _heroControl.StartWay(Hexagon);
                        }
                        else
                        {
                            _heroControl.StartWayElevation(Hexagon);
                        }
                    }
                    else if (Collider.tag == "Enemy")
                    {
                        var Enemy = Collider.GetComponent<EnemyControl>();

                        if (_heroControl.gameObject.layer == 8)
                        {
                            _heroControl.StartWayEnemy(Enemy.HexagonMain, Enemy);
                        }
                        else
                        {
                            _heroControl.StartWayElevation(Enemy.HexagonMain);
                        }
                    }
                }
            }
            if (Collider != null)
            {
                var Hexagon = Collider.GetComponent<HexagonControl>();

                if ((Collider.gameObject.layer == 9 || Collider.gameObject.layer == 10) && Hexagon.TypeHexagon != 1)
                {
                    List<HexagonControl> Way = MapControl.MapNav[0, 0].GetWay(Hexagon);
                    for (int i = 0; i < Way.Count; i++)
                    {
                        Way[i].Flag();
                    }
                }

            }
            _heroControl = null;
        }

    }

}
