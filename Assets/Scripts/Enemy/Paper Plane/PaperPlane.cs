using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperPlane : MonoBehaviour
{
    public float detectRange = 10f;
    public float flySpeed = 10f;
    public float hitRange = 1f;
    public int hitDamage = 10;

    [Header("Audio")]
    public AudioClip detectSound;
    public AudioClip hitSound;
    public AudioClip missSound;

    private SpriteRenderer sprite;
    private float floorLevel = 0f;
    private bool falling = false;
    private bool hit = false;

    private void Start()
    {
        TryGetComponent(out sprite);

        flySpeed += Random.Range(-5f, 2f);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Invisible"));
        if (hit.transform != null)
        {
            floorLevel = hit.point.y;
        }
        else
        {
            floorLevel = transform.position.y - 10f;
        }
    }

    private void Update()
    {
        if (falling)
        {
            Fall();
        }
        else if(!hit)
        {
            CheckIfInRange();
        }
    }

    void CheckIfInRange()
    {
        RaycastHit2D hit = Physics2D.CircleCast(new Vector2(transform.position.x, floorLevel), detectRange, Vector2.down, detectRange, LayerMask.GetMask("Player"));

        if (hit.transform != null)
        {
            AudioManager.PlayAudio(AudioType.soundFX, detectSound, null, transform.position, null, 1f, 1f, 1f, 0f, 60f);
            falling = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!falling)
        {
            return;
        }

        Miss();
    }

    void Fall()
    {
        transform.position -= new Vector3(0f, 1f) * flySpeed * Time.deltaTime;

        if(transform.position.y < floorLevel)
        {
            Miss();
        }

        DetectHits();
    }

    void DetectHits()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, hitRange, Vector2.down, hitRange, LayerMask.GetMask("Player"));
        
        if(hit.transform != null)
        {
            Hit(hit.transform);
        }
    }

    void Hit(Transform player)
    {
        AudioManager.PlayAudio(AudioType.soundFX, hitSound, null, transform.position, null, 1f, 1f, 1f, 0f, 60f);
        player.GetComponent<PlayerCharacter2>().GetHit(hitDamage);
        falling = false;
        hit = true;

        transform.position -= new Vector3(0f, .25f);
        transform.parent = player;

        StartCoroutine(SpriteFade(2f));
    }

    void Miss()
    {
        falling = false;
        hit = true;

        AudioManager.PlayAudio(AudioType.soundFX, missSound, null, transform.position, null, 1f, 1f, 1f, 0f, 60f);
        StartCoroutine(SpriteFade(2f));
    }

    IEnumerator SpriteFade(float speed)
    {
        yield return new WaitForSeconds(2f);

        while(sprite.color != Color.clear)
        {
            sprite.color = Color.Lerp(sprite.color, Color.clear, Time.deltaTime * speed);
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
