using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicBook : Enemy
{
    public int health = 50;

    public float detectRange = 5f;
    public float flySpeed = 150f;
    public float hitRange = 2f;
    public int hitDamage = 10;

    [Header("Audio")]
    public AudioClip detectSound;
    public AudioClip attackSound;
    public AudioClip[] hitSound;
    public AudioClip dieSound;

    private Transform playerTransform;
    private SpriteRenderer sprite;
    private Rigidbody2D rbody;
    private Collider2D col;

    private float hitCooldown = 0, idleTime = 0f;
    private bool dead = false, attacking = false;

    private Vector2 destination;
    private List<GameObject> dashFrames = new();

    private void Start()
    {
        TryGetComponent(out sprite);
        TryGetComponent(out rbody);
        TryGetComponent(out col);

        spawnPosition = transform.position;
        NewRandomDest();
    }

    private void Update()
    {
        if (dead)
        {
            return;
        }

        hitCooldown = Mathf.MoveTowards(hitCooldown, 0f, Time.deltaTime);

        if (playerTransform == null)
        {
            CheckIfInRange();
        }
        else
        {
            if (Vector2.Distance(transform.position, playerTransform.position) > detectRange * 2f)
            {
                playerTransform = null;
            }
            else
            {
                destination = playerTransform.position;
            }
        }

        Fly();
    }

    void Fly()
    {
        if (Vector2.Distance(transform.position, destination) > hitRange)
        {
            sprite.flipX = (destination.x > transform.position.x);
            rbody.AddForce(flySpeed * Time.deltaTime * (destination - (Vector2)transform.position).normalized, ForceMode2D.Force);

            attacking = false;
        }
        else
        {
            idleTime -= Time.deltaTime;

            if (idleTime <= 0) NewRandomDest(); // used for idle flying

            if (!attacking && playerTransform != null)
            {
                Attack();
            }
        }
    }

    void NewRandomDest()
    {
        destination = (Vector2)spawnPosition + Random.insideUnitCircle * Random.Range(detectRange / 2f, detectRange);
        idleTime = 4f;
    }

    void CheckIfInRange()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, detectRange, Vector2.down, detectRange, LayerMask.GetMask("Player"));

        if (hit.transform != null)
        {
            playerTransform = hit.transform;
            AudioManager.PlayAudio(AudioType.soundFX, detectSound, null, transform.position, null, 1f, 1f, 1f, 0f, 60f);
        }
    }

    void DetectHits()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, hitRange, Vector2.down, hitRange, LayerMask.GetMask("Player"));

        if (hit.transform != null && hitCooldown <= 0f && attacking)
        {
            Hit(hit.transform);
        }
    }

    void Hit(Transform player)
    {
        AudioManager.PlayAudio(AudioType.soundFX, attackSound, null, transform.position, null, 1f, 1f, 1f, 0f, 60f);
        player.GetComponent<PlayerCharacter>().GetHit(hitDamage);

        hitCooldown = 2f;
        attacking = false;
    }

    public override void GetHit(int damage)
    {
        if (dead) return;

        base.GetHit(damage);

        hitCooldown = 4f;
        health -= damage;

        attacking = false;

        if (health <= 0)
        {
            Die();
            return;
        }
        AudioManager.PlayAudio(AudioType.soundFX, null, hitSound, transform.position, null, 1f, 1f, 1f, 0f, 60f);

        rbody.AddForce(-destination.normalized * 5f, ForceMode2D.Impulse); // used for knock-back effect
    }

    private void Die()
    {
        AudioManager.PlayAudio(AudioType.soundFX, dieSound, null, transform.position, null, 1f, 1f, 1f, 0f, 60f);
        dead = true;

        rbody.constraints = RigidbodyConstraints2D.None;
        rbody.gravityScale = 1f;

        rbody.AddForce(transform.up * Random.Range(6f, 8f), ForceMode2D.Impulse);
        rbody.AddTorque(Random.Range(40f, 80f));

        col.enabled = false;

        EmptyDashFrames();

        sprite.color = Color.black;

        StopAllCoroutines();
        StartCoroutine(SpriteFade(sprite, .4f, true));
    }

    private void Attack()
    {
        attacking = true;
        EmptyDashFrames();
        StopAllCoroutines();
        StartCoroutine(DashEffect());

        destination = playerTransform.position;

        rbody.velocity = Vector2.zero;
        rbody.AddForce((destination - (Vector2)transform.position).normalized * 10f, ForceMode2D.Impulse); // dashs towards player
    }

    IEnumerator DashEffect()
    {
        int frames = 0;

        while (attacking)
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
                spriteObj = dashFrames[frames];
                dashFrame = dashFrames[frames].GetComponent<SpriteRenderer>();
            }

            spriteObj.transform.position = transform.position;
            spriteObj.transform.localScale = transform.localScale;

            dashFrame.sprite = sprite.sprite;
            dashFrame.color = Color.white * (.8f * frames);

            frames++;

            DetectHits();

            StartCoroutine(SpriteFade(dashFrame, 16f));
            yield return new WaitForSeconds(.05f);
        }
    }

    IEnumerator SpriteFade(SpriteRenderer sprite, float rate, bool destroy = false)
    {
        while (sprite != null && sprite.color.a > 0f)
        {
            sprite.color = Color.Lerp(sprite.color, Color.clear, Time.deltaTime * rate);
            if(destroy && sprite.color.a <= .1f) Destroy(gameObject);
            yield return new WaitForEndOfFrame();
        }
    }

    private void EmptyDashFrames()
    {
        for (int i = 0; i < dashFrames.Count; i++) Destroy(dashFrames[i]);
        dashFrames.Clear();
    }
}
