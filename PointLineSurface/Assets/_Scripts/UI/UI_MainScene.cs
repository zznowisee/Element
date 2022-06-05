using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_MainScene : MonoBehaviour
{
    public static UI_MainScene Instance { get; private set; }
    private UI_Page currentPage;

    
    [Header("预制体")]
    [SerializeField] UI_LevelBtn pfLevelBtn;
    [SerializeField] UI_SolutionPage pfSolutionPage;
    [SerializeField] UI_PageBtn pfChapterBtn;
    [SerializeField] UI_Page pfChapterPage;
    [SerializeField] UI_SandboxBtn pfSandboxBtn;
    [Header("按钮")]
    [SerializeField] Button backBtn;
    [SerializeField] Button quitBtn;
    [SerializeField] Button sandboxBtn;
    [Header("父级")]
    [SerializeField] UI_Page mainSelectionPage;
    [SerializeField] UI_SandboxPage sandboxPage;
    [SerializeField] Transform solutionPagePanel;
    [SerializeField] Transform ui;

    private void Init()
    {
        // sandbox button
        sandboxBtn.GetComponent<UI_PageBtn>().Init(mainSelectionPage, sandboxPage);
        //print(DataManager.GameArchiveData.SandboxDatas.Count);
        for (int i = 0; i < DataManager.GameArchiveData.SandboxDatas.Count; i++)
        {
            UI_SandboxBtn sandboxBtn = Instantiate(pfSandboxBtn, sandboxPage.transform);
            sandboxBtn.Setup(sandboxPage, DataManager.GameArchiveData.SandboxDatas[i]);
        }
        sandboxPage.gameObject.SetActive(false);
        sandboxPage.Setup(mainSelectionPage);

        SetCurrentActivePage(mainSelectionPage);

        Dictionary<Chapter, UI_Page> chapterPageDictionary = new Dictionary<Chapter, UI_Page>();
        for (Chapter chapter = Chapter.Tutorial; chapter <= Chapter.Tutorial; chapter++)
        {
            UI_PageBtn chapterBtn = Instantiate(pfChapterBtn, mainSelectionPage.transform);
            UI_Page chapterPage = Instantiate(pfChapterPage, ui);
            chapterPageDictionary[chapter] = chapterPage;
            chapterPage.gameObject.name = $"{chapter}_Page";

            chapterBtn.Init(mainSelectionPage, chapterPage);
            chapterBtn.gameObject.name = $"{chapter}_Btn";
            TextMeshProUGUI chapterBtnText = chapterBtn.GetComponentInChildren<TextMeshProUGUI>();
            chapterBtnText.text = $"Chapter - {(int)chapter}";
            chapterBtnText.ForceMeshUpdate();
            chapterBtn.GetComponent<RectTransform>().sizeDelta = Vector2.one * chapterBtnText.GetRenderedValues(false);
            chapterPage.Init(mainSelectionPage);
            chapterBtn.transform.SetSiblingIndex((int)chapter);
        }

        // set all levels
        for (int i = 0; i < DataManager.GameArchiveData.LevelDatas.Count; i++)
        {
            print("Spawn one level.");
            Data_Level levelData = DataManager.GameArchiveData.LevelDatas[i];

            UI_Page chapterPage = chapterPageDictionary[levelData.chapter];
            UI_LevelBtn levelBtn = Instantiate(pfLevelBtn, chapterPage.transform);
            UI_SolutionPage solutionPage = Instantiate(pfSolutionPage, solutionPagePanel);

            levelBtn.Setup(levelData, chapterPage, solutionPage);
            solutionPage.Setup(chapterPage, levelData);
            solutionPage.gameObject.SetActive(false);
        }

        foreach (var chapterPage in chapterPageDictionary.Values)
            chapterPage.gameObject.SetActive(false);

        #region Refac
        /*        for (int i = 0; i < DataManager.GameArchiveData.LevelDatas.Count; i++)
                {
                    Data_Level levelData = DataManager.GameArchiveData.LevelDatas[i];
                    UI_Page currentPage = chapterPageDictionary[levelData.chapter];
                    UI_LevelBtn levelBtn = Instantiate(pfLevelBtn, currentPage.transform);
                    UI_SolutionPage solutionPage = Instantiate(pfSolutionPage, solutionPagePanel);
                    levelBtn.Setup(levelData, currentPage.gameObject, solutionPage.gameObject);
                    solutionPage.Setup(levelBuildDatas[i], levelData);
                }


                for (int i = 0; i < levelBuildDatas.Count; i++)
                {
                    Data_Level levelData = gameArchiveData.LevelDatas[i];
                    UI_LevelBtn levelBtn = Instantiate(pfLevelBtn, pfChapterPage.transform);
                    UI_SolutionPage solutionPage = Instantiate(pfSolutionPage, solutionPagePanel);

                    solutionPage.Init(pfChapterPage.gameObject);

                    // init all solutions
                    for (int j = 0; j < levelData.SolutionDatas.Count; j++)
                    {
                        UI_SolutionBtn solutionBtn = Instantiate(pfSolutionBtn, solutionPage.transform);
                        //solutionBtn.Setup(levelData.SolutionDatas[i]);
                    }

                    UI_PageBtn levelPageBtn = levelBtn.GetComponent<UI_PageBtn>();
                    UI_PageBtn solutionPageBtn = solutionPage.GetComponent<UI_PageBtn>();

                    levelBtn.Init(pfChapterPage.gameObject, solutionPage.gameObject);
                    //solutionPage.Init(solutionPage.gameObject, chapterPage.gameObject);

                    //levelBtn.Setup(levelData, solutionPage);
                    solutionPage.Setup(levelBuildDatas[i], levelData);

                    solutionPage.gameObject.SetActive(false);
                }*/
        #endregion
    }

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Init();
        BindButtonEvent();
    }

    #region Button Event
    void BindButtonEvent()
    {
        quitBtn.onClick.AddListener(Quit);
        sandboxBtn.onClick.AddListener(Sandbox);
        backBtn.onClick.AddListener(Back);
    }

    private void Quit()
    {
        DataManager.SaveGameData();
        Application.Quit();
    }
    private void Sandbox()
    {
        print("sandbox");
    }

    private void Back()
    {
        currentPage.Upper();
        DataManager.SaveGameData();
    }

    #endregion

    private void Update()
    {
        if (GameManager.Instance.GameState == GameState.MainScene && Input.GetKeyDown(KeyCode.Escape))
            Back();
    }

    public void SetCurrentActivePage(UI_Page current_)
    {
        currentPage = current_;
        if (currentPage == mainSelectionPage)
        {
            backBtn.gameObject.SetActive(false);
        }
        else
        {
            backBtn.gameObject.SetActive(true);
        }
    }

    public void EnterOperateScene()
    {
        GameManager.Instance.GameState = GameState.OperateScene;
        DisableUI();
        UI_OperateScene.Instance.EnableUI();
        MapGenerator.Instance.EnableMap();
    }

    public void EnableUI() => ui.gameObject.SetActive(true);
    public void DisableUI() => ui.gameObject.SetActive(false);
}
