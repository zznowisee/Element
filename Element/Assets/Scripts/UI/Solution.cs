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
    public OperatorDataSO operatorData;

    public event Action<Solution> OnPressSolutionBtn;
    public event Action<OperatorDataSO> OnPressDeleteBtn;
    public void Setup(OperatorDataSO operatorData_)
    {
        operatorData = operatorData_;
        text.text = $"Solution {operatorData_.solutionIndex}";
        openBtn.onClick.AddListener(() =>
        {
            OnPressSolutionBtn?.Invoke(this);
        });

        deleteBtn.onClick.AddListener(() =>
        {
            OnPressDeleteBtn?.Invoke(operatorData);
            Destroy(gameObject);
        });

        transform.SetSiblingIndex(operatorData_.solutionIndex - 1);
        completeCheck.SetActive(operatorData.complete);
    }

    public void SetComplete() => completeCheck.gameObject.SetActive(true);
}
