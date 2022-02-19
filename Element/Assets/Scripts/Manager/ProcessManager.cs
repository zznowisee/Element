using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProcessManager : MonoBehaviour
{

    public enum ProcessType
    {
        EDIT,
        PLAY,
        PAUSE,
        STEP,
        WARNING,
        JUMP,
        FINISHED
    }

    [SerializeField] ProcessType processType;

    public event Action<int> OnRunNewLine;

    public event Action OnRead;
    public event Action OnClear;
    public event Action OnRecord;
    public event Action OnTriggerBUG;
    public event Action OnReachEndLine;
    public event Action OnLevelComplete;
    public event Action OnPause;
    public event Action OnReachBreakPoint;

    public static ProcessManager Instance { get; private set; }

    [HideInInspector] public List<HexCell> recordCells;
    [SerializeField] RunTime defaultTimer;
    [SerializeField] RunTime jumpTimer;
    [SerializeField] ProcessCounter processCounter;

    public int LineIndex => processCounter.currentLineIndex;
    public float ExecuteTime => defaultTimer.executeTime;

    void Awake()
    {
        Instance = this;
        recordCells = new List<HexCell>();

        processType = ProcessType.EDIT;

        processCounter.Clear();
    }

    public bool CanOperate()
    {
        return processType == ProcessType.EDIT;
    }

    void Start()
    {
        OperatingRoomUI.Instance.OnSwitchToMainScene += SwitchToMainScene;
        OperatingRoomUI.Instance.OnStop += Stop;
        OperatingRoomUI.Instance.OnCompleteExit += CompleteExit;
    }

    private void SwitchToMainScene()
    {
        processCounter.Clear();
    }



    public void RegisterController(Controller controller)
    {
        controller.OnFinishCommand += ReleaserFinishCommand;
        controller.OnDestoryByPlayer += UnregisterController;
    }

    public void UnregisterController(Controller controller)
    {
        controller.OnFinishCommand -= ReleaserFinishCommand;
        controller.OnDestoryByPlayer -= UnregisterController;
    }

    public void TriggerBUG(Vector3 position, WarningType type)
    {
        processType = ProcessType.WARNING;

        TooltipManager.Instance.ShowWarning(position, type);
        OnTriggerBUG?.Invoke();
    }

    void ReleaserFinishCommand()
    {
        print("Command Finished");
        processCounter.currentFinishNumber++;
        //check if completed
        if (CheckManager.Instance.Complete())
        {
            if (processCounter.ReachEndLine())
            {
                print("Reach End Line");
                processType = ProcessType.FINISHED;
                OnReachEndLine?.Invoke();
                return;
            }
            else
            {
                processType = ProcessType.PAUSE;
                OnPause?.Invoke();
                return;
            }
        }

        if (processType == ProcessType.WARNING)
            return;

        if (processCounter.Finished())
            FinishLine();
    }

    void FinishLine()
    {
        print("Finished Once Command");
        if (processCounter.ReachEndLine())
        {
            print("Reach End Line in FinishLine");
            processType = ProcessType.FINISHED;
            OnReachEndLine?.Invoke();
        }
        else
        {
            processCounter.currentFinishNumber = 0;

            switch (processType)
            {
                case ProcessType.PLAY: ReadNewLine(); break;
                case ProcessType.STEP: processType = ProcessType.PAUSE; break;
            }
        }
    }

    void ReadNewLine()
    {
        processCounter.targetLineIndex = DeviceManager.Instance.MaxLineIndex;
        if(processCounter.targetLineIndex == -1)
        {
            OnReachEndLine?.Invoke();
            return;
        }
        StartCoroutine(RunOnce());
    }

    IEnumerator RunOnce()
    {
        yield return new WaitForSeconds(defaultTimer.spacingTime);
        processCounter.currentLineIndex++;
        // running new line
        processCounter.targetFinishNumber = DeviceManager.Instance.TargetFinishNumber;
        processCounter.currentFinishNumber = 0;

        OnRunNewLine?.Invoke(processCounter.currentLineIndex);
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
            recordCells[i].ResetCell();
        }
        recordCells.Clear();
    }

    public void PlayPause(bool isPlayCommand)
    {
        if (isPlayCommand)
        {
            switch (processType)
            {
                case ProcessType.EDIT:
                    Record();
                    ReadNewLine();
                    processType = ProcessType.PLAY;
                    break;
                case ProcessType.PAUSE:
                    ReadNewLine();
                    processType = ProcessType.PLAY;
                    break;
                case ProcessType.PLAY:
                    print("PlayBtn BUG, can press playBtn in play state");
                    break;
                case ProcessType.STEP:
                    processType = ProcessType.PLAY;
                    break;
                case ProcessType.WARNING:
                    print("PlayBtn BUG , can press playBtn in warning state");
                    break;
                case ProcessType.JUMP:
                    print("PlayBtn BUG, can press PlayBtn in jump state");
                    break;
            }
        }
        else
        {
            processType = ProcessType.PAUSE;
        }
    }

    public void Step()
    {
        switch (processType)
        {
            case ProcessType.EDIT:
                print(processCounter.targetFinishNumber);
                Record();
                ReadNewLine();
                processType = ProcessType.STEP;
                break;
            case ProcessType.PAUSE:
                ReadNewLine();
                processType = ProcessType.STEP;
                break;
            case ProcessType.PLAY:
                processType = ProcessType.STEP;
                break;
            case ProcessType.STEP:
                print("unvalid input , press step btn in step state");
                break;
            case ProcessType.WARNING:
                print("stepBtn BUG, press step btn in warning state");
                break;
            case ProcessType.JUMP:
                print("stepBtn BUG, press stepBtn in jump state");
                break;
        }
    }

    public void Stop()
    {
        StopAllCoroutines();

        processType = ProcessType.EDIT;
        processCounter.Clear();
        // read all infos
        Read();

        TooltipManager.Instance.HideWarning();
    }

    public void Jump()
    {
        switch (processType)
        {
            case ProcessType.EDIT:
                Record();
                ReadNewLine();
                processType = ProcessType.JUMP;
                break;
            case ProcessType.PAUSE:
                ReadNewLine();
                processType = ProcessType.JUMP;
                break;
            case ProcessType.PLAY:
                print("Switch to jump state in play");
                processType = ProcessType.JUMP;
                break;
            case ProcessType.STEP:
                print("Switch to jump state in step");
                processType = ProcessType.JUMP;
                break;
            case ProcessType.WARNING:
                print("JumpBtn BUG, can press jumpBtn in warning state");
                break;
        }
    }

    public void CompleteExit()
    {
        processCounter.Clear();
        processType = ProcessType.EDIT;
        recordCells.ForEach(cell => cell.ResetCell());
        recordCells.Clear();

        TooltipManager.Instance.HideWarning();
    }

    [Serializable]
    public struct ProcessCounter
    {
        public int targetFinishNumber;
        public int currentFinishNumber;
        public int currentLineIndex;
        public int targetLineIndex;

        public void Clear()
        {
            targetFinishNumber = 0;
            currentFinishNumber = 0;
            currentLineIndex = -1;
            targetLineIndex = 0;
        }
        public bool ReachEndLine() => currentLineIndex == targetLineIndex;
        public bool Finished() => currentFinishNumber == targetFinishNumber;
    }

    [Serializable]
    public struct RunTime
    {
        public float spacingTime;
        public float executeTime;
    }
}
