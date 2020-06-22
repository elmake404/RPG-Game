using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyControl : MonoBehaviour, IControl
{
    private EnemyManager _enemyManager;
    private HexagonControl _hexagonMain;

    [SerializeField]
    private float _healthPoints, _attackPower, _atackDistens;
    private float _atackDistensConst;

    [HideInInspector]
    public List<HexagonControl> AnApproac = new List<HexagonControl>();
    [HideInInspector]
    public HeroControl HeroTarget;
    [HideInInspector]
    public List<HeroControl> Pursuer = new List<HeroControl>();

    //[HideInInspector]
    public bool IsAttack;
    public IMove IMoveMain;
    public IControl IControlMain;

    private void Awake()
    {
        _atackDistensConst = (1.73f * (_atackDistens * 2)) + 0.1f;
        IMoveMain = GetComponent<IMove>();
        IControlMain = this;
    }
    private void Start()
    {

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < AnApproac.Count; i++)
            {
                if (AnApproac[i] == null)
                {
                    Debug.Log(i + " " + name);
                    continue;
                }
                AnApproac[i].Flag();
            }
        }

        if (_healthPoints <= 0)
        {
            if (HeroTarget != null)
            {
                HeroTarget.RemoveEnemy(this);
            }
            _hexagonMain.Gap();
            _hexagonMain.GapFly();
            Destroy(gameObject);
        }
    }
    private void FixedUpdate()
    {
        if (StaticLevelManager.IsGameFlove)
        {
            if (HeroTarget != null)
            {
                if (IMoveMain.IsFlight())
                {
                    Vector2 differenceHero = Vector2.zero;
                    Vector2 differenceMain = Vector2.zero;

                    if (gameObject.layer == 8 && HeroTarget.gameObject.layer == 11)
                    {
                        differenceHero = new Vector2(MapControl.X, MapControl.Y);
                    }

                    if (gameObject.layer == 11 && HeroTarget.gameObject.layer == 8)
                    {
                        differenceMain = new Vector2(MapControl.X, MapControl.Y);
                    }

                    if ((((Vector2)HeroTarget.transform.position - differenceHero) - ((Vector2)transform.position - differenceMain)).magnitude <= _atackDistensConst)
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
            }
            else
            {
                _enemyManager.GoalSelection(this, name);
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
        AnApproac.Clear();

        //hexagon.Flag();
        if (IMoveMain.IsFlight())
        {
            if (hexagon.Row != 0)
            {
                HexagonControl hexagonCon = MapControl.MapNav[hexagon.Row - 1, hexagon.Column].GetHexagonMain();

                if (hexagonCon != null)
                    AnApproac.Add(hexagonCon);

                if ((hexagon.Row % 2) == 0)//1
                {
                    if (hexagon.Column < MapControl.MapNav.GetLength(1) - 1)//2
                    {
                        HexagonControl hexagonControl = MapControl.MapNav[hexagon.Row - 1, hexagon.Column + 1].GetHexagonMain();
                        if (hexagonControl != null)
                            AnApproac.Add(hexagonControl);
                    }
                }
                else
                {
                    if (hexagon.Column > 0)//2
                    {
                        HexagonControl hexagonControl = MapControl.MapNav[hexagon.Row - 1, hexagon.Column - 1].GetHexagonMain();
                        if (hexagonControl != null)
                            AnApproac.Add(hexagonControl);
                    }

                }
            }

            if (hexagon.Column > 0)
            {
                HexagonControl hexagonControl = MapControl.MapNav[hexagon.Row, hexagon.Column - 1].GetHexagonMain();
                if (hexagonControl != null)
                    AnApproac.Add(hexagonControl);
            }

            if (hexagon.Column < MapControl.MapNav.GetLength(1) - 1)
            {
                HexagonControl hexagonControl = MapControl.MapNav[hexagon.Row, hexagon.Column + 1].GetHexagonMain();
                if (hexagonControl != null)
                    AnApproac.Add(hexagonControl);
            }

            if (hexagon.Row < MapControl.MapNav.GetLength(0) - 1)
            {
                HexagonControl hexagonCon = MapControl.MapNav[hexagon.Row + 1, hexagon.Column].GetHexagonMain();
                if (hexagonCon != null)
                    AnApproac.Add(hexagonCon);

                if ((hexagon.Row % 2) == 0)
                {
                    if (hexagon.Column < MapControl.MapNav.GetLength(1) - 1)//2
                    {
                        HexagonControl hexagonControl = MapControl.MapNav[hexagon.Row + 1, hexagon.Column + 1].GetHexagonMain();
                        if (hexagonControl != null)
                            AnApproac.Add(hexagonControl);
                    }
                }
                else
                {
                    if (hexagon.Column > 0)//2
                    {
                        HexagonControl hexagonControl = MapControl.MapNav[hexagon.Row + 1, hexagon.Column - 1].GetHexagonMain();
                        if (hexagonControl != null)
                            AnApproac.Add(hexagonControl);

                    }

                }
            }


        }
        else
        {
            if (hexagon.Row != 0)
            {
                HexagonControl hexagonCon = MapControl.MapNav[hexagon.Row - 1, hexagon.Column].GetHexagonMain(elevation);

                if (hexagonCon != null)
                    AnApproac.Add(hexagonCon);

                if ((hexagon.Row % 2) == 0)//1
                {
                    if (hexagon.Column < MapControl.MapNav.GetLength(1) - 1)//2
                    {
                        HexagonControl hexagonControl = MapControl.MapNav[hexagon.Row - 1, hexagon.Column + 1].GetHexagonMain(elevation);
                        if (hexagonControl != null)
                            AnApproac.Add(hexagonControl);
                    }
                }
                else
                {
                    if (hexagon.Column > 0)//2
                    {
                        HexagonControl hexagonControl = MapControl.MapNav[hexagon.Row - 1, hexagon.Column - 1].GetHexagonMain(elevation);
                        if (hexagonControl != null)
                            AnApproac.Add(hexagonControl);
                    }

                }
            }

            if (hexagon.Column > 0)
            {
                HexagonControl hexagonControl = MapControl.MapNav[hexagon.Row, hexagon.Column - 1].GetHexagonMain(elevation);
                if (hexagonControl != null)
                    AnApproac.Add(hexagonControl);
            }

            if (hexagon.Column < MapControl.MapNav.GetLength(1) - 1)
            {
                HexagonControl hexagonControl = MapControl.MapNav[hexagon.Row, hexagon.Column + 1].GetHexagonMain(elevation);
                if (hexagonControl != null)
                    AnApproac.Add(hexagonControl);
            }

            if (hexagon.Row < MapControl.MapNav.GetLength(0) - 1)
            {
                HexagonControl hexagonCon = MapControl.MapNav[hexagon.Row + 1, hexagon.Column].GetHexagonMain(elevation);
                if (hexagonCon != null)
                    AnApproac.Add(hexagonCon);

                if ((hexagon.Row % 2) == 0)
                {
                    if (hexagon.Column < MapControl.MapNav.GetLength(1) - 1)//2
                    {
                        HexagonControl hexagonControl = MapControl.MapNav[hexagon.Row + 1, hexagon.Column + 1].GetHexagonMain(elevation);
                        if (hexagonControl != null)
                            AnApproac.Add(hexagonControl);
                    }
                }
                else
                {
                    if (hexagon.Column > 0)//2
                    {
                        HexagonControl hexagonControl = MapControl.MapNav[hexagon.Row + 1, hexagon.Column - 1].GetHexagonMain(elevation);
                        if (hexagonControl != null)
                            AnApproac.Add(hexagonControl);

                    }

                }
            }

        }
    }
    private void CollisionMain(Vector2 NextPos)
    {
        HexagonControl hex;
        if (IMoveMain.IsFlight())
        {
            hex = MapControl.FieldPositionFly(gameObject.layer, NextPos);
        }
        else
            hex = MapControl.FieldPosition(gameObject.layer, NextPos);

        if (_hexagonMain != hex)
        {
            if (!hex.GetFree(IMoveMain.IsFlight()))
            {
                if ((HeroTarget != null) && hex.ObjAbove == HeroTarget.IMoveMain)
                {
                    IMoveMain.StopMoveTarget();
                }
                else
                    IMoveMain.StopMove(hex);
            }
            else
            {
                if (IMoveMain.IsFlight())
                {
                    _hexagonMain.GapFly();
                    _hexagonMain = hex;
                    _hexagonMain.ContactFly(IMoveMain);
                }
                else
                {
                    _hexagonMain.Gap();
                    _hexagonMain = hex;
                    _hexagonMain.Contact(IMoveMain);
                }

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

        if (IMoveMain.IsFlight())
        {
            _hexagonMain = MapControl.FieldPositionFly(gameObject.layer, transform.position);
        }
        else
            _hexagonMain = MapControl.FieldPosition(gameObject.layer, transform.position);

        if (IMoveMain.IsFlight())
        {
            _hexagonMain.ContactFly(IMoveMain);
        }
        else
        {
            _hexagonMain.Contact(IMoveMain);
        }
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
    public void Damage(float atack)
    {
        _healthPoints -= atack;
    }

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
        return AnApproac;
    }

    #endregion
}
