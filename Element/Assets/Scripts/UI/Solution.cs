using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class Solution : MonoBehaviour
{
    [SerializeField] Button openBtn;
    [SerializeField] Button deleteBtn;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject completeCheck;
    public SolutionData solutionData;

    public event Action<Solution> OnPressSolutionBtn;
    public event Action<SolutionData> OnPressDeleteBtn;

    public void Setup(SolutionData solutionData_)
    {
        solutionData = solutionData_;
        text.text = $"解法 {solutionData_.solutionIndex}";
        gameObject.name = $"解法 {solutionData_.solutionIndex}";
        openBtn.onClick.AddListener(() =>
        {
            OnPressSolutionBtn?.Invoke(this);
        });

        deleteBtn.onClick.AddListener(() =>
        {
            OnPressDeleteBtn?.Invoke(solutionData);
            Destroy(gameObject);
        });

        transform.SetSiblingIndex(solutionData_.solutionIndex - 1);
        completeCheck.SetActive(solutionData.complete);
    }

    public void SetComplete() => completeCheck.gameObject.SetActive(true);
}
