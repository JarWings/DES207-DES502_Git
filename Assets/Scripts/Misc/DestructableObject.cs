using System.Collections;
using UnityEngine;

public class DestructableObject : Enemy
{
    public float health = 40;
    private Collider2D col;
    private SpriteRenderer spriteRenderer;

    public GameObject destroyPrefab;
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

        if (health <= 0)
        {
            Destroyed();
            return;
        }

        StopAllCoroutines();
        StartCoroutine(HitShake());
        AudioManager.PlayAudio(AudioType.soundFX, hitSound, null, transform.position, null, 1, Random.Range(.8f, 1.2f), 1, 0, 80);
    }

    private void Destroyed()
    {
        col.enabled = false;

        Rigidbody2D rbody = gameObject.AddComponent<Rigidbody2D>();

        rbody.AddForce(transform.up * Random.Range(8f, 12f), ForceMode2D.Impulse);
        rbody.AddTorque(Random.Range(40f, 80f));

        AudioManager.PlayAudio(AudioType.soundFX, destroySound, null, transform.position, null, 1, Random.Range(.8f, 1.2f), 1, 0, 80);

        if (destroyPrefab != null) Instantiate(destroyPrefab, transform.position, Quaternion.identity);

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
        int totalShakes = Random.Range(8, 14);

        while (shakeCount < totalShakes)
        {
            Vector3 targetPos = spawnPosition + (Vector3)Random.insideUnitCircle * Random.Range(.5f, 1f);
            if (shakeCount >= totalShakes) targetPos = spawnPosition;

            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * 64f);
            shakeCount++;

            yield return new WaitForSeconds(.05f);
        }
    }
}
