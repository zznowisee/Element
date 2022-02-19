using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class OperatingRoomUI : MonoBehaviour
{

    public event Action OnSwitchToMainScene;
    public event Action OnCompleteExit;
    public event Action OnStop;

    public static OperatingRoomUI Instance { get; private set; }
    [SerializeField] MainUI mainUISystem;
    [SerializeField] Color btnDisable;
    [SerializeField] Color btnEnable;
    [Header("引用")]
    [SerializeField] HexMap map;
    [Header("父级")]
    [SerializeField] Transform commandPanel;
    [SerializeField] Transform brushesPanel;
    [SerializeField] Transform completePanel;
    [SerializeField] Transform ui;
    [Header("按钮")]
    [SerializeField] Button stepBtn;
    [SerializeField] Button playPauseBtn;
    [SerializeField] Button stopBtn;
    [SerializeField] Button quitBtn;
    [SerializeField] Button jumpBtn;
    [SerializeField] Button completeContinueBtn;
    [SerializeField] Button completeExitBtn;
    [SerializeField] TextMeshProUGUI completePanelCycleNumText;
    [SerializeField] TextMeshProUGUI developerCycleNumText;

    [SerializeField] ControllerBtn controllerBtn;

    [SerializeField] TextMeshProUGUI cycleNumText;
    [Header("预制体")]
    [SerializeField] BrushBtn pfBrushBtn;
    [SerializeField] CommandBtn pfCommandBtn;

    public List<CommandSO> commandSOList;
    KeyCode[] keys;

    bool isPlayBtn = true;

    void Awake()
    {
        Instance = this;
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
        }

        // buttons

        //step
        stepBtn.onClick.AddListener(() =>
        {
            ProcessManager.Instance.Step();
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

            ProcessManager.Instance.PlayPause(!isPlayBtn);
        });

        //stop
        stopBtn.onClick.AddListener(() =>
        {
            OnStop?.Invoke();

            cycleNumText.text = "0";
            ButtonDisable(stopBtn);
            ButtonEnable(playPauseBtn);
            ButtonEnable(stepBtn);
            if(BreakPointConsole.GetBreakPointIndex() != -1)
            {
                ButtonEnable(jumpBtn);
            }
            if (!isPlayBtn)
            {
                SwitchToPlayBtn();
            }
        });

        //jump
        jumpBtn.onClick.AddListener(() =>
        {
            ProcessManager.Instance.Jump();
            ButtonEnable(stopBtn);
            if (!isPlayBtn) SwitchToPlayBtn();
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
            OnCompleteExit?.Invoke();
            completePanel.gameObject.SetActive(false);
            SwitchToMainScene();
        });
    }

    void Start()
    {
        ProcessManager.Instance.OnRunNewLine += ProcessSystem_OnEnterNextCycle;

        ProcessManager.Instance.OnReachEndLine += OnInitializedOperatorUI;
        ProcessManager.Instance.OnTriggerBUG += OnInitializedOperatorUI;

        ProcessManager.Instance.OnLevelComplete += FinishedLevel;
        ProcessManager.Instance.OnReachBreakPoint += ReachBreakPoint;
        BreakPointConsole.OnSettingBreakPoint += BreakPointConsole_OnSettingBreakPoint;
        BreakPointConsole.OnCancelSettingBreakPoint += BreakPointConsole_OnCancelSettingBreakPoint;

        DisableUI();
    }

    public void EnableUI() => ui.gameObject.SetActive(true);
    public void DisableUI() => ui.gameObject.SetActive(false);

    private void ReachBreakPoint()
    {
        ButtonDisable(jumpBtn);
        if (!isPlayBtn) SwitchToPlayBtn();
    }

    private void BreakPointConsole_OnCancelSettingBreakPoint()
    {
        ButtonDisable(jumpBtn);
    }

    private void BreakPointConsole_OnSettingBreakPoint()
    {
        ButtonEnable(jumpBtn);
    }

    public ControllerBtn GetControllerBtn()
    {
        return controllerBtn;
    }

    void OnInitializedOperatorUI()
    {
        if (!isPlayBtn)
        {
            SwitchToPlayBtn();
        }

        ButtonDisable(playPauseBtn);
        ButtonDisable(stepBtn);
        ButtonEnable(stopBtn);
        ButtonDisable(jumpBtn);
    }

    void Quit()
    {
        completePanel.gameObject.SetActive(false);
        if (!ProcessManager.Instance.CanOperate())
        {
            ProcessManager.Instance.Stop();
        }

        SwitchToMainScene();
    }

    private void FinishedLevel()
    {
        completePanel.gameObject.SetActive(true);
        completePanelCycleNumText.text = $"周期数:{ProcessManager.Instance.LineIndex}";
    }

    public void SwitchToOperatorScene(LevelBuildDataSO levelBuildDataSO_, LevelData levelData_, SolutionData solutionData_)
    {
        developerCycleNumText.text = $"开发者周期数:{levelBuildDataSO_.developerCycle}";
        BrushBtn[] brushBtnArray = new BrushBtn[solutionData_.brushBtnDatas.Count];
        for (int i = 0; i < solutionData_.brushBtnDatas.Count; i++)
        {
            var brushBtnData = solutionData_.brushBtnDatas[i];
            var brushBtn = Instantiate(pfBrushBtn, brushesPanel);
            ColorSO colorSO = ColorManager.Instance.GetColorSOFromColorType(brushBtnData.colorType);
            brushBtn.Setup(brushBtnData, colorSO);
            brushBtnArray[i] = brushBtn;
        }

        for (int i = 0; i < solutionData_.brushDatas.Count; i++)
        {
            BrushData data = solutionData_.brushDatas[i];
            BrushType brushType = data.brushType;
            ColorType colorType = data.colorType;
            print("colorType : " + colorType);
            print("brushType : " + brushType);
            var brush = DeviceManager.Instance.RebuildBrush(brushType);
            HexCell cell = map.GetCellFromIndex(data.cellIndex);
            brush.Rebuild(cell, data);
            foreach(var brushBtn in brushBtnArray)
            {
                if(brushBtn.data.colorType == colorType && brushBtn.data.brushType == brushType)
                {
                    //print("colorType : " + brushBtn.data.colorType);
                    //print("brushType : " + brushBtn.data.brushType);
                    brush.OnDestoryByPlayer += brushBtn.BrushDestory;
                    break;
                }
            }
        }

        ButtonEnable(playPauseBtn);
        ButtonEnable(stepBtn);
        ButtonDisable(stopBtn);
        ButtonDisable(jumpBtn);
        if (!isPlayBtn)
        {
            SwitchToPlayBtn();
        }
    }

    void SwitchToMainScene()
    {
        OnSwitchToMainScene?.Invoke();
        MainUI.Instance.EnableUI();
        DisableUI();

        cycleNumText.text = "0";

        for (int i = 0; i < brushesPanel.childCount; i++)
        {
            Destroy(brushesPanel.GetChild(i).gameObject);
        }
        //save datas
        mainUISystem.SaveGameData();

        Camera.main.transform.position = new Vector3(0, 0, -10f);
        GameManager.Instance.GameState = GameState.Main;
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

    private void ProcessSystem_OnEnterNextCycle(int commandLineIndex)
    {
        cycleNumText.text = (commandLineIndex + 1).ToString();
    }

    public void Esc()
    {
        Quit();
    }
}
