using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pg : MonoBehaviour
{
    private IEnumerator MoveCorotine;
    private List<HexagonControl> PassedPoints = new List<HexagonControl>();//пройденные вершины 
    private List<HexagonControl> ListPoints = new List<HexagonControl>();//пройденные вершины 


    [SerializeField]
    private float _speed;
    private int _namberPoint;

    public List<HexagonControl> VertexList = new List<HexagonControl>();


    [System.NonSerialized]
    public int HexagonRow = 0, HexagonColumn = 0;

    void Start()
    {
        MoveCorotine = Movement();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(1);
        }

    }
    private IEnumerator Movement()
    {
        List<HexagonControl> PointList = new List<HexagonControl>();
        ListPoints.Reverse();
        PointList.AddRange(ListPoints);
        Debug.Log(ListPoints.Count);
        ListPoints.Clear();
        while (PointList.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, PointList[PointList.Count - 1].transform.position, _speed);
            if ((PointList[PointList.Count - 1].transform.position - transform.position).magnitude <= 3.65f)
            {
                HexagonRow = PointList[_namberPoint].Row;
                HexagonColumn = PointList[_namberPoint].Column;
                PointList.Remove(PointList[PointList.Count - 1]);
            }
            yield return new WaitForSeconds(0.02f);
        }

    }
    private void SearchForAWay(HexagonControl hexagon)
    {
        List<RaycastHit2D> hit2Ds = new List<RaycastHit2D>();
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        float distance = (transform.position - hexagon.transform.position).magnitude;
        Physics2D.Raycast(transform.position, -(transform.position - hexagon.transform.position).normalized, contactFilter2D, hit2Ds, distance);
        if (hit2Ds.Count >= 0)
        {
            for (int i = 0; i < hit2Ds.Count; i++)
            {
                var GetHit = hit2Ds[i].collider.GetComponent<HexagonControl>();
                if (!GetHit.FreedomTest())
                {
                    PassedPoints.Clear();

                    hit2Ds[i].collider.GetComponent<HexagonControl>().Flag();
                    BreakingTheDeadlock(GetHit.Row, GetHit.Column);
                    return;
                }
            }
        }
        if (VertexList.Count > 1)
        {
            //здесь добавляем первую точку
            Debug.Log("-----------------------------------------------------------------------------");
            Debug.Log("namePoint " + hexagon.Row + " " + (hexagon.Column));
            ListPoints.Add(hexagon);
            _namberPoint = PointSelection(transform, VertexList);
            SearchForAWay(_namberPoint);
        }
        else
        {
            StopCoroutine(MoveCorotine);
            VertexList.Clear();
            PassedPoints.Clear();
            ListPoints.Clear();

            ListPoints.Add(hexagon);
            MoveCorotine = Movement();
            StartCoroutine(MoveCorotine);
        }

    }
    private void SearchForAWay(int I)
    {
        //VertexList.Add(hexagon);
        _namberPoint = PointSelection(VertexList[I].transform, VertexList);
        List<RaycastHit2D> hit2Ds = new List<RaycastHit2D>();
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        float distance = (VertexList[I].transform.position - VertexList[_namberPoint].transform.position).magnitude;
        Physics2D.Raycast(VertexList[I].transform.position, -(VertexList[I].transform.position - VertexList[_namberPoint].transform.position).normalized, contactFilter2D, hit2Ds, distance);
        //Debug.Log(hit2Ds.Count);
        if (hit2Ds.Count > 0)
        {
            for (int i = 0; i < hit2Ds.Count; i++)
            {
                var GetHit = hit2Ds[i].collider.GetComponent<HexagonControl>();

                if (!GetHit.FreedomTest())
                {
                    PassedPoints.Clear();
                    hit2Ds[i].collider.GetComponent<HexagonControl>().Flag();
                    //здесь чистим у нас есть проблемы 
                    BreakingTheDeadlock(GetHit.Row, GetHit.Column);
                    return;
                }
            }
        }
        //Debug.Log("eee");

        if (_namberPoint != 0)
        {
            Debug.Log("namePoint " + VertexList[_namberPoint].Row + " " + (VertexList[_namberPoint].Column));
            ListPoints.Add(VertexList[_namberPoint]);

            SearchForAWay(_namberPoint);
        }
        else
        {
            //Debug.Log(_namberPoint);
            Debug.Log("namePoint " + VertexList[_namberPoint].Row + " " + (VertexList[_namberPoint].Column));
            ListPoints.Add(VertexList[_namberPoint]);
            VertexList.Clear();
            PassedPoints.Clear();
            StopCoroutine(MoveCorotine);

            MoveCorotine = Movement();

            StartCoroutine(MoveCorotine);


            //for (int i = 0; i < ListPoints.Count; i++)
            //{
            //    ListPoints[i].Flag();
            //}
        }
    }
    private void BreakingTheDeadlock(int row, int column)    //метод писка выхода из тупика  
    {
        //Debug.Log("-----------------------------------------------------------------------------");
        List<HexagonControl> hexagons = new List<HexagonControl>();
        hexagons.AddRange(MapControlStatic.mapNav[row, column].peaks);
        HexagonControl hexagonControl = null;//нужный 6-ти угольник  
        float Magnitude = 0;

        if (hexagons.Count > 1)
        {
            for (int i = 0; i < VertexList.Count; i++)
            {
                for (int j = 0; j < hexagons.Count; j++)
                {
                    if (MapControlStatic.mapNav[hexagons[j].Row, hexagons[j].Column] == MapControlStatic.mapNav[VertexList[i].Row, VertexList[i].Column])
                    {
                        hexagons.Remove(hexagons[j]);
                    }
                }
            }
            for (int i = 0; i < hexagons.Count; i++)
            {
                //Debug.Log("namePoint " + hexagons[i].Row + " " + (hexagons[i].Column));
                //Debug.Log((MapControlStatic.mapNav[hexagons[i].Row, hexagons[i].Column].transform.position - MapControlStatic.mapNav[row, column].transform.position).magnitude);
                //Debug.Log((MapControlStatic.mapNav[hexagons[i].Row, hexagons[i].Column].transform.position - MapControlStatic.mapNav[VertexList[0].Row, VertexList[0].Column].transform.position).magnitude);

                if (hexagonControl == null)
                {
                    Magnitude = (MapControlStatic.mapNav[hexagons[i].Row, hexagons[i].Column].transform.position - MapControlStatic.mapNav[row, column].transform.position).magnitude +
                        (MapControlStatic.mapNav[hexagons[i].Row, hexagons[i].Column].transform.position - MapControlStatic.mapNav[VertexList[0].Row, VertexList[0].Column].transform.position).magnitude;
                    hexagonControl = hexagons[i];
                }

                if (Magnitude > (MapControlStatic.mapNav[hexagons[i].Row, hexagons[i].Column].transform.position - MapControlStatic.mapNav[row, column].transform.position).magnitude +
                        (MapControlStatic.mapNav[hexagons[i].Row, hexagons[i].Column].transform.position - MapControlStatic.mapNav[VertexList[0].Row, VertexList[0].Column].transform.position).magnitude)
                {
                    Magnitude = (MapControlStatic.mapNav[hexagons[i].Row, hexagons[i].Column].transform.position - MapControlStatic.mapNav[row, column].transform.position).magnitude +
                        (MapControlStatic.mapNav[hexagons[i].Row, hexagons[i].Column].transform.position - MapControlStatic.mapNav[VertexList[0].Row, VertexList[0].Column].transform.position).magnitude;
                    hexagonControl = hexagons[i];
                    //Debug.Log(" Magnitude " + Magnitude);
                }
            }
        }
        else
        {
            hexagonControl = hexagons[0];
        }
        //Debug.Log("hexagonControl " + hexagonControl.Row + " " + (hexagonControl.Column));

        VertexList.Add(hexagonControl);
        if (hexagonControl != null)
        {
            SearchForAWay(hexagonControl);
        }
        else
        {
            Debug.LogError("No vertices available");
        }
        //StartCoroutine(MoveCorotine);

        //for (int i = 0; i < VertexList.Count; i++)
        //{
        //    VertexList[i].Flag();
        //}
    }
    private int PointSelection(Transform Point, List<HexagonControl> Vertex)
    {
        float Mag = 0;
        int I = 0;
        int IPoint = 0;
        List<HexagonControl> hexagonControls = new List<HexagonControl>();
        hexagonControls.AddRange(Vertex);

        if (PassedPoints.Count > 0)
        {
            for (int j = 0; j < PassedPoints.Count; j++)
            {
                for (int i = 0; i < hexagonControls.Count; i++)
                {
                    if (MapControlStatic.mapNav[PassedPoints[j].Row, PassedPoints[j].Column] == MapControlStatic.mapNav[hexagonControls[i].Row, hexagonControls[i].Column])
                    {
                        hexagonControls.Remove(hexagonControls[i]);
                    }
                }
            }
        }

        if (hexagonControls.Count == 1)
        {
            I = 0;
        }
        else
        {
            hexagonControls.Remove(hexagonControls[0]);

            for (int i = 0; i < hexagonControls.Count; i++)
            {
                if (Mag == 0)
                {
                    Mag = (MapControlStatic.mapNav[hexagonControls[i].Row, hexagonControls[i].Column].transform.position - Point.transform.position).magnitude;
                    IPoint = i;
                }
                if ((MapControlStatic.mapNav[hexagonControls[i].Row, hexagonControls[i].Column].transform.position - Point.transform.position).magnitude < Mag)
                {
                    Mag = (MapControlStatic.mapNav[hexagonControls[i].Row, hexagonControls[i].Column].transform.position - Point.transform.position).magnitude;
                    IPoint = i;
                }
            }

            for (int i = 0; i < Vertex.Count; i++)
            {
                if (MapControlStatic.mapNav[Vertex[i].Row, Vertex[i].Column] == MapControlStatic.mapNav[hexagonControls[IPoint].Row, hexagonControls[IPoint].Column])
                {
                    I = i;
                }
            }
        }
        if (hexagonControls.Count > 0)
        {
            //Debug.Log(I);
        }
        else
        {
            Debug.LogError("lack of available peaks");
        }

        PassedPoints.Add(Vertex[I]);

        return I;
    }

    public void SatrtWay(HexagonControl hexagon)
    {
        VertexList.Add(hexagon);
        SearchForAWay(VertexList[PointSelection(transform, VertexList)]);
    }
}
