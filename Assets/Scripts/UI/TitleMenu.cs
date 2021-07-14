using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMenu : MonoBehaviour {
    public string gameScene;

    public Toggle enableAudioOutput;
    public Toggle enableVoiceControl;

    void Start() {
    }

    void Update() {
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
