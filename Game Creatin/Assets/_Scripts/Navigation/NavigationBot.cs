using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationBot : MonoBehaviour, IMove
{
    private IEnumerator MoveCorotine;
    private IEnumerator BypassCorotine;
    private List<HexagonControl> ListPoints = new List<HexagonControl>();//точки через который надо пройти 
    private HexagonControl _targetHexagon;
    private AlgorithmDijkstra _algorithmDijkstra = new AlgorithmDijkstra();
    [SerializeField]
    private EnemyControl _enemyMain;
    private Vector2 CurrentPos;

    private bool _isStop = false, _isMove;

    [SerializeField]
    private float _speed;
    private void Awake()
    {
        MoveCorotine = Movement();
    }

    void Update()
    {
        if (_isStop)
        {
            BypassCheck();
        }
    }
    private IEnumerator Movement()//коротина движения
    {
        _isMove = true;
        List<HexagonControl> PointList = new List<HexagonControl>();
        PointList.AddRange(ListPoints);
        ListPoints.Clear();
        _targetHexagon = PointList[0].Elevation != null ? PointList[0].Elevation : PointList[0];
        while (PointList.Count > 0)
        {
            Vector2 positionCurrent = _targetHexagon.transform.position;
            Vector2 Pos = Vector2.MoveTowards(transform.position, positionCurrent, _speed);
            CurrentPos = Pos + (Vector2)(_targetHexagon.transform.position - transform.position).normalized * 1.8f;
            _enemyMain.Collision(CurrentPos, MoveCorotine);
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
        ContinueMove(MoveCorotine);
    }
    private List<HexagonControl> Bypass(List<HexagonControl> hexagons)//метод обхода
    {
        List<Node> nodesList = _algorithmDijkstra.Dijkstra(MapControlStatic.CreatingEdge(hexagons,this));
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
            else
            {
                List<HexagonControl> controls = new List<HexagonControl>();
                controls.Add(MapControlStatic.FieldPosition(gameObject.layer, transform.position));
                controls.AddRange(move.GetSurroundingHexes());
                controls.Add(_targetHexagon);
                List<HexagonControl> Points = new List<HexagonControl>();
                Points = Bypass(controls);
                if (Points != null)
                {
                    //Debug.Log(Points.Count + " " + name);
                    _isStop = false;
                    BypassCorotine = MovementBypass(Points);
                    StartCoroutine(BypassCorotine);
                }
            }
        }

    }
    public void StartWay(HexagonControl hexagonFinish)
    {
        if (MoveCorotine != null)
        {
            StopCoroutine(MoveCorotine);
        }
        ListPoints.Clear();
        ListPoints.AddRange(SearchForAWay(hexagonFinish));

        if (ListPoints == null)
        {
            Debug.Log("No Way");
            return;
        }

        MoveCorotine = Movement();
        StartCoroutine(MoveCorotine);
    }
    public void StopMove(IEnumerator Corotine)
    {
        _isMove = false;

        StopCoroutine(Corotine);
        _isStop = true;
        //StartCoroutine(SatrMove());
    }
    public void ContinueMove(IEnumerator Corotine)
    {
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
