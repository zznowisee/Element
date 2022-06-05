using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Page : MonoBehaviour
{
    public UI_Page upperPage;
    public void Init(UI_Page upper)
    {
        upperPage = upper;
    }
    public virtual void Upper()
    {
        if (upperPage == null) return;
        upperPage.gameObject.SetActive(true);
        UI_MainScene.Instance.SetCurrentActivePage(upperPage);
        gameObject.SetActive(false);
    }

    public void Hide() => gameObject.SetActive(false);
    public void Show() => gameObject.SetActive(true);
}
