using System.Collections.Generic;
using System;

[Serializable]
public class Data_GameArchive
{
    private List<Data_Level> levelDatas;
    private List<Data_Sandbox> sandboxDatas;

    public List<Data_Level> LevelDatas
    {
        get
        {
            if (levelDatas == null)
            {
                levelDatas = new List<Data_Level>();
            }
            return levelDatas;
        }
    }

    public List<Data_Sandbox> SandboxDatas
    {
        get
        {
            if (sandboxDatas == null)
            {
                sandboxDatas = new List<Data_Sandbox>();
            }
            return sandboxDatas;
        }
    }
}
