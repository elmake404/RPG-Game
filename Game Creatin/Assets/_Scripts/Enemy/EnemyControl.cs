using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    private HexagonControl _hexagonTarget;
    [SerializeField]
    private NavigationBot _navigationBot;

    private bool _isCatch = false;// true когда герой рядом 

    [HideInInspector]
    public Dictionary<int, AnApproacData> AnApproac = new Dictionary<int, AnApproacData>();

    [HideInInspector]
    public HexagonControl HexagonMain;
    [HideInInspector]
    public HeroControl HeroTarget;

    private void Awake()
    {
        for (int i = 0; i < 6; i++)
        {
            AnApproac[i] = new AnApproacData();
        }
    }
    private void Start()
    {

    }

    void Update()
    {
        //    if (Input.GetKeyDown(KeyCode.Space))
        //    {
        //        List<HexagonControl> g = GetSurroundingHexes();
        //        for (int i = 0; i < g.Count; i++)
        //        {
        //            if (g[i].ObjAbove!=null )
        //            {
        //                if (g[i].ObjAbove.GetHero()==HeroTarget)
        //                {
        //                    Debug.Log("eeee pok");
        //                }
        //            }
        //        }
        //    }
    }
    private void RecordApproac()
    {
        bool elevation = gameObject.layer != 8;
        HexagonControl Hex = HexagonMain;
        HexagonControl hexagon = Hex.Floor != null ? Hex.Floor : Hex;

        if (hexagon.Row != 0)
        {
            AnApproac[0].hexagon = MapControlStatic.mapNav[hexagon.Row - 1, hexagon.Column].GetHexagonMain(elevation);

            if ((hexagon.Row % 2) == 0)//1
            {
                if (hexagon.Column < MapControlStatic.mapNav.GetLength(1) - 1)//2
                {
                    AnApproac[1].hexagon = MapControlStatic.mapNav[hexagon.Row - 1, hexagon.Column + 1].GetHexagonMain(elevation);
                }
                else
                {
                    AnApproac[1].hexagon = null;
                }
            }
            else
            {
                if (hexagon.Column > 0)//2
                {
                    AnApproac[1].hexagon = MapControlStatic.mapNav[hexagon.Row - 1, hexagon.Column - 1].GetHexagonMain(elevation);
                }
                else
                {
                    AnApproac[1].hexagon = null;
                }

            }
        }
        else
        {
            AnApproac[0].hexagon = null;
            AnApproac[1].hexagon = null;
        }

        if (hexagon.Column > 0)
        {
            AnApproac[2].hexagon = MapControlStatic.mapNav[hexagon.Row, hexagon.Column - 1].GetHexagonMain(elevation);
        }
        else
        {
            AnApproac[2].hexagon = null;
        }

        if (hexagon.Column < MapControlStatic.mapNav.GetLength(1) - 1)
        {
            AnApproac[3].hexagon = MapControlStatic.mapNav[hexagon.Row, hexagon.Column + 1].GetHexagonMain(elevation);
        }
        else
        {
            AnApproac[3].hexagon = null;
        }

        if (hexagon.Row < MapControlStatic.mapNav.GetLength(0) - 1)
        {
            //Debug.Log(hexagon.Row);
            AnApproac[4].hexagon = MapControlStatic.mapNav[hexagon.Row + 1, hexagon.Column].GetHexagonMain(elevation);

            if ((hexagon.Row % 2) == 0)
            {
                if (hexagon.Column < MapControlStatic.mapNav.GetLength(1) - 1)//2
                {
                    AnApproac[5].hexagon = MapControlStatic.mapNav[hexagon.Row + 1, hexagon.Column + 1].GetHexagonMain(elevation);
                }
                else
                {
                    AnApproac[5].hexagon = null;
                }
            }
            else
            {
                if (hexagon.Column > 0)//2
                {
                    AnApproac[5].hexagon = MapControlStatic.mapNav[hexagon.Row + 1, hexagon.Column - 1].GetHexagonMain(elevation);
                }
                else
                {
                    AnApproac[5].hexagon = null;
                }
            }
        }
        else
        {
            AnApproac[4].hexagon = null;
            AnApproac[5].hexagon = null;
        }

        //for (int i = 0; i < AnApproac.Count; i++)
        //{
        //    if (AnApproac[i].hexagon != null)
        //    {
        //        AnApproac[i].hexagon.Flag();

        //    }
        //}
    }
    private bool HeroPresence()
    {
        List<HexagonControl> hexagons = GetSurroundingHexes();
        for (int i = 0; i < hexagons.Count; i++)
        {
            if (hexagons[i].ObjAbove != null)
            {
                if (hexagons[i].ObjAbove.GetHero() == HeroTarget)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void HeroConnect(HeroControl hero)
    {
        _isCatch = true;
        hero.AddNewEnemy(this);
        _navigationBot.StartWayHero(hero);
        HeroTarget = hero;

    }
   public void HeroConnect()
    {
        _isCatch = true;
        HeroTarget.AddNewEnemy(this);
    }
    public void Collision(Vector2 NextPos, IEnumerator MoveCor)
    {
        HexagonControl hex = MapControlStatic.FieldPosition(gameObject.layer, NextPos);

        if (HexagonMain != hex)
        {
            if (!hex.IsFree)
            {
                if (hex.ObjAbove.GetHero()!=null)
                {
                    HeroConnect(hex.ObjAbove.GetHero());
                }
                else
                _navigationBot.StopMove(MoveCor);
            }
            else
            {
                HexagonMain.Gap();
                HexagonMain = hex;
                HexagonMain.Contact(_navigationBot);
            }
        }
    }
    public void ReachRheck()
    {
        if (_isCatch)
        {
            if (!HeroPresence())
            {
                _navigationBot.StartWayHero(HeroTarget);
            }
        }
        else
        {
            StartWay(HeroTarget.HexagonMain);
        }
    }
    
    public List<HexagonControl> GetSurroundingHexes()
    {
        RecordApproac();
        List<HexagonControl> SurroundingHexes = new List<HexagonControl>();
        for (int i = 0; i < AnApproac.Count; i++)
        {
            if (AnApproac[i].hexagon != null)
                SurroundingHexes.Add(AnApproac[i].hexagon.GetHexagonMain());
        }
        return SurroundingHexes;
    }
    public void First()
    {
        HexagonMain = MapControlStatic.FieldPosition(gameObject.layer, transform.position);
        HexagonMain.Contact(_navigationBot);
    }
    public void InitializationHeroTarget(HeroControl hero)
    {
        HeroTarget = hero;
    }
    public void StartWay(HexagonControl hexagonFinish)
    {
        _navigationBot.StartWay(hexagonFinish);
    }
}
