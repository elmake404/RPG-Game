using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    private EnemyManager _enemyManager;
    private HexagonControl _hexagonTarget;
    [SerializeField]
    private NavigationBot _navigationBot;

    private bool _isCatch = false;// true когда герой рядом 
    private float _timeBetweenAttacks;//выведи потом константу
    [SerializeField]
    private float _healthPoints, _attackPower;

    [HideInInspector]
    public Dictionary<int, AnApproacData> AnApproac = new Dictionary<int, AnApproacData>();

    //[HideInInspector]
    public HexagonControl HexagonMain;
    [HideInInspector]
    public HeroControl HeroTarget;
    [HideInInspector]
    public List<HeroControl> Pursuer = new List<HeroControl>();

    [HideInInspector]
    public bool IsAttack;
    public IMove ImoveMain;

    private void Awake()
    {
        ImoveMain = _navigationBot;
        for (int i = 0; i < 6; i++)
        {
            AnApproac[i] = new AnApproacData();
        }
    }
    private void Start()
    {
        //First();

    }

    void Update()
    {
        if (_healthPoints <= 0)
        {
            if (HeroTarget != null)
            {
                HeroTarget.RemoveEnemy(this);
            }
            HexagonMain.Gap();
            Destroy(gameObject);
        }

        RecordApproac();//надо с этим чтото делать 

        // атака
        //if (HeroTarget != null)
        //{
        //    if (HeroPresence())
        //    {
        //        if (_timeBetweenAttacks <= 0)
        //            if (!IsAttack && System.Math.Round(((Vector2)HeroTarget.transform.position - (Vector2)transform.position).magnitude, 2) == 3.46)
        //            {
        //                StartCoroutine(Attack());
        //            }
        //    }
        //}
        //else
        //{
        //    _enemyManager.GoalSelection(this);
        //}
    }
    private void FixedUpdate()
    {
        if (_timeBetweenAttacks > 0)
        {
            _timeBetweenAttacks -= Time.deltaTime;
        }
    }
    private IEnumerator Attack()
    {
        IsAttack = true;
        //_navigationBot.ResetPath();
        //Debug.Log(1111111111);
        StartCoroutine(_navigationBot.StopSpeed(0.6f));
        yield return new WaitForSeconds(0.5f);
        HeroTarget.Damage(_attackPower);
        _timeBetweenAttacks = 0.2f;
        IsAttack = false;
        //_navigationBot.Continue();
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
            HexagonControl hexagonCon = MapControlStatic.mapNav[hexagon.Row + 1, hexagon.Column].GetHexagonMain(elevation);
            AnApproac[4].hexagon = hexagonCon;
            if ((hexagonCon != null) && !hexagonCon.IsFree)
            {
                AnApproac[4].busy = true;
            }

            if ((hexagon.Row % 2) == 0)
            {
                if (hexagon.Column < MapControlStatic.mapNav.GetLength(1) - 1)//2
                {
                    HexagonControl hexagonControl = MapControlStatic.mapNav[hexagon.Row + 1, hexagon.Column + 1].GetHexagonMain(elevation);
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
    public bool HeroPresence()
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
    private void TravelMessage()
    {
        for (int i = 0; i < Pursuer.Count; i++)
        {
            Pursuer[i].ReachRheck();
        }
    }
    private void DisConectHero()
    {
        _isCatch = false;
        HeroTarget.RemoveEnemy(this);
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
    public bool Collision(Vector2 NextPos, IEnumerator MoveCor)
    {
        HexagonControl hex = MapControlStatic.FieldPosition(gameObject.layer, NextPos);

        if (HexagonMain != hex)
        {
            if (!hex.IsFree)
            {
                if (hex.ObjAbove.GetHero() != null)
                {
                    if (hex.ObjAbove.GetHero() == HeroTarget)
                    {
                        HeroConnect(hex.ObjAbove.GetHero());
                    }
                    else
                    {
                        DisConectHero();
                        HeroConnect(hex.ObjAbove.GetHero());
                    }
                }
                else
                    _navigationBot.StopMove(MoveCor);
                return false;
            }
            else
            {
                if (hex.TypeHexagon == 1)
                {
                    Debug.LogError("da");
                }
                HexagonMain.Gap();
                HexagonMain = hex;
                HexagonMain.Contact(_navigationBot);
                RecordApproac();

                TravelMessage();
                return true;
            }
        }
        return true;
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
        //RecordApproac();
        List<HexagonControl> SurroundingHexes = new List<HexagonControl>();
        for (int i = 0; i < AnApproac.Count; i++)
        {
            if (AnApproac[i].hexagon != null)
                SurroundingHexes.Add(AnApproac[i].hexagon.GetHexagonMain());
        }
        return SurroundingHexes;
    }
    public void First(EnemyManager enemyManager)
    {
        _enemyManager = enemyManager;
        HexagonMain = MapControlStatic.FieldPosition(gameObject.layer, transform.position);
        HexagonMain.Contact(_navigationBot);
        RecordApproac();
    }
    public void InitializationHeroTarget(HeroControl hero)
    {
        HeroTarget = hero;
    }
    public void StartWay(HexagonControl hexagonFinish)
    {
        _navigationBot.StartWay(hexagonFinish);
    }
    public void AddNewHero(HeroControl hero)
    {
        if (Pursuer.IndexOf(hero) == -1)
        {
            Pursuer.Add(hero);
        }
    }
    public void Damage(float AttackPower)
    {
        _healthPoints -= AttackPower;
    }

    public void RemoveHero(HeroControl hero)
    {
        Pursuer.Remove(hero);
    }

}
