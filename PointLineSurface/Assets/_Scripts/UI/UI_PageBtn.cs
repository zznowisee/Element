using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PageBtn : MonoBehaviour
{
    private UI_Page currentPage;
    private UI_Page nextPage;
    private Button pageBtn;

    public void Init(UI_Page current, UI_Page next)
    {
        currentPage = current;
        nextPage = next;

        pageBtn = GetComponent<Button>();
        pageBtn.onClick.AddListener(() => 
        {
            nextPage.Show();
            currentPage.Hide();
            UI_MainScene.Instance.SetCurrentActivePage(nextPage);
        });
    }
}