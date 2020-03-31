using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroControl : MonoBehaviour
{
    public Navigation NavigationHero;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            NavigationHero.StopMove();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            NavigationHero.ContinueMove();
        }
    }
}
