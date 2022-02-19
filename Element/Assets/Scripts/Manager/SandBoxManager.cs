using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SandBoxManager : MonoBehaviour
{
    [SerializeField] Button backBtn;
    [SerializeField] Button newSandBoxBtn;
    [SerializeField] SandBoxLevel pfSandBoxLevel;
    void Awake()
    {
            
    }

    void Start()
    {
        backBtn.onClick.AddListener(() =>
        {

        });

        newSandBoxBtn.onClick.AddListener(() =>
        {

        });
    }
}
