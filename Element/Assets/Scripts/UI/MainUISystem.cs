using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor;

public enum LevelType
{
    Tutorial,
    Low,
    Middle,
    High,
    Ex
}

public class MainUISystem : MonoBehaviour
{

    [SerializeField] Color pageDisable;
    [SerializeField] Color pageEnable;
    public static MainUISystem Instance { get; private set; }

    public event Action<LevelDataSO, OperatorDataSO> OnSwitchToOperatorScene;
    [SerializeField] OperatorUISystem operatorUISystem;

    [SerializeField] LevelSelectBtn pfLevelSelectBtn;
    [SerializeField] SolutionSystem pfLevelSolution;

    [SerializeField] Button tutorialLevelBtn;
    [SerializeField] Button lowLevelBtn;
    [SerializeField] Button middleLevelBtn;
    [SerializeField] Button highLevelBtn;
    [SerializeField] Button exLevelBtn;

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

    public List<LevelDataSO> levels;
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

        tutorialLevelBtn.onClick.AddListener(() =>
        {
            SelectLevelPage(LevelType.Tutorial);
        });
        lowLevelBtn.onClick.AddListener(() =>
        {
            SelectLevelPage(LevelType.Low);
        });
        middleLevelBtn.onClick.AddListener(() =>
        {
            SelectLevelPage(LevelType.Middle);
        });
        highLevelBtn.onClick.AddListener(() =>
        {
            SelectLevelPage(LevelType.High);
        });
        exLevelBtn.onClick.AddListener(() =>
        {
            SelectLevelPage(LevelType.Ex);
        });

        for (int i = 0; i < levels.Count; i++)
        {
            Transform levelPageParent = transform;
            Transform levelSelectParent = transform;
            LevelPage page = tutorialLevelPage;
            LevelDataSO levelData = levels[i];
            switch (levelData.levelType)
            {
                case LevelType.Tutorial:
                    levelPageParent = tutorialLevelPage.transform;
                    levelSelectParent = tutorialLevelSolutionPanel;
                    page = tutorialLevelPage;
                    break;
                case LevelType.Low:
                    levelPageParent = lowLevelPage.transform;
                    levelSelectParent = lowLevelSolutionPanel;
                    page = lowLevelPage;
                    break;
                case LevelType.Middle:
                    levelPageParent = middleLevelPage.transform;
                    levelSelectParent = middleLevelSolutionPanel;
                    page = middleLevelPage;
                    break;
                case LevelType.High:
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
            levelSelectBtn.gameObject.name = $"{levelData.name}SelectBtn";
            SolutionSystem solution = Instantiate(pfLevelSolution, levelPageParent);
            solution.gameObject.name = $"{levelData.name}SolutionBtnPanel";

            levelSelectBtn.Setup(page, levelData, solution, levelSelectParent.gameObject);
            solution.Setup(page, levelData, levelSelectParent.gameObject);
            solution.gameObject.SetActive(false);
        }

        SelectLevelPage(LevelType.Tutorial);

        OnSwitchToOperatorScene += operatorUISystem.OnSwitchToOperatorScene;
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
            case LevelType.Low:
                lowLevelPage.gameObject.SetActive(true);
                current = lowLevelPage;
                lowLevelBtnImage.color = pageEnable;
                break;
            case LevelType.Middle:
                middleLevelPage.gameObject.SetActive(true);
                current = middleLevelPage;
                middleLevelBtnImage.color = pageEnable;
                break;
            case LevelType.High:
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
    }
    private void ProcessSystem_OnLevelComplete()
    {
        currentSolution.SetComplete();
    }

    public void SwitchToOperatorScene(LevelDataSO levelData, Solution currentSolution_)
    {
        operatorUISystem.gameObject.SetActive(true);
        operatorUISystem.levelData = levelData;

        OnSwitchToOperatorScene?.Invoke(levelData, currentSolution_.operatorData);

        Camera.main.transform.position = new Vector3(300f, 0f, -10f);
        currentSolution = currentSolution_;
        gameObject.SetActive(false);
    }
}
