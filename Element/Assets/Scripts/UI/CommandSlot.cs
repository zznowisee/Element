using System;
using UnityEngine;
using UnityEngine.UI;

public class CommandSlot : MonoBehaviour
{
    Image background;
    [SerializeField] Image highlightImage;
    [HideInInspector] public Command command;
    public event Action<int, CommandSO> OnCommandChanged;
    public int index { get; private set; }

    public void Setup(Color normalCol_, Color highlightCol_, int index_, CommandConsole brushConsole_)
    {
        background = GetComponent<Image>();
        index = index_;

        background.color = normalCol_;
        highlightImage.color = highlightCol_;
    }

    public void SetHighlight()
    {
        highlightImage.gameObject.SetActive(true);
    }

    public void SetNormal()
    {
        highlightImage.gameObject.SetActive(false);
    }

    public void ClearCommand()
    {
        command = null;
        OnCommandChanged?.Invoke(index, null);
    }

    public void DroppedOn(Command newCommand)
    {
        if(command != null)
        {
            Destroy(command.gameObject);
        }

        command = newCommand;
        OnCommandChanged?.Invoke(index, command.commandSO);
    }
}
