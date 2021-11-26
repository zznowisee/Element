using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProcessSystem : MonoBehaviour
{
    bool isRunning;

    public enum ProcessType
    {
        EDIT,
        PLAY,
        PAUSE
    }

    [SerializeField] ProcessType processType;

    public event Action<int> OnReadNextCommandLine;
    public event Action OnFinishAllCommands;
    public static ProcessSystem Instance { get; private set; }

    [Header("Command SO")]
    [SerializeField] CommandSO splitSO;
    [SerializeField] CommandSO clockwiseRotateSO;
    [SerializeField] CommandSO counterClockwiseRotateSO;
    [SerializeField] CommandSO connectSO;
    [SerializeField] CommandSO goAheadSO;
    [SerializeField] CommandSO backSO;
    [SerializeField] CommandSO dropSO;
    [SerializeField] CommandSO pickSO;
    [SerializeField] CommandSO delaySO;

    Dictionary<CommandSO, Action<ICommandReaderObj>> commandDictionary;
    public List<Connecter> connecters;
    public List<ConnecterBrushSlot> connecterSlots;
    public List<Brush> brushes;
    public List<HexCell> colorCells;
    public float commandSpacingTime = .3f;
    public float commandDurationTime = .3f;

    int targetNum = 0;
    int currentNum;
    public int commandLineIndex = 0;
    public int commandLineMaxIndex = 0;

    UISystem uiSystem;
    void Awake()
    {
        Instance = this;
        commandDictionary = new Dictionary<CommandSO, Action<ICommandReaderObj>>();
        connecters = new List<Connecter>();
        connecterSlots = new List<ConnecterBrushSlot>();
        colorCells = new List<HexCell>();
        brushes = new List<Brush>();

        commandDictionary[splitSO] = Split;
        commandDictionary[clockwiseRotateSO] = ClockwiseRotate;
        commandDictionary[counterClockwiseRotateSO] = CounterClockwiseRotate;
        commandDictionary[connectSO] = Connect;
        commandDictionary[dropSO] = Drop;
        commandDictionary[pickSO] = Pick;
        commandDictionary[delaySO] = Delay;
        commandDictionary[goAheadSO] = GoAhead;
        commandDictionary[backSO] = Back;

        processType = ProcessType.EDIT;
        uiSystem = FindObjectOfType<UISystem>();

        isRunning = false;
    }

    void Start()
    {
        BuildSystem.Instance.OnCreateNewBrush += BuildSystem_OnCreateNewBrush;
        BuildSystem.Instance.OnDestoryBrush += BuildSystem_OnDestoryBrush;
        BuildSystem.Instance.OnCreateNewConnecter += BuildSystem_OnCreateNewConnecter;
        BuildSystem.Instance.OnDestoryConnecter += BuildSystem_OnDestoryConnecter;
        BuildSystem.Instance.OnCreateNewSlot += BuildSystem_OnCreateNewSlot;
        BuildSystem.Instance.OnDestorySlot += BuildSystem_OnDestorySlot;
    }

    private void BuildSystem_OnDestoryBrush(Brush brush)
    {
        brushes.Remove(brush);

    }

    private void BuildSystem_OnCreateNewBrush(Brush brush)
    {
        brushes.Add(brush);
    }

    private void BuildSystem_OnDestorySlot(ConnecterBrushSlot slot)
    {
        connecterSlots.Remove(slot);
        slot.OnFinishCommand -= OnReaderFinishCommand;
    }

    private void BuildSystem_OnCreateNewSlot(ConnecterBrushSlot slot)
    {
        connecterSlots.Add(slot);
        slot.OnFinishCommand += OnReaderFinishCommand;
    }

    private void BuildSystem_OnDestoryConnecter(Connecter connecter)
    {
        connecters.Remove(connecter);
        connecter.OnFinishCommand -= OnReaderFinishCommand;
    }

    private void BuildSystem_OnCreateNewConnecter(Connecter connecter)
    {
        connecters.Add(connecter);
        connecter.OnFinishCommand += OnReaderFinishCommand;
    }

    void Split(ICommandReaderObj reader) => reader.Split();
    void Connect(ICommandReaderObj reader) => reader.Connect();
    void ClockwiseRotate(ICommandReaderObj reader) => reader.ClockwiseRotate();
    void CounterClockwiseRotate(ICommandReaderObj reader) => reader.CounterClockwiseRotate();
    void Drop(ICommandReaderObj reader) => reader.Drop();
    void Pick(ICommandReaderObj reader) => reader.Pick();
    void Delay(ICommandReaderObj reader) => reader.Delay();
    void GoAhead(ICommandReaderObj reader) => reader.GoAhead();
    void Back(ICommandReaderObj reader) => reader.Back();

    public void OnReaderFinishCommand()
    {
        currentNum++;

        if (targetNum == currentNum)
        {
            switch (processType)
            {
                case ProcessType.PLAY:
                    StartCoroutine(Spacing(commandSpacingTime));
                    break;
                case ProcessType.PAUSE:
                    print("Pause");
                    break;
            }
        }
    }

    IEnumerator Spacing(float spacingTime)
    {
        yield return new WaitForSeconds(spacingTime);
        if (isRunning)
        {
            RunOnce();
        }
    }

    void RunOnce()
    {
        if (commandLineIndex <= commandLineMaxIndex)
        {
            OnReadNextCommandLine?.Invoke(commandLineIndex);
            targetNum = connecters.Count + connecterSlots.Count;
            currentNum = 0;
            for (int i = 0; i < connecters.Count; i++)
            {
                ICommandReaderObj readerObj = connecters[i].GetComponent<ICommandReaderObj>();
                CommandSO cmd = uiSystem.GetEachReaderCommandSO(commandLineIndex, readerObj);
                if (cmd != null)
                {
                    commandDictionary[cmd].Invoke(readerObj);
                }
                else
                {
                    readerObj.Delay();
                }
            }

            for (int i = 0; i < connecterSlots.Count; i++)
            {
                ICommandReaderObj readerObj = connecterSlots[i].GetComponent<ICommandReaderObj>();
                CommandSO cmd = uiSystem.GetEachReaderCommandSO(commandLineIndex, readerObj);
                if (cmd != null)
                {
                    commandDictionary[cmd].Invoke(readerObj);
                }
                else
                {
                    readerObj.Delay();
                }
            }

            commandLineIndex++;
        }
        else
        {
            print("Finished");
            processType = ProcessType.PAUSE;
            OnFinishAllCommands?.Invoke();
        }
    }

    void Record()
    {
        for (int i = 0; i < connecters.Count; i++)
        {
            connecters[i].Record();
        }
        for (int i = 0; i < connecterSlots.Count; i++)
        {
            connecterSlots[i].Record();
        }
        for (int i = 0; i < brushes.Count; i++)
        {
            brushes[i].Record();
        }
    }

    void Read()
    {
        for (int i = 0; i < connecters.Count; i++)
        {
            connecters[i].Read();
        }
        for (int i = 0; i < connecterSlots.Count; i++)
        {
            connecterSlots[i].Read();
        }
        for (int i = 0; i < brushes.Count; i++)
        {
            brushes[i].Read();
        }
    }

    public void PlayPause(bool isPlayCommand)
    {
        if (isPlayCommand)
        {
            if (!isRunning)
            {
                isRunning = true;
                Record();
            }
            commandLineMaxIndex = uiSystem.GetCommandLineMaxIndex();
            processType = ProcessType.PLAY;
            RunOnce();
        }
        else
        {
            processType = ProcessType.PAUSE;
        }
    }

    public void Step()
    {
        if (!isRunning)
        {
            isRunning = true;
            Record();
        }

        commandLineMaxIndex = uiSystem.GetCommandLineMaxIndex();
        processType = ProcessType.PAUSE;
        RunOnce();
    }

    void ClearColor()
    {
        for (int i = 0; i < colorCells.Count; i++)
        {
            HexCell cell = colorCells[i];
            cell.ClearColor();
        }

        colorCells.Clear();
    }

    public void Stop()
    {
        for (int i = 0; i < connecters.Count; i++)
        {
            connecters[i].StopAllCoroutines();
        }
        for (int i = 0; i < connecterSlots.Count; i++)
        {
            connecterSlots[i].StopAllCoroutines();
        }

        isRunning = false;
        commandLineIndex = 0;
        // read all infos
        Read();
        ClearColor();
        processType = ProcessType.EDIT;
    }
}
