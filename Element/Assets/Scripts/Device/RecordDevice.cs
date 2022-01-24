using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public abstract class RecordDevice : MonoBehaviour
{
    [HideInInspector] public int index;
    [HideInInspector] public HexCell cell;
    [HideInInspector] public HexCell recordCell;
    public abstract void Record();
    public abstract void ClearCurrentInfo();
    public abstract void ReadPreviousInfo();
}
