using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class Voice : MonoBehaviour
{

    // Voice command vars
    private Dictionary<string, Action> keyActs = new Dictionary<string, Action>();
    private KeywordRecognizer recognizer;

    // Var needed for color manipulation
    private MeshRenderer cubeRend;
    //Var needed for spin manipulation
    private bool spinningRight;

    //Vars needed for sound playback.
    private AudioSource soundSource;
    public AudioClip[] sounds;
    public float rtimer = 0.0f;

    private PlayerController playerController;
    private HUD hud;

    // Start is called before the first frame update
    void Start()
    {
        cubeRend = GetComponent<MeshRenderer>();
        soundSource = GetComponent<AudioSource>();
        keyActs.Add("run", Run);
        //Voice commands for spinning
        keyActs.Add("spin right", SpinRight);
        keyActs.Add("spin left", SpinLeft);
        //Voice commands for playing sound
        keyActs.Add("Talk", Talk);
        recognizer = new KeywordRecognizer(keyActs.Keys.ToArray());
        recognizer.OnPhraseRecognized += OnKeywordsRecognized;
        recognizer.Start();

        playerController = GetComponent<PlayerController>();
        hud = GetComponent<HUD>();
    }

    void OnKeywordsRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log("Command: " + args.text);
        keyActs[args.text].Invoke();
    }


    //movement speed in units per second
    
    //[SerializeField] float movementsSpeed = 50f;

    public bool running = false;

    void Run()
    {
        running = !running;
        if (running) {
            //hud.runner = true;
            playerController.walkSpeed = 15.0f;
        } else {
            //hud.runner = false;
            playerController.walkSpeed = 3.0f;
            rtimer= 0.0f;
        }
    }

    void SpinRight()
    {
        spinningRight = true;
        StartCoroutine(RotateObject(3f));
    }
    void SpinLeft()
    {
        spinningRight = false;
        StartCoroutine(RotateObject(1f));
    }

    private IEnumerator RotateObject(float duration)
    {
        float startRot = transform.eulerAngles.x;
        float endRot;
        if (spinningRight)
            endRot = startRot + 360f;
        else
            endRot = startRot - 360f;
        float t = 0f;
        float yRot;
        while (t < duration)
        {
            t += Time.deltaTime;
            yRot = Mathf.Lerp(startRot, endRot, t / duration) % 360.0f;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRot, transform.eulerAngles.z);
            yield return null;
        }
    }

    void Talk()
    {
        soundSource.clip = sounds[UnityEngine.Random.Range(0, sounds.Length)];
        soundSource.Play();
    }


    void FactAcknowledgement()
    {
        Debug.Log("How right you are.");
    }


    void Update()
    {
        if (running){
            rtimer += Time.deltaTime;
            int seconds = (int) (rtimer % 60);
            if (seconds < playerController.stamina){
                //Debug.Log( "Rtimer:"+ rtimer + "___ delta:"+ Time.deltaTime);
                //hud.akstam= playerController.stamina * (playerController.stamina/rtimer);
            }
            else {
                running = false;
                rtimer=0.0f;
                playerController.walkSpeed = 3.0f;
            }
        }
        else {
            rtimer= 0.0f;
            playerController.walkSpeed = 3.0f;
            //Debug.Log("Help:"+rtimer);
        }
        /* if (running)
        {
            //get the Input from Horizontal axis
            float horizontalInput = Input.GetAxis("Horizontal");
            //get the Input from Vertical axis
            float verticalInput = Input.GetAxis("Vertical");

            //update the position
            transform.position = transform.position + new Vector3(horizontalInput * (movementsSpeed * 2) * Time.deltaTime, verticalInput * (movementsSpeed * 2) * Time.deltaTime, 0);
            Debug.Log(new Vector3(horizontalInput * (movementsSpeed * 2) * Time.deltaTime, verticalInput * (movementsSpeed * 2) * Time.deltaTime, 0));
        } */
    }
}
