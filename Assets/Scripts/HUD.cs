using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Slider stambar;
    private MazeController mazeController;
    private PlayerController playerController;
    private float stam=0;
    public float akstam=0;
    public bool runner = false;

    // Start is called before the first frame update
    void Start()
    {
        mazeController = GetComponentInParent<MazeController>();
        playerController = mazeController.GetComponentInChildren<PlayerController>();

        stambar.maxValue= playerController.stamina;
        stam= playerController.stamina;
        stambar.value = stam;
        runner= false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!runner){
                stambar.value = akstam;
        }
        else {
                stambar.value += stam * playerController.restorestam;
            }
    }
}
