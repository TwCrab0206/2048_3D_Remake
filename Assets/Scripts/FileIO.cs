using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Text.Json;

public static class FileIO
{
    readonly private static JsonSerializerOptions _options = new () { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    #region Methods for environment settings

    public static string EnvironmentDataFilePath { get; private set; } = Application.persistentDataPath + "/EnvironmentData.json";

    public static void ReloadInputManager()
    {
        if (!File.Exists(EnvironmentDataFilePath)) return;

        string json = File.ReadAllText(EnvironmentDataFilePath);
        EnvironmentData data = JsonSerializer.Deserialize<EnvironmentData>(json, _options);

        EnvironmentSettings.InputManager.Disable();
        EnvironmentSettings.InputManager.Dispose();
        EnvironmentSettings.InputManager = new();
        EnvironmentSettings.InputManager.LoadBindingOverridesFromJson(data.InputManager_Json);
        EnvironmentSettings.InputManager.Enable();
    }

    public static void LoadEnvironmentSettings()
    {
        if (!File.Exists(EnvironmentDataFilePath)) return;

        string json = File.ReadAllText(EnvironmentDataFilePath);
        EnvironmentData data = JsonSerializer.Deserialize<EnvironmentData>(json, _options);
        UnpackEnvironmentData(data);
    }

    public static void SaveEnvironmentSettings()
    {
        EnvironmentData data = PackEnvironmentSettings();
        string json = JsonSerializer.Serialize<EnvironmentData>(data, _options);

        File.WriteAllText(EnvironmentDataFilePath, json);
    }

    private static EnvironmentData PackEnvironmentSettings()
    {
        EnvironmentData data = new()
        {
            XAxisSensitive = EnvironmentSettings.XAxisSensitive,
            YAxisSensitive = EnvironmentSettings.YAxisSensitive,
            ResolutionIndex = EnvironmentSettings.ResolutionIndex,
            Resolution_width = EnvironmentSettings.Resolution.width,
            Resolution_height = EnvironmentSettings.Resolution.height,
            IsFullScreen = EnvironmentSettings.IsFullScreen,
            TargetFPS = EnvironmentSettings.TargetFPS,
            IsVSync = EnvironmentSettings.IsVSync,
            CurrentUsingDevice_EnumIndex = (int)EnvironmentSettings.CurrentUsingDevice,
            CurrentUsingKeysMap = new (EnvironmentSettings.CurrentUsingKeysMap),
            InputManager_Json = EnvironmentSettings.InputManager.SaveBindingOverridesAsJson(),
            CurrentUsingLanguage_EnumIndex = (int)EnvironmentSettings.CurrentUsingLanguage
        };

        return data;
    }

    private static void UnpackEnvironmentData(EnvironmentData data)
    {
        EnvironmentSettings.XAxisSensitive = data.XAxisSensitive;
        EnvironmentSettings.YAxisSensitive = data.YAxisSensitive;
        EnvironmentSettings.ResolutionIndex = data.ResolutionIndex;
        EnvironmentSettings.Resolution = new() { width = data.Resolution_width, height = data.Resolution_height };
        EnvironmentSettings.IsFullScreen = data.IsFullScreen;
        EnvironmentSettings.TargetFPS = data.TargetFPS;
        EnvironmentSettings.IsVSync = data.IsVSync;
        EnvironmentSettings.CurrentUsingDevice = (EnvironmentSettings.AvailableDevices)data.CurrentUsingDevice_EnumIndex;
        EnvironmentSettings.CurrentUsingKeysMap = data.CurrentUsingKeysMap;
        EnvironmentSettings.InputManager = new();
        EnvironmentSettings.InputManager.LoadBindingOverridesFromJson(data.InputManager_Json);
        EnvironmentSettings.CurrentUsingLanguage = (EnvironmentSettings.SupportLanguage)data.CurrentUsingLanguage_EnumIndex;
    }

    #endregion

    #region Methods for game settings

    public static string GameDataFilePath { get; private set; } = Application.persistentDataPath + "/GameData.json";

    public static void LoadGameSettings()
    {
        if (!File.Exists(GameDataFilePath)) return;

        string json = File.ReadAllText(GameDataFilePath);
        GameData data = JsonSerializer.Deserialize<GameData>(json, _options);

        UnpackGameData(data);
    }

    public static void SaveGameSettings()
    {
        GameData data = PackGameSettings();
        string json = JsonSerializer.Serialize<GameData>(data, _options);

        File.WriteAllText(GameDataFilePath, json);
    }

    private static GameData PackGameSettings()
    {
        GameData data = new()
        {
            CurrentGameMode_EnumIndex = (int)GameSettings.CurrentGameMode,
            AimScore = GameSettings.AimScore,
            AimBlockValue = GameSettings.AimBlockValue,
            TimeLimit = GameSettings.TimeLimit,
            GridSize = GameSettings.GridSize,
            SpawnAmount = GameSettings.SpawnAmount,
        };

        return data;
    }

    private static void UnpackGameData(GameData data)
    {
        GameSettings.CurrentGameMode = (GameSettings.GameModes)data.CurrentGameMode_EnumIndex;
        GameSettings.AimScore = data.AimScore;
        GameSettings.AimBlockValue = data.AimBlockValue;
        GameSettings.TimeLimit = data.TimeLimit;
        GameSettings.GridSize = data.GridSize;
        GameSettings.SpawnAmount = data.SpawnAmount;
    }

    #endregion
}
