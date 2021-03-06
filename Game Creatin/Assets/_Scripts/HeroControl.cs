﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroControl : MonoBehaviour, IControl
{
    private EnemyManager _enemyManager;
    private HexagonControl _hexagonMain;

    [SerializeField]
    private int _maxCountEnemy;
    [SerializeField]
    private float _healthPoints, _attackPower, _atackDistens;
    private float _atackDistensConst;

    public IMove IMoveMain;
    public IControl IControlMain;

    [HideInInspector]
    public EnemyControl EnemyTarget;
    [HideInInspector]
    public bool IsAttack;
    public Animator Animator;
    [HideInInspector]
    public List<HexagonControl> AnApproac = new List<HexagonControl>();
    [HideInInspector]
    public List<EnemyControl> Pursuer = new List<EnemyControl>();

    private void Awake()
    {
        _atackDistensConst = (1.73f * (_atackDistens * 2)) + 0.1f;
        //_navigationHero.Control = this;
        IMoveMain = GetComponent<IMove>();
        IControlMain = this;
    }
    private void Start()
    {
        //временно 
        _hexagonMain = MapControl.FieldPosition(gameObject.layer, transform.position);
        _hexagonMain.Contact(IMoveMain);
        //незабудь удалить
        transform.position = (Vector2)_hexagonMain.transform.position;

        RecordApproac();
    }
    private void Update()
    {
        if (_healthPoints <= 0)
        {
            _enemyManager.RemoveHero(this);
            if (EnemyTarget != null)
            {
                EnemyTarget.RemoveHero(this);
            }
            _hexagonMain.Gap();
            Destroy(gameObject);
        }
    }
    private void FixedUpdate()
    {
        if (EnemyTarget != null)
        {
            if (((Vector2)EnemyTarget.transform.position - (Vector2)transform.position).magnitude <= _atackDistensConst)
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
    private IEnumerator Atack()
    {
        IsAttack = true;
        EnemyTarget.Damage(_attackPower);
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
    private void CollisionMain(Vector2 NextPos)
    {
        HexagonControl hex = MapControl.FieldPosition(gameObject.layer, NextPos);
        if (_hexagonMain != hex)
        {
            if (!hex.IsFree)
            {
                if ((EnemyTarget != null) && hex.ObjAbove == EnemyTarget.IMoveMain)
                {
                    IMoveMain.StopMoveTarget();
                }
                else
                    IMoveMain.StopMove(hex);
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
            Pursuer[i].StartWay(this);
        }
    }
    public void DisConectEnemy()
    {
        if (EnemyTarget != null)
        {
            EnemyTarget.RemoveHero(this);
            EnemyTarget = null;
        }
    }
    public void StartWayEnemy(EnemyControl enemy)
    {
        EnemyTarget = enemy;
        enemy.AddNewHero(this);
        IMoveMain.StartWay(enemy.IControlMain.HexagonMain(), enemy.IMoveMain);
    }
    public void StartWay(HexagonControl hexagonFinish)
    {
        IMoveMain.StartWay(hexagonFinish, null);
    }
    public void Initialization(EnemyManager enemyManager)
    {
        _enemyManager = enemyManager;
    }
    public void AddNewEnemy(EnemyControl enemy)
    {
        if (Pursuer.IndexOf(enemy) == -1)
        {
            Pursuer.Add(enemy);
            _maxCountEnemy--;
        }
    }
    public void RemoveEnemy(EnemyControl enemy)
    {
        Pursuer.Remove(enemy);

        _maxCountEnemy++;
    }
    public int CountEnemy()
    {
        return _maxCountEnemy;
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
        if (EnemyTarget == null)
        {
            return null;
        }
        else
            return EnemyTarget.IMoveMain;
    }
    public List<HexagonControl> GetSurroundingHexes()
    {
        //RecordApproac();
        return AnApproac;
    }
    #endregion
}
