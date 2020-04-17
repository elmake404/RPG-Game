using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroControl : MonoBehaviour
{
    [SerializeField]
    private Navigation NavigationHero;

    public Animator Animator;
    [HideInInspector]
    public Dictionary<int, AnApproacData> AnApproac = new Dictionary<int, AnApproacData>();

    private void Awake()
    {
        for (int i = 0; i < 6; i++)
        {
            AnApproac[i] = new AnApproacData();
        }
    }
    private void Start()
    {
        RecordApproac();
        //Invoke("RecordApproac", 0.5f);
    }

    private void Update()
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
        //    else
        //    {
        //        Debug.Log(i);
        //    }
        //}
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
                AnApproac[i].enemy = enemyControl;
                break;
            }
        }
        return hexagon;
    }
    public void StartWay(HexagonControl hexagonFinish)
    {
        NavigationHero.StartWay(hexagonFinish);
    }
    public void StartWayElevation(HexagonControl hexagonFinish)
    {
        NavigationHero.StartWayElevation(hexagonFinish);
    }
    public void InitializationVertexNavigation(HexagonControl[] _arrey)
    {
        NavigationHero.Initialization(_arrey, this);
    }
}
