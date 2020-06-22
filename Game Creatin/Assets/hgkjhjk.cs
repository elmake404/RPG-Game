using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hgkjhjk : MonoBehaviour
{
    void Start()
    {
        transform.position= MapControl.FieldPosition(gameObject.layer, transform.position).GetArrayElement().position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 y = MapControl.MapNav[5, 13].transform.position;

            transform.position = Vector2.MoveTowards(transform.position, y, 0.4f);

            HexagonControl[] controls = null;
            controls = MapControl.GetPositionOnTheMap(y, transform.position);
            for (int i = 0; i < controls.Length; i++)
            {
                if (controls[i] == null)
                {
                    Debug.Log(controls.Length);
                }
                controls[i].GetHexagonMain().Flag();

            }
        }
    }
}
