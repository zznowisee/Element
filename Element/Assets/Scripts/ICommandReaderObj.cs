using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommandReaderObj
{
    void Drop();
    void Pick();
    void ClockwiseRotate();
    void CounterClockwiseRotate();
    void Connect();
    void Split();
    void Delay();
    void GoAhead();
    void Back();
}
