using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class SolutionSystem : MonoBehaviour
{
    public LevelPage page;
    public LevelDataSO levelDataSO;
    public OperatorDataSO operatorDataSO;
    [HideInInspector] public Solution current;
    [SerializeField] Button backBtn;
    [SerializeField] Button createSolutionBtn;

    [SerializeField] Solution pfSolution;

    int[] solutionIndices;
    GameObject levelSelectBtnPanel;
    public void Setup(LevelPage page_, LevelDataSO levelDataSO_, GameObject levelSelectBtnPanel_)
    {
        page = page_;
        levelDataSO = levelDataSO_;
        solutionIndices = new int[15];
        levelSelectBtnPanel = levelSelectBtnPanel_;
        for (int i = 0; i < levelDataSO.operatorDataList.Count; i++)
        {
            OperatorDataSO operatorData = levelDataSO.operatorDataList[i];
            solutionIndices[operatorData.solutionIndex - 1] = operatorData.solutionIndex;
            Solution solution = Instantiate(pfSolution, transform);
            solution.OnPressSolutionBtn += OnPressSolutionBtn;
            solution.OnPressDeleteBtn += OnPressDeleteBtn;
            solution.Setup(operatorData);
        }

        backBtn.onClick.AddListener(() =>
        {
            page.current = null;
            levelSelectBtnPanel_.SetActive(true);
            gameObject.SetActive(false);
        });

        createSolutionBtn.onClick.AddListener(() =>
        {
            int solutionIndex = InputHelper.GetIndexFromIndicesArray(solutionIndices);
            Solution solution = Instantiate(pfSolution, transform);
            solution.OnPressSolutionBtn += OnPressSolutionBtn;
            solution.OnPressDeleteBtn += OnPressDeleteBtn;
            //create data instance
            OperatorDataSO operatorData = ScriptableObject.CreateInstance<OperatorDataSO>();
            string dataSavePath = $"Assets/OperatorDatas/{levelDataSO.name}_s{solutionIndex}.asset";
            AssetDatabase.CreateAsset(operatorData, dataSavePath);

            operatorData.Setup();
            levelDataSO.operatorDataList.Insert(solutionIndex - 1, operatorData);
            operatorData.solutionIndex = solutionIndex;
            operatorData.level = levelDataSO;

            solution.Setup(operatorData);
            InputHelper.Save(operatorData, levelDataSO);
        });

        createSolutionBtn.transform.SetSiblingIndex(createSolutionBtn.transform.parent.childCount - 1);
    }

    void OnPressDeleteBtn(OperatorDataSO data_)
    {
        print("Delete");
        solutionIndices[data_.solutionIndex - 1] = 0;

        string dataPath = $"Assets/OperatorDatas/{levelDataSO.name}_s{data_.solutionIndex}.asset";
        levelDataSO.operatorDataList.Remove(data_);
        AssetDatabase.DeleteAsset(dataPath);
        InputHelper.Save(levelDataSO);
    }

    void OnPressSolutionBtn(Solution solution)
    {
        MainUISystem.Instance.SwitchToOperatorScene(levelDataSO, solution);
    }

    public void ResetPage()
    {
        levelSelectBtnPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
