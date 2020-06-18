using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMove
{
    void StopMove();
    void StopMoveTarget();
    void StopSpeedAtack(float timeStop);
    void StartWay(HexagonControl hexagonFinish, IMove EnemyTarget);
    bool IsGo();
    List<HexagonControl> GetSurroundingHexes();
}
