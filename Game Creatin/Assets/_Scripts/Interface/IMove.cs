using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMove
{
    void StopMove(HexagonControl CollcionHex);
    void StopMoveTarget();
    void StopSpeedAtack(float timeStop);
    void StartWay(HexagonControl hexagonFinish, IMove EnemyTarget);
    bool IsGo();
    bool IsFlight();
    List<HexagonControl> GetSurroundingHexes();
}
