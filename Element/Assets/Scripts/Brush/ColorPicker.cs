using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPicker : MonoBehaviour
{
    [HideInInspector] public ColorSO color;
    MeshRenderer meshRenderer;
    public void Setup(ColorSO color_)
    {
        meshRenderer = GetComponent<MeshRenderer>();

        color = color_;
        meshRenderer.material.color = color.color;
    }
}
