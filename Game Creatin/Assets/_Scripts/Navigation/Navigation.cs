﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour, IMove
{
    private IEnumerator MoveCorotine;
    private IEnumerator BypassCorotine;
    private List<HexagonControl> ListPoints = new List<HexagonControl>();//точки через который надо пройти 
    private HeroControl _heroMain;
    private HexagonControl _targetHexagon;
    private AlgorithmDijkstra _algorithmDijkstra = new AlgorithmDijkstra();
    private List<HexagonControl> _listVertex;
    private Vector2 CurrentPos;

    [SerializeField]
    private float _speed;
    private bool _isStop = false, _isMove, _isBypass;
    [System.NonSerialized]
    public int HexagonRow = 0, HexagonColumn = 0;

    void Start()
    {
        MoveCorotine = Movement();
        _isMove = false;
        if (_listVertex == null || _listVertex.Count == 0)
        {
            Debug.LogError("The lack of vertex in the hero");
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
        _isMove = true;
        _heroMain.Animator.SetBool("Run", true);
        List<HexagonControl> PointList = new List<HexagonControl>();
        PointList.AddRange(ListPoints);
        ListPoints.Clear();
        _targetHexagon = PointList[0].Elevation != null ? PointList[0].Elevation : PointList[0];
        while (PointList.Count > 0)
        {
            Vector2 positionCurrent = _targetHexagon.transform.position;
            Vector2 Pos = Vector2.MoveTowards(transform.position, positionCurrent, _speed);
            CurrentPos = Pos + (Vector2)(_targetHexagon.transform.position - transform.position).normalized * 1.8f;
            _heroMain.Collision(CurrentPos, MoveCorotine);
            transform.position = Pos;
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
        _heroMain.Animator.SetBool("Run", false);
        _isMove = false;
    }
    private IEnumerator MovementBypass(List<HexagonControl> hexagonControls)//коротина обхода
    {
        //for (int i = 0; i < hexagonControls.Count; i++)
        //{
        //    hexagonControls[i].Flag();
        //}
        _isMove = true;
        List<HexagonControl> PointList = new List<HexagonControl>();
        PointList.AddRange(hexagonControls);
        _targetHexagon = PointList[0].Elevation != null ? PointList[0].Elevation : PointList[0];
        while (PointList.Count > 1)
        {
            Vector2 positionCurrent = _targetHexagon.transform.position;
            CurrentPos = Vector2.MoveTowards(transform.position, positionCurrent, _speed);
            _heroMain.Collision(CurrentPos, BypassCorotine);
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
        ContinueMove(MoveCorotine);
    }

    private List<HexagonControl> SearchForAWay(HexagonControl hexagon, Transform startingPoint, bool elevation)//возврашет все вершины по которым надо пройти 
    {
        List<HexagonControl> ListOfNecessaryVertices;
        float Magnitude;

        if (elevation)
        {
            if (!MapControlStatic.CollisionCheckElevation(out Magnitude, startingPoint.position, hexagon.transform.position, elevation, this))
            {
                ListOfNecessaryVertices = (BreakingTheDeadlock(GetArrayHex(), hexagon));
            }
            else
            {
                ListOfNecessaryVertices = new List<HexagonControl> { hexagon };
            }
        }
        else
        {
            if (!MapControlStatic.CollisionCheck(out Magnitude, startingPoint.position, hexagon.transform.position, elevation, this))
            {
                ListOfNecessaryVertices = (BreakingTheDeadlock(GetArrayHex(), hexagon));
            }
            else
            {
                ListOfNecessaryVertices = new List<HexagonControl> { hexagon };
            }
        }

        if (ListOfNecessaryVertices == null)
        {
            return null;
        }
        else
            return ListOfNecessaryVertices;
    }
    private List<HexagonControl> BreakingTheDeadlock(HexagonControl From ,HexagonControl to)//выстравивает пути обхода
    {
        Graph graphMain = new Graph(MapControlStatic.EdgeCheck( MapControlStatic.GraphStatic, this));

        graphMain.AddNodeFirst(From);
        graphMain.AddNode(to);

        List<Node> nodesList = _algorithmDijkstra.Dijkstra(MapControlStatic.CreatingEdge(graphMain,this));

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
    private void BypassCheck()
    {
        Vector2 MainPos = transform.position;
        Vector2 HexPos = _heroMain.HexagonMain.transform.position;
        if ((MainPos - HexPos).magnitude > 0.001f)
        {
            transform.position = Vector2.MoveTowards(transform.position, HexPos, _speed);
        }
        else
        {
            HexagonControl hexagonNext = MapControlStatic.FieldPosition(gameObject.layer, CurrentPos);
            IMove move = hexagonNext.ObjAbove;
            if ((move == null) || (move.IsGo()))
            {
                if (hexagonNext.IsFree)
                {
                    _isStop = false;
                    ContinueMove(MoveCorotine);
                }
            }
            else if (_isBypass)
            {
                List<HexagonControl> controls = new List<HexagonControl>
                {
                    GetArrayHex(),
                    _targetHexagon
                };

                List<HexagonControl> Points = Bypass(controls);
                if (Points != null)
                {
                    _isStop = false;
                    BypassCorotine = MovementBypass(Points);
                    StartCoroutine(BypassCorotine);
                }
            }
            else
            {
                _isBypass = false;
            }
        }

    }
    private HexagonControl GetArrayHex()
    {
        HexagonControl hexagon = MapControlStatic.FieldPosition(gameObject.layer, transform.position);
        return hexagon.Floor != null ? hexagon.Floor : hexagon;
    }
    public void StartWay(HexagonControl hexagonFinish)
    {
        //всегда вноси новый слой
        StopCoroutine(MoveCorotine);
        ListPoints.Clear();
        HexagonControl hexagon = hexagonFinish.Floor != null ? hexagonFinish.Floor : hexagonFinish;
        List<HexagonControl> ListHexgon = (SearchForAWay(hexagon, transform, false));
        if (ListHexgon == null)
        {
            Debug.Log("No Way");
            return;
        }
        else
            ListPoints.AddRange(ListHexgon);

        _isStop = false;

        MoveCorotine = Movement();
        StartCoroutine(MoveCorotine);
    }
    public void StartWayElevation(HexagonControl hexagonFinish)
    {
        StopCoroutine(MoveCorotine);
        ListPoints.Clear();
        HexagonControl hexagon = hexagonFinish.Floor != null ? hexagonFinish.Floor : hexagonFinish;

        List<HexagonControl> ListHexgon = (SearchForAWay(hexagon, transform, false));
        if (ListHexgon == null)
        {
            Debug.Log("No Way");
            return;
        }
        else
            ListPoints.AddRange(ListHexgon);
        _isStop = false;

        MoveCorotine = Movement();
        StartCoroutine(MoveCorotine);
    }
    public void Initialization(HexagonControl[] hexagons, HeroControl hero)
    {
        _heroMain = hero;
        _listVertex = new List<HexagonControl>();
        _listVertex.AddRange(hexagons);

    }
    public void StopMove(IEnumerator Corotine)
    {
        _heroMain.Animator.SetBool("Run", false);
        _isStop = true;
        _isBypass = true;
        _isMove = false;
        StopCoroutine(Corotine);
    }
    public void ContinueMove(IEnumerator Corotine)
    {
        //if (name == "Hero")
        //{
        //    Debug.Log(Corotine);
        //}

        _heroMain.Animator.SetBool("Run", true);
        _isMove = true;

        StartCoroutine(Corotine);
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

    public HeroControl GetHero()
    {
        return _heroMain;
    }

    public List<HexagonControl> GetSurroundingHexes()
    {
        return _heroMain.GetSurroundingHexes();
    }
    #endregion
}