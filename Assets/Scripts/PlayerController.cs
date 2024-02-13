using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    PlayerCharacter player;
    public float h, v;
    public bool jump;

    void Start()
    {
        player = GetComponent<PlayerCharacter>();
    }


    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        jump = Input.GetButtonDown("Jump");
    }
}
