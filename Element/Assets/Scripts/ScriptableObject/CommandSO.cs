using UnityEngine;

[CreateAssetMenu()]
public class CommandSO : ScriptableObject
{
    public new string name;
    [TextArea]
    public string description;
    public KeyCode key;
    public Sprite icon;
    public CommandType type;
}
