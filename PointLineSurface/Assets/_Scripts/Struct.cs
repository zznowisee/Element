using System;

[System.Serializable]
public struct ControllerConsole
{
    public Device_Controller controller;
    public Console console;
    public ControllerConsole(Device_Controller controller_, Console console_)
    {
        controller = controller_;
        console = console_;
    }

    public void RunCommand(int lineIndex_, float executeTime_)
    {
        CommandType cmd = console.GetCommand(lineIndex_);

        controller.ReleaseCommand(cmd, executeTime_);
        console.ExecuteCommand(lineIndex_);
    }
}

public struct Device_WaitingChecker
{
    private int target;
    private Action finishCallback;
    public Device_WaitingChecker(Action finishCallback)
    {
        target = 0;
        this.finishCallback = finishCallback;
    }
    public void SetFinishCallback(Action callback) => finishCallback = callback;
    public void SetTarget(int target) => this.target = target;
    public void CheckFinish()
    {
        if (--target != 0)
            return;

        Finish();
    }

    public void Finish() => finishCallback?.Invoke();
    public void Reset()
    {
        target = 0;
        finishCallback = null;
    }
}
