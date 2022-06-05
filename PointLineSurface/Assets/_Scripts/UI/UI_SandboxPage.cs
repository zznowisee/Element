using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SandboxPage : UI_Page
{
    private Button creatBtn;
    [SerializeField] private UI_SandboxBtn pfUI_SandboxBtn;

    int[] sandboxIndices;

    private void Awake()
    {
        creatBtn = transform.Find("createBtn").GetComponent<Button>();
        sandboxIndices = new int[15];
    }

    public void Setup(UI_Page upperPage_)
    {
        Init(upperPage_);

        creatBtn.onClick.AddListener(() =>
        {
            UI_SandboxBtn sandbox = Instantiate(pfUI_SandboxBtn, transform);
            int sandboxIndex = InputHelper.GetIndexFromIndicesArray(sandboxIndices) - 1;

            Data_Sandbox sandboxData = new Data_Sandbox(sandboxIndex);
            sandbox.Setup(this, sandboxData);
            DataManager.Instance.AddNewSandboxData(sandboxData);
            print(DataManager.GameArchiveData.SandboxDatas.Count);
        });

        creatBtn.transform.SetSiblingIndex(creatBtn.transform.parent.childCount - 1);
    }

    public void DeleteSandboxBtn(Data_Sandbox data_)
    {
        sandboxIndices[data_.sandboxIndex] = 0;
        DataManager.Instance.RemoveSandboxData(data_);
        DataManager.SaveGameData();
    }
}
