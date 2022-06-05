using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_LevelBtn : UI_PageBtn
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] GameObject completeCheck;

    public void Setup(Data_Level levelData_, UI_Page chapterPage_, UI_Page solutionPage_)
    {
        Init(chapterPage_, solutionPage_);

        text.text = levelData_.levelName;
        text.ForceMeshUpdate();
        rectTransform.sizeDelta = Vector2.one * text.GetRenderedValues(false);
        //print(text.GetRenderedValues(false));
        gameObject.name = $"{levelData_.levelName}_Btn";
        completeCheck.SetActive(levelData_.completed);

        GetComponent<Button>().onClick.AddListener(() => DataManager.LevelData = levelData_);
    }

    public void SetComplete() => completeCheck.gameObject.SetActive(true);
}
