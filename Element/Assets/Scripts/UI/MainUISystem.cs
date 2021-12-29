using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum LevelType
{
    Tutorial,
    I,
    II,
    III,
    Ex
}

public class MainUISystem : MonoBehaviour
{
    GameData gameData;
    [SerializeField] Color pageDisable;
    [SerializeField] Color pageEnable;
    public static MainUISystem Instance { get; private set; }

    public event Action<LevelBuildDataSO, LevelData, SolutionData> OnLoadSolution;
    [SerializeField] OperatorUISystem operatorUISystem;

    [SerializeField] LevelSelectBtn pfLevelSelectBtn;
    [SerializeField] SolutionSystem pfLevelSolution;

    [SerializeField] Button tutorialLevelBtn;
    [SerializeField] Button lowLevelBtn;
    [SerializeField] Button middleLevelBtn;
    [SerializeField] Button highLevelBtn;
    [SerializeField] Button exLevelBtn;
    [SerializeField] Button quitBtn;
    [SerializeField] Button thinkingBtn;

    [SerializeField] LevelPage tutorialLevelPage;
    [SerializeField] LevelPage lowLevelPage;
    [SerializeField] LevelPage middleLevelPage;
    //
    [SerializeField] LevelPage highLevelPage;
    [SerializeField] LevelPage exLevelPage;
    //
    [SerializeField] Transform tutorialLevelSolutionPanel;
    [SerializeField] Transform lowLevelSolutionPanel;
    [SerializeField] Transform middleLevelSolutionPanel;
    [SerializeField] Transform highLevelSolutionPanel;
    [SerializeField] Transform exLevelSolutionPanel;

    [HideInInspector] public Solution currentSolution;
    public LevelSelectBtn currentLevelSelectBtn;

    public List<LevelBuildDataSO> levels;
    LevelPage[] levelPages;
    LevelPage current;

    Image tutorialBtnImage;
    Image lowLevelBtnImage;
    Image middleLevelBtnImage;
    Image highLevelBtnImage;
    Image exLevelBtnImage;

    void Awake()
    {
        Instance = this;
        current = tutorialLevelPage;
        tutorialLevelPage.gameObject.SetActive(true);
        levelPages = new LevelPage[] { tutorialLevelPage, lowLevelPage, middleLevelPage, highLevelPage, exLevelPage };

        tutorialBtnImage = tutorialLevelBtn.GetComponent<Image>();
        lowLevelBtnImage = lowLevelBtn.GetComponent<Image>();
        middleLevelBtnImage = middleLevelBtn.GetComponent<Image>();
        highLevelBtnImage = highLevelBtn.GetComponent<Image>();
        exLevelBtnImage = exLevelBtn.GetComponent<Image>();

        string path = Application.persistentDataPath + "/saves/" + "Data" + ".save";
        gameData = (GameData)SerializationSystem.Load(path);

        if (gameData == null)
        {
            gameData = new GameData();
            for (int i = 0; i < levels.Count; i++)
            {
                gameData.LevelDatas.Add(new LevelData(levels[i].name, levels[i].levelIndex, levels[i].levelType));
            }
            SaveGameData();
        }
        thinkingBtn.gameObject.SetActive(gameData.FinishedAllLevels());
        exLevelBtn.gameObject.SetActive(gameData.FinishedAllLevels());
        tutorialLevelBtn.onClick.AddListener(() =>
        {
            SelectLevelPage(LevelType.Tutorial);
        });
        lowLevelBtn.onClick.AddListener(() =>
        {
            SelectLevelPage(LevelType.I);
        });
        middleLevelBtn.onClick.AddListener(() =>
        {
            SelectLevelPage(LevelType.II);
        });
        highLevelBtn.onClick.AddListener(() =>
        {
            SelectLevelPage(LevelType.III);
        });
        exLevelBtn.onClick.AddListener(() =>
        {
            SelectLevelPage(LevelType.Ex);
        });
        quitBtn.onClick.AddListener(() =>
        {
            SaveGameData();
            Application.Quit();
        });
        thinkingBtn.onClick.AddListener(() =>
        {
            Application.OpenURL(Environment.CurrentDirectory + "/Data/Thinking.txt");
        });
        for (int i = 0; i < levels.Count; i++)
        {
            Transform levelPageParent = transform;
            Transform levelSelectParent = transform;
            LevelPage page = tutorialLevelPage;
            LevelBuildDataSO levelBuildDataSO = levels[i];

            LevelData levelData = gameData.LevelDatas[levelBuildDataSO.levelIndex];

            switch (levelBuildDataSO.levelType)
            {
                case LevelType.Tutorial:
                    levelPageParent = tutorialLevelPage.transform;
                    levelSelectParent = tutorialLevelSolutionPanel;
                    page = tutorialLevelPage;
                    break;
                case LevelType.I:
                    levelPageParent = lowLevelPage.transform;
                    levelSelectParent = lowLevelSolutionPanel;
                    page = lowLevelPage;
                    break;
                case LevelType.II:
                    levelPageParent = middleLevelPage.transform;
                    levelSelectParent = middleLevelSolutionPanel;
                    page = middleLevelPage;
                    break;
                case LevelType.III:
                    levelPageParent = highLevelPage.transform;
                    levelSelectParent = highLevelSolutionPanel;
                    page = highLevelPage;
                    break;
                case LevelType.Ex:
                    levelPageParent = exLevelPage.transform;
                    levelSelectParent = exLevelSolutionPanel;
                    page = exLevelPage;
                    break;
            }
            LevelSelectBtn levelSelectBtn = Instantiate(pfLevelSelectBtn, levelSelectParent);
            levelSelectBtn.gameObject.name = $"{levelBuildDataSO.name}SelectBtn";
            SolutionSystem solutionSystem = Instantiate(pfLevelSolution, levelPageParent);
            solutionSystem.gameObject.name = $"{levelBuildDataSO.name}SolutionBtnPanel";

            levelSelectBtn.Setup(page, levelData, solutionSystem, levelSelectParent.gameObject);
            solutionSystem.Setup(page, levelBuildDataSO, levelData, levelSelectParent.gameObject);
            solutionSystem.gameObject.SetActive(false);
        }

        SelectLevelPage(LevelType.Tutorial);

        OnLoadSolution += operatorUISystem.OnLoadSolution;
    }

