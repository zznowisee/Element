using System;

public enum BrushState
{
    PUTDOWN,
    PUTUP
}

public interface IReciever
{
    void ExecutePutDownUp(Action releaserCallback, float time, BrushState brushState);               // 1
    void ExecuteConnect(Action releaserCallback, float time);                                // 2
    void ExecuteSplit(Action releaserCallback, float time);                                  // 3
    void ExecuteMove(Action releaserCallback, float time, Direction moveDirection);          // 4
    void ExecuteRotate(Action releaserCallback, float time, RotateDirection rotateDirection);// 5
}
