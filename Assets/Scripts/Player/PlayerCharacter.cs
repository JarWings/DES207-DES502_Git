using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacter : MonoBehaviour
{
    public static PlayerCharacter Instance { get; private set; } // ����ģʽ�ľ�̬ʵ��

    SpriteRenderer playerSprite;
    PlayerController controller;
    Rigidbody2D rigid;
    Animator anim;

    [Header("CD")]
    private Image DashCDImage;
    private Image GetHitCDImage;

    [Header("Basic Parameters")]
    public int maxHp;
    public int hp;
    public float speed = 8.0f;
    public bool faceLeft = false;
    public bool isFullHealth;

    [Header("Dash Parameters")]
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

    [Header("Invincible Time Parameters")]
    private float outControlTime = 0;
    public float invincibilityDuration; // �޵г���ʱ�䣬��λΪ��
    private float invincibleTimer = 0; // �޵�ʱ���ʱ��
    private bool isInvincible = false; // �Ƿ����޵�״̬


    [Header("Audio")]
    public AudioClip[] swingSounds;
    public AudioClip hitSound;

    public  Vector3 startPos;

    [Header("Attack Parameters")]
    public int attackDamage;
    public float attackRange;
    public float attackCooldown = 0.5f; // ������ȴʱ��
    private float lastAttackTime = -1f; // ��һ�ι�����ʱ��


    public List<GameObject> dashFrames = new();

    private GameObject dashCDGameObject;
    private GameObject getHitCDGameObject;

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
        isFullHealth = true;
    }

    void Start()
    {
        controller = GetComponent<PlayerController>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
        hp = maxHp;

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        Physics2D.IgnoreLayerCollision(playerLayer, enemyHitLayer, false);

        startPos = transform.position;

    }

    void Update()
    {
        dashCDGameObject = GameObject.FindWithTag("DashCD");
        if (dashCDGameObject != null)
        {
            DashCDImage = dashCDGameObject.GetComponent<Image>();
        }

        getHitCDGameObject = GameObject.FindWithTag("GetHitCD");
        if (getHitCDGameObject != null)
        {
            GetHitCDImage = getHitCDGameObject.GetComponent<Image>();
        }

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

        if(DashCDImage!=null && GetHitCDImage != null)
        {
            DashCDImage.fillAmount -= 1.0f / dashCoolDown * Time.deltaTime;
            GetHitCDImage.fillAmount -= 1.0f / invincibilityDuration * Time.deltaTime;
        }
        // ���¶���״̬
        anim.SetFloat("Speed", Mathf.Abs(controller.h));
        if (controller.attack && !DialogueManager.inDialogue && !isDashing)
        {
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
        if (isAttacking())
        {
            return;
        }

        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        EmptyDashFrames();
        StopAllCoroutines();
        StartCoroutine(DashEffect());


        DashCDImage.fillAmount = 1;
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

        AudioManager.PlayAudio(AudioType.soundFX, hitSound, null, transform.position, null, 1, Random.Range(.9f, 1.1f));

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

        GetHitCDImage.fillAmount = 1;
        // ���˺�����޵�״̬
        isInvincible = true;
        isFullHealth = false;
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
            GameOverManager.GameOver();
        }

        //����ʱ���򷴷��򵯷�
        Vector2 force = new (50 * HorizontalDir(faceLeft), 50);
        rigid.AddForce(force);

        outControlTime = 30;
    }

    public void Health(int health)
    {
        hp += health;
        if (hp > maxHp) 
        { 
            hp = maxHp; 
            isFullHealth=true;
        }
        BarUIManager.Instance.SetPlayerHp(hp, maxHp);
    }

    void Attack()
    {
        // ����Ƿ񳬹���ȴʱ��
        if (Time.time - lastAttackTime < attackCooldown) return;

        // ִ�й�������������һ�ι���ʱ��
        lastAttackTime = Time.time;

        // ʹ��OverlapCircleAll����ȡ��Χ�ڵ����е���
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Enemy", "Destructible"));
        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.GetHit(attackDamage);
            }
        }

        AudioManager.PlayAudio(AudioType.soundFX, null, swingSounds, transform.position, null, 1, Random.Range(.9f, 1.1f));
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

    IEnumerator DashEffect()
    {
        int frames = 0;

        while (isDashing)
        {
            GameObject spriteObj;
            SpriteRenderer dashFrame;

            if (dashFrames.Count < 12 || frames >= dashFrames.Count)
            {
                spriteObj = new("DashFrame (" + frames + ")");
                dashFrame = spriteObj.AddComponent<SpriteRenderer>();

                dashFrames.Add(spriteObj);
            }
            else
            {
                Debug.Log("frame: " + frames + ", total: " + dashFrames.Count);
                spriteObj = dashFrames[frames];
                dashFrame = dashFrames[frames].GetComponent<SpriteRenderer>();
            }

            spriteObj.transform.position = transform.position;
            spriteObj.transform.localScale = transform.localScale;

            dashFrame.sprite = playerSprite.sprite;
            dashFrame.color = Color.white * (.8f * frames);

            frames++;

            StartCoroutine(SpriteFade(dashFrame, 16f));
            yield return new WaitForSeconds(.03f);
        }
    }

    IEnumerator SpriteFade(SpriteRenderer sprite, float rate)
    {
        while(sprite != null && sprite.color.a > 0)
        {
            sprite.color = Color.Lerp(sprite.color, Color.clear, Time.deltaTime * rate);
            yield return new WaitForEndOfFrame();
        }
    }

    private void EmptyDashFrames()
    {
        for(int i = 0; i < dashFrames.Count; i++)
        {
            Destroy(dashFrames[i]);
        }

        dashFrames.Clear();
    }
}
