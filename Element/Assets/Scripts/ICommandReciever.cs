using System;

public interface ICommandReciever
{
    public event Action OnFinishSecondLevelCommand;
    void RunPutDownUp(bool coloring);
    void RunRotate(int rotateDirection);
    void RunMove(Direction moveDirection);
    void RunConnect();
    void RunSplit();
    void RunDelay();
}
