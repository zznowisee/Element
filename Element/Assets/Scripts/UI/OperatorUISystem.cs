using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEditor;

public class OperatorUISystem : MonoBehaviour
{
    public LevelDataSO levelData;
    public OperatorDataSO operatorData;
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

    [SerializeField] TextMeshProUGUI cycleNumText;
    [SerializeField] BrushBtn pfBrushBtn;
    [SerializeField] CommandBtn pfCommandBtn;
    [SerializeField] Command pfCommand;
    [SerializeField] CommandConsole pfCommandConsole;
    [SerializeField] CommandGhost commandGhost;

    public List<CommandSO> commandSOList;
    Dictionary<ICommandReader, CommandConsole> commandReaderConsoleDictionary;
    Dictionary<KeyCode, CommandSO> keyCodeCommandSODictionary;
    KeyCode[] keys;

    CommandSO trackingCommandSO;
    bool isPlayBtn = true;

    private void Awake()
    {
        Instance = this;
        commandReaderConsoleDictionary = new Dictionary<ICommandReader, CommandConsole>();
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

    void Update()
    {
        trackingCommandSO = null;
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
        if (!ProcessSystem.Instance.CanOperate())
        {
            ProcessSystem.Instance.Stop();
        }

        SwitchToMainScene();
    }

    private void ProcessSystem_OnPlayerFinishedLevel()
    {
        completePanel.gameObject.SetActive(true);
        levelData.completed = true;
    }

    public void OnSwitchToOperatorScene(LevelDataSO levelDataSO_, OperatorDataSO operatorDataSO_)
    {
        levelData = levelDataSO_;
        operatorData = operatorDataSO_;
        if (!operatorDataSO_.hasInitialized)
        {
            for (int i = 0; i < levelData.brushBtnDatas.Count; i++)
            {
                BrushBtnDataInit brushBtnDataInit = levelData.brushBtnDatas[i];
                BrushBtnDataSolution data = new BrushBtnDataSolution(brushBtnDataInit);
                
                BrushBtn brushBtn = Instantiate(pfBrushBtn, brushesPanel);
                brushBtn.Setup(data);
                operatorDataSO_.brushBtnDataSolutions.Add(data);
            }
            operatorDataSO_.hasInitialized = true;
        }
        else
        {
            for (int i = 0; i < operatorDataSO_.brushBtnDataSolutions.Count; i++)
            {
                BrushBtnDataSolution brushBtnDataSolution = operatorDataSO_.brushBtnDataSolutions[i];
                BrushBtn brushBtn = Instantiate(pfBrushBtn, brushesPanel);
                brushBtn.Setup(brushBtnDataSolution);

                for (int j = 0; j < brushBtnDataSolution.brushDatas.Count; j++)
                {
                    BuildSystem.Instance.InitBrushFromData(brushBtnDataSolution.brushDatas[j], brushBtn);
                }
            }
        }

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
        InputHelper.Save(operatorData, levelData);

        levelData = null;
        operatorData = null;
        OnSwitchToMainScene?.Invoke();
        gameObject.SetActive(false);
        mainUISystem.gameObject.SetActive(true);

        Camera.main.transform.position = new Vector3(0, 0, -10f);
    }

    void SwitchToPauseBtn()
    {
        isPlayBtn = false;
        playPauseBtn.transform.Find("text").GetComponent<TextMeshProUGUI>().text = "PAUSE";
    }

    void SwitchToPlayBtn()
    {
        isPlayBtn = true;
        playPauseBtn.transform.Find("text").GetComponent<TextMeshProUGUI>().text = "PLAY";
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

        for (int i = 0; i < consoleData.commandDatas.Length; i++)
        {
            if (consoleData.commandDatas[i] == null) continue;
            Command command = Instantiate(pfCommand, transform);
            command.Setup(consoleData.commandDatas[i]);
            command.DroppedOnSlot(console.slots[i]);
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

    public CommandSO GetEachReaderCommandSO(int lineIndex, ICommandReader commandReader)
    {
        return commandReaderConsoleDictionary[commandReader].GetCommandSOFromLineIndex(lineIndex);
    }
}
