using UnityEngine;

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
    public AudioClip hitSound;
    public AudioClip dieSound;

    private Transform playerTransform;
    private SpriteRenderer sprite;
    private Rigidbody2D rbody;
    private Collider2D col;

    private float hitCooldown = 0, idleTime = 0f;
    private bool dead = false;

    private Vector2 destination;

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
            if(Vector2.Distance(transform.position, playerTransform.position) > detectRange * 2f)
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
        if(Vector2.Distance(transform.position, destination) > hitRange && hitCooldown <= 0)
        {
            sprite.flipX = (destination.x > transform.position.x);
            rbody.AddForce(flySpeed * Time.deltaTime * (destination - (Vector2)transform.position).normalized, ForceMode2D.Force);
        }
        else
        {
            idleTime -= Time.deltaTime;

            if(idleTime <= 0)
            {
                NewRandomDest();
            }

            DetectHits();
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

        if (hit.transform != null && hitCooldown <= 0f)
        {
            Hit(hit.transform);
        }
    }

    void Hit(Transform player)
    {
        AudioManager.PlayAudio(AudioType.soundFX, attackSound, null, transform.position, null, 1f, 1f, 1f, 0f, 60f);
        player.GetComponent<PlayerCharacter>().GetHit(hitDamage);

        hitCooldown = 2f;
    }

    public override void GetHit(int damage)
    {
        if (dead)
        {
            return;
        }

        base.GetHit(damage);

        hitCooldown = 4f;
        health -= damage;

        if(health <= 0)
        {
            Die();
            return;
        }
        AudioManager.PlayAudio(AudioType.soundFX, hitSound, null, transform.position, null, 1f, 1f, 1f, 0f, 60f);
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

        Destroy(gameObject, 2f);
    }
}
