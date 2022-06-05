using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainScene, OperateScene, SandBox
}

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }
    public GameState GameState;

    void Awake()    
    {
        Instance = this;
        GameState = GameState.MainScene;    
    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (GameState)
            {
                case GameState.MainScene:
                    //UI_MainScene.Instance.Esc();
                    break;
                case GameState.OperatingScene:
                    UI_OperateScene.Instance.Esc();
                    break;
                case GameState.SandBox:

                    break;
            }
        }    */
    }

    public void LoadSandboxScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
