using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControl 
{
    void Collision(Vector2 NextPos);

    HexagonControl HexagonMain();

    IMove Target();
    List<HexagonControl> GetSurroundingHexes();

}
