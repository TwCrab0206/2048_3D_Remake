using UnityEngine;

public static class GameSettings
{
    public enum GameModes
    {
        None,
        Free,
        BlockAttack,
        ScoreAttack,
        LimitedTime
    }

    public static GameModes CurrentGameMode { get; set; } = GameModes.Free;

    public static int AimScore { get; set; } = 1024;

    public static int AimBlockValue { get; set; } = 2048;

    public static int TimeLimit { get; set; } = 180; //In seconds

    public static int GridSize { get; set; } = 4;

    public static int SpawnAmount { get; set; } = 2;
}
