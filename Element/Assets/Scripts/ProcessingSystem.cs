using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessingSystem : MonoBehaviour
{
    public static ProcessingSystem Instance { get; private set; }

    public List<FullColorBrush> brushes;

    void Awake()
    {
        Instance = this;

        brushes = new List<FullColorBrush>();
    }

    public void Play()
    {

    }

    public void Step()
    {

    }

    public void Pause()
    {

    }
}
