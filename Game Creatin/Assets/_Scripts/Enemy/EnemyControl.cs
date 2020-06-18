using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyControl : MonoBehaviour, IControl
{
    private EnemyManager _enemyManager;
    //[SerializeField]
    //private NavAgent _navigationBot;
    private HexagonControl _hexagonMain;

    [SerializeField]
    private float _healthPoints, _attackPower,_atackDistens;
    private float _atackDistensConst;

    [HideInInspector]
    public Dictionary<int, AnApproacData> AnApproac = new Dictionary<int, AnApproacData>();

    [HideInInspector]
    public HeroControl HeroTarget;
    [HideInInspector]
    public List<HeroControl> Pursuer = new List<HeroControl>();

    [HideInInspector]
    public bool IsAttack;
    public IMove IMoveMain;
    public IControl IControlMain;

    private void Awake()
    {
        _atackDistensConst = (1.73f * (_atackDistens * 2))+0.1f;
        IMoveMain = GetComponent<IMove>();
        IControlMain = this;

        for (int i = 0; i < 6; i++)
        {
            AnApproac[i] = new AnApproacData();
        }

    }
    private void Start()
    {
        //First();
        _hexagonMain = MapControl.FieldPosition(gameObject.layer, transform.position);
        _hexagonMain.Contact(IMoveMain);
        ////незабудь удалить
        //transform.position = (Vector2)_hexagonMain.transform.position;

        RecordApproac();

    }
    private void Update()
    {
        if (_healthPoints <= 0)
        {
            if (HeroTarget != null)
            {
                HeroTarget.RemoveEnemy(this);
            }
            _hexagonMain.Gap();
            Destroy(gameObject);
        }
    }
    private void FixedUpdate()
    {
        if (StaticLevelManager.IsGameFlove)
        {
            if (HeroTarget != null)
            {
                if (((Vector2)HeroTarget.transform.position - (Vector2)transform.position).magnitude <= _atackDistensConst)
                {
                    if (IMoveMain.IsGo())
                    {
                        IMoveMain.StopMoveTarget();
                    }

                    if (!IsAttack)
                    {
                        StartCoroutine(Atack());
                    }
                }

            }
            else
            {
                _enemyManager.GoalSelection(this);
            }
        }
    }
    private IEnumerator Atack()
    {
        IsAttack = true;
        HeroTarget.Damage(_attackPower);
        IMoveMain.StopSpeedAtack(0.5f);
        yield return new WaitForSeconds(0.5f);
        IsAttack = false;
    }
    private void RecordApproac()
    {
        bool elevation = gameObject.layer != 8;
        HexagonControl Hex = _hexagonMain;
        HexagonControl hexagon = Hex.Floor != null ? Hex.Floor : Hex;
        //hexagon.Flag();
        for (int i = 0; i < AnApproac.Count; i++)
        {
            AnApproac[i].Ban();
        }

        if (hexagon.Row != 0)
        {
            HexagonControl hexagonCon = MapControl.MapNav[hexagon.Row - 1, hexagon.Column].GetHexagonMain(elevation);
            AnApproac[0].hexagon = hexagonCon;
            if ((hexagonCon != null) && !hexagonCon.IsFree)
            {
                AnApproac[0].busy = true;
            }
            if ((hexagon.Row % 2) == 0)//1
            {
                if (hexagon.Column < MapControl.MapNav.GetLength(1) - 1)//2
                {
                    HexagonControl hexagonControl = MapControl.MapNav[hexagon.Row - 1, hexagon.Column + 1].GetHexagonMain(elevation);
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
                    HexagonControl hexagonControl = MapControl.MapNav[hexagon.Row - 1, hexagon.Column - 1].GetHexagonMain(elevation);

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
            HexagonControl hexagonControl = MapControl.MapNav[hexagon.Row, hexagon.Column - 1].GetHexagonMain(elevation);
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

        if (hexagon.Column < MapControl.MapNav.GetLength(1) - 1)
        {
            HexagonControl hexagonControl = MapControl.MapNav[hexagon.Row, hexagon.Column + 1].GetHexagonMain(elevation);
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

        if (hexagon.Row < MapControl.MapNav.GetLength(0) - 1)
        {
            HexagonControl hexagonCon = MapControl.MapNav[hexagon.Row + 1, hexagon.Column].GetHexagonMain(elevation);
            AnApproac[4].hexagon = hexagonCon;
            if ((hexagonCon != null) && !hexagonCon.IsFree)
            {
                AnApproac[4].busy = true;
            }

            if ((hexagon.Row % 2) == 0)
            {
                if (hexagon.Column < MapControl.MapNav.GetLength(1) - 1)//2
                {
                    HexagonControl hexagonControl = MapControl.MapNav[hexagon.Row + 1, hexagon.Column + 1].GetHexagonMain(elevation);
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
                    HexagonControl hexagonControl = MapControl.MapNav[hexagon.Row + 1, hexagon.Column - 1].GetHexagonMain(elevation);
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
    private void CollisionMain(Vector2 NextPos)
    {
        HexagonControl hex = MapControl.FieldPosition(gameObject.layer, NextPos);

        if (_hexagonMain != hex)
        {
            if (!hex.IsFree)
            {
                if ((HeroTarget != null) && hex.ObjAbove == HeroTarget.IMoveMain)
                {
                    IMoveMain.StopMoveTarget();
                }
                else
                    IMoveMain.StopMove();
            }
            else
            {
                if (hex.TypeHexagon == 1)
                {
                    Debug.LogError("da");
                }

                _hexagonMain.Gap();
                _hexagonMain = hex;
                _hexagonMain.Contact(IMoveMain);
                RecordApproac();
                TravelMessage();
            }
        }
    }
    private void TravelMessage()
    {
        for (int i = 0; i < Pursuer.Count; i++)
        {
            Pursuer[i].StartWayEnemy(this);
        }
    }
    public void First(EnemyManager manager)
    {
        //_navigationBot.Control = this;
        _enemyManager = manager;
        _hexagonMain = MapControl.FieldPosition(gameObject.layer, transform.position);
        _hexagonMain.Contact(IMoveMain);
        RecordApproac();
    }
    public void StartWay(HeroControl hero)
    {
        IMoveMain.StartWay(hero.IControlMain.HexagonMain(), hero.IMoveMain);
    }
    public void AddNewHero(HeroControl hero)
    {
        if (Pursuer.IndexOf(hero) == -1)
        {
            Pursuer.Add(hero);
        }
    }
    public void RemoveHero(HeroControl hero)
    {
        Pursuer.Remove(hero);
    }

    #region Atack
    public void Damage(float atack)
    {
        _healthPoints -= atack;
    }
    #endregion

    #region Interface
    public void Collision(Vector2 next)
    {
        CollisionMain(next);
    }

    public HexagonControl HexagonMain()
    {
        return _hexagonMain;
    }

    public IMove Target()
    {
        return HeroTarget.IMoveMain;
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

    #endregion
}
