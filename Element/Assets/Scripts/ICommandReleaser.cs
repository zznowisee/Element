using System;

public interface ICommandReleaser
{
    public event Action OnFinishedOneLineCommand;
    void ReleaseCommand(CommandType commandType, float executeTime);
    void RecieverFinishedCommand();
}

