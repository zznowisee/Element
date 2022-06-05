using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProcessManager : MonoBehaviour
{

    public enum ProcessState
    {
        EDIT,
        PLAY,
        PAUSE,
        STEP,
        WARNING,
        JUMP,
        FINISHED
    }

    [SerializeField] ProcessState processState;

    public event Action<int> OnRunNewLine;

    public event Action OnRead;
    public event Action OnClear;
    public event Action OnRecord;
    //public event Action OnBUG;
    public event Action OnReachEndLine;
    public event Action OnLevelComplete;
    //public event Action OnPause;
    public event Action OnReachBreakPoint;

    public static ProcessManager Instance { get; private set; }

    private List<HexCell> recordCells;
    private List<ControllerConsole> controllerConsolesList;
    [SerializeField] ExecuteTimer defaultTimer;
    //[SerializeField] ExecuteTimer jumpTimer;
    private ProcessCounter processCounter;
    public int LineIndex => processCounter.commandCurrentIndex;
    public float ExecuteTime => defaultTimer.executeTime;

    void Awake()
    {
        Instance = this;
        recordCells = new List<HexCell>();
        controllerConsolesList = new List<ControllerConsole>();
        processState = ProcessState.EDIT;

        processCounter.Reset();
    }

    public bool CanOperate()
    {
        return processState == ProcessState.EDIT;
    }

    void Start()
    {
        UI_OperateScene.Instance.OnExitOperateScene += SwitchToMainScene;
        UI_OperateScene.Instance.OnCompleteExit += CompleteExit;
        UI_WarningManager.Instance.OnWarning += OnDeviceTriggerBUG;
    }

    private void OnDeviceTriggerBUG()
    {
        processState = ProcessState.WARNING;
        //TODO: animation vfx

    }

    private void OnDisable()
    {
        UI_WarningManager.Instance.OnWarning -= OnDeviceTriggerBUG;
    }

    private void SwitchToMainScene()
    {
        processCounter.Reset();
    }

    public void SignNewControllerWithConsole(ControllerConsole controllerConsole_)
    {
        controllerConsolesList.Add(controllerConsole_);
        controllerConsole_.controller.OnFinishCommand += ReleaserFinishCommand;
        controllerConsole_.controller.OnDestory += (Device device) =>
        {
            Device_Controller controller = device as Device_Controller;
            controllerConsolesList.Remove(controllerConsole_);
            Destroy(controllerConsole_.console.gameObject);
        };
    }

    public void ResetCommands() => controllerConsolesList.ForEach(cc => cc.console.ResetAll());

    void ReleaserFinishCommand()
    {
        print("Command Finished");
        processCounter.controllerWaitingNumber--;
        //check if completed
        if (CheckManager.Instance.Complete())
        {
            if (processCounter.ReachEndLine())
            {
                print("Reach End Line");
                processState = ProcessState.FINISHED;
                OnReachEndLine?.Invoke();
                return;
            }
            else
            {
                processState = ProcessState.PAUSE;
                return;
            }
        }

        if (processState == ProcessState.WARNING)
            return;

        if (processCounter.AllDeviceFinishedCommand())
            OnFinishLine();
    }

    void OnFinishLine()
    {
        //print("Finished Once Command");
        if (processCounter.ReachEndLine())
        {
            print("ProcessManager - [FINISHED] ");
            processState = ProcessState.FINISHED;
            OnReachEndLine?.Invoke();
        }
        else
        {
            switch (processState)
            {
                case ProcessState.PLAY: ReadNewLine(); break;
                case ProcessState.STEP: processState = ProcessState.PAUSE; break;
            }
        }
    }

    private void ReadNewLine()
    {
        processCounter.commandMaxIndex = GetCommandMaxIndex();
        print(processCounter.commandMaxIndex);
        if(controllerConsolesList.Count == -1 || processCounter.ReachEndLine())
        {
            OnReachEndLine?.Invoke();
            return;
        }

        StartCoroutine(RunLine());

        int GetCommandMaxIndex()
        {
            int maxIndex = -1;
            foreach (var controllerConsole in controllerConsolesList)
            {
                int consoleMaxIndex = controllerConsole.console.GetMaxCommandIndex();
                if (consoleMaxIndex > maxIndex)
                {
                    maxIndex = consoleMaxIndex;
                }
            }
            return maxIndex;
        }

        IEnumerator RunLine()
        {
            yield return new WaitForSeconds(defaultTimer.spacingTime);

            processCounter.controllerWaitingNumber = controllerConsolesList.Count;

            processCounter.commandCurrentIndex++;

            controllerConsolesList.ForEach(cc => cc.RunCommand(processCounter.commandCurrentIndex, ExecuteTime));
            OnRunNewLine?.Invoke(processCounter.commandCurrentIndex);
        }
    }


    void Record()
    {
        OnRecord?.Invoke();
    }

    void Read()
    {
        OnClear?.Invoke();
        OnRead?.Invoke();

        for (int i = 0; i < recordCells.Count; i++)
        {
            recordCells[i].EraseColor();
        }
        recordCells.Clear();
    }

    public void Play()
    {
        switch (processState)
        {
            case ProcessState.EDIT:
                Record();
                ReadNewLine();
                break;
            case ProcessState.PAUSE:
                ReadNewLine();
                break;
            case ProcessState.PLAY:
                print("PlayBtn BUG, can press playBtn in play state");
                break;
            case ProcessState.STEP:
                processState = ProcessState.PLAY;
                break;
            case ProcessState.WARNING:
                print("PlayBtn BUG , can press playBtn in warning state");
                break;
            case ProcessState.JUMP:
                print("PlayBtn BUG, can press PlayBtn in jump state");
                break;
        }
        processState = ProcessState.PLAY;
    }

    public void Pause()
    {
        processState = ProcessState.PAUSE;
    }

    public void Step()
    {
        switch (processState)
        {
            case ProcessState.EDIT:
                Record();
                ReadNewLine();
                processState = ProcessState.STEP;
                break;
            case ProcessState.PAUSE:
                ReadNewLine();
                processState = ProcessState.STEP;
                break;
            case ProcessState.PLAY:
                processState = ProcessState.STEP;
                break;
            case ProcessState.STEP:
                print("unvalid input , press step btn in step state");
                break;
            case ProcessState.WARNING:
                print("stepBtn BUG, press step btn in warning state");
                break;
            case ProcessState.JUMP:
                print("stepBtn BUG, press stepBtn in jump state");
                break;
        }
    }

    public void Stop()
    {
        StopAllCoroutines();

        processState = ProcessState.EDIT;
        processCounter.Reset();
        // read all infos
        Read();
        ResetCommands();
        //UI_TooltipManager.Instance.HideWarning();
        UI_WarningManager.HideWarning();
    }

    public void Jump()
    {
        switch (processState)
        {
            case ProcessState.EDIT:
                Record();
                ReadNewLine();
                processState = ProcessState.JUMP;
                break;
            case ProcessState.PAUSE:
                ReadNewLine();
                processState = ProcessState.JUMP;
                break;
            case ProcessState.PLAY:
                print("Switch to jump state in play");
                processState = ProcessState.JUMP;
                break;
            case ProcessState.STEP:
                print("Switch to jump state in step");
                processState = ProcessState.JUMP;
                break;
            case ProcessState.WARNING:
                print("JumpBtn BUG, can press jumpBtn in warning state");
                break;
        }
    }

    public void SignColoredHexCell(HexCell cell_)
    {
        if (recordCells.Contains(cell_))
            return;

        recordCells.Add(cell_);
    }

    public void CompleteExit()
    {
        processCounter.Reset();
        processState = ProcessState.EDIT;
        recordCells.ForEach(cell => cell.EraseColor());
        recordCells.Clear();
        UI_WarningManager.HideWarning();
    }

    [Serializable]
    public struct ProcessCounter
    {
        public int controllerWaitingNumber;
        public int commandCurrentIndex;
        public int commandMaxIndex;

        public void Reset()
        {
            commandMaxIndex = 0;
            commandCurrentIndex = -1;
            controllerWaitingNumber = 0;
        }
        public bool ReachEndLine() => commandCurrentIndex == commandMaxIndex;
        public bool AllDeviceFinishedCommand() => controllerWaitingNumber == 0;
    }

    [Serializable]
    public struct ExecuteTimer
    {
        public float spacingTime;
        public float executeTime;
    }
}