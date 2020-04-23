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

    [HideInInspector]
    public Dictionary<int, AnApproacData> AnApproac = new Dictionary<int, AnApproacData>();

    [HideInInspector]
    public HexagonControl HexagonMain;

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
    }
    private void RecordApproac()
    {
        bool elevation = gameObject.layer != 8;
        HexagonControl hexagon = MapControlStatic.FieldPosition(gameObject.layer, transform.position);

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
        //for (int i = 0; i < AnApproac.Count; i++)
        //{
        //    if (AnApproac[i].hexagon != null)
        //    {
        //        AnApproac[i].hexagon.Flag();

        //    }
        //}
    }
    public void Collision(Vector2 NextPos, IEnumerator MoveCor)
    {
        HexagonControl hex = MapControlStatic.FieldPosition(gameObject.layer, NextPos);

        if (HexagonMain != hex)
        {
            if (!hex.IsFree)
            {
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
