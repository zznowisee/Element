using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor;

public class MainUISystem : MonoBehaviour
{

    public static MainUISystem Instance { get; private set; }

    public event Action<LevelDataSO, OperatorDataSO> OnSwitchToOperatorScene;
    [SerializeField] OperatorUISystem operatorUISystem;

    [SerializeField] LevelSelectBtn pfLevelSelectBtn;
    [SerializeField] SolutionSystem pfLevelSolution;

    [SerializeField] Button tutorialLevelBtn;
    [SerializeField] Button middleLevelBtn;
    [SerializeField] Button highLevelBtn;

    [SerializeField] LevelPage tutorialLevelPage;
    [SerializeField] LevelPage middleLevelPage;
    [SerializeField] LevelPage highLevelPage;

    [SerializeField] Transform tutorialLevelSolutionPanel;
    [SerializeField] Transform middleLevelSolutionPanel;
    [SerializeField] Transform highLevelSolutionPanel;

    public List<LevelDataSO> levels;
    LevelPage[] levelPages;
    LevelPage current;
    void Awake()
    {
        Instance = this;
        current = tutorialLevelPage;
        tutorialLevelPage.gameObject.SetActive(true);
        levelPages = new LevelPage[] { tutorialLevelPage, middleLevelPage, highLevelPage };
        tutorialLevelBtn.onClick.AddListener(() =>
        {
            current.ResetPage();
            for (int i = 0; i < levelPages.Length; i++)
            {
                levelPages[i].gameObject.SetActive(false);
            }

            tutorialLevelPage.gameObject.SetActive(true);
            current = tutorialLevelPage;
        });
        middleLevelBtn.onClick.AddListener(() =>
        {
            current.ResetPage();
            for (int i = 0; i < levelPages.Length; i++)
            {
                levelPages[i].gameObject.SetActive(false);
            }

            middleLevelPage.gameObject.SetActive(true);
            current = middleLevelPage;
        });
        highLevelBtn.onClick.AddListener(() =>
        {
            current.ResetPage();
            for (int i = 0; i < levelPages.Length; i++)
            {
                levelPages[i].gameObject.SetActive(false);
            }

            highLevelPage.gameObject.SetActive(true);
            current = highLevelPage;
        });

        for (int i = 0; i < levels.Count; i++)
        {
            Transform levelPageParent = transform;
            Transform levelSelectParent = transform;
            LevelPage page = tutorialLevelPage;
            LevelDataSO levelData = levels[i];
            switch (levelData.levelType)
            {
                case LevelType.Tutorials:
                    levelPageParent = tutorialLevelPage.transform;
                    levelSelectParent = tutorialLevelSolutionPanel;
                    page = tutorialLevelPage;
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
            }

            LevelSelectBtn levelSelectBtn = Instantiate(pfLevelSelectBtn, levelSelectParent);
            levelSelectBtn.gameObject.name = $"{levelData.name}SelectBtn";
            SolutionSystem solution = Instantiate(pfLevelSolution, levelPageParent);
            solution.gameObject.name = $"{levelData.name}SolutionBtnPanel";

            levelSelectBtn.Setup(page, levelData, solution, levelSelectParent.gameObject);
            solution.Setup(page, levelData, levelSelectParent.gameObject);
            solution.gameObject.SetActive(false);
        }

        tutorialLevelPage.gameObject.SetActive(true);
        middleLevelPage.gameObject.SetActive(false);
        highLevelPage.gameObject.SetActive(false);

        OnSwitchToOperatorScene += operatorUISystem.OnSwitchToOperatorScene;
    }

    public void SwitchToOperatorScene(LevelDataSO levelData, OperatorDataSO operatorData)
    {
        operatorUISystem.gameObject.SetActive(true);
        operatorUISystem.levelData = levelData;

        OnSwitchToOperatorScene?.Invoke(levelData, operatorData);

        Camera.main.transform.position = new Vector3(300f, 0f, -10f);

        gameObject.SetActive(false);
    }
}
