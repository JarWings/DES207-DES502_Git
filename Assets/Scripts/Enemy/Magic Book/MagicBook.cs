using System.Collections;
using UnityEngine;

public class MagicBook : Enemy
{
    public float detectRange = 10f;
    public float flySpeed = 10f;
    public float hitRange = 1f;
    public int hitDamage = 10;

    [Header("Audio")]
    public AudioClip detectSound;
    public AudioClip attackSound;
    public AudioClip hitSound;

    private Transform playerTransform;
    private SpriteRenderer sprite;
    private float hitCooldown = 0;

    private void Start()
    {
        TryGetComponent(out sprite);

        spawnPosition = transform.position;
    }

    private void Update()
    {
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
        }
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
}
