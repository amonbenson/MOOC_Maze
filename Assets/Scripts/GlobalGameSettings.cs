using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class GlobalGameSettings {
    public static int seed = Environment.TickCount; // initial value for when the GameScene is started standalone
    public static string seedString = ""; // used by the title screen to restore a custom seed

    public static bool audioOutputEnabled = true;
    public static bool voiceControlEnabled = true;

    public static List<float> scoreList = new List<float>();
}