    public void SaveGameData()
    {
        string saveName = "Data";
        SerializationSystem.Save(saveName, gameData);
    }

    void Start()
    {
        ProcessSystem.Instance.OnLevelComplete += ProcessSystem_OnLevelComplete;    
    }

    void SelectLevelPage(LevelType levelType)
    {
        current.ResetPage();
        for (int i = 0; i < levelPages.Length; i++)
        {
            levelPages[i].gameObject.SetActive(false);
        }

        tutorialBtnImage.color = pageDisable;
        lowLevelBtnImage.color = pageDisable;
        middleLevelBtnImage.color = pageDisable;
        highLevelBtnImage.color = pageDisable;
        exLevelBtnImage.color = pageDisable;
        switch (levelType)
        {
            case LevelType.Tutorial:
                tutorialLevelPage.gameObject.SetActive(true);
                current = tutorialLevelPage;
                tutorialBtnImage.color = pageEnable;
                break;
            case LevelType.I:
                lowLevelPage.gameObject.SetActive(true);
                current = lowLevelPage;
                lowLevelBtnImage.color = pageEnable;
                break;
            case LevelType.II:
                middleLevelPage.gameObject.SetActive(true);
                current = middleLevelPage;
                middleLevelBtnImage.color = pageEnable;
                break;
            case LevelType.III:
                highLevelPage.gameObject.SetActive(true);
                current = highLevelPage;
                highLevelBtnImage.color = pageEnable;
                break;
            case LevelType.Ex:
                exLevelPage.gameObject.SetActive(true);
                current = exLevelPage;
                exLevelBtnImage.color = pageEnable;
                break;
        }

        currentLevelSelectBtn = null;
    }
    private void ProcessSystem_OnLevelComplete(LevelData levelData)
    {
        currentSolution.SetComplete();
        currentLevelSelectBtn.SetComplete();
        levelData.completed = true;
        thinkingBtn.gameObject.SetActive(gameData.FinishedAllLevels());
        exLevelBtn.gameObject.SetActive(gameData.FinishedAllLevels());
    }

    public void SwitchToOperatorScene(LevelBuildDataSO levelBuildDataSO_, LevelData levelData, Solution currentSolution_)
    {
        operatorUISystem.gameObject.SetActive(true);
        operatorUISystem.levelBuildDataSO = levelBuildDataSO_;

        currentSolution = currentSolution_;
        OnLoadSolution?.Invoke(levelBuildDataSO_, levelData, currentSolution_.solutionData);
        print(levelBuildDataSO_.name);
        Camera.main.transform.position = new Vector3(300f, 0f, -10f);
        gameObject.SetActive(false);
    }
}
