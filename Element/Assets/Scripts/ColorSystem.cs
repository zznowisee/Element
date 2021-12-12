using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSystem : MonoBehaviour
{
    public Dictionary<ColorSO, ColorSO[]> recipeDictionary;
    public ColorSO red;
    public ColorSO blue;
    public ColorSO yellow;
    public ColorSO purple;
    public ColorSO green;
    public ColorSO white;
    public ColorSO orange;
    public ColorSO empty;
    private void Awake()
    {
        recipeDictionary[purple] = new ColorSO[] { blue, red };
        recipeDictionary[green] = new ColorSO[] { blue, yellow };
        recipeDictionary[orange] = new ColorSO[] { red, yellow };
    }

    public ColorSO GetRecipeOutput(ColorSO[] inputs)
    {
        foreach (ColorSO colorSO in recipeDictionary.Keys)
        {
            ColorSO[] recipe = recipeDictionary[colorSO];
            if((inputs[0] == recipe[0] && inputs[1] == recipe[1]) ||
                (inputs[0] == recipe[0] && inputs[1] == recipe[1]))
            {
                return colorSO;
            }
        }

        return empty;
    }
}
