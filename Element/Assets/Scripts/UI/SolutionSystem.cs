using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class SolutionSystem : MonoBehaviour
{
    public LevelPage page;
    public LevelDataSO levelDataSO;
    SolutionBtn current;
    //public 
    [SerializeField] GameObject SolutionOpenDeleteBtnPanel;
    [SerializeField] Button deleteBtn;
    [SerializeField] Button openBtn;
    [SerializeField] Button backBtn;
    [SerializeField] Button createSolutionBtn;

    [SerializeField] SolutionBtn pfLevelSolutionBtn;

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
            SolutionBtn solutionBtn = Instantiate(pfLevelSolutionBtn, transform);
            solutionBtn.OnPressSolutionBtn += OnPressSolutionBtn;
            solutionBtn.Setup(operatorData, operatorData.solutionIndex);
        }

        backBtn.onClick.AddListener(() =>
        {
            page.current = null;
            SolutionOpenDeleteBtnPanel.SetActive(false);
            levelSelectBtnPanel_.SetActive(true);
            gameObject.SetActive(false);
        });

        createSolutionBtn.onClick.AddListener(() =>
        {
            int solutionIndex = InputHelper.GetIndexFromIndicesArray(solutionIndices);
            SolutionBtn solutionBtn = Instantiate(pfLevelSolutionBtn, transform);
            solutionBtn.OnPressSolutionBtn += OnPressSolutionBtn;
            OperatorDataSO operatorData = ScriptableObject.CreateInstance<OperatorDataSO>();
            string dataSavePath = $"Assets/OperatorDatas/{levelDataSO.name}_s{solutionIndex}.asset";
            AssetDatabase.CreateAsset(operatorData, dataSavePath);
            //
            operatorData.Setup();
            //
            levelDataSO.operatorDataList.Insert(solutionIndex - 1, operatorData);
            operatorData.solutionIndex = solutionIndex;
            operatorData.level = levelDataSO;

            solutionBtn.Setup(operatorData, solutionIndex);
        });

        deleteBtn.onClick.AddListener(() =>
        {
            OperatorDataSO data = current.operatorData;
            solutionIndices[data.solutionIndex - 1] = 0;
            string dataPath = $"Assets/OperatorDatas/{levelDataSO.name}_s{data.solutionIndex}.asset";
            levelDataSO.operatorDataList.Remove(data);
            AssetDatabase.DeleteAsset(dataPath);
            Destroy(current.gameObject);
            SolutionOpenDeleteBtnPanel.SetActive(false);
        });

        openBtn.onClick.AddListener(() =>
        {
            MainUISystem.Instance.SwitchToOperatorScene(levelDataSO, current.operatorData);
            SolutionOpenDeleteBtnPanel.SetActive(false);
            current = null;
        });

        SolutionOpenDeleteBtnPanel.SetActive(false);
    }

    private void OnPressSolutionBtn(SolutionBtn solutionBtn_)
    {
        current = solutionBtn_;
        SolutionOpenDeleteBtnPanel.gameObject.SetActive(true);
    }

    public void ResetPage()
    {
        SolutionOpenDeleteBtnPanel.SetActive(false);
        levelSelectBtnPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
