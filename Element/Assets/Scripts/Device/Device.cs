using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Device : MonoBehaviour
{
    [HideInInspector] public int index;
    [HideInInspector] public HexCell cell;
    [HideInInspector] public HexCell recordCell;
    public virtual void Record() { }
    public virtual void ClearCurrentInfo() { }
    public virtual void ReadPreviousInfo() { }
}
