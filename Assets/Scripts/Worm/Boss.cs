using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossState
{
    Idle,
    Run,
    Skill_FireBall,
    Skill_FireRain,
}

public class Boss : MonoBehaviour
{

    Animator anim;
    Rigidbody2D rigid;

    public int maxHp = 20;
    public float speed = 8;
    public FireBall prefabFireBall;
    public FireRain prefabFireRain;
    int hp;

    BossState state;
    float lastChangeStateTime = 0;
    bool faceRight = true;
    Transform firePoint;

    void Start()
    {
        hp = maxHp;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        lastChangeStateTime = Time.time;
        firePoint = transform.Find("FirePoint");
    }


    private void Update()
    {
        if (hp <= 0)
        {
            return;
        }

        switch (state)
        {
            case BossState.Idle:
                {
                    // 在合适的时机切换状态
                    if (Time.time - lastChangeStateTime > 3)
                    {
                        //int r = Random.Range(1, 4);
                        int r = 3;

                        if (r == 1)
                        {
                            state = BossState.Run;
                        }
                        else if (r == 2)
                        {
                            state = BossState.Skill_FireBall;
                            // 开启吐火球协程
                            //StartCoroutine(CoFireBallState());
                            
                        }
                        else if (r == 3)
                        {
                            state = BossState.Skill_FireRain;
                            // 开启火雨协程
                            //StartCoroutine(CoFireRainState());
                        }

                        // 随机切换不同的状态（不同技能）
                        lastChangeStateTime = Time.time;
                        break;
                    }
                    rigid.velocity = new Vector2(0, rigid.velocity.y);
                }
                break;
            case BossState.Run:
                {
                    // 状态转移条件
                    if (faceRight && transform.position.x >= 20 || !faceRight && transform.position.x < -20)
                    {
                        Flip();
                        state = BossState.Idle;
                        lastChangeStateTime = Time.time;
                        break;
                    }

                    // 移动的持续逻辑
                    Vector2 move = new Vector2(speed, rigid.velocity.y - 0.5f);
                    if (!faceRight)
                    {
                        move.x *= -1;
                    }
                    rigid.velocity = move;
                }
                break;
            case BossState.Skill_FireBall:
                {
                    // 定时吐火球的逻辑可以写到协程中
                }
                break;
            case BossState.Skill_FireRain:
                {
                }
                break;
        }
    }

    IEnumerator CoFireBallState()
    {
        for (int i = 0; i < 3; i++)
        {
            anim.SetTrigger("Attack");
            // Attack动画播放时，通过动画帧事件调用Fire函数

            yield return new WaitForSeconds(1.5f);
        }
        state = BossState.Idle;
        lastChangeStateTime = Time.time;
    }

    public void FireBall()
    {
        if(state != BossState.Skill_FireBall)
        {
            for (int j = 0; j < 10; j++)
            {
                float r = Random.Range(-15, 15);
                FireRain firerain = Instantiate(prefabFireRain, new Vector3(r, 11, 0), Quaternion.identity);
            }
        }
        if(state == BossState.Skill_FireBall)
        {
            FireBall ball = Instantiate(prefabFireBall, firePoint.position, Quaternion.identity);
            if (!faceRight)
            {
                ball.transform.right = Vector3.left;
            }
            Destroy(ball.gameObject, 5f);
        }
    }

    IEnumerator CoFireRainState()
    {
        for (int i = 0; i < 2; i++)
        {
            anim.SetTrigger("Attack");
            // Attack动画播放时，通过动画帧事件调用Fire函数
            yield return new WaitForSeconds(1.5f);
        }
        state = BossState.Idle;
        lastChangeStateTime = Time.time;
    }


    public void GetHit(int damage)
    {
        if (hp <= 0)
        {
            return;
        }

        hp -= damage;
        //if (hp < 0) { hp = 0; }
        BarUIManager.Instance.SetBossHp(hp, maxHp);
        anim.SetTrigger("GetHit");

        if (hp <= 0)
        {
            anim.SetTrigger("Die");
            rigid.isKinematic = true;
            rigid.velocity = Vector2.zero; // 停止所有运动
            state = BossState.Idle;
            Collider2D[] colliders = GetComponents<Collider2D>();
            foreach (var c in colliders)
            {
                c.enabled = false;
            }
            this.enabled = false;
        }
    }

    void Flip()
    {
        faceRight = !faceRight;
        Vector3 scaleRight = new Vector3(1, 1, 1);
        Vector3 scaleLeft = new Vector3(-1, 1, 1);
        if (faceRight)
        {
            transform.localScale = scaleRight;
        }
        else
        {
            transform.localScale = scaleLeft;
        }
    }
}
