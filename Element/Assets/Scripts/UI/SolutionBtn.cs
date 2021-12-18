using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class SolutionBtn : MonoBehaviour
{
    [SerializeField] Button solutionBtn;
    [SerializeField] TextMeshProUGUI text;
    public OperatorDataSO operatorData;
    public event Action<SolutionBtn> OnPressSolutionBtn;
    
    public void Setup(OperatorDataSO operatorData_, int solutionIndex)
    {
        operatorData = operatorData_;
        text.text = $"Solution {solutionIndex}";
        solutionBtn.onClick.AddListener(() =>
        {
            OnPressSolutionBtn?.Invoke(this);
        });

        transform.SetSiblingIndex(solutionIndex - 1);
    }
}
