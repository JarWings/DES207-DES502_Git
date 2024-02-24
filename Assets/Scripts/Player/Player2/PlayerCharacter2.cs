using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacter2 : MonoBehaviour
{
    PlayerController2 controller;
    Rigidbody2D rigid;
    Animator anim;

    public float speed = 8.0f;
    public float crushSpeed = 100.0f;
    public int maxHp = 50;
    int hp;

    bool faceLeft = true;
    float outControlTime = 0;

    [Header("Dash����")]
    public float dashTime;//dashʱ��
    private float dashTimeLeft;//���ʣ��ʱ��
    private float lastDash=-10.0f;//��һ�γ��ʱ���
    public float dashCoolDown;
    public float dashSpeed;
    bool isDashing = false;

    void Start()
    {
        controller = GetComponent<PlayerController2>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        hp = maxHp;
    }

    void Update()
    {
        if (controller.dash && (Time.time >= (lastDash + dashCoolDown))) 
        {
            ReadyToDash();
        }

        // ���¶���״̬
        anim.SetFloat("Speed", Mathf.Abs(controller.h));
        if (controller.attack)
        {
            anim.SetTrigger("Attack");
        }

        // ���Թ���
        if (controller.getHurt)
        {
            GetHit(25);
        }
    }

    private void FixedUpdate()
    {
        if (!isAttacking())
        {
            Move(controller.h);
            Dash();
        }
        else
        {
            rigid.velocity = new Vector2(0, 0);
        }
        outControlTime--;
    }

    bool isAttacking()
    {
        AnimatorStateInfo asi = anim.GetCurrentAnimatorStateInfo(0);
        return asi.IsName("Attack1") || asi.IsName("Attack2");
    }

    private int HorizontalDir(bool isLeft)
    {
        if (isLeft == true)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }

    private void ReadyToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        
    }

    void Dash()
    {
        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                anim.SetBool("isDashing", true); // ��ʼ��̶���
                // �ر��������ٶ�
                rigid.gravityScale = 0;
                // ������ײ��
                ToggleCollider(false);
                rigid.velocity = new Vector2(dashSpeed * HorizontalDir(faceLeft), rigid.velocity.y);
                dashTimeLeft -= Time.deltaTime;
            }
            else
            {
                isDashing = false;
                anim.SetBool("isDashing", false); 
                ResetAfterDash();
            }
        }
    }


    private void Move(float h)
    {
        if (outControlTime > 0 || isDashing)
        {
            return;
        }

        Flip(h);
        float vx = h * speed;
        rigid.velocity = new Vector2(vx, rigid.velocity.y);
    }

    private void ToggleCollider(bool enabled)
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = enabled;
        }
    }

    private void ResetAfterDash()
    {
        ToggleCollider(true);
        rigid.gravityScale = 1; // ����1��Ĭ�ϵ��������ٶȱ���
        rigid.velocity = Vector2.zero; // ������Ҫֹͣ��̺�Ķ����ƶ�
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
            if (this.CompareTag("Player"))
            {
                GetHit(1);
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Boss") || collision.transform.CompareTag("BossHit"))
        {
            if (this.CompareTag("Player"))
            {
                GetHit(1);
            }
        }

        if (collision.transform.CompareTag("HealthItem"))
        {
            Health(25);
        }
    }

    void GetHit(int damage)
    {
        int difficultyMultiplier = 1;

        switch (SettingsManager.data.difficulty)
        {
            case DifficultySettings.easy:
                difficultyMultiplier = 1;
                break;
            case DifficultySettings.medium:
                difficultyMultiplier = 2;
                break;
            case DifficultySettings.hard:
                difficultyMultiplier = 3;
                break;
            case DifficultySettings.insane:
                difficultyMultiplier = 4;
                break;
        }
        rigid.velocity = new Vector2(0, 0);
        hp -= damage * difficultyMultiplier;
        if (hp < 0) { hp = 0; }
        BarUIManager.Instance.SetPlayerHp(hp, maxHp);
        //���˶���
        anim.SetTrigger("GetHit");
        //����ʱ���򷴷��򵯷�
        Vector2 force = new Vector2(50, 50);
        if (!faceLeft)
        {
            force.x *= -1;
        }
        rigid.AddForce(force);

        outControlTime = 30;
    }

    void Health(int health)
    {
        hp += health;
        if (hp > maxHp) { hp = maxHp; }
        BarUIManager.Instance.SetPlayerHp(hp, maxHp);
    }

    

}
