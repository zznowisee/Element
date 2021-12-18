using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPage : MonoBehaviour
{
    public SolutionSystem current;
    public void ResetPage()
    {
        if(current != null)
        {
            current.ResetPage();
        }
    }
}
