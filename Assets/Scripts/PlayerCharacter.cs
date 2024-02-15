using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    PlayerController controller;

    Rigidbody2D rigid;
    Animator anim;

    Transform checkGround;

    public AudioClip jumpSound;

    public float speed = 3.0f;
    public float jumpspeed = 6.0f;

    bool jump = false;
    bool isGround = false;

    void Start()
    {
        controller = GetComponent<PlayerController>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        checkGround = transform.Find("CheckGround");
    }

    
    void Update()
    {
        if (controller.jump)
        {
            jump = true;
        }

        //¸üÐÂ¶¯»­×´Ì¬
        //update anim state
        anim.SetBool("IsGround", isGround);
        anim.SetFloat("Speed", Mathf.Abs(controller.h));
    }

    private void FixedUpdate()
    {
        CheckGround();
        Move(controller.h);
        jump = false;
    }

    private void Move(float h)
    {
        Flip(h);
        float vy = rigid.velocity.y;
        if (jump && isGround)
        {
            AudioManager.PlayAudio(jumpSound, null, transform.position, transform, 1, Random.Range(.9f, 1.1f));
            anim.SetTrigger("Jump");
            vy = jumpspeed;
        }
        rigid.velocity = new Vector2(h * speed, vy);
    }

    void CheckGround()
    {
        isGround = Physics2D.OverlapCircle(checkGround.position, 0.1f, ~LayerMask.GetMask("Player"));
    }

    void Flip(float h)
    {
        Vector3 scaleLeft = new Vector3(1, 1, 1);
        Vector3 scaleRight = new Vector3(-1, 1, 1);
        if (h > 0.1f)
        {
            transform.localScale = scaleRight;
        }
        else if (h < -0.1f)
        {
            transform.localScale = scaleLeft;
        }
    }
}
