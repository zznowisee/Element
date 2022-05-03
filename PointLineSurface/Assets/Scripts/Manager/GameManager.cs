using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Main, OperatingRoom, SandBox
}

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }
    public GameState GameState;

    void Awake()    
    {
        Instance = this;
        GameState = GameState.Main;    
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (GameState)
            {
                case GameState.Main:
                    MainUI.Instance.Esc();
                    break;
                case GameState.OperatingRoom:
                    OperatingRoomUI.Instance.Esc();
                    break;
                case GameState.SandBox:

                    break;
            }
        }    
    }
}
