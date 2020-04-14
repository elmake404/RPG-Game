using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    private List<HeroControl> _listHeroControls;
    private HeroControl _heroTarget;
    private HexagonControl _hexagonTarget;
    bool GG;

    public NavigationBot NavigationBot;

    void Start()
    {
        NavigationBot.StartWay(MapControlStatic.mapNav[0,0]);
        //Navigation.FieldPosition().Flag();
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    GG = !GG;
        //}
        ////if(GG)
        //if (_heroTarget==null)
        //{
        //    _heroTarget = SearchForTheClosestHero();
        //}
        //else if (_hexagonTarget != _heroTarget.NavigationHero.FieldPosition())
        //{
        //    _hexagonTarget = _heroTarget.NavigationHero.FieldPosition();
        //    if (gameObject.layer == 8)
        //    {
        //        Navigation.StartWay(_hexagonTarget.FieldPosition(transform));
        //    }
        //    else
        //    {
        //        Navigation.StartWayElevation(_hexagonTarget.FieldPosition(transform));
        //    }
        //    //_hexagonTarget.FieldPosition(transform).Flag();
        //    //_hexagonTarget.Flag();
        //}
    }

    private HeroControl SearchForTheClosestHero()    //непродуман случай где лист героев пуст
    {
        HeroControl hero = null;
        float Magnitude = float.PositiveInfinity;
        for (int i = 0; i < _listHeroControls.Count; i++)
        {
            Vector2 mainPos = transform.position;
            Vector2 targetPos = _listHeroControls[i].transform.position;

            if (hero==null)
            {
                hero = _listHeroControls[i];
                Magnitude = (targetPos - mainPos).magnitude;
            }
            if (Magnitude> (targetPos - mainPos).magnitude)
            {
                hero = _listHeroControls[i];
                Magnitude = (targetPos - mainPos).magnitude;
            }
        }
        return hero;
    }
    public void InitializationHero(HeroControl[] heroes)
    {
        _listHeroControls = new List<HeroControl>();
        _listHeroControls.AddRange(heroes);
    }
}
