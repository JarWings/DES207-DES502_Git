using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadTeacher : Enemy
{
    public float health = 200;

    [Header("Slam")]
    public Sprite warningSprite;
    private GameObject warnObj;
    public float warningIconScale = .4f;
    public Sprite[] Debris;
    public float debrisScale = .4f;
    public float fallSpeed = 5;
    public int debrisDamage = 5;

    [Header("Shout")]
    public Sprite shoutSprite;
    public float shoutSpriteScale = 1f;
    public int shoutDamage = 10;
    public float shoutDist = 24, shoutSpeed;
    public Vector3 shoutOffset;

    [Header("Audio")]
    public AudioClip shoutSound;
    public AudioClip slamSound, hitSound, dieSound;

    public PlayerCharacter player;
    private bool alive = true;
    private Animator anim;

    private void Awake()
    {
        TryGetComponent(out anim);
    }
    private void Update()
    {
        if (!alive) return;

        if (Input.GetKeyDown(KeyCode.Y)) Shout();
        if (Input.GetKeyDown(KeyCode.U)) Slam();
    }

    void Shout()
    {
        AudioManager.PlayAudio(AudioType.soundFX, shoutSound, null, transform.position, null, 1f, Random.Range(.9f, 1.1f));
        anim.SetTrigger("shout");
        StartCoroutine(ShoutProjectile());
    }

    IEnumerator ShoutProjectile() 
    {
        yield return new WaitForSeconds(.4f);

        GameObject shoutObj = new("shout proj");
        shoutObj.transform.localScale = Vector3.one * shoutSpriteScale;
        SpriteRenderer shoutSpriteRender = shoutObj.AddComponent<SpriteRenderer>();
        shoutSpriteRender.sprite = shoutSprite;
        shoutSpriteRender.sortingOrder = 1;

        Transform proj = Instantiate(shoutObj, transform.position + shoutOffset, transform.rotation).transform;

        while (proj.position.x < (-transform.right * shoutDist).x) 
        {
            proj.Translate(-proj.right * shoutSpeed * Time.deltaTime);

            if (Vector2.Distance(proj.position, player.transform.position) < .6f) 
            {
                player.GetHit(shoutDamage);
            }

            yield return new WaitForEndOfFrame();
        }

        Destroy(proj.gameObject);
    }

    void Slam() 
    {
        AudioManager.PlayAudio(AudioType.soundFX, slamSound, null, transform.position, null, 1f, Random.Range(.9f, 1.1f));
        anim.SetTrigger("slam");
        StartCoroutine(SlamProjectiles());
    }

    IEnumerator SlamProjectiles() 
    {
        yield return new WaitForSeconds(.5f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 20f, LayerMask.GetMask("Destructible"));
        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.GetHit(Random.Range(5, 10));
            }
        }

        int projectileCount = Random.Range(4, 8);
        List<Transform> projectileTransforms = new();

        for (int i = 0; i < projectileCount; i++) 
        {
            GameObject debrisObj = new("debris");
            SpriteRenderer debrisSprite = debrisObj.AddComponent<SpriteRenderer>();
            debrisSprite.sprite = Debris[Random.Range(0, Debris.Length)];
            debrisObj.transform.localScale = Vector3.one * debrisScale;
            debrisObj.transform.position = (transform.position + -transform.right * 5f) + new Vector3((Time.frameCount % 10 == 0) ? player.transform.position.x - transform.position.x : Random.Range(-10f, 10f), Random.Range(10f, 26f));
            debrisSprite.sortingOrder = 10;
            projectileTransforms.Add(debrisObj.transform);
        }

        Collider2D[] cols = Physics2D.OverlapBoxAll(transform.position, Vector2.one * 40f, 360f);

        for (int c = 0; c < cols.Length; c++) 
        {
            if (cols[c].GetComponent<Rigidbody2D>()) cols[c].GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-.4f, .4f), 1) * 4200f * Time.deltaTime, ForceMode2D.Impulse);
        }

        for (int d = 0; d < projectileTransforms.Count; d++) 
        {
            bool hasWarn = warnObj != null;
            GameObject warningObj = hasWarn ? warnObj : new("debris warning sprite");
            warningObj.SetActive(true);

            SpriteRenderer warnSpriteRender = hasWarn ? warningObj.GetComponent<SpriteRenderer>() : warningObj.AddComponent<SpriteRenderer>();
            warnSpriteRender.sprite = warningSprite;
            warnSpriteRender.sortingOrder = 9999;

            warningObj.transform.localScale = Vector3.one * warningIconScale;
            warningObj.transform.position = new Vector3(projectileTransforms[d].position.x, transform.position.y);

            while (projectileTransforms[d].position.y > transform.position.y - 4f) 
            {
                if(Time.frameCount % 60 == 0) warnSpriteRender.enabled = !warnSpriteRender.enabled;
                projectileTransforms[d].Translate(-Vector3.up * fallSpeed * Time.deltaTime, Space.World);
                projectileTransforms[d].Rotate(transform.forward * (d % 2 == 0 ? -64f : 64f) * Time.deltaTime);

                if (Vector2.Distance(projectileTransforms[d].position, player.transform.position) < 1.2f)
                {
                    player.GetHit(debrisDamage);
                }


                yield return new WaitForEndOfFrame();
            }

            warningObj.SetActive(false);
            Destroy(projectileTransforms[d].gameObject);
        }
    }

    void Hit()
    {
        anim.SetTrigger("hit");
    }

    void Die() 
    {
        alive = false;

        anim.SetTrigger("die");
    }
}
