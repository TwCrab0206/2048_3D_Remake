using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentData
{
    //Mouse
    public float XAxisSensitive { get; set; }

    public float YAxisSensitive { get; set; }

    //Graphic
    public int ResolutionIndex { get; set; }

    public int Resolution_width { get; set; }

    public int Resolution_height { get; set; }

    public bool IsFullScreen { get; set; }

    public int TargetFPS { get; set; }

    public bool IsVSync { get; set; }

    //Control
    public int CurrentUsingDevice_EnumIndex { get; set; }

    public Dictionary<string, string> CurrentUsingKeysMap { get; set; }

    public string InputManager_Json { get; set; }

    public int CurrentUsingLanguage_EnumIndex { get; set; }
}

[System.Serializable]
public class GameData
{
    public int CurrentGameMode_EnumIndex { get; set; }

    public int AimScore { get; set; }

    public int AimBlockValue { get; set; }

    public int TimeLimit { get; set; } //In seconds

    public int GridSize { get; set; }

    public int SpawnAmount { get; set; }
}
