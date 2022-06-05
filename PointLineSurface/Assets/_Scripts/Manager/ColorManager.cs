using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager Instance { get; private set; }

    public ColorSO red;
    public ColorSO blue;
    public ColorSO yellow;
    public ColorSO purple;
    public ColorSO green;
    public ColorSO orange;

    void Awake()
    {
        Instance = this;
    }

    public Color GetColor(ColorType type, bool init)
    {
        switch (type)
        {
            default:
            case ColorType.Blue: return init ? blue.initColor : blue.drawColor;
            case ColorType.Yellow: return init ? yellow.initColor : yellow.drawColor;
            case ColorType.Red: return init ? red.initColor : red.drawColor;
            case ColorType.Purple: return init ? purple.initColor : purple.drawColor;
            case ColorType.Green: return init ? green.initColor : green.drawColor;
            case ColorType.Orange: return init ? orange.initColor : orange.drawColor;
        }
    }
}
