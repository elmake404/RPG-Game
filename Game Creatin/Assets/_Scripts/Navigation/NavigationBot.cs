using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationBot : MonoBehaviour, IMove
{
    private IEnumerator MoveCorotine;
    private IEnumerator BypassCorotine;
    private List<HexagonControl> ListPoints = new List<HexagonControl>();//точки через который надо пройти 
    [SerializeField]
    private HexagonControl _targetHexagon, g;
    private AlgorithmDijkstra _algorithmDijkstra = new AlgorithmDijkstra();
    [SerializeField]
    private EnemyControl _enemyMain;
    [SerializeField]
    private Vector2 CurrentPos;
    [SerializeField]
    private bool _isStop = false, _isMove, _isBypass, _isToTheHero = false, _isMoveHex, m, b;

    [SerializeField]
    private float _speed;
    private float _speedConst;
    [SerializeField]
    private int _namberAnApproac;
    private void Awake()
    {
        _speedConst = _speed;
        MoveCorotine = Movement();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(MoveCorotine);
        }
    }

    void FixedUpdate()
    {
        if (_isStop)
        {
            BypassCheck();
        }
    }
    private IEnumerator Movement()//коротина движения
    {
        g = ListPoints[ListPoints.Count - 1];
        _isMove = true;
        List<HexagonControl> PointList = new List<HexagonControl>();
        PointList.AddRange(ListPoints);
        ListPoints.Clear();
        _targetHexagon = PointList[0].Elevation != null ? PointList[0].Elevation : PointList[0];
        while (PointList.Count > 0)
        {
            if (m)
            {
                Debug.Log(1);
            }

            if (_targetHexagon.TypeHexagon == 2 && gameObject.layer == 8)
            {
                gameObject.layer = 11;
            }
            else if (_targetHexagon.TypeHexagon == 0 && gameObject.layer == 11)
            {
                gameObject.layer = 8;
            }

            Vector2 positionCurrent = _targetHexagon.transform.position;
            Vector2 Pos = Vector2.MoveTowards(transform.position, positionCurrent, _speed);
            CurrentPos = Pos + (Vector2)(_targetHexagon.transform.position - transform.position).normalized * 2f;

            if (_enemyMain.Collision(CurrentPos, MoveCorotine))
            {
                transform.position = Vector2.MoveTowards(transform.position, positionCurrent, _speed);


                if ((positionCurrent - (Vector2)transform.position).magnitude <= 0f)
                {
                    PointList.Remove(PointList[0]);
                    if (PointList.Count > 0)
                    {
                        _targetHexagon = PointList[0].Elevation != null ? PointList[0].Elevation : PointList[0];
                    }
                }
            }

            yield return new WaitForSeconds(0.02f);
        }
        if (_isToTheHero)
        {
            transform.position = (Vector2)_targetHexagon.transform.position;
            _isToTheHero = false;
            _enemyMain.HeroTarget.AnApproac[_namberAnApproac].busy = true;
        }
        _isMove = false;
    }
    private IEnumerator MovementBypass(List<HexagonControl> hexagonControls)//коротина обхода
    {
        _isMove = true;
        List<HexagonControl> PointList = new List<HexagonControl>();
        PointList.AddRange(hexagonControls);
        _targetHexagon = PointList[0].Elevation != null ? PointList[0].Elevation : PointList[0];
        while (PointList.Count > 1)
        {
            if (b)
            {
                Debug.Log(2);
            }

            Vector2 positionCurrent = _targetHexagon.transform.position;
            CurrentPos = Vector2.MoveTowards(transform.position, positionCurrent, _speed);
            _enemyMain.Collision(CurrentPos, BypassCorotine);
            transform.position = CurrentPos;
            Vector2 positionMain = transform.position;
            if (_targetHexagon.TypeHexagon == 2 && gameObject.layer == 8)
            {
                gameObject.layer = 11;
            }
            else if (_targetHexagon.TypeHexagon == 0 && gameObject.layer == 11)
            {
                gameObject.layer = 8;
            }
            if ((positionCurrent - positionMain).magnitude <= 0.001f)
            {
                PointList.Remove(PointList[0]);
                if (PointList.Count > 0)
                {
                    _targetHexagon = PointList[0].Elevation != null ? PointList[0].Elevation : PointList[0];
                }
            }
            yield return new WaitForSeconds(0.02f);
        }
        _isMove = false;
        Debug.Log(12233333);
        ContinueMove(MoveCorotine);
    }
    public void ResetPath()
    {
        StopCoroutine(MoveCorotine);
        if (BypassCorotine != null)
        {
            StopCoroutine(BypassCorotine);
        }
        _isMove = false;
    }
    private HexagonControl GetNearestPlace(HeroControl hero)
    {
        Dictionary<int, AnApproacData> place = hero.AnApproac;
        float Magnitude = float.PositiveInfinity;
        int namber = int.MaxValue;
        for (int i = 0; i < place.Count; i++)
        {
            if (place[i].hexagon != null)
            {
                if (!place[i].busy)
                {
                    if ((place[i].hexagon.transform.position - transform.position).magnitude < Magnitude)
                    {
                        Magnitude = (place[i].hexagon.transform.position - transform.position).magnitude;
                        namber = i;
                    }
                }
                else
                {
                    IMove moveThis = this;
                    if (place[i].hexagon.ObjAbove == moveThis)
                    {
                        if ((place[i].hexagon.transform.position - transform.position).magnitude < Magnitude)
                        {
                            Magnitude = (place[i].hexagon.transform.position - transform.position).magnitude;
                            namber = i;
                        }
                    }
                }
            }
        }
        _isToTheHero = true;

        if (namber < int.MaxValue)
        {
            _namberAnApproac = namber;
            return place[namber].hexagon;
        }
        else
        {
            return hero.RandomPlace(out _namberAnApproac);
        }
    }
    private List<HexagonControl> Bypass(List<HexagonControl> hexagons)//метод обхода
    {
        List<Node> nodesList = _algorithmDijkstra.Dijkstra(MapControlStatic.CreatingEdge(hexagons, this));

        if (nodesList == null)
        {
            return null;
        }
        List<HexagonControl> ListVertex = new List<HexagonControl>();

        for (int i = 0; i < nodesList.Count; i++)
        {
            if (i != nodesList.Count - 1)
            {
                if (i != 0)
                    ListVertex.Add(nodesList[i].NodeHexagon);

                List<HexagonControl> Bending = nodesList[i].GetHexagonsBending(nodesList[i + 1]);
                if (Bending != null)
                {
                    ListVertex.AddRange(Bending);
                }
            }
            else
                ListVertex.Add(nodesList[i].NodeHexagon);
        }

        return ListVertex;
    }
    private List<HexagonControl> SearchForAWay(HexagonControl hexagon)//возврашет все вершины по которым надо пройти 
    {
        Graph graphMain = new Graph(MapControlStatic.GraphStatic);
        graphMain.AddNodeFirst(MapControlStatic.FieldPosition(gameObject.layer, transform.position));
        graphMain.AddNode(hexagon);

        List<Node> nodesList = _algorithmDijkstra.Dijkstra(MapControlStatic.CreatingEdge(graphMain));

        if (nodesList == null)
        {
            return null;
        }

        List<HexagonControl> ListVertex = new List<HexagonControl>();

        for (int i = 1; i < nodesList.Count; i++)
        {
            ListVertex.Add(nodesList[i].NodeHexagon);
        }

        return ListVertex;
    }
    private void BypassCheck()
    {
        Vector2 MainPos = transform.position;
        Vector2 HexPos = _enemyMain.HexagonMain.transform.position;
        if ((MainPos - HexPos).magnitude > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, HexPos, _speedConst);
        }
        //else
        //{
        //    HexagonControl hexagonNext = MapControlStatic.FieldPosition(gameObject.layer, CurrentPos);
        //    IMove move = hexagonNext.ObjAbove;
        //    if ((move == null) || (move.IsGo()))
        //    {
        //        if (hexagonNext.IsFree || _enemyMain.ImoveMain != move)
        //        {

        //            _isStop = false;
        //            ContinueMove(MoveCorotine);
        //        }
        //    }
        //    else if (_isBypass)
        //    {
        //        List<HexagonControl> controls = new List<HexagonControl>
        //        {
        //            MapControlStatic.FieldPosition(gameObject.layer, transform.position),
        //            _targetHexagon
        //        };

        //        List<HexagonControl> Points = Bypass(controls);
        //        if (Points != null)
        //        {
        //            _isStop = false;
        //            if (BypassCorotine != null)
        //                StopCoroutine(BypassCorotine);

        //            _isMove = false;

        //            BypassCorotine = MovementBypass(Points);
        //            StartCoroutine(BypassCorotine);
        //        }
        //        else if (_enemyMain.HeroTarget != null)
        //        {
        //            HexagonControl hexagonFinish = GetNearestPlace(_enemyMain.HeroTarget);
        //            HexagonControl hexagon = hexagonFinish.Floor != null ? hexagonFinish.Floor : hexagonFinish;
        //            List<HexagonControl> controls2 = new List<HexagonControl>
        //            {
        //                MapControlStatic.FieldPosition(gameObject.layer, transform.position),
        //               hexagon
        //            };

        //            List<HexagonControl> Points2 = Bypass(controls2);

        //            if (Points2 != null)
        //            {
        //                _enemyMain.HeroConnect();
        //                _isStop = false;

        //                if (BypassCorotine != null)
        //                    StopCoroutine(BypassCorotine);
        //                _isMove = false;


        //                BypassCorotine = MovementBypass(Points2);
        //                StartCoroutine(BypassCorotine);
        //            }
        //            else
        //            {
        //                _isBypass = false;
        //            }
        //        }
        //        else
        //        {
        //            _isBypass = false;
        //        }
        //    }
        //}
    }
    public IEnumerator StopSpeed(float pause)
    {
        _speed = 0;
        yield return new WaitForSeconds(pause);
        _speed = _speedConst;
    }
    public void StartWay(HexagonControl hexagonFinish)
    {
        if (hexagonFinish == null)
        {
            Debug.LogError("lack of end point");
            return;
        }

        if (MoveCorotine != null)
        {
            StopCoroutine(MoveCorotine);
        }

        ListPoints.Clear();
        HexagonControl hexagon = hexagonFinish.Floor != null ? hexagonFinish.Floor : hexagonFinish;

        List<HexagonControl> hexagons = SearchForAWay(hexagon);

        if (hexagons == null)
        {
            Debug.Log("No Way");
            ListPoints.Add(_enemyMain.HexagonMain);
        }
        else
        {
            ListPoints.AddRange(hexagons);
        }

        MoveCorotine = Movement();
        StartCoroutine(MoveCorotine);
        _isStop = false;

    }
    public void StartWayHero(HeroControl hero)
    {
        ResetPath();
        StartWay(GetNearestPlace(hero));
    }
    public void StopMove(IEnumerator Corotine)
    {
        _isMove = false;
        StopCoroutine(Corotine);
        _isStop = true;
        _isBypass = true;
        //StartCoroutine(StopSpeed(0.2f));
    }
    public void ContinueMove(IEnumerator Corotine)
    {
        Debug.Log(123);
        StopCoroutine(Corotine);

        StartCoroutine(Corotine);
    }

    #region interface 
    public bool IsGo()
    {
        return _isMove;
    }

    public EnemyControl GetEnemy()
    {
        return _enemyMain;
    }

    public HeroControl GetHero()
    {
        return null;
    }

    public List<HexagonControl> GetSurroundingHexes()
    {
        return _enemyMain.GetSurroundingHexes();
    }
    #endregion
}
