using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacter : MonoBehaviour
{
    public static PlayerCharacter Instance { get; private set; } // 单例模式的静态实例

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
    public float dashTime;//dash时长
    private float dashTimeLeft;//冲锋剩余时长
    private float lastDash=-10.0f;//上一次冲锋时间点
    public float dashCoolDown;
    public float dashSpeed;
    private bool isDashing = false;

    [Header("Dash Layer")]
    private int playerLayer = 6;
    private int enemyLayer = 9;
    private int enemyHitLayer = 10;

    [Header("Invincible Time Parameters")]
    private float outControlTime = 0;
    public float invincibilityDuration; // 无敌持续时间，单位为秒
    private float invincibleTimer = 0; // 无敌时间计时器
    private bool isInvincible = false; // 是否处于无敌状态


    [Header("Audio")]
    public AudioClip[] swingSounds;
    public AudioClip hitSound;

    public  Vector3 startPos;

    [Header("Attack Parameters")]
    public int attackDamage;
    public float attackRange;
    public float attackCooldown = 0.5f; // 攻击冷却时间
    private float lastAttackTime = -1f; // 上一次攻击的时间


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
                isInvincible = false; // 无敌时间结束
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
        // 更新动画状态
        anim.SetFloat("Speed", Mathf.Abs(controller.h));
        if (controller.attack && !DialogueManager.inDialogue && !isDashing)
        {
            anim.SetTrigger("Attack");
            Attack();
        }

        // 测试功能
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
                anim.SetBool("isDashing", true); // 开始冲刺动画
               
                // 关闭重力加速度
                rigid.gravityScale = 10;
                // 禁用碰撞体
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
                rigid.velocity = Vector2.zero; // 可能需要停止冲刺后的额外移动
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
        Vector3 scaleRight = new Vector3(1, 1, 1); // 朝右的缩放值
        Vector3 scaleLeft = new Vector3(-1, 1, 1); // 朝左的缩放值
        if (h > 0.1f)
        {
            faceLeft = false;
            transform.localScale = scaleRight; // 修改为朝右
        }
        else if (h < -0.1f)
        {
            faceLeft = true;
            transform.localScale = scaleLeft; // 修改为朝左
        }
    }

    

    public void GetHit(int damage)
    {
        if (isDashing) return;
        if (isInvincible) return; // 如果处于无敌状态，则不执行以下受伤逻辑

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
        // 受伤后进入无敌状态
        isInvincible = true;
        isFullHealth = false;
        invincibleTimer = invincibilityDuration;
        // 其他受伤逻辑...
        //受伤动画
        anim.SetTrigger("GetHit");
        rigid.velocity = new Vector2(0, 0);

        hp -= damage * difficultyMultiplier;

        if (hp < 0) { hp = 0; }
        BarUIManager.Instance.SetPlayerHp(hp, maxHp);

        if(hp <= 0)
        {
            GameOverManager.GameOver();
        }

        //受伤时，向反方向弹飞
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
        // 检查是否超过冷却时间
        if (Time.time - lastAttackTime < attackCooldown) return;

        // 执行攻击，并更新上一次攻击时间
        lastAttackTime = Time.time;

        // 使用OverlapCircleAll来获取范围内的所有敌人
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
