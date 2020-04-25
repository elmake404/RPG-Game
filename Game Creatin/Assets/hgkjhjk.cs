using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hgkjhjk : MonoBehaviour
{
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 y = MapControlStatic.mapNav[1, 17].transform.position;
            transform.position = Vector2.MoveTowards(transform.position, y, 1f);
            HexagonControl[] controls = null;
            controls = MapControlStatic.GetPositionOnTheMap(y.x - transform.position.x, transform.position);
            for (int i = 0; i < controls.Length; i++)
            {
                controls[i].Flag();
            }
        }
    }
}
