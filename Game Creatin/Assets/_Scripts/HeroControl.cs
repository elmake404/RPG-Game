using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroControl : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public IEnumerator Movement(List<Vector3> ListPos)
    {
        while (ListPos.Count>0)
        {
            transform.position = Vector2.Lerp(transform.position,ListPos[0], _speed);
            if ((ListPos[0]-transform.position).magnitude <0.2f)
            {
                ListPos.Remove(ListPos[0]);
            }
            yield return new WaitForSeconds(0.02f);
        }
    }
}
