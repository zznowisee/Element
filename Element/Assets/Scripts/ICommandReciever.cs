using System;
using System.Collections;

public interface ICommandReciever
{
    void RunPutDownUp(Action callback, bool coloring);               // 1
    void RunConnect(Action callback);                                // 2
    void RunSplit(Action callback);                                  // 3
    void RunMove(Action callback, Direction moveDirection);          // 4
    void RunRotate(Action callback, RotateDirection rotateDirection);// 5
    void RunDelay(Action callback);                                  // 6
}
