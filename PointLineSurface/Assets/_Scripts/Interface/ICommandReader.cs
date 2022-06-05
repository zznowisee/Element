using System;

public interface ICommandReader
{
    public event Action OnFinishedOneLineCommand;
    void RunCommand(CommandType commandType);
}
