using System;

public interface ICommandReader
{
    public event Action OnFinishCommand;
    void Putdown();
    void Putup();
    void ConnectorCWRotate();
    void ConnectorCCWRotate();
    void Connect();
    void Split();
    void Delay();
    void Push();
    void Pull();
    void ControllerCCWRotate();
    void ControllerCWRotate();
}
