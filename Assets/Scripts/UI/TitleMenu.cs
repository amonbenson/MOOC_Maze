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
        GlobalGameSettings.enableAudioOutput = enableAudioOutput.isOn;
        GlobalGameSettings.enableVoiceControl = enableVoiceControl.isOn;

        SceneManager.LoadScene(gameScene);
    }

    public void Quit() {
        Application.Quit();
    }
}
