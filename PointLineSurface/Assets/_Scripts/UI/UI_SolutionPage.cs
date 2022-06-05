using UnityEngine;
using UnityEngine.UI;

public class UI_SolutionPage : UI_Page
{
    private Button createBtn;
    [SerializeField] private UI_SolutionBtn pfUI_SolutionBtn;

    private int[] solutionIndices;

    private void Awake()
    {
        createBtn = transform.Find("createBtn").GetComponent<Button>();
    }

    public void Setup(UI_Page upperPage, Data_Level levelData_)
    {
        Init(upperPage);

        solutionIndices = new int[15];
        gameObject.name = $"{levelData_.levelName}SolutionBtnPanel";
        for (int i = 0; i < levelData_.SolutionDatas.Count; i++)
        {
            Data_Solution solutionData = levelData_.SolutionDatas[i];
            solutionIndices[solutionData.solutionIndex - 1] = solutionData.solutionIndex;
            UI_SolutionBtn solution = Instantiate(pfUI_SolutionBtn, transform);
            solution.Setup(this, levelData_, solutionData);
        }

        createBtn.onClick.AddListener(() =>
        {
            int solutionIndex = InputHelper.GetIndexFromIndicesArray(solutionIndices);
            UI_SolutionBtn solutionBtn = Instantiate(pfUI_SolutionBtn, transform);
            Data_Solution solutionData = new Data_Solution(solutionIndex, levelData_);
            levelData_.SolutionDatas.Insert(solutionIndex - 1, solutionData);
            solutionBtn.Setup(this, levelData_, solutionData);

            if (solutionIndex == solutionIndices.Length)
            {
                print("MAXIUM SIZE SOLUTION ! (15)");
                createBtn.gameObject.SetActive(false);
            }
        });

        createBtn.transform.SetSiblingIndex(createBtn.transform.parent.childCount - 1);
    }

    public override void Upper()
    {
        base.Upper();
        DataManager.LevelData = null;
    }

    public void DeleteSolution(Data_Solution data_)
    {
        solutionIndices[data_.solutionIndex - 1] = 0;
        DataManager.Instance.DeleteSolutionData(data_);
        DataManager.SaveGameData();
        createBtn.gameObject.SetActive(true);
    }
}
