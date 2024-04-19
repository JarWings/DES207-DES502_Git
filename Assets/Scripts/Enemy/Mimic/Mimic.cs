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

    [Header("�޵�ʱ�����")]
    public float invincibilityDuration; // �޵г���ʱ�䣬��λΪ��
    private float invincibleTimer = 0; // �޵�ʱ���ʱ��
    private bool isInvincible = false; // �Ƿ����޵�״̬

    Vector2 startPos;

    public float jumpHeight ; // ��Ծ�ĸ߶�
    public float jumpInterval ; // ��Ծ��ʱ����
    private float lastJumpTime; // �ϴ���Ծ��ʱ���¼


    void Start()
    {
        hp = maxHp;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        lastChangeStateTime = Time.time;
        startPos = transform.position;

        spawnPosition = startPos;

        lastJumpTime = Time.time; // ��ʼ���ϴ���Ծʱ��
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
                isInvincible = false; // �޵�ʱ�����
            }
        }

        switch (state)
        {
            case MimicState.Idle:
                {
                    // �ں��ʵ�ʱ���л�״̬
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

                        // ����л���ͬ��״̬����ͬ���ܣ�
                        lastChangeStateTime = Time.time;
                        break;
                    }
                    rigid.velocity = new Vector2(0, rigid.velocity.y);
                }
                break;
            case MimicState.Run:
                {
                    // ״̬ת������
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
                        // �ƶ��ĳ����߼�
                        Vector2 move = new Vector2(speed, rigid.velocity.y - 0.5f);
                        if (!faceRight)
                        {
                            move.x *= -1;
                        }
                        // �����ԾЧ��
                        if (Time.time - lastJumpTime > jumpInterval)
                        {
                            move.y = jumpHeight; // �������ϵ��ٶ���ģ����Ծ
                            lastJumpTime = Time.time; // �����ϴ���Ծʱ��
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
                // �ڵ��˵�ǰλ��ʵ������Ʒ
                //GameObject spawnedItem = Instantiate(itemToSpawn, transform.position, Quaternion.identity);
                // ʹ��GameManager���ӳ�������Ʒ
                ItemManager.Instance.StartCoroutine(ItemManager.Instance.DelaySpawnItem(transform.position, itemToSpawn));
            }
            Destroy(gameObject); // ���ٵ���
        }
        else
        {
            isInvincible = true;
            invincibleTimer = invincibilityDuration;
            // ����������������˶�����Ч��
        }
    }

    void Flip()
    {
        faceRight = !faceRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1; // ֱ�ӷ�תx���scale
        transform.localScale = scaler;
    }
}
