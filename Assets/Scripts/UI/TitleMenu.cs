using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMenu : MonoBehaviour {
    public string gameScene;

    public InputField seed;
    private MD5 seedHasher;

    public Toggle enableAudioOutput;
    public Toggle enableVoiceControl;

    public Text scoreList;

    void Start() {
        seedHasher = MD5.Create();

        enableAudioOutput.isOn = GlobalGameSettings.audioOutputEnabled;
        enableVoiceControl.isOn = GlobalGameSettings.voiceControlEnabled;
        seed.text = GlobalGameSettings.seedString;

        UpdateScoreList();
    }

    void Update() {
    }

    void UpdateScoreList() {
        scoreList.text = "\nLap   Score\n";

        if (GlobalGameSettings.scoreList.Count == 0) {
            scoreList.text += "(no data)\n";
        } else {
            var i = 0;
            foreach (var time in GlobalGameSettings.scoreList) {
                scoreList.text += "  #" + (++i) + "   " + TimeSpan.FromSeconds(time).ToString("ss\\.fff") + "s\n";
            }
        }
    }

    public void StartGame() {
        // setup the global settings
        GlobalGameSettings.audioOutputEnabled = enableAudioOutput.isOn;
        GlobalGameSettings.voiceControlEnabled = enableVoiceControl.isOn;

        if (String.IsNullOrEmpty(seed.text)) {
            // use random seed
            GlobalGameSettings.seed = Environment.TickCount;
            GlobalGameSettings.seedString = "";
        } else {
            // use string hash as seed
            var hash = seedHasher.ComputeHash(Encoding.UTF8.GetBytes(seed.text));
            GlobalGameSettings.seed = BitConverter.ToInt32(hash, 0);
            GlobalGameSettings.seedString = seed.text;
        }

        // load the scene. It will access the global settings set above
        SceneManager.LoadScene(gameScene);
    }

    public void Quit() {
        Application.Quit();
    }
}
