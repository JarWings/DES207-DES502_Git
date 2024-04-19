using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MimicState
{
    Idle,
    Run,
}

public class Mimic : Enemy
{
    Animator anim;
    Rigidbody2D rigid;

    public int hp=1000;
    public int maxHp=1000;
    public float speed;
    public GameObject prefabItem1;
    public GameObject prefabItem2;

    MimicState state;
    float lastChangeStateTime = 0;
    bool faceRight = false;

    [Header("无敌时间参数")]
    public float invincibilityDuration; // 无敌持续时间，单位为秒
    private float invincibleTimer = 0; // 无敌时间计时器
    private bool isInvincible = false; // 是否处于无敌状态

    Vector2 startPos;

    public float jumpHeight ; // 跳跃的高度
    public float jumpInterval ; // 跳跃的时间间隔
    private float lastJumpTime; // 上次跳跃的时间记录


    void Start()
    {
        hp = maxHp;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        lastChangeStateTime = Time.time;
        startPos = transform.position;

        spawnPosition = startPos;

        lastJumpTime = Time.time; // 初始化上次跳跃时间
    }


    private void FixedUpdate()
    {
        if (hp <= 0)
        {
            return;
        }

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0)
            {
                isInvincible = false; // 无敌时间结束
            }
        }

        switch (state)
        {
            case MimicState.Idle:
                {
                    // 在合适的时机切换状态
                    if (Time.time - lastChangeStateTime > 3)
                    {
                        int r = Random.Range(1, 2);

                        if (r == 1)
                        {
                            state = MimicState.Run;
                        }
                        else if (r == 2)
                        {
                            

                        }

                        // 随机切换不同的状态（不同技能）
                        lastChangeStateTime = Time.time;
                        break;
                    }
                    rigid.velocity = new Vector2(0, rigid.velocity.y);
                }
                break;
            case MimicState.Run:
                {
                    // 状态转移条件
                    if ((faceRight && transform.position.x >= startPos.x + 20f || !faceRight && transform.position.x < startPos.x - 20f))
                    {
                        rigid.velocity = new Vector2(rigid.velocity.x, 0f);
                        Flip();
                        state = MimicState.Idle;
                        lastChangeStateTime = Time.time;
                        break;
                    }

                    else
                    {
                        // 移动的持续逻辑
                        Vector2 move = new Vector2(speed, rigid.velocity.y - 0.5f);
                        if (!faceRight)
                        {
                            move.x *= -1;
                        }
                        // 添加跳跃效果
                        if (Time.time - lastJumpTime > jumpInterval)
                        {
                            move.y = jumpHeight; // 设置向上的速度来模拟跳跃
                            lastJumpTime = Time.time; // 更新上次跳跃时间
                        }
                        rigid.velocity = move;
                    }                 
                }
                break;
        }
    }



    public  override void GetHit(int damage)
    {
        if (isInvincible || hp <= 0)
        {
            return;
        }

        hp -= damage;

        if (hp <= 0)
        {
            float chance = Random.Range(1, 3);
            GameObject itemToSpawn = null;

            if (chance == 1)
            {
                itemToSpawn = prefabItem1;
            }
            else if (chance == 2)
            {
                itemToSpawn = prefabItem2;
            }

            if (itemToSpawn != null)
            {
                // 在敌人当前位置实例化物品
                //GameObject spawnedItem = Instantiate(itemToSpawn, transform.position, Quaternion.identity);
                // 使用GameManager来延迟生成物品
                ItemManager.Instance.StartCoroutine(ItemManager.Instance.DelaySpawnItem(transform.position, itemToSpawn));
            }
            Destroy(gameObject); // 销毁敌人
        }
        else
        {
            isInvincible = true;
            invincibleTimer = invincibilityDuration;
            // 可以在这里添加受伤动画或效果
        }
    }

    void Flip()
    {
        faceRight = !faceRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1; // 直接翻转x轴的scale
        transform.localScale = scaler;
    }
}
