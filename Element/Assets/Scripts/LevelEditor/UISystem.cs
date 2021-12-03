using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UISystem : MonoBehaviour
{
    public static UISystem Instance { get; private set; }
    [SerializeField] Color btnDisable;
    [SerializeField] Color btnEnable;
    [SerializeField] Transform colorPickerPanel;
    [SerializeField] Transform consolePanel;
    [SerializeField] Transform commandPanel;
    [SerializeField] ColorPickerBtn colorPickerBtn;
    [SerializeField] ColorPaletteSO colorPaletteSO;

    [SerializeField] Button stepBtn;
    [SerializeField] Button playPauseBtn;
    [SerializeField] Button stopBtn;
    [SerializeField] TextMeshProUGUI cycleNumText;
    [SerializeField] CommandBtn pfCommandBtn;
    [SerializeField] Command pfCommand;
    [SerializeField] CommandConsole pfCommandConsole;
    public List<CommandSO> commandSOList;
    Dictionary<ICommandReader, CommandConsole> commandReaderConsoleDictionary;

    public PointerEventData pointerData;
    bool isPlayBtn = true;
    bool finished = false;
    private void Awake()
    {
        Instance = this;
        commandReaderConsoleDictionary = new Dictionary<ICommandReader, CommandConsole>();
        for (int i = 0; i < colorPaletteSO.colors.Count; i++)
        {
            Color color = colorPaletteSO.colors[i].color;
            ColorPickerBtn colorPicker = Instantiate(colorPickerBtn, colorPickerPanel);
            colorPicker.GetComponent<Image>().color = color;
            colorPicker.Color = colorPaletteSO.colors[i];
        }

        for (int i = 0; i < commandSOList.Count; i++)
        {
            CommandBtn command = Instantiate(pfCommandBtn, commandPanel);
            command.Setup(commandSOList[i]);
        }

        stepBtn.onClick.AddListener(() =>
        {
            ProcessSystem.Instance.Step();

            EnableStopBtn();
        });

        playPauseBtn.onClick.AddListener(() =>
        {
            EnableStopBtn();
            if (isPlayBtn) SwitchToPauseBtn();
            else SWitchToPlayBtn();

            ProcessSystem.Instance.PlayPause(!isPlayBtn);
        });

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
            DisableStopBtn();
            playPauseBtn.gameObject.SetActive(true);

            if (finished)
            {
                finished = false;
                playPauseBtn.GetComponent<Image>().color = btnEnable;
                stepBtn.GetComponent<Image>().color = btnEnable;
                playPauseBtn.enabled = true;
                stepBtn.enabled = true;
            }
        });

        DisableStopBtn();
    }

    void Update()
    {

    }

    void SwitchToPauseBtn()
    {
        playPauseBtn.transform.Find("text").GetComponent<TextMeshProUGUI>().text = "PAUSE";
        isPlayBtn = false;
    }

    void SWitchToPlayBtn()
    {
        playPauseBtn.transform.Find("text").GetComponent<TextMeshProUGUI>().text = "PLAY";
        isPlayBtn = true;
    }

    void DisableStopBtn()
    {
        stopBtn.GetComponent<Image>().color = btnDisable;
        stopBtn.enabled = false;
        if (!isPlayBtn)
        {
            SWitchToPlayBtn();
        }
    }

    void EnableStopBtn()
    {
        stopBtn.GetComponent<Image>().color = btnEnable;
        stopBtn.enabled = true;
    }

    void Start()
    {
        BuildSystem.Instance.OnCreateNewBrush += BuildSystem_OnCreateNewBrush;
        BuildSystem.Instance.OnDestoryBrush += BuildSystem_OnDestoryBrush;
        //
        BuildSystem.Instance.OnCreateNewConnector += BuildSystem_OnCreateNewConnecter;
        BuildSystem.Instance.OnDestoryConnector += BuildSystem_OnDestoryConnecter;
        //
        BuildSystem.Instance.OnCreateNewController += BuildSystem_OnCreateNewController;
        BuildSystem.Instance.OnDestoryController += BuildSystem_OnDestoryController;
        // can only use two event : CommandReader
        ProcessSystem.Instance.OnReadNextCommandLine += ProcessSystem_OnEnterNextCycle;
        ProcessSystem.Instance.OnFinishAllCommands += ProcessSystem_OnFinishAllCommands;
    }

    private void BuildSystem_OnDestoryController(Controller controller)
    {
        Destroy(commandReaderConsoleDictionary[controller].gameObject);
        commandReaderConsoleDictionary.Remove(controller);
    }

    private void BuildSystem_OnCreateNewController(Controller controller)
    {
        CommandConsole connecterConsole = Instantiate(pfCommandConsole, consolePanel);
        connecterConsole.Setup(controller);
        connecterConsole.transform.SetSiblingIndex(connecterConsole.index);

        commandReaderConsoleDictionary[controller] = connecterConsole;
    }

    private void ProcessSystem_OnFinishAllCommands()
    {
        finished = true;
        playPauseBtn.GetComponent<Image>().color = btnDisable;
        if (!isPlayBtn)
        {
            SWitchToPlayBtn();
        }
        stepBtn.GetComponent<Image>().color = btnDisable;
        playPauseBtn.enabled = false;
        stepBtn.enabled = false;
    }

    private void BuildSystem_OnDestoryConnecter(Connector connecter) { }
    private void BuildSystem_OnCreateNewConnecter(Connector connecter) { }
    private void BuildSystem_OnDestoryBrush(Brush brush) { }
    private void BuildSystem_OnCreateNewBrush(Brush brush) { }
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
