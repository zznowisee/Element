using UnityEngine;

[CreateAssetMenu()]
public class CommandSO : ScriptableObject
{
    public new string name;
    public string description;
    public KeyCode key;
    public Sprite icon;
}
