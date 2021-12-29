using System;

public interface ICommandReader
{
    public event Action OnFinishCommand;
    void RunCommand(CommandType commandType);
}

[System.Serializable]
public enum CommandType
{
    Empty,
    PutDown,
    PutUp,
    ConnectorCR,
    ConnectorCCR,
    Connect,
    Split,
    Delay,
    Push,
    Pull,
    ControllerCCR,
    ControllerCR,
}
