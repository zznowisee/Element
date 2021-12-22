using System;

public interface ICommandReader
{
    public event Action<Controller> OnFinishCommand;
    void RunCommand(CommandType commandType);
}

public enum CommandType
{
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
    ControllerCR
}
