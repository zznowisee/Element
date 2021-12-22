using System;

public interface ICommandReciever
{
    void RunPutDownUp(Action callback, bool coloring);
    void RunRotate(Action callback, RotateDirection rotateDirection);
    void RunMove(Action callback, Direction moveDirection);
    void RunConnect(Action callback);
    void RunSplit(Action callback);
    void RunDelay(Action callback);
}
