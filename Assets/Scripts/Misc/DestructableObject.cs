using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : Enemy
{
    public float health = 40;
    private Collider2D col;
    private SpriteRenderer spriteRenderer;

    public AudioClip hitSound, destroySound;

    private void Start()
    {
        spawnPosition = transform.position;

        TryGetComponent(out col);
        TryGetComponent(out spriteRenderer);
    }

    public override void GetHit(int damage)
    {
        base.GetHit(damage);
        health -= damage;

        if(health <= 0)
        {
            Destroyed();
            return;
        }

        StartCoroutine(HitShake());
        AudioManager.PlayAudio(AudioType.soundFX, hitSound, null, transform.position, null, 1, Random.Range(.8f, 1.2f), 1, 0, 80);
    }

    private void Destroyed()
    {
        col.enabled = false;

        Rigidbody2D rbody = gameObject.AddComponent<Rigidbody2D>();

        rbody.AddForce(transform.up * Random.Range(4f, 12f), ForceMode2D.Impulse);
        rbody.AddTorque(Random.Range(40f, 80f));

        AudioManager.PlayAudio(AudioType.soundFX, destroySound, null, transform.position, null, 1, Random.Range(.8f, 1.2f), 1, 0, 80);

        StopAllCoroutines();
        StartCoroutine(SpriteFade());
    }

    IEnumerator SpriteFade()
    {
        spriteRenderer.color = Color.white;

        yield return new WaitForSeconds(2f);
        while (spriteRenderer.color != Color.clear)
        {
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.clear, Time.deltaTime * .5f);
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }

    IEnumerator HitShake()
    {
        int shakeCount = 0;
        while (shakeCount < Random.Range(8, 14))
        {
            Vector2 targetPos = (Vector2)spawnPosition + Random.insideUnitCircle * Random.Range(1f, 2f);
            transform.position = Vector2.MoveTowards(transform.position, targetPos, Time.deltaTime * 64f);
            shakeCount++;

            yield return new WaitForSeconds(.05f);
        }
    }
}
