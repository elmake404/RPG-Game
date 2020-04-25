using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroControl : MonoBehaviour
{
    [SerializeField]
    private Navigation _navigationHero;

    private int _maxCountEnemy;

    public Animator Animator;
    [HideInInspector]
    public Dictionary<int,AnApproacData> AnApproac = new Dictionary<int, AnApproacData>();
    [HideInInspector]
    public Dictionary<EnemyControl,bool> Pursuer = new Dictionary<EnemyControl, bool>();
    [HideInInspector]
    public HexagonControl HexagonMain;

    private void Start()
    {
        HexagonMain = MapControlStatic.FieldPosition(gameObject.layer, transform.position);
        HexagonMain.Contact(_navigationHero);
    }

    private void Update()
    {
    }
    private void RecordApproac()
    {
        bool elevation = gameObject.layer != 8;
        HexagonControl Hex = MapControlStatic.FieldPosition(gameObject.layer, transform.position);
        HexagonControl hexagon = Hex.Floor != null ? Hex.Floor : Hex;
        //hexagon.Flag();

        if (hexagon.Row != 0)
        {
            AnApproac[0].hexagon = MapControlStatic.mapNav[hexagon.Row - 1, hexagon.Column].GetHexagonMain(elevation);
            //MapControlStatic.mapNav[hexagon.Row - 1, hexagon.Column].Flag();
            if ((hexagon.Row % 2) == 0)//1
            {
                if (hexagon.Column < MapControlStatic.mapNav.GetLength(1) - 1)//2
                {
                    AnApproac[1].hexagon = MapControlStatic.mapNav[hexagon.Row - 1, hexagon.Column + 1].GetHexagonMain(elevation);
                    //MapControlStatic.mapNav[hexagon.Row - 1, hexagon.Column + 1].Flag();
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
                    //MapControlStatic.mapNav[hexagon.Row - 1, hexagon.Column - 1].Flag();
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
            //MapControlStatic.mapNav[hexagon.Row, hexagon.Column - 1].Flag();
        }
        else
        {
            AnApproac[2].hexagon = null;
        }

        if (hexagon.Column < MapControlStatic.mapNav.GetLength(1) - 1)
        {
            AnApproac[3].hexagon = MapControlStatic.mapNav[hexagon.Row, hexagon.Column + 1].GetHexagonMain(elevation);
            //MapControlStatic.mapNav[hexagon.Row, hexagon.Column + 1].Flag();
        }
        else
        {
            AnApproac[3].hexagon = null;
        }

        if (hexagon.Row < MapControlStatic.mapNav.GetLength(0) - 1)
        {
            //Debug.Log(hexagon.Row);
            AnApproac[4].hexagon = MapControlStatic.mapNav[hexagon.Row + 1, hexagon.Column].GetHexagonMain(elevation);
            //MapControlStatic.mapNav[hexagon.Row + 1, hexagon.Column].Flag();

            if ((hexagon.Row % 2) == 0)
            {
                if (hexagon.Column < MapControlStatic.mapNav.GetLength(1) - 1)//2
                {
                    AnApproac[5].hexagon = MapControlStatic.mapNav[hexagon.Row + 1, hexagon.Column + 1].GetHexagonMain(elevation);
                    //MapControlStatic.mapNav[hexagon.Row + 1, hexagon.Column + 1].Flag();
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
                    //MapControlStatic.mapNav[hexagon.Row + 1, hexagon.Column - 1].Flag();
                }
                else
                {
                    AnApproac[5].hexagon = null;
                }

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
                SurroundingHexes.Add(AnApproac[i].hexagon);
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
                TravelMessage();
            }
        }
    }

    public void TravelMessage()
    {
        RecordApproac();
        for (int i = 0; i < AnApproac.Count; i++)
        {
            for (int j = 0; j < AnApproac[i].enemy.Count; j++)
            {
                if(AnApproac[i].hexagon!=null)
                AnApproac[i].enemy[j].StartWay(AnApproac[i].hexagon);
            }
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
                AnApproac[i].enemy.Add(enemyControl);
                break;
            }
        }
        Pursuer[enemyControl] = true;

        return hexagon;
    }
    public HexagonControl RandomPlace(EnemyControl enemyControl)
    {
        int namber = Random.Range(0, AnApproac.Count);

        while (AnApproac[namber].hexagon==null)
        {
            namber = Random.Range(0, AnApproac.Count);
        }
        Pursuer[enemyControl] = false;
        HexagonControl hexagon = AnApproac[namber].hexagon;
        AnApproac[namber].busy = true;
        AnApproac[namber].enemy.Add(enemyControl);
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
    public void InitializationVertexNavigation(HexagonControl[] _arrey)
    {
        _maxCountEnemy = 6;
        for (int i = 0; i < 6; i++)
        {
            AnApproac[i] = new AnApproacData();
        }

        RecordApproac();
        _navigationHero.Initialization(_arrey, this);
    }
    public void AddNewEnemy()
    {
        _maxCountEnemy--;        
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
