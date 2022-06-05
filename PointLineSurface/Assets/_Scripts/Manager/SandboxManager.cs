using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SandboxManager : MonoBehaviour
{
    public static SandboxManager Instance { get; private set; }
    static Data_Sandbox data_Sandbox;

    public static void LoadSandboxData(Data_Sandbox data)
    {
        data_Sandbox = data;
    }

    private void Awake()
    {
        Instance = this;
        GameObject test = new GameObject("test");
    }

    private void Start()
    {
        if (data_Sandbox != null)
        {
            GameObject.Find("test").name = "data_Sandbox";
        }
    }

    public void LoadSandboxScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
