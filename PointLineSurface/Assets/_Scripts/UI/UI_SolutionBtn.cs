using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class UI_SolutionBtn : MonoBehaviour
{
    private Button solutionBtn;
    private Button deleteBtn;
    private GameObject completeCheck;
    [HideInInspector] public Data_Solution solutionData;

    private void Awake()
    {
        solutionBtn = transform.Find("solutionBtn").GetComponent<Button>();
        deleteBtn = transform.Find("deleteBtn").GetComponent<Button>();
        completeCheck = transform.Find("completeCheck").gameObject;
    }

    public void Setup(UI_SolutionPage solutionPage_, Data_Level levelData_, Data_Solution solutionData_)
    {
        solutionData = solutionData_;
        TextMeshProUGUI solutionBtnText = solutionBtn.GetComponentInChildren<TextMeshProUGUI>();
        solutionBtnText.text = $"Solution {solutionData_.solutionIndex}";
        gameObject.name = $"Solution {solutionData_.solutionIndex}";
        solutionBtn.onClick.AddListener(() =>
        {
            // setup UI, Camera 
            UI_MainScene.Instance.EnterOperateScene();
            // build product 
            CheckManager.Instance.BuildProductCells(levelData_);
            // build ui
            UI_OperateScene.Instance.BuildToolsUI(solutionData_);
            // build exist device
            DeviceManager.Instance.BuildDevices(solutionData_);
            // update current tracking data
            DataManager.SolutionData = solutionData_;
            // update game scene
            GameManager.Instance.GameState = GameState.OperateScene;
        });

        deleteBtn.onClick.AddListener(() =>
        {
            solutionPage_.DeleteSolution(solutionData_);
            DataManager.Instance.DeleteSolutionData(solutionData_);
            // save game data
            DataManager.SaveGameData();
            Destroy(gameObject);
        });

        transform.SetSiblingIndex(solutionData_.solutionIndex - 1);
        completeCheck.SetActive(solutionData.complete);
    }

    public void SetComplete() => completeCheck.gameObject.SetActive(true);
}
