using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelSelectBtn : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Button levelSelectBtn;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] GameObject completeCheck;

    public void Setup(LevelData levelData_, SolutionPage solutionPage, GameObject levelSelectionPanel_)
    {
        text.text = levelData_.levelName;
        text.ForceMeshUpdate();
        rectTransform.sizeDelta = Vector2.one * text.GetRenderedValues(false);
        gameObject.name = $"{levelData_.levelName}SelectBtn";
        levelSelectBtn.onClick.AddListener(() =>
        {
            solutionPage.gameObject.SetActive(true);
            levelSelectionPanel_.gameObject.SetActive(false);
            MainUI.Instance.SelectingLevelBtn(this, solutionPage);
            DataManager.Instance.LoadLevelData(levelData_);
        });

        completeCheck.SetActive(levelData_.completed);
    }

    public void SetComplete() => completeCheck.gameObject.SetActive(true);
}
