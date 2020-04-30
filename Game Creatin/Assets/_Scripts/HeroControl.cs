using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroControl : MonoBehaviour
{
    [SerializeField]
    private Navigation _navigationHero;

    private int _maxCountEnemy;
    public bool d;

    public Animator Animator;
    [HideInInspector]
    public Dictionary<int, AnApproacData> AnApproac = new Dictionary<int, AnApproacData>();
    [HideInInspector]
    public List<EnemyControl> Pursuer = new List<EnemyControl>();
    [HideInInspector]
    public HexagonControl HexagonMain;

    private void Start()
    {
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    for (int i = 0; i < AnApproac.Count; i++)
        //    {
        //        if (AnApproac[i].hexagon!=null==HexagonMain)
        //        {
        //            AnApproac[i].hexagon.Flag();
        //            AnApproac[0].hexagon.Flag();
        //        }
        //    }
        //}
    }
    private void RecordApproac()
    {
        bool elevation = gameObject.layer != 8;
        HexagonControl Hex = HexagonMain;
        HexagonControl hexagon = Hex.Floor != null ? Hex.Floor : Hex;
        //hexagon.Flag();
        for (int i = 0; i < AnApproac.Count; i++)
        {
            AnApproac[i].Ban();
        }

        if (hexagon.Row != 0)
        {
            HexagonControl hexagonCon = MapControlStatic.mapNav[hexagon.Row - 1, hexagon.Column].GetHexagonMain(elevation);
            AnApproac[0].hexagon = hexagonCon;
            if ((hexagonCon != null) && !hexagonCon.IsFree)
            {
                AnApproac[0].busy = true;
            }
            if ((hexagon.Row % 2) == 0)//1
            {
                if (hexagon.Column < MapControlStatic.mapNav.GetLength(1) - 1)//2
                {
                    HexagonControl hexagonControl = MapControlStatic.mapNav[hexagon.Row - 1, hexagon.Column + 1].GetHexagonMain(elevation);
                    AnApproac[1].hexagon = hexagonControl;
                    if ((hexagonControl!=null) &&!hexagonControl.IsFree)
                    {
                        AnApproac[1].busy = true;
                    }
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
                    HexagonControl hexagonControl = MapControlStatic.mapNav[hexagon.Row - 1, hexagon.Column - 1].GetHexagonMain(elevation);

                    AnApproac[1].hexagon = hexagonControl;
                    if ((hexagonControl != null) && !hexagonControl.IsFree)
                    {
                        AnApproac[1].busy = true;
                    }
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
            HexagonControl hexagonControl = MapControlStatic.mapNav[hexagon.Row, hexagon.Column - 1].GetHexagonMain(elevation);
            AnApproac[2].hexagon = hexagonControl;
            if ((hexagonControl != null) && !hexagonControl.IsFree)
            {
                AnApproac[2].busy = true;
            }
        }
        else
        {
            AnApproac[2].hexagon = null;
        }

        if (hexagon.Column < MapControlStatic.mapNav.GetLength(1) - 1)
        {
            HexagonControl hexagonControl = MapControlStatic.mapNav[hexagon.Row, hexagon.Column + 1].GetHexagonMain(elevation);
            AnApproac[3].hexagon = hexagonControl;
            if ((hexagonControl != null) && !hexagonControl.IsFree)
            {
                AnApproac[3].busy = true;
            }
        }
        else
        {
            AnApproac[3].hexagon = null;
        }

        if (hexagon.Row < MapControlStatic.mapNav.GetLength(0) - 1)
        {
            HexagonControl hexagonCon =MapControlStatic.mapNav[hexagon.Row + 1, hexagon.Column].GetHexagonMain(elevation);
            AnApproac[4].hexagon = hexagonCon;
            if ((hexagonCon != null) && !hexagonCon.IsFree)
            {
                AnApproac[4].busy = true;
            }

            if ((hexagon.Row % 2) == 0)
            {
                if (hexagon.Column < MapControlStatic.mapNav.GetLength(1) - 1)//2
                {
                    HexagonControl hexagonControl =MapControlStatic.mapNav[hexagon.Row + 1, hexagon.Column + 1].GetHexagonMain(elevation);
                    AnApproac[5].hexagon = hexagonControl;
                    if ((hexagonControl != null) && !hexagonControl.IsFree)
                    {
                        AnApproac[5].busy = true;
                    }
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
                    HexagonControl hexagonControl = MapControlStatic.mapNav[hexagon.Row + 1, hexagon.Column - 1].GetHexagonMain(elevation);
                    AnApproac[5].hexagon = hexagonControl;
                    if ((hexagonControl != null) && !hexagonControl.IsFree)
                    {
                        AnApproac[5].busy = true;
                    }
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
    }
    public List<HexagonControl> GetSurroundingHexes()
    {
        //RecordApproac();
        List<HexagonControl> SurroundingHexes = new List<HexagonControl>();
        for (int i = 0; i < AnApproac.Count; i++)
        {
            if (AnApproac[i].hexagon != null)
            {
                SurroundingHexes.Add(AnApproac[i].hexagon);
            }
        }
        return SurroundingHexes;
    }
    public void Collision(Vector2 NextPos, IEnumerator MoveCor)
    {
        HexagonControl hex = MapControlStatic.FieldPosition(gameObject.layer, NextPos);

        if (HexagonMain != hex)
        {
            if (!hex.IsFree)
            {
                _navigationHero.StopMove(MoveCor);
            }
            else
            {
                HexagonMain.Gap();
                HexagonMain = hex;
                HexagonMain.Contact(_navigationHero);
                RecordApproac();

                TravelMessage();
            }
        }
    }

    public void TravelMessage()
    {
        for (int i = 0; i < Pursuer.Count; i++)
        {
            Pursuer[i].ReachRheck();
        }
    }

    public bool IsFreePlace()
    {
        for (int i = 0; i < AnApproac.Count; i++)
        {
            if (AnApproac[i].Suitability())
            {
                return true;
            }
        }
        return false;
    }
    public HexagonControl FreePlace(EnemyControl enemyControl)
    {
        HexagonControl hexagon = null;
        for (int i = 0; i < AnApproac.Count; i++)
        {
            if (AnApproac[i].Suitability())
            {
                hexagon = AnApproac[i].hexagon;
                AnApproac[i].busy = true;
                break;
            }
        }

        return hexagon;
    }
    public HexagonControl RandomPlace(out int namber)
    {
         namber = Random.Range(0, AnApproac.Count);

        while (AnApproac[namber].hexagon == null)
        {
            namber = Random.Range(0, AnApproac.Count);
        }
        HexagonControl hexagon = AnApproac[namber].hexagon;
        AnApproac[namber].busy = true;
        return hexagon;
    }
    public void StartWay(HexagonControl hexagonFinish)
    {
        _navigationHero.StartWay(hexagonFinish);
    }
    public void StartWayElevation(HexagonControl hexagonFinish)
    {
        _navigationHero.StartWayElevation(hexagonFinish);
    }
    public void Initialization(HexagonControl[] _arrey)
    {
        _maxCountEnemy = 6;
        for (int i = 0; i < 6; i++)
        {
            AnApproac[i] = new AnApproacData();
        }
        HexagonMain = MapControlStatic.FieldPosition(gameObject.layer, transform.position);
        HexagonMain.Contact(_navigationHero);

        RecordApproac();
        _navigationHero.Initialization(_arrey, this);
    }
    public void AddNewEnemy(EnemyControl enemy)
    {
        if (Pursuer.IndexOf(enemy) == -1)
        {
            Pursuer.Add(enemy);
            _maxCountEnemy--;
        }
    }
    public void RemoveEnemy()
    {
        _maxCountEnemy++;
    }
    public int CountEnemy()
    {
        return _maxCountEnemy;
    }
}
