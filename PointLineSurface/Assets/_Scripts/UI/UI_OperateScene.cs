using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_OperateScene : MonoBehaviour
{

    public event Action OnExitOperateScene;
    public event Action OnCompleteExit;
    public event Action OnStop;

    public static UI_OperateScene Instance { get; private set; }
    [Header("父级")]
    [SerializeField] Transform commandPanel;
    [SerializeField] Transform brushesPanel;
    [SerializeField] Transform completePanel;
    [SerializeField] Transform ui;
    [Header("按钮")]
    [SerializeField] PlayPauseBtn playPauseBtn;
    [SerializeField] StepBtn stepBtn;
    [SerializeField] StopBtn stopBtn;
    [SerializeField] Button quitBtn;
    [SerializeField] Button completeContinueBtn;
    [SerializeField] Button completeExitBtn;
    [SerializeField] TextMeshProUGUI completePanelCycleNumText;

    [SerializeField] ControllerBtn controllerBtn;
    [Header("预制体")]
    [SerializeField] BrushBtn pfBrushBtn;
    [SerializeField] CommandBtn pfCommandBtn;

    public CommandSO[] commandSOArray;

    public void UpdateControllerBtnNumber(Device device)
    {
        controllerBtn.OnControllerDestory();
    }

    void InitAllCommands()
    {
        for (int i = 0; i < commandSOArray.Length; i++)
        {
            CommandSO commandSO = commandSOArray[i];
            CommandBtn command = Instantiate(pfCommandBtn, commandPanel);
            command.Setup(commandSO);
            TooltipTrigger tooltipTrigger = command.GetComponent<TooltipTrigger>();
            tooltipTrigger.header = commandSO.name;
            tooltipTrigger.content = commandSO.description;
        }
    }

    void Awake()
    {
        Instance = this;
        InitAllCommands();
        BindButtonEvent();
    }

    #region _Button Event
    void BindButtonEvent()
    {
        //playPauseBtn.onClick.AddListener(PlayPause);
        stopBtn.SetButtonCallback(Stop);
        stepBtn.SetButtonCallback(Step);
        playPauseBtn.SetButtonCallback(PlayPause);
        quitBtn.onClick.AddListener(Quit);
        completeContinueBtn.onClick.AddListener(CompleteContinue);
        completeExitBtn.onClick.AddListener(CompleteExit);
    }
    void CompleteContinue()
    {
        completePanel.gameObject.SetActive(false);
        playPauseBtn.SetPlay();
    }

    void CompleteExit()
    {
        OnCompleteExit?.Invoke();
        completePanel.gameObject.SetActive(false);
        EnterMainScene();
    }

    void Step()
    {
        ProcessManager.Instance.Step();
        playPauseBtn.SetPlay();
        stopBtn.Enable();
    }

    void PlayPause()
    {
        if (playPauseBtn.IsPlay) ProcessManager.Instance.Play();
        else ProcessManager.Instance.Pause();

        stopBtn.Enable();
    }

    void Jump()
    {
        ProcessManager.Instance.Jump();
        stopBtn.Enable();
        playPauseBtn.SetPlay();
    }

    void Stop()
    {
        stopBtn.Disable();
        playPauseBtn.SetPlay();
        playPauseBtn.Enable();
        stepBtn.Enable();
        ProcessManager.Instance.Stop();
    }

    void Quit()
    {
        completePanel.gameObject.SetActive(false);
        if (!ProcessManager.Instance.CanOperate())
        {
            ProcessManager.Instance.Stop();
        }

        EnterMainScene();
    }

    #endregion
    void Start()
    {
        ProcessManager.Instance.OnRunNewLine += ProcessSystem_OnEnterNextCycle;

        ProcessManager.Instance.OnReachEndLine += ReachEndLine;
        UI_WarningManager.Instance.OnWarning += OnInitializedOperatorUI;

        ProcessManager.Instance.OnLevelComplete += FinishedLevel;
        ProcessManager.Instance.OnReachBreakPoint += ReachBreakPoint;

        stopBtn.Disable();
        DisableUI();
    }

    public void EnableUI() => ui.gameObject.SetActive(true);
    public void DisableUI() => ui.gameObject.SetActive(false);

    private void ReachBreakPoint()
    {
        //jumpBtn.interactable = false;
        playPauseBtn.SetPlay();
    }

    void OnInitializedOperatorUI()
    {
        playPauseBtn.SetPlay();
        playPauseBtn.Disable();

        stepBtn.Disable();
        stopBtn.Enable();
        //jumpBtn.interactable = false;
    }

    void ReachEndLine()
    {
        stopBtn.Enable();
        stepBtn.Disable();
        playPauseBtn.Disable();
    }

    private void FinishedLevel()
    {
        completePanel.gameObject.SetActive(true);
        completePanelCycleNumText.text = $"周期数:{ProcessManager.Instance.LineIndex}";
    }

    public void BuildToolsUI(Data_Solution solutionData_)
    {
        //developerCycleNumText.text = $"开发者周期数:{levelBuildDataSO_.developerCycle}";
        BrushBtn[] brushBtnArray = new BrushBtn[solutionData_.levelBuildData.BrushBtnDatas.Count];
        for (int i = 0; i < solutionData_.levelBuildData.BrushBtnDatas.Count; i++)
        {
            var brushBtnData = solutionData_.levelBuildData.BrushBtnDatas[i];
            var brushBtn = Instantiate(pfBrushBtn, brushesPanel);
            Color color = ColorManager.Instance.GetColor(brushBtnData.colorType, false);
            brushBtn.Setup(brushBtnData, color);
            brushBtnArray[i] = brushBtn;
        }

        for (int i = 0; i < solutionData_.BrushDatas.Count; i++)
        {
            Data_Brush data = solutionData_.BrushDatas[i];
            BrushType brushType = data.brushType;
            ColorType colorType = data.colorType;
            var brush = DeviceManager.Instance.RebuildBrush(brushType);
            brush.Rebuild(MapGenerator.Instance.GetHexCell(data.index.x, data.index.y), data);
            foreach(var brushBtn in brushBtnArray)
            {
                if(brushBtn.data.colorType == colorType && brushBtn.data.brushType == brushType)
                {
                    brush.OnDestory += brushBtn.BrushDestory;
                    break;
                }
            }
        }

        controllerBtn.Init(solutionData_);

        playPauseBtn.Enable();
        stepBtn.Enable();
        stopBtn.Disable();
        //jumpBtn.Disable();
        playPauseBtn.SetPlay();
    }

    void EnterMainScene()
    {
        GameManager.Instance.GameState = GameState.MainScene;

        OnExitOperateScene?.Invoke();

        DisableUI();
        UI_MainScene.Instance.EnableUI();
        MapGenerator.Instance.DisableMap();
        //cycleNumText.text = "0";

        for (int i = 0; i < brushesPanel.childCount; i++)
        {
            Destroy(brushesPanel.GetChild(i).gameObject);
        }

        DataManager.SaveGameData();

        Camera.main.transform.position = new Vector3(0, 0, -10f);
    }

    private void ProcessSystem_OnEnterNextCycle(int commandLineIndex)
    {
        //cycleNumText.text = (commandLineIndex + 1).ToString();
    }
}
