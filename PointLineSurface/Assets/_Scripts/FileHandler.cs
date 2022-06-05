using UnityEngine;

public static class FileHandler
{
    public static void RunFile()
    {
        Application.OpenURL(System.Environment.CurrentDirectory + "/data/Thinking.txt");
    }
}

