using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager Instance { get; private set; }

    public ColorSO red;
    public ColorSO blue;
    public ColorSO yellow;
    public ColorSO white;

    void Awake()
    {
        Instance = this;
    }

    public ColorSO GetColorSOFromColorType(ColorType colorType)
    {
        ColorSO color;
        switch (colorType)
        {
            case ColorType.Blue:
                return blue;
            case ColorType.Red:
                return red;
            case ColorType.Yellow:
                return yellow;
            default:
            case ColorType.White:
                return white;
        }
    }
}
