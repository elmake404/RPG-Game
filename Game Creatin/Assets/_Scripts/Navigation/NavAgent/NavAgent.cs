using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NavAgent : MonoBehaviour, IMove
{
    private List<HexagonControl> _wayList = new List<HexagonControl>();
    private HexagonControl _targetHexagon;
    private Vector2 _currentPos;

    [SerializeField]
    private bool _isMove, _isClever;
    [SerializeField]
    private float _speed;
    private float _speedMove;

    public IControl Control;
    private void Awake()
    {
        _speedMove = _speed;
    }
    private void FixedUpdate()
    {
        if (_wayList.Count <= 0)
        {
            _isMove = false;
        }

        if (_isMove)
        {
            _targetHexagon = _wayList[0];

            if (_targetHexagon.TypeHexagon == 2 && gameObject.layer == 8)
            {
                gameObject.layer = 11;
            }
            else if (_targetHexagon.TypeHexagon == 0 && gameObject.layer == 11)
            {
                gameObject.layer = 8;
            }

            transform.position = Vector2.MoveTowards(transform.position, _targetHexagon.transform.position, _speedMove);

            _currentPos = (Vector2)transform.position + (Vector2)(_targetHexagon.transform.position - transform.position).normalized * 1.8f;

            Control.Collision(_currentPos);

            if (_wayList.Count > 0)
            {
                if (((Vector2)transform.position - (Vector2)_wayList[0].transform.position).magnitude <= 0.01f)
                {
                    _wayList.Remove(_wayList[0]);
                }
            }
        }
        else
        {
            if (((Vector2)transform.position - Control.HexagonMain().position).magnitude >= 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position, Control.HexagonMain().position, _speed);
            }

            if (_wayList.Count > 0)
            {
                HexagonControl hexagonNext = MapControl.FieldPosition(gameObject.layer, _currentPos);
                IMove move = hexagonNext.ObjAbove;
                if ((move == null))
                {
                    if (hexagonNext.IsFree)
                    {
                        _isMove = true;
                    }
                }
                else if (!move.IsGo())
                {
                    IMove enemy = Control.Target() != null ? Control.Target() : null;
                    WayBypass(_wayList[_wayList.Count - 1], enemy, hexagonNext.ObjAbove);
                }
            }
        }
    }
    private IEnumerator StopSpeed(float time)
    {
        _speedMove = 0;
        yield return new WaitForSeconds(time);
        _speedMove = _speed;
    }
    private void Way(HexagonControl hexagonFinish, IMove EnemyTarget)
    {
        List<HexagonControl> Way = Control.HexagonMain().GetWay(hexagonFinish);
        if (Way != null)
        {
            if (_isClever)
            {
                Way = NavStatic.PathCheck(Way, EnemyTarget);
            }

            _wayList.AddRange(Way);
            _isMove = true;
        }

    }
    private void WayBypass(HexagonControl hexagonFinish, IMove EnemyTarget,IMove Collision)
    {
        List<HexagonControl> Way = Control.HexagonMain().GetWay(hexagonFinish);
        if (Way != null)
        {
            Way = NavStatic.PathCheckBypass(Way,Collision, EnemyTarget);
            if (Way.Count > 0)
            {
                _wayList.Clear();

                _wayList.AddRange(Way);
                _isMove = true;
            }
        }

    }
    public async void StartWay(HexagonControl hexagonFinish, IMove EnemyTarget)
    {
        _wayList.Clear();

        await Task.Run(() => Way(hexagonFinish, EnemyTarget));
    }
    public void StopMove()
    {
        _isMove = false;
    }
    public void StopMoveTarget()
    {
        _isMove = false;
        _wayList.Clear();
    }
    public void StopSpeedAtack(float timeStop)
    {
        StopMoveTarget();
        StartCoroutine(StopSpeed(timeStop));
    }

    #region interface 
    public bool IsGo()
    {
        return _isMove;
    }

    public EnemyControl GetEnemy()
    {
        return null;
    }

    public List<HexagonControl> GetSurroundingHexes()
    {
        return Control.GetSurroundingHexes();
    }
    #endregion
}
