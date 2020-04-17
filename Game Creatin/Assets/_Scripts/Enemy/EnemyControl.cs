using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    private List<HeroControl> _listHeroControls;
    private HeroControl _heroTarget;
    private HexagonControl _hexagonTarget;
    [SerializeField]
    private NavigationBot _navigationBot;

    void Start()
    {
        //NavigationBot.StartWay(MapControlStatic.mapNav[0,0]);
        //Navigation.FieldPosition().Flag();
    }

    void Update()
    {
    }
    public void StartWay(HexagonControl hexagonFinish)
    {
        _navigationBot.StartWay(hexagonFinish);
    }
    public void InitializationHero(HeroControl[] heroes)
    {
        _listHeroControls = new List<HeroControl>();
        _listHeroControls.AddRange(heroes);
    }
}
