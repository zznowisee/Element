using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum CommandReaderLevel
{
    Parent,
    Child
}
public abstract class CommandReader : MonoBehaviour
{
    [HideInInspector] public int index;
    public CommandReaderLevel readerLevel;
    public abstract void Finish();
    public abstract void Record();
    public abstract void Read();
}
