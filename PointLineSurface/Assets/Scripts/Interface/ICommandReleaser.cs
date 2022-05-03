using System;

public interface ICommandReleaser
{
    public event Action OnFinishCommand;
    void ReleaseCommand(CommandType commandType, float executeTime);
    void RecieverFinishedCommand();
}

