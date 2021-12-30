using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class SolutionSystem : MonoBehaviour
{
    public LevelPage page;
    public LevelData levelData;
    public LevelBuildDataSO levelBuildDataSO;

    [HideInInspector] public Solution current;
    [SerializeField] Button backBtn;
    [SerializeField] Button createSolutionBtn;

    [SerializeField] Solution pfSolution;

    int[] solutionIndices;
    GameObject levelSelectBtnPanel;
    public void Setup(LevelPage page_, LevelBuildDataSO levelBuildDataSO_, LevelData levelData_, GameObject levelSelectBtnPanel_)
    {
        page = page_;
        levelData = levelData_;
        levelBuildDataSO = levelBuildDataSO_;
        solutionIndices = new int[15];
        levelSelectBtnPanel = levelSelectBtnPanel_;

        for (int i = 0; i < levelData.SolutionDatas.Count; i++)
        {
            SolutionData solutionData = levelData.SolutionDatas[i];
            solutionIndices[solutionData.solutionIndex - 1] = solutionData.solutionIndex;
            Solution solution = Instantiate(pfSolution, transform);
            solution.OnPressSolutionBtn += OnPressSolutionBtn;
            solution.OnPressDeleteBtn += OnPressDeleteBtn;
            solution.Setup(solutionData);
        }

        backBtn.onClick.AddListener(() =>
        {
            page.current = null;
            levelSelectBtnPanel_.SetActive(true);
            gameObject.SetActive(false);
            MainUISystem.Instance.currentLevelSelectBtn = null;
        });

        createSolutionBtn.onClick.AddListener(() =>
        {
            int solutionIndex = InputHelper.GetIndexFromIndicesArray(solutionIndices);
            Solution solution = Instantiate(pfSolution, transform);
            solution.OnPressSolutionBtn += OnPressSolutionBtn;
            solution.OnPressDeleteBtn += OnPressDeleteBtn;
            SolutionData solutionData = new SolutionData(solutionIndex, levelBuildDataSO.controllerNum);

            levelData.SolutionDatas.Insert(solutionIndex - 1, solutionData);

            solution.Setup(solutionData);
            string saveName = $"{levelData_.levelIndex}_{solutionData.solutionIndex}_solution";
        });

        createSolutionBtn.transform.SetSiblingIndex(createSolutionBtn.transform.parent.childCount - 1);
    }

    void OnPressDeleteBtn(SolutionData data_)
    {
        solutionIndices[data_.solutionIndex - 1] = 0;
        levelData.SolutionDatas.Remove(data_);
    }

    void OnPressSolutionBtn(Solution solution)
    {
        MainUISystem.Instance.SwitchToOperatorScene(levelBuildDataSO, levelData, solution);
    }

    public void ResetPage()
    {
        levelSelectBtnPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
