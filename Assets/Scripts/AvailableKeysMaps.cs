using System.Collections.Generic;

public class AvailableKeysMaps
{
    public Dictionary<string, string> KeyboardKeyMap { get; private set; } = new()
    {
        //[Path] = Display Name
        ["<Keyboard>/a"] = "A",
        ["<Keyboard>/b"] = "B",
        ["<Keyboard>/c"] = "C",
        ["<Keyboard>/d"] = "D",
        ["<Keyboard>/e"] = "E",
        ["<Keyboard>/f"] = "F",
        ["<Keyboard>/g"] = "G",
        ["<Keyboard>/h"] = "H",
        ["<Keyboard>/i"] = "I",
        ["<Keyboard>/j"] = "J",
        ["<Keyboard>/k"] = "K",
        ["<Keyboard>/l"] = "L",
        ["<Keyboard>/m"] = "M",
        ["<Keyboard>/n"] = "N",
        ["<Keyboard>/o"] = "O",
        ["<Keyboard>/p"] = "P",
        ["<Keyboard>/q"] = "Q",
        ["<Keyboard>/r"] = "R",
        ["<Keyboard>/s"] = "S",
        ["<Keyboard>/t"] = "T",
        ["<Keyboard>/u"] = "U",
        ["<Keyboard>/v"] = "V",
        ["<Keyboard>/w"] = "W",
        ["<Keyboard>/x"] = "X",
        ["<Keyboard>/y"] = "Y",
        ["<Keyboard>/z"] = "Z",
        ["<Keyboard>/1"] = "1",
        ["<Keyboard>/2"] = "2",
        ["<Keyboard>/3"] = "3",
        ["<Keyboard>/4"] = "4",
        ["<Keyboard>/5"] = "5",
        ["<Keyboard>/6"] = "6",
        ["<Keyboard>/7"] = "7",
        ["<Keyboard>/8"] = "8",
        ["<Keyboard>/9"] = "9",
        ["<Keyboard>/0"] = "0",
        ["<Keyboard>/space"] = "Space",
        ["<Keyboard>/enter"] = "Enter",
        ["<Keyboard>/backspace"] = "Backspace",
        ["<Keyboard>/leftCtrl"] = "L-Ctrl",
        ["<Keyboard>/rightCtrl"] = "R-Ctrl",
        ["<Keyboard>/leftShift"] = "L-Shift",
        ["<Keyboard>/rightShift"] = "R-Shift",
        ["<Keyboard>/leftAlt"] = "L-Alt",
        ["<Keyboard>/rightAlt"] = "R-Alt",
        ["<Keyboard>/capsLock"] = "CapsLock",
        ["<Keyboard>/tab"] = "Tab",
        ["<Keyboard>/f1"] = "F1",
        ["<Keyboard>/f2"] = "F2",
        ["<Keyboard>/f3"] = "F3",
        ["<Keyboard>/f4"] = "F4",
        ["<Keyboard>/f5"] = "F5",
        ["<Keyboard>/f6"] = "F6",
        ["<Keyboard>/f7"] = "F7",
        ["<Keyboard>/f8"] = "F8",
        ["<Keyboard>/f9"] = "F9",
        ["<Keyboard>/f10"] = "F10",
        ["<Keyboard>/f11"] = "F11",
        ["<Keyboard>/f12"] = "F12",
        ["<Keyboard>/comma"] = ",",
        ["<Keyboard>/period"] = ".",
        ["<Keyboard>/slash"] = "/",
        ["<Keyboard>/backslash"] = "\\",
        ["<Keyboard>/semicolon"] = ";",
        ["<Keyboard>/quote"] = "\'",
        ["<Keyboard>/backquote"] = "~",
        ["<Keyboard>/leftBracket"] = "[",
        ["<Keyboard>/rightBracket"] = "]",
        ["<Keyboard>/minus"] = "-",
        ["<Keyboard>/equals"] = "=",
    };

    public Dictionary<string, string> PSGamepadKeyMap { get; private set; } = new()
    {
        //[Path] = Display name
        ["<Gamepad>/buttonSouth"] = "X",
        ["<Gamepad>/buttonEast"] = "O",
        ["<Gamepad>/buttonWest"] = "\u25a1",
        ["<Gamepad>/buttonNorth"] = "\u25b2",
        ["<Gamepad>/leftStickPress"] = "L-Stick\nPress",
        ["<Gamepad>/rightStickPress"] = "R-Stick\nPress",
        ["<Gamepad>/leftShoulder"] = "L1",
        ["<Gamepad>/rightShoulder"] = "R1",
        ["<Gamepad>/leftTrigger"] = "L2",
        ["<Gamepad>/rightTrigger"] = "R2",
        ["<Gamepad>/dpad/left"] = "D-Pad\nLeft",
        ["<Gamepad>/dpad/right"] = "D-Pad\nRight",
        ["<Gamepad>/dpad/up"] = "D-Pad\nUp",
        ["<Gamepad>/dpad/down"] = "D-Pad\nDown"
    };

    public Dictionary<string, string> XboxGamepadKeyMap { get; private set; } = new()
    {
        //[Path] = Display name
        ["<Gamepad>/buttonSouth"] = "A",
        ["<Gamepad>/buttonEast"] = "B",
        ["<Gamepad>/buttonWest"] = "X",
        ["<Gamepad>/buttonNorth"] = "Y",
        ["<Gamepad>/leftStickPress"] = "L-Stick\nPress",
        ["<Gamepad>/rightStickPress"] = "R-Stick\nPress",
        ["<Gamepad>/leftShoulder"] = "LB",
        ["<Gamepad>/rightShoulder"] = "RB",
        ["<Gamepad>/leftTrigger"] = "LT",
        ["<Gamepad>/rightTrigger"] = "RT",
        ["<Gamepad>/dpad/left"] = "D-Pad\nLeft",
        ["<Gamepad>/dpad/right"] = "D-Pad\nRight",
        ["<Gamepad>/dpad/up"] = "D-Pad\nUp",
        ["<Gamepad>/dpad/down"] = "D-Pad\nDown"
    };

    public Dictionary<string, string> FindKeyMap(int index)
    {
        return index switch
        {
            0 => KeyboardKeyMap,
            1 => PSGamepadKeyMap,
            2 => XboxGamepadKeyMap,
            _ => null,
        };
    }
}