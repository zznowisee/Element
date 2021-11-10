using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = InputHelper.MouseWorldPositionIn2D;
            Vector3 dir = (mousePos - transform.position).normalized;

            float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

            Debug.DrawLine(transform.position, InputHelper.GetWireEndPositionFromMouse(transform.position, InputHelper.GetDirectionFromAngleInHex(angle)));
            print(InputHelper.GetDirectionFromAngleInHex(angle));
        }
    }
}
