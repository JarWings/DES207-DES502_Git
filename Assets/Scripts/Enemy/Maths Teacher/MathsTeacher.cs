using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathsTeacher : MonoBehaviour
{
    public int health = 3;

    public float walkSpeed = 10f;
    public int attackDamage = 20;
    public float attackRange = 4f;
    public float viewRange = 10f;

    [Header("Audio")]
    public AudioClip alertSound;
    public AudioClip attackSound;
    public AudioClip hitSound;
    public AudioClip dieSound;

    private Transform playerTransform;
    private bool chasing = false;
    private float idleTime = 0f;

    private float attackTime = 0f;
    private float attackDelay = 0f;

    private float destination;
    private SpriteRenderer spriteRender;
    private Rigidbody2D rbody;

    private void Start()
    {
        TryGetComponent(out spriteRender);
        TryGetComponent(out rbody);
    }

    private void Update()
    {
        attackDelay = Mathf.MoveTowards(attackDelay, 0f, Time.deltaTime);
        attackTime = Mathf.MoveTowards(attackTime, 0f, Time.deltaTime);

        DestinationCheck();
        Walk();
    }

    private void DestinationCheck()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, viewRange, Vector2.up, viewRange, LayerMask.GetMask("Player"));
        if (hit.transform != null && !chasing)
        {
            chasing = true;
            playerTransform = hit.transform;

            AudioManager.PlayAudio(AudioType.soundFX, alertSound, null, transform.position, null, 1, Random.Range(.8f, 1.2f), 1, 0, 80f);
        }
        else
        {
            if (playerTransform == null || Vector2.Distance(transform.position, playerTransform.position) > viewRange * 2f)
            {
                idleTime -= Time.deltaTime;

                chasing = false;
                if (idleTime <= 0f)
                {
                    destination = Random.Range(5f, 20f);
                    idleTime = Random.Range(5f, 10f);
                }
            }
        }
    }

    private void Walk()
    {
        if (chasing && playerTransform != null)
        {
            destination = playerTransform.position.x;
        }

        bool targetRight = destination > transform.position.x;
        spriteRender.flipX = targetRight;
        float speed = walkSpeed;

        if (!targetRight)
        {
            speed = -speed;
        }

        bool inRange = Vector2.Distance(new Vector2(destination, transform.position.y), transform.position) < attackRange;

        if (inRange || attackTime > 0f)
        {
            speed = 0f;

            if (inRange && attackDelay <= 0 && attackTime <= 0)
            {
                Attack();
            }
        }

        Vector2 move = new(speed, Physics2D.gravity.y);

        rbody.velocity = move;
    }

    public void GetHit(int hits)
    {
        attackDelay = 2f;
        health -= hits;

        if (health <= 0)
        {
            AudioManager.PlayAudio(AudioType.soundFX, dieSound, null, transform.position, null, 1, Random.Range(.8f, 1.2f), 1, 0, 80f);

            Destroy(gameObject);
            return;
        }

        AudioManager.PlayAudio(AudioType.soundFX, hitSound, null, transform.position, null, 1, Random.Range(.8f, 1.2f), 1, 0, 80f);
    }

    private void Attack()
    {
        destination = transform.position.x;
        attackTime = 3f; // attack time
        attackDelay = attackTime * 2f;

        AudioManager.PlayAudio(AudioType.soundFX, attackSound, null, transform.position, null, 1, Random.Range(.8f, 1.2f), 1, 0, 80f);

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, attackRange, Vector2.up, attackRange, LayerMask.GetMask("Player"));

        if (hit.transform != null)
        {
            hit.transform.GetComponent<PlayerCharacter2>().GetHit(10);
        }
    }
}