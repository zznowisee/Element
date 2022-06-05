using System;

public interface IReleaser
{
    public event Action OnFinishCommand;
    void ReleaseCommand(CommandType commandType, float executeTime);
    void RecieverFinishedCommand();
}

