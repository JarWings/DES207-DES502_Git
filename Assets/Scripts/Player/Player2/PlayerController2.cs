using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    PlayerCharacter player;
    public float h, v;
    public bool dash;
    public bool attack;
    //testing function
    public bool getHurt;

    void Start()
    {
        player = GetComponent<PlayerCharacter>();
    }


    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        dash = Input.GetButtonDown("Dash");
        attack = Input.GetButtonDown("Attack");
        getHurt = Input.GetKeyDown(KeyCode.H);
    }
}
