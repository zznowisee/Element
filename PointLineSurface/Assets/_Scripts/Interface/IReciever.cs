using System;

public interface IReciever
{
    void ExecutePutdown(Action releaserCallback, float time);
    void ExecutePutup(Action releaserCallback, float time);              
    void ExecuteConnect(Action releaserCallback, float time);                               
    void ExecuteSplit(Action releaserCallback, float time);                                  
    void ExecuteMove(Action releaserCallback, float time, Direction moveDirection);          
    void ExecuteRotate(Action releaserCallback, float time, RotateDirection rotateDirection);
}
