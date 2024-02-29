using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MimicState
{
    Idle,
    Run,
}

public class Mimic : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rigid;

    public int maxHp;
    public float speed;
    public GameObject prefabItem1;
    public GameObject prefabItem2;

    int hp;

    MimicState state;
    float lastChangeStateTime = 0;
    bool faceRight = false;

    Vector2 startPos;

    void Start()
    {
        hp = maxHp;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        lastChangeStateTime = Time.time;

        startPos = transform.position;
    }


    private void Update()
    {
        if (hp <= 0)
        {
            return;
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
                    if ((faceRight && transform.position.x >= startPos.x + 5f || !faceRight && transform.position.x < startPos.x - 5f))
                    {
                        Flip();
                        state = MimicState.Idle;
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
        }
    }



    public void GetHit(int damage)
    {
        if (hp <= 0)
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
    }

    void Flip()
    {
        faceRight = !faceRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1; // 直接翻转x轴的scale
        transform.localScale = scaler;
    }
}
