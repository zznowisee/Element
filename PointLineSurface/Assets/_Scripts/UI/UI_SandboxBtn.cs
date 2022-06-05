using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SandboxBtn : MonoBehaviour
{
    private Button sandboxBtn;
    private Button deleteBtn;
    private TextMeshProUGUI text;
    private void Awake()
    {
        sandboxBtn = transform.Find("sandboxBtn").GetComponent<Button>();
        deleteBtn = transform.Find("deleteBtn").GetComponent<Button>();
        text = sandboxBtn.transform.Find("text").GetComponent<TextMeshProUGUI>();
    }

    public void Setup(UI_SandboxPage page_, Data_Sandbox sandboxData_)
    {
        text.text = $"Sandbox {sandboxData_.sandboxIndex + 1}";
        transform.SetSiblingIndex(sandboxData_.sandboxIndex);
        sandboxBtn.onClick.AddListener(() =>
        {
            SandboxManager.LoadSandboxData(sandboxData_);
            GameManager.Instance.LoadSandboxScene();
        });

        deleteBtn.onClick.AddListener(() =>
        {
            page_.DeleteSandboxBtn(sandboxData_);

            DataManager.SaveGameData();

            Destroy(gameObject);
        });
    }
}
