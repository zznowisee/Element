using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class LevelSelectBtn : MonoBehaviour
{
    [SerializeField] LevelDataSO levelData;
    public LevelPage page;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Button levelSelectBtn;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] GameObject completeCheck;

    public void Setup(LevelPage page_, LevelDataSO levelData_, SolutionSystem levelSolution_, GameObject levelSelectBtnPanel_)
    {
        page = page_;
        levelData = levelData_;

        text.text = levelData.name;
        text.ForceMeshUpdate();
        rectTransform.sizeDelta = Vector2.one * text.GetRenderedValues(false);

        levelSelectBtn.onClick.AddListener(() =>
        {
            page.current = levelSolution_;
            levelSolution_.gameObject.SetActive(true);
            levelSelectBtnPanel_.SetActive(false);
            MainUISystem.Instance.currentLevelSelectBtn = this;
        });

        completeCheck.SetActive(levelData_.completed);
    }

    public void SetComplete() => completeCheck.gameObject.SetActive(true);
}
