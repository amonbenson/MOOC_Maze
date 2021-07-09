using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Slider stambar;
    public MazeController mazeController;
    public PlayerController playerController;
    public Voice vv;
    public float stam=1;
    public float akstam=0;
    public bool runner = false;

    // Start is called before the first frame update
    void Start()
    {
        stambar.maxValue= playerController.stamina;
        Debug.Log("Max: " + stambar.maxValue);
        akstam= playerController.stamina;
        stam= akstam;
        Debug.Log("Stam: " + stam);
        Debug.Log("akStam: " + akstam);
        stambar.value = stam;
        runner= false;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (!runner){
            stam=akstam;
        }*/
        if (vv.running){
                //runner= true;
                if (akstam <= 0){
                    vv.running= false;
                    akstam = 0.01f;
                }
                Debug.Log("akstam: " + akstam);
                
                //stamina =3, rtimer 0.00+  
                akstam= stam - vv.rtimer;
                stambar.value = akstam;
        }
        else {
                //runner=false;
                stam=akstam;
                if (stambar.value <   stambar.maxValue)
                stambar.value += akstam * playerController.restorestam;
                Debug.Log("Bar: " + akstam);
                akstam = stambar.value;
            }
    }
}
