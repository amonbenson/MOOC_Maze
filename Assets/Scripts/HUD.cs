using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Slider stambar;
    public Text text;
    public MazeController mazeController;
    public PlayerController playerController;
    public Voice vv;
    public float stam=1;
    public float akstam=0;
    public bool runner = false;

    [SerializeField] int maxcoins = 3;
    private int anzcoins= 0;

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

        
        text.text= anzcoins+"/"+maxcoins;
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
                anzcoins ++; 
                text.text= anzcoins+"/"+maxcoins;
                playerController.collectcoin= false;
                                                            // 6            220         212
                if (anzcoins >= maxcoins) text.color= new Color(0.02352941f,0.7490196f,0.7450981f,1);
        }
    }
}
