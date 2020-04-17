using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapControl : MonoBehaviour
{
    [SerializeField]
    private EnemyManager enemyManager;
    [SerializeField]
    private Transform[] hexagons;
    [SerializeField]
    private HeroControl[] _heroControls;
    [SerializeField]
    private EnemyControl[] _enemyControls;
    [SerializeField]
    private HexagonControl[] _arreyVertex,_elevation;

    void Awake()
    {
        MapControlStatic.MapPos = transform.position;
        enemyManager.InitializationList(_heroControls,_enemyControls);
        //MapControlStatic.Elevation = new HexagonControl[_elevation.Length];
        //for (int i = 0; i < _elevation.Length; i++)
        //{
        //    MapControlStatic.Elevation[i] = _elevation[i];
        //}
        for (int i = 0; i < hexagons.Length; i++)
        {
            hexagons[i].name = i.ToString();
            for (int j = 0; j < hexagons[i].childCount; j++)
            {
                HexagonControl hexagon = hexagons[i].GetChild(j).GetComponent<HexagonControl>();
                MapControlStatic.mapNav[i, j] = hexagon;
                hexagons[i].GetChild(j).name = j.ToString();
                hexagon.NamberHex();
            }
        }
        for (int i = 0; i < _heroControls.Length; i++)
        {
            _heroControls[i].InitializationVertexNavigation(_arreyVertex);
        }
        GraphRecord();

        //for (int i = 0; i < _enemyControls.Length; i++)
        //{
        //    _enemyControls[i].InitializationHero(_heroControls);
        //    _enemyControls[i].Navigation.InitializationVertex(_arreyVertex);
        //}
    }

    void Update()
    {
        
    }
    private void GraphRecord()
    {
        List<HexagonControl> hexagonsVartex = new List<HexagonControl>();
        hexagonsVartex.AddRange(_arreyVertex);
        MapControlStatic.GraphStatic = new Graph(hexagonsVartex);
        //Debug.Log(graph.Length);
        for (int i = 0; i < MapControlStatic.GraphStatic.Length - 1; i++)
        {
            bool IsElevation = false;

            for (int j = i + 1; j < MapControlStatic.GraphStatic.Length; j++)
            {

                Vector2 StartPosition = MapControlStatic.GraphStatic[i].NodeHexagon.transform.position;
                Vector2 direction = MapControlStatic.GraphStatic[j].NodeHexagon.transform.position;

                bool NoRibs = false;

                if (hexagonsVartex[i].TypeHexagon <= 0 || (hexagonsVartex[i].TypeHexagon == 3 && MapControlStatic.GraphStatic[j].NodeHexagon.gameObject.layer != 10))
                {
                    IsElevation = false;
                    if (!MapControlStatic.CollisionCheck(StartPosition, direction, IsElevation))
                    {
                        NoRibs = true;
                    }
                }
                else
                {
                    IsElevation = true;
                    if (!MapControlStatic.CollisionCheckElevation(StartPosition, direction, IsElevation))
                    {
                        NoRibs = true;
                    }
                }
                if (!NoRibs)
                {
                    float magnitude = (MapControlStatic.GraphStatic[i].NodeHexagon.transform.position - MapControlStatic.GraphStatic[j].NodeHexagon.transform.position).magnitude;
                    MapControlStatic.GraphStatic[i].Connect(MapControlStatic.GraphStatic[j], magnitude);
                }
            }
        }
    }
}
