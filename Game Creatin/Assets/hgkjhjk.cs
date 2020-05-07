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
            //Vector2 y = MapControlStatic.mapNav[5, 10].transform.position;
            //Vector2 y = MapControlStatic.mapNav[5, 11].transform.position;
            Vector2 y = MapControlStatic.mapNav[2, 14].transform.position;
            //Vector2 y = MapControlStatic.mapNav[2, 5].transform.position;
            transform.position = Vector2.MoveTowards(transform.position, y, 0.4f);
            //MapControlStatic.GetPositionOnTheMap(transform.position).Flag();

            HexagonControl[] controls = null;
            controls = MapControlStatic.GetPositionOnTheMap(y, transform.position);
            for (int i = 0; i < controls.Length; i++)
            {
                if (controls[i] == null)
                {
                    Debug.Log(controls.Length);
                }
                controls[i].Flag();

            }
        }
    }
}
