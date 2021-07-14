using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Slider stambar;
    public Text tokenText, timerText;
    public MazeController mazeController;
    public PlayerController playerController;
    public Voice vv;
    public float stam=1;
    public float akstam=0;
    public bool runner = false;

    [SerializeField] int maxcoins = 3;
    private int anzcoins= 0;
    private float timer = 0.0f;

    private Boolean gameRunning;
    
    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Start is called before the first frame update
    void Start()
    {
        stambar.maxValue= playerController.stamina;
        //Debug.Log("Max: " + stambar.maxValue);
        akstam= playerController.stamina;
        stam= akstam;
        //Debug.Log("Stam: " + stam);
        //Debug.Log("akStam: " + akstam);
        stambar.value = stam;
        runner= false;

        timer = 0.0f;
        gameRunning = false;

        UpdateTokenText();
        UpdateTimerText();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameRunning) {
            timer += Time.deltaTime;
            UpdateTimerText();
        }

        /*if (!runner){
            stam=akstam;
        }*/
        if (vv.running){
                //runner= true;
                if (akstam <= 0){
                    vv.running= false;
                    akstam = 0.01f;
                }
                //Debug.Log("akstam: " + akstam);
                
                //stamina =3, rtimer 0.00+  
                akstam= stam - vv.rtimer;
                stambar.value = akstam;
        }
        else {
                //runner=false;
                stam=akstam;
                if (stambar.value <   stambar.maxValue)
                stambar.value += playerController.restorestam *0.01f;
                //Debug.Log("Bar: " + akstam);
                akstam = stambar.value;
        }

        if (playerController.collectcoin){
            playerController.collectcoin = false;

            anzcoins++;
            if (anzcoins >= maxcoins) {
                anzcoins = maxcoins;
                tokenText.color= new Color(0.02352941f,0.7490196f,0.7450981f,1);
            }

            UpdateTokenText();
        }

        if (playerController.targetreached) {
            playerController.targetreached = false;

            if (anzcoins >= maxcoins) {
                EndGame();
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // start the game (an therefore the timer) only when everything has loaded
        StartGame();
    }

    public void StartGame() {
        gameRunning = true;
    }

    public void EndGame() {
        gameRunning = false;

        Debug.Log("Game Finished in " + timer + " seconds.");

        // TODO: switch to some Win Scene
    }

    void UpdateTokenText() {
        tokenText.text = anzcoins + "/" + maxcoins;
    }

    void UpdateTimerText() {
        timerText.text = "Time: " + TimeSpan.FromSeconds(timer).ToString("ss\\.fff") + "s";
    }
}
