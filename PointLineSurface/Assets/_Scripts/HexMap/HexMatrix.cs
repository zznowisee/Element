using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMatrix
{
    public const float outerRadius = 5f;
    public const float innerRadius = outerRadius * 0.866025404f;

    public static Vector3[] conrners =
    {
        new Vector3(0f, outerRadius),
        new Vector3(innerRadius, 0.5f * outerRadius),
        new Vector3(innerRadius, -0.5f * outerRadius),
        new Vector3(0f, -outerRadius),
        new Vector3(-innerRadius, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0.5f * outerRadius),
        new Vector3(0f, outerRadius)
    };
}
