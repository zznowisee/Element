using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class OperatorUISystem : MonoBehaviour
{
    public LevelBuildDataSO levelBuildDataSO;
    public SolutionData solutionData;
    public event Action OnSwitchToMainScene;
    public static OperatorUISystem Instance { get; private set; }

    [SerializeField] MainUISystem mainUISystem;
    [SerializeField] Color btnDisable;
    [SerializeField] Color btnEnable;
    [SerializeField] Transform consolePanel;
    [SerializeField] Transform commandPanel;
    [SerializeField] Transform brushesPanel;
    [SerializeField] Transform completePanel;

    [SerializeField] Button stepBtn;
    [SerializeField] Button playPauseBtn;
    [SerializeField] Button stopBtn;
    [SerializeField] Button quitBtn;
    [SerializeField] Button completeContinueBtn;
    [SerializeField] Button completeExitBtn;
    [SerializeField] TextMeshProUGUI completePanelCycleNumText;

    [SerializeField] ControllerBtn controllerBtn;

    [SerializeField] TextMeshProUGUI cycleNumText;
    [SerializeField] BrushBtn pfBrushBtn;
    [SerializeField] CommandBtn pfCommandBtn;
    [SerializeField] Command pfCommand;
    [SerializeField] CommandConsole pfCommandConsole;
    [SerializeField] CommandGhost commandGhost;

    [SerializeField] CommandSO putup;
    [SerializeField] CommandSO putdown;
    [SerializeField] CommandSO connect;
    [SerializeField] CommandSO split;
    [SerializeField] CommandSO connectorCCR;
    [SerializeField] CommandSO connectorCR;
    [SerializeField] CommandSO controllerCCR;
    [SerializeField] CommandSO controllerCR;
    [SerializeField] CommandSO delay;
    [SerializeField] CommandSO push;
    [SerializeField] CommandSO pull;

    [SerializeField] ColorSO yellow;
    [SerializeField] ColorSO blue;
    [SerializeField] ColorSO red;
    [SerializeField] ColorSO white;

    public List<CommandSO> commandSOList;
    Dictionary<ICommandReleaser, CommandConsole> commandReaderConsoleDictionary;
    Dictionary<KeyCode, CommandSO> keyCodeCommandSODictionary;
    KeyCode[] keys;

    CommandSO trackingCommandSO;
    bool isPlayBtn = true;
    Command currentTrackingCommand;

    public void SetCurrentTrackingCommand(Command command_)
    {
        currentTrackingCommand = command_;
    }

    private void Awake()
    {
        Instance = this;
        commandReaderConsoleDictionary = new Dictionary<ICommandReleaser, CommandConsole>();
        keyCodeCommandSODictionary = new Dictionary<KeyCode, CommandSO>();
        keys = new KeyCode[commandSOList.Count];
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i] = commandSOList[i].key;
        }

        for (int i = 0; i < commandSOList.Count; i++)
        {
            CommandSO commandSO = commandSOList[i];
            CommandBtn command = Instantiate(pfCommandBtn, commandPanel);
            command.Setup(commandSO);
            TooltipTrigger tooltipTrigger = command.GetComponent<TooltipTrigger>();
            tooltipTrigger.header = commandSO.name;
            tooltipTrigger.content = commandSO.description;

            keyCodeCommandSODictionary[commandSO.key] = commandSO;
        }

        // buttons

        //step
        stepBtn.onClick.AddListener(() =>
        {
            ProcessSystem.Instance.Step();
            if (!isPlayBtn)
            {
                SwitchToPlayBtn();
            }
            ButtonEnable(stopBtn);
        });

        //play pause
        playPauseBtn.onClick.AddListener(() =>
        {
            ButtonEnable(stopBtn);
            if (isPlayBtn) SwitchToPauseBtn();
            else SwitchToPlayBtn();

            ProcessSystem.Instance.PlayPause(!isPlayBtn);
        });

        //stop
        stopBtn.onClick.AddListener(() =>
        {
            foreach (CommandConsole console in commandReaderConsoleDictionary.Values)
            {
                if (ProcessSystem.Instance.commandLineIndex > 0)
                {
                    console.slots[ProcessSystem.Instance.commandLineIndex - 1].SetNormal();
                }
            }
            cycleNumText.text = "0";
            ProcessSystem.Instance.Stop();

            ButtonDisable(stopBtn);
            ButtonEnable(playPauseBtn);
            ButtonEnable(stepBtn);
            if (!isPlayBtn)
            {
                SwitchToPlayBtn();
            }
        });

        //quit
        quitBtn.onClick.AddListener(() =>
        {
            Quit();
        });

        //complete continue
        completeContinueBtn.onClick.AddListener(() =>
        {
            completePanel.gameObject.SetActive(false);
            SwitchToPlayBtn();
        });

        //complete exit
        completeExitBtn.onClick.AddListener(() =>
        {
            completePanel.gameObject.SetActive(false);
            ProcessSystem.Instance.OnFinishedExit();
            SwitchToMainScene();
        });
    }

    void Start()
    {
        BuildSystem.Instance.OnCreateNewController += BuildSystem_OnCreateNewController;
        BuildSystem.Instance.OnDestoryController += BuildSystem_OnDestoryController;
        ProcessSystem.Instance.OnReadNextCommandLine += ProcessSystem_OnEnterNextCycle;
        ProcessSystem.Instance.OnFinishAllCommandsOrWarning += ProcessSystem_OnFinishAllCommands;
        ProcessSystem.Instance.OnLevelComplete += ProcessSystem_OnPlayerFinishedLevel;
    }

    public ControllerBtn GetControllerBtn()
    {
        return controllerBtn;
    }

    void Update()
    {
        trackingCommandSO = null;

        if(currentTrackingCommand != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                currentTrackingCommand.TryToDropOnSlot();
                currentTrackingCommand = null;
                return;
            }
            currentTrackingCommand.transform.position = InputHelper.MouseWorldPositionIn2D;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
            return;
        }

        KeyCode key = InputHelper.HeldKeysInThese(keys);
        if (keyCodeCommandSODictionary.ContainsKey(key))
        {
            trackingCommandSO = keyCodeCommandSODictionary[key];
        }
        else
        {
            commandGhost.gameObject.SetActive(false);
        }

        if (trackingCommandSO != null)
        {
            CommandSlot slot = InputHelper.GetCommandSlotUnderPosition2D();
            if (slot != null)
            {
                commandGhost.Setup(trackingCommandSO);
                commandGhost.transform.position = slot.transform.position;
                commandGhost.gameObject.SetActive(true);

                if (Input.GetMouseButtonDown(0))
                {
                    Command command = Instantiate(pfCommand, transform);
                    command.Setup(trackingCommandSO);
                    command.DroppedOnSlot(slot);
                }
            }
            else
            {
                commandGhost.gameObject.SetActive(false);
            }
        }
    }

    private void ProcessSystem_OnFinishAllCommands()
    {
        if (!isPlayBtn)
        {
            SwitchToPlayBtn();
        }

        ButtonDisable(playPauseBtn);
        ButtonDisable(stepBtn);
        ButtonEnable(stopBtn);
    }

    void Quit()
    {
        completePanel.gameObject.SetActive(false);
        if (!ProcessSystem.Instance.CanOperate())
        {
            ProcessSystem.Instance.Stop();
        }

        SwitchToMainScene();
    }

    private void ProcessSystem_OnPlayerFinishedLevel(LevelData levelData)
    {
        completePanel.gameObject.SetActive(true);
        completePanelCycleNumText.text = $"周期数:{ProcessSystem.Instance.commandLineIndex}";
    }

    public void OnLoadSolution(LevelBuildDataSO levelBuildDataSO_, LevelData levelData_, SolutionData solutionData_)
    {
        levelBuildDataSO = levelBuildDataSO_;
        solutionData = solutionData_;

        if (!solutionData_.hasInitialized)
        {
            for (int i = 0; i < levelBuildDataSO.brushBtnDatas.Count; i++)
            {
                print(levelBuildDataSO_.name);
                BrushBtnInitData brushBtnDataInit = levelBuildDataSO.brushBtnDatas[i];
                BrushBtnSolutionData data = new BrushBtnSolutionData(brushBtnDataInit);
                print(data.colorType);
                BrushBtn brushBtn = Instantiate(pfBrushBtn, brushesPanel);
                brushBtn.Setup(data, brushBtnDataInit.colorSO);
                solutionData_.brushBtnDataSolutions.Add(data);
            }
            solutionData_.hasInitialized = true;
        }
        else
        {
            for (int i = 0; i < solutionData_.brushBtnDataSolutions.Count; i++)
            {
                BrushBtnSolutionData brushBtnSolutionData = solutionData_.brushBtnDataSolutions[i];
                BrushBtn brushBtn = Instantiate(pfBrushBtn, brushesPanel);
                ColorSO colorSO;
                switch (brushBtnSolutionData.colorType)
                {
                    case ColorType.Blue:
                        colorSO = blue;
                        break;
                    case ColorType.Red:
                        colorSO = red;
                        break;
                    case ColorType.White:
                        colorSO = white;
                        break;
                    case ColorType.Yellow:
                        colorSO = yellow;
                        break;
                    default:
                        colorSO = white;
                        break;
                }
                brushBtn.Setup(brushBtnSolutionData, colorSO);
                for (int j = 0; j < brushBtnSolutionData.brushDatas.Count; j++)
                {
                    BuildSystem.Instance.InitBrushFromData(brushBtnSolutionData.brushDatas[j], brushBtn);
                }
            }
        }
        controllerBtn.Setup(solutionData_);
        ButtonEnable(playPauseBtn);
        ButtonEnable(stepBtn);
        ButtonDisable(stopBtn);
        if (!isPlayBtn)
        {
            SwitchToPlayBtn();
        }
    }

    void SwitchToMainScene()
    {
        cycleNumText.text = "0";
        foreach (CommandConsole console in commandReaderConsoleDictionary.Values)
        {
            Destroy(console.gameObject);
        }

        for (int i = 0; i < brushesPanel.childCount; i++)
        {
            Destroy(brushesPanel.GetChild(i).gameObject);
        }

        commandReaderConsoleDictionary.Clear();
        //save datas
        mainUISystem.SaveGameData();
        levelBuildDataSO = null;
        solutionData = null;
        OnSwitchToMainScene?.Invoke();
        gameObject.SetActive(false);
        mainUISystem.gameObject.SetActive(true);

        Camera.main.transform.position = new Vector3(0, 0, -10f);
    }

    void SwitchToPauseBtn()
    {
        isPlayBtn = false;
        playPauseBtn.transform.Find("text").GetComponent<TextMeshProUGUI>().text = "暂停";
    }

    void SwitchToPlayBtn()
    {
        isPlayBtn = true;
        playPauseBtn.transform.Find("text").GetComponent<TextMeshProUGUI>().text = "运行";
    }

    void ButtonEnable(Button btn)
    {
        btn.enabled = true;
        btn.GetComponent<Image>().color = btnEnable;
    }

    void ButtonDisable(Button btn)
    {
        btn.enabled = false;
        btn.GetComponent<Image>().color = btnDisable;
    }

    private void BuildSystem_OnDestoryController(Controller controller)
    {
        Destroy(commandReaderConsoleDictionary[controller].gameObject);
        commandReaderConsoleDictionary.Remove(controller);
    }

    private void BuildSystem_OnCreateNewController(Controller controller)
    {
        CommandConsole console = Instantiate(pfCommandConsole, consolePanel);
        console.Setup(controller);
        console.transform.SetSiblingIndex(console.index - 1);

        commandReaderConsoleDictionary[controller] = console;
    }

    public void InitConsole(Controller controller, ConsoleData consoleData)
    {
        CommandConsole console = Instantiate(pfCommandConsole, consolePanel);
        console.transform.SetSiblingIndex(console.index - 1);

        console.Setup(controller, consoleData);
        commandReaderConsoleDictionary[controller] = console;
        console.consoleData = consoleData;
        CommandSO commandSO;
        for (int i = 0; i < consoleData.commandTypes.Length; i++)
        {
            switch (consoleData.commandTypes[i])
            {
                case CommandType.Connect:
                    commandSO = connect;
                    break;
                case CommandType.Split:
                    commandSO = split;
                    break;
                case CommandType.ConnectorCCW:
                    commandSO = connectorCCR;
                    break;
                case CommandType.ConnectorCW:
                    commandSO = connectorCR;
                    break;
                case CommandType.ControllerCCR:
                    commandSO = controllerCCR;
                    break;
                case CommandType.ControllerCR:
                    commandSO = controllerCR;
                    break;
                case CommandType.Delay:
                    commandSO = delay;
                    break;
                case CommandType.Empty:
                    commandSO = null;
                    break;
                case CommandType.Pull:
                    commandSO = pull;
                    break;
                case CommandType.Push:
                    commandSO = push;
                    break;
                case CommandType.PutDown:
                    commandSO = putdown;
                    break;
                case CommandType.PutUp:
                    commandSO = putup;
                    break;
                default:
                    commandSO = null;
                    break;
            }
            if(commandSO != null)
            {
                Command command = Instantiate(pfCommand, transform);
                command.Setup(commandSO);
                command.DroppedOnSlot(console.slots[i]);
            }
        }
    }

    private void ProcessSystem_OnEnterNextCycle(int commandLineIndex)
    {
        foreach (CommandConsole console in commandReaderConsoleDictionary.Values)
        {
            if (commandLineIndex > 0)
            {
                console.slots[commandLineIndex - 1].SetNormal();
            }
            console.slots[commandLineIndex].SetHighlight();
        }
        cycleNumText.text = (commandLineIndex + 1).ToString();
    }

    public int GetCommandLineMaxIndex()
    {
        int maxIndex = 0;
        foreach(CommandConsole console in commandReaderConsoleDictionary.Values)
        {
            int index = console.GetLastCommandIndex();
            if (index >= maxIndex)
            {
                maxIndex = index;
            }
        }
        return maxIndex;
    }

    public CommandSO GetEachReaderCommandSO(int lineIndex, ICommandReleaser releaser)
    {
        return commandReaderConsoleDictionary[releaser].GetCommandSOFromLineIndex(lineIndex);
    }
}
