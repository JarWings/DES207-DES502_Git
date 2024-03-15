using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacter : MonoBehaviour
{
    public static PlayerCharacter Instance { get; private set; } // ����ģʽ�ľ�̬ʵ��

    PlayerController controller;
    Rigidbody2D rigid;
    Animator anim;

    [Header("��������")]
    public int maxHp;
    public int hp;
    public int attackDamage;
    public float attackRange;
    public float speed = 8.0f;
    public bool faceLeft = false;

    [Header("Dash����")]
    public float dashTime;//dashʱ��
    private float dashTimeLeft;//���ʣ��ʱ��
    private float lastDash=-10.0f;//��һ�γ��ʱ���
    public float dashCoolDown;
    public float dashSpeed;
    private bool isDashing = false;

    [Header("Dash Layer")]
    private int playerLayer = 6;
    private int enemyLayer = 9;
    private int enemyHitLayer = 10;

    [Header("�޵�ʱ�����")]
    private float outControlTime = 0;
    public float invincibilityDuration; // �޵г���ʱ�䣬��λΪ��
    private float invincibleTimer = 0; // �޵�ʱ���ʱ��
    private bool isInvincible = false; // �Ƿ����޵�״̬


    [Header("Audio")]
    public AudioClip[] swingSounds;

    public  Vector3 startPos;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else if (Instance != this)
        {
            Destroy(gameObject); 
        }
    }

    void Start()
    {
        controller = GetComponent<PlayerController>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        hp = maxHp;

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        Physics2D.IgnoreLayerCollision(playerLayer, enemyHitLayer, false);

        startPos = transform.position;
    }

    void Update()
    {
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0)
            {
                isInvincible = false; // �޵�ʱ�����
            }
        }

        if (controller.dash && (Time.time >= (lastDash + dashCoolDown)) && !DialogueManager.inDialogue) 
        {
            ReadyToDash();
        }

        // ���¶���״̬
        anim.SetFloat("Speed", Mathf.Abs(controller.h));
        if (controller.attack && !DialogueManager.inDialogue)
        {
            AudioManager.PlayAudio(AudioType.soundFX, null, swingSounds, transform.position, null, 1, Random.Range(.9f, 1.1f));
            anim.SetTrigger("Attack");
            Attack();
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

    public static void ResetPosition()
    {
        if(Instance == null)
        {
            return;
        }

        Instance.transform.position = Instance.startPos;
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

#region Dash
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
                rigid.gravityScale = 10;
                // ������ײ��
                Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
                Physics2D.IgnoreLayerCollision(playerLayer, enemyHitLayer, true);
                rigid.velocity = new Vector2(dashSpeed * HorizontalDir(faceLeft), rigid.velocity.y);
                dashTimeLeft -= Time.deltaTime;
            }
            else
            {
                isDashing = false;
                anim.SetBool("isDashing", false);
                Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
                Physics2D.IgnoreLayerCollision(playerLayer, enemyHitLayer, false);
                rigid.velocity = Vector2.zero; // ������Ҫֹͣ��̺�Ķ����ƶ�
            }
        }
    }

#endregion

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

    void Flip(float h)
    {
        Vector3 scaleRight = new Vector3(1, 1, 1); // ���ҵ�����ֵ
        Vector3 scaleLeft = new Vector3(-1, 1, 1); // ���������ֵ
        if (h > 0.1f)
        {
            faceLeft = false;
            transform.localScale = scaleRight; // �޸�Ϊ����
        }
        else if (h < -0.1f)
        {
            faceLeft = true;
            transform.localScale = scaleLeft; // �޸�Ϊ����
        }
    }

    

    public void GetHit(int damage)
    {
        if (isDashing) return;
        if (isInvincible) return; // ��������޵�״̬����ִ�����������߼�

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

        // ���˺�����޵�״̬
        isInvincible = true;
        invincibleTimer = invincibilityDuration;
        // ���������߼�...
        //���˶���
        anim.SetTrigger("GetHit");
        rigid.velocity = new Vector2(0, 0);

        hp -= damage * difficultyMultiplier;

        if (hp < 0) { hp = 0; }
        BarUIManager.Instance.SetPlayerHp(hp, maxHp);

        if(hp <= 0)
        {
            SceneChangeManager.LoadScene("MainMenu");
        }

        //����ʱ���򷴷��򵯷�
        Vector2 force = new Vector2(50 * HorizontalDir(faceLeft), 50);
        rigid.AddForce(force);

        outControlTime = 30;
    }

    public void Health(int health)
    {
        hp += health;
        if (hp > maxHp) { hp = maxHp; }
        BarUIManager.Instance.SetPlayerHp(hp, maxHp);
    }

    void Attack()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, attackRange, Vector2.up, attackRange, LayerMask.GetMask("Enemy"));
        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.GetHit(attackDamage);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Boss") || collision.transform.CompareTag("Enemy"))
        {
            if (this.CompareTag("Player") && !isDashing)
            {
                GetHit(10);
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BossHit"))
        {
            GetHit(1);
        }
    }

}
