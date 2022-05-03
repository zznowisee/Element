using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MainUI : MonoBehaviour
{
    GameData gameData;

    public static MainUI Instance { get; private set; }

    public event Action<LevelBuildDataSO, LevelData, SolutionData> OnSwitchToOperatingRoom;
    
    [Header("预制体")]
    [SerializeField] LevelSelectBtn pfLevelSelectBtn;
    [SerializeField] SolutionPage pfSolutionPage;
    [Header("按钮")]
    [SerializeField] Button quitBtn;
    [Header("父级")]
    [SerializeField] Transform levelSelectionPanel;
    [SerializeField] Transform solutionPagePanel;
    [SerializeField] Transform ui;

    SolutionBtn selectingSolution;
    SolutionPage selectingSolutionPage;
    LevelSelectBtn selectingLevelBtn;

    public List<LevelBuildDataSO> levelBuildDatas;
    void Awake()
    {
        Instance = this;

        string path = Application.persistentDataPath + "/saves/" + "GameData" + ".save";
        gameData = (GameData)SerializationHelper.Load(path);

        if (gameData == null)
        {
            gameData = new GameData();
            for (int i = 0; i < levelBuildDatas.Count; i++)
            {
                gameData.LevelDatas.Add(new LevelData(levelBuildDatas[i].name, levelBuildDatas[i].levelIndex));
            }
            SaveGameData();
        }

        quitBtn.onClick.AddListener(() =>
        {
            SaveGameData();
            Application.Quit();
        });

        for (int i = 0; i < levelBuildDatas.Count; i++)
        {
            LevelBuildDataSO levelBuildDataSO = levelBuildDatas[i];
            LevelData levelData = gameData.LevelDatas[i];

            LevelSelectBtn levelSelectBtn = Instantiate(pfLevelSelectBtn, levelSelectionPanel);
            SolutionPage solutionPage = Instantiate(pfSolutionPage, solutionPagePanel);

            levelSelectBtn.Setup(levelData, solutionPage, levelSelectionPanel.gameObject);
            solutionPage.Setup(levelBuildDatas[i], levelData, levelSelectionPanel.gameObject);
            solutionPage.gameObject.SetActive(false);
        }

        OnSwitchToOperatingRoom += OperatingRoomUI.Instance.SwitchToOperatorScene;
    }
    void Start()
    {
        ProcessManager.Instance.OnLevelComplete += LevelComplete;
    }

    public void SaveGameData()
    {
        string saveName = "GameData";
        SerializationHelper.Save(saveName, gameData);
    }

    private void LevelComplete()
    {
        selectingSolution.SetComplete();
        selectingLevelBtn.SetComplete();
    }

    public void SwitchToOperatorScene(LevelBuildDataSO levelBuildDataSO_, LevelData levelData, SolutionBtn currentSolution_)
    {
        GameManager.Instance.GameState = GameState.OperatingRoom;

        OperatingRoomUI.Instance.EnableUI();
        DisableUI();

        selectingSolution = currentSolution_;
        OnSwitchToOperatingRoom?.Invoke(levelBuildDataSO_, levelData, currentSolution_.solutionData);
        Camera.main.transform.position = new Vector3(300f, 0f, -10f);
    }

    public void EnableUI() => ui.gameObject.SetActive(true);
    public void DisableUI() => ui.gameObject.SetActive(false);

    public void SelectingLevelBtn(LevelSelectBtn levelSelectBtn_, SolutionPage solutionPage_)
    {
        selectingLevelBtn = levelSelectBtn_;
        selectingSolutionPage = solutionPage_;
    }

    public void ClearSelecting()
    {
        selectingLevelBtn = null;
        selectingSolutionPage = null;
    }

    public void Esc()
    {
        if (selectingLevelBtn != null)
        {
            selectingSolutionPage.Back();
        }
    }
}
