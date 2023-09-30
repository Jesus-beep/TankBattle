using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Map
{
    Default
}

public enum GameMode
{
    Default
}

public enum GameQueue
{
    Solo, 
    Team
}

[Serializable]
public class UserData
{
    public string UserName;
    public string UserAuthID;
    public GameInfo UserGamePreferences;
}

[SerializeField]
public class GameInfo
{
    public Map Map;
    public GameMode GameMode;
    public GameQueue GameQueue;

    public string ToMultiplayQueue()
    {
        return "";
    }
}