using UnityEngine;
using UnityEngine.UI;

public class SolutionPage : MonoBehaviour
{
    LevelData levelData;
    LevelBuildDataSO levelBuildDataSO;

    [HideInInspector] public SolutionBtn current;
    [SerializeField] Button backBtn;
    [SerializeField] Button createSolutionBtn;

    [SerializeField] SolutionBtn pfSolution;

    GameObject levelSelectionPanel;

    int[] solutionIndices;
    public void Setup(LevelBuildDataSO levelBuildDataSO_, LevelData levelData_, GameObject levelSelectionPanel_)
    {
        levelData = levelData_;
        levelSelectionPanel = levelSelectionPanel_;
        levelBuildDataSO = levelBuildDataSO_;
        solutionIndices = new int[15];
        gameObject.name = $"{levelBuildDataSO.name}SolutionBtnPanel";
        for (int i = 0; i < levelData.SolutionDatas.Count; i++)
        {
            SolutionData solutionData = levelData.SolutionDatas[i];
            solutionIndices[solutionData.solutionIndex - 1] = solutionData.solutionIndex;
            SolutionBtn solution = Instantiate(pfSolution, transform);
            solution.OnEnterSolution += EnterSolution;
            solution.OnDeleteSolution += DeleteSolution;
            solution.Setup(solutionData);
        }

        backBtn.onClick.AddListener(Back);

        createSolutionBtn.onClick.AddListener(() =>
        {
            int solutionIndex = InputHelper.GetIndexFromIndicesArray(solutionIndices);
            SolutionBtn solution = Instantiate(pfSolution, transform);
            solution.OnEnterSolution += EnterSolution;
            solution.OnDeleteSolution += DeleteSolution;
            SolutionData solutionData = new SolutionData(solutionIndex, levelBuildDataSO);

            levelData.SolutionDatas.Insert(solutionIndex - 1, solutionData);

            solution.Setup(solutionData);
            string saveName = $"{levelData_.levelIndex}_{solutionData.solutionIndex}_solution";
        });

        createSolutionBtn.transform.SetSiblingIndex(createSolutionBtn.transform.parent.childCount - 1);
    }

    void DeleteSolution(SolutionData data_)
    {
        solutionIndices[data_.solutionIndex - 1] = 0;
        DataManager.Instance.DeleteSolution(data_);
    }

    void EnterSolution(SolutionBtn solution)
    {
        MainUI.Instance.SwitchToOperatorScene(levelBuildDataSO, levelData, solution);
    }

    public void Back()
    {
        levelSelectionPanel.SetActive(true);
        gameObject.SetActive(false);
        MainUI.Instance.ClearSelecting();
    }
}
