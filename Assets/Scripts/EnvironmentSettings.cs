using System.Collections.Generic;
using UnityEngine;

public static class EnvironmentSettings
{
    //Mouse settings
    public static float XAxisSensitive { get; set; } = 1.4f;

    public static float YAxisSensitive { get; set; } = 1.4f;

    //Graphic settings
    public static int ResolutionIndex { get; set; } = 0;

    public static Resolution Resolution { get; set; } = new() { width = 1920, height = 1080 };

    public static bool IsFullScreen { get; set; } = true;

    public static int TargetFPS { get; set; } = 170;

    public static bool IsVSync { get; set; } = true;

    //Control settings
    public enum AvailableDevices
    {
        KeyboardMouse,
        PSGamepad,
        XboxGamepad
    }

    public static AvailableDevices CurrentUsingDevice { get; set; } = AvailableDevices.KeyboardMouse;

    public static AvailableKeysMaps AvailableKeysMaps { get; private set; } = new();

    public static Dictionary<string, string> CurrentUsingKeysMap { get; set; } = AvailableKeysMaps.FindKeyMap((int)CurrentUsingDevice);

    public static InputSystem_Actions InputManager { get; set; } = new();

    public enum SupportLanguage
    {
        en_us,
        zh_tw
    }

    public static SupportLanguage CurrentUsingLanguage { get; set; } = SupportLanguage.en_us;
}
