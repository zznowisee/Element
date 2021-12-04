using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandGhost : MonoBehaviour
{
    [SerializeField] Image icon;
    public void Setup(CommandSO commandSO)
    {
        icon.sprite = commandSO.icon;
    }
}
