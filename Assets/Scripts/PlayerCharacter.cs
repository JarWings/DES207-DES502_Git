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
    public int maxHp = 5;
    int hp;

    bool jump = false;
    bool isGround = false;
    bool faceLeft = true;
    //失去控制的时间(物理帧)
    float outControlTime = 0;

    void Start()
    {
        controller = GetComponent<PlayerController>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        checkGround = transform.Find("CheckGround");
        hp = maxHp;
    }

    void Update()
    {
        if (controller.jump)
        {
            jump = true;
        }

        //更新动画状态
        //update anim state
        anim.SetBool("IsGround", isGround);
        anim.SetFloat("Speed", Mathf.Abs(controller.h));
        if (controller.attack)
        {
            anim.SetTrigger("Attack");
        }
    }

    private void FixedUpdate()
    {
        CheckGround();
        if (!isAttacking())
        {
            Move(controller.h);
        }
        jump = false;
        outControlTime--;
    }

    bool isAttacking()
    {
        AnimatorStateInfo asi = anim.GetCurrentAnimatorStateInfo(0);
        return asi.IsName("Attack1") || asi.IsName("Attack2") || asi.IsName("Attack3");
    }

    private void Move(float h)
    {
        if (outControlTime > 0)
        {
            return;
        }

        Flip(h);
        float vy = rigid.velocity.y;
        if (jump && isGround)
        {
            AudioManager.PlayAudio(AudioType.soundFX, jumpSound, null, transform.position, transform, 1, Random.Range(.9f, 1.1f));
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
            faceLeft = false;
            transform.localScale = scaleRight;
        }
        else if (h < -0.1f)
        {
            faceLeft = true;
            transform.localScale = scaleLeft;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Boss") || collision.transform.CompareTag("BossHit"))
        {
            GetHit(1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Boss") || collision.transform.CompareTag("BossHit"))
        {
            GetHit(1);
        }
    }

    void GetHit(int damage)
    {
        hp -= damage;
        if (hp < 0) { hp = 0; }
        BarUIManager.Instance.SetPlayerHp(hp, maxHp);
        //受伤动画
        anim.SetTrigger("GetHit");
        //受伤时，向反方向弹飞
        Vector2 force = new Vector2(200, 200);
        if (!faceLeft)
        {
            force.x *= -1;
        }
        rigid.AddForce(force);

        outControlTime = 30;
    }
}
