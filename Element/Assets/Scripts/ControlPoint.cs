using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPoint : MonoBehaviour
{
    [HideInInspector] public HexCell cell;
    [HideInInspector] public GameObject obj;
    [HideInInspector] public IControlPointRotateObj controlBody;
    void Awake()
    {
        gameObject.SetActive(false);    
    }
    public void Setup(HexCell cell_, GameObject obj_, IControlPointRotateObj controlBody_)
    {
        cell = cell_;
        obj = obj_;
        controlBody = controlBody_;
    }
}
