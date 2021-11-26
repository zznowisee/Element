using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] CommandBtn pfCommandIcon;
    [SerializeField] CommandConsole pfCommandConsole;
    public List<CommandSO> commandSOList;
    Dictionary<ICommandReaderObj, CommandConsole> commandReaderConsoleDictionary;

    bool isPlayBtn = true;
    bool finished = false;
    private void Awake()
    {
        Instance = this;
        commandReaderConsoleDictionary = new Dictionary<ICommandReaderObj, CommandConsole>();
        for (int i = 0; i < colorPaletteSO.colors.Count; i++)
        {
            Color color = colorPaletteSO.colors[i].color;
            ColorPickerBtn colorPicker = Instantiate(colorPickerBtn, colorPickerPanel);
            colorPicker.GetComponent<Image>().color = color;
            colorPicker.Color = colorPaletteSO.colors[i];
        }

        for (int i = 0; i < commandSOList.Count; i++)
        {
            CommandBtn command = Instantiate(pfCommandIcon, commandPanel);
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
        BuildSystem.Instance.OnCreateNewConnecter += BuildSystem_OnCreateNewConnecter;
        BuildSystem.Instance.OnDestoryConnecter += BuildSystem_OnDestoryConnecter;
        BuildSystem.Instance.OnCreateNewSlot += BuildSystem_OnCreateNewSlot;
        BuildSystem.Instance.OnDestorySlot += BuildSystem_OnDestorySlot;
        // can only use two event : CommandReader
        ProcessSystem.Instance.OnReadNextCommandLine += ProcessSystem_OnEnterNextCycle;
        ProcessSystem.Instance.OnFinishAllCommands += ProcessSystem_OnFinishAllCommands;
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

    private void BuildSystem_OnDestorySlot(ConnecterBrushSlot slot)
    {
        if (commandReaderConsoleDictionary.ContainsKey(slot))
        {
            Destroy(commandReaderConsoleDictionary[slot].gameObject);
            commandReaderConsoleDictionary.Remove(slot);
        }
    }

    private void BuildSystem_OnCreateNewSlot(ConnecterBrushSlot slot)
    {
        CommandConsole connecterConsole = Instantiate(pfCommandConsole, consolePanel);
        connecterConsole.Setup(slot);
        connecterConsole.transform.SetSiblingIndex(connecterConsole.index);

        commandReaderConsoleDictionary[slot] = connecterConsole;
    }

    private void BuildSystem_OnDestoryConnecter(Connecter connecter)
    {
        Destroy(commandReaderConsoleDictionary[connecter].gameObject);
        commandReaderConsoleDictionary.Remove(connecter);
    }

    private void BuildSystem_OnCreateNewConnecter(Connecter connecter)
    {
        CommandConsole connecterConsole = Instantiate(pfCommandConsole, consolePanel);
        connecterConsole.Setup(connecter);
        connecterConsole.transform.SetSiblingIndex(connecterConsole.index);

        commandReaderConsoleDictionary[connecter] = connecterConsole;
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

    private void BuildSystem_OnDestoryBrush(Brush brush) { }

    private void BuildSystem_OnCreateNewBrush(Brush brush) { }

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

    public CommandSO GetEachReaderCommandSO(int lineIndex, ICommandReaderObj commandReader)
    {
        return commandReaderConsoleDictionary[commandReader].GetCommandSOFromLineIndex(lineIndex);
    }
}
