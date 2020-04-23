using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMove
{
    bool IsGo();
    EnemyControl GetEnemy();//если аозвращает  null надо брать героя 
    HeroControl GetHero();
    List<HexagonControl> GetSurroundingHexes();
}
