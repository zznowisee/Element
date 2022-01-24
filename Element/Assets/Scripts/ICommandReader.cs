using System;

public interface ICommandReader
{
    public event Action OnFinishedOneLineCommand;
    void RunCommand(CommandType commandType);
}

[System.Serializable]
public enum CommandType
{
    Empty = 0,
    PutDown,
    PutUp,
    ConnectorCW,
    ConnectorCCW,
    Connect,
    Split,
    Delay,
    Push,
    Pull,
    ControllerCCR,
    ControllerCR,
}
