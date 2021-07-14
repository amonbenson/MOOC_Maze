using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour {
    public string gameScene;

    void Start() {
    }

    void Update() {
    }

    public void StartGame() {
        SceneManager.LoadScene(gameScene);
    }

    public void Quit() {
        Application.Quit();
    }
}
