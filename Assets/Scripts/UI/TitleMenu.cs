using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMenu : MonoBehaviour {
    public string gameScene;

    public Toggle enableAudioOutput;
    public Toggle enableVoiceControl;

    public Text scoreList;

    void Start() {
        UpdateScoreList();
    }

    void Update() {
    }

    void UpdateScoreList() {
        if (GlobalGameSettings.scoreList.Count == 0) {
            scoreList.text = "";
        } else {
            scoreList.text = "Scores: " + string.Join(", ", GlobalGameSettings.scoreList
                    .Select(time => TimeSpan.FromSeconds(time).ToString("ss\\.fff") + "s"));
        }
    }

    public void StartGame() {
        // setup the global settings
        GlobalGameSettings.audioOutputEnabled = enableAudioOutput.isOn;
        GlobalGameSettings.voiceControlEnabled = enableVoiceControl.isOn;

        // load the scene. It will access the global settings set above
        SceneManager.LoadScene(gameScene);
    }

    public void Quit() {
        Application.Quit();
    }
}
