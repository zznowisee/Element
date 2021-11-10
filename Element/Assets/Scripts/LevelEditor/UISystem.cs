using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISystem : MonoBehaviour
{

    [SerializeField] Transform colorPickerPanel;
    [SerializeField] Transform consolePanel;
    [SerializeField] Transform commandPanel;
    [SerializeField] ColorPickerBtn colorPickerBtn;
    [SerializeField] ColorPaletteSO colorPaletteSO;

    [SerializeField] Button stepBtn;
    [SerializeField] Button playBtn;
    [SerializeField] Button pauseBtn;

    [SerializeField] CodeConsole pfCodeConsole;
    [SerializeField] CommandIcon pfCommandIcon;
    public List<CommandSO> commandSOList;

    Dictionary<ControlPoint, CodeConsole> controlPointConsoleDictionary;

    private void Awake()
    {
        controlPointConsoleDictionary = new Dictionary<ControlPoint, CodeConsole>();
        for (int i = 0; i < colorPaletteSO.colors.Count; i++)
        {
            Color color = colorPaletteSO.colors[i].color;
            ColorPickerBtn colorPicker = Instantiate(colorPickerBtn, colorPickerPanel);
            colorPicker.GetComponent<Image>().color = color;
            colorPicker.Color = colorPaletteSO.colors[i];
        }

        for (int i = 0; i < commandSOList.Count; i++)
        {
            CommandIcon command = Instantiate(pfCommandIcon, commandPanel);
            command.Setup(commandSOList[i]);
        }

        stepBtn.onClick.AddListener(() =>
        {
            ProcessingSystem.Instance.Step();
        });
        playBtn.onClick.AddListener(() =>
        {
            ProcessingSystem.Instance.Play();
        });
        pauseBtn.onClick.AddListener(() =>
        {
            ProcessingSystem.Instance.Pause();
        });
    }

    void Start()
    {
        BuildSystem.Instance.OnCreateNewControlPoint += BuildSystem_OnCreateNewControlPoint;
    }

    private void BuildSystem_OnCreateNewControlPoint(ControlPoint controlPoint)
    {
        CodeConsole codeConsole = Instantiate(pfCodeConsole, consolePanel);
        codeConsole.transform.SetSiblingIndex(controlPoint.index);
        codeConsole.Setup(controlPoint);
        controlPointConsoleDictionary[controlPoint] = codeConsole;
        controlPoint.OnDestory += ControlPoint_OnDestory;
    }

    private void ControlPoint_OnDestory(ControlPoint controlPoint)
    {
        Destroy(controlPointConsoleDictionary[controlPoint].gameObject);
        controlPointConsoleDictionary.Remove(controlPoint);
    }
}
