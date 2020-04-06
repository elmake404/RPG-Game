using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroControl : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    public Navigation NavigationHero;
    private void Update()
    {
        if (NavigationHero.IsGo&& !_animator.GetBool("Run"))
        {
            _animator.SetBool("Run", true);
        }
        else if (!NavigationHero.IsGo && _animator.GetBool("Run"))
        {
            _animator.SetBool("Run", false);
        }
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
