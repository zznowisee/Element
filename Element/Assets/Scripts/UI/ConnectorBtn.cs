using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConnectorBtn : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (ProcessSystem.Instance.CanOperate())
        {
            BuildSystem.Instance.CreateNewConnecter();
        }
    }
}
