using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadTeacher : Enemy
{
    public float health = 200;

    private int slamsTillWin;

    [Header("Slam")]
    public float slamDelayTime = 4f;
    private float curSlamTime = 0f;

    public Sprite warningSprite;
    private GameObject warnObj;
    public float warningIconScale = .4f;
    public Sprite[] Debris;
    public float debrisScale = .4f;
    public float fallSpeed = 5;
    public int debrisDamage = 5;

    [Header("Shout")]
    public float shoutDelayTime = 2f;
    private float curShoutTime = 0f;

    public Sprite shoutSprite;
    public float shoutSpriteScale = 1f;
    public int shoutDamage = 10;
    public float shoutDist = 24, shoutSpeed;
    public Vector3 shoutOffset;

    [Header("Audio")]
    public AudioClip shoutSound;
    public AudioClip slamSound, hitSound, dieSound;

    private bool alive = true, shouting = false, slamming = false;
    private Animator anim;
    private SpriteRenderer sRenderer;

    private void Awake()
    {
        TryGetComponent(out anim);
        TryGetComponent(out sRenderer);
    }
    private void Update()
    {
        if (!alive || Vector2.Distance(transform.position, PlayerCharacter.Instance.transform.position) > shoutDist) return;

        curSlamTime = Mathf.MoveTowards(curSlamTime, 0f, Time.deltaTime);
        curShoutTime = Mathf.MoveTowards(curShoutTime, 0f, Time.deltaTime);

        if (Time.frameCount % 2 == 0 && curShoutTime <= 0f && !slamming) Shout();
        if (Time.frameCount % 3 == 0 && curSlamTime <= 0f && !shouting) Slam();
    }

    void Shout()
    {
        curShoutTime = Random.Range(shoutDelayTime, shoutDelayTime * 2f);
        shouting = true;

        AudioManager.PlayAudio(AudioType.soundFX, shoutSound, null, transform.position, null, 1f, Random.Range(.9f, 1.1f));
        anim.SetTrigger("shout");
        StartCoroutine(ShoutProjectile());
    }

    IEnumerator ShoutProjectile() 
    {
        yield return new WaitForSeconds(.4f);
        shouting = false;

        GameObject shoutObj = new("shout proj");
        shoutObj.transform.localScale = Vector3.one * shoutSpriteScale;
        SpriteRenderer shoutSpriteRender = shoutObj.AddComponent<SpriteRenderer>();
        shoutSpriteRender.sprite = shoutSprite;
        shoutSpriteRender.sortingOrder = 1;

        Transform proj = Instantiate(shoutObj, transform.position + shoutOffset, transform.rotation).transform;
        proj.GetComponent<SpriteRenderer>().flipX = sRenderer.flipX;

        while (proj.position.x < (sRenderer.flipX ? -transform.right : transform.right * shoutDist).x) 
        {
            proj.Translate((sRenderer.flipX ? proj.right : -proj.right) * shoutSpeed * Time.deltaTime);

            if (Vector2.Distance(proj.position, PlayerCharacter.Instance.transform.position) < .6f) 
            {
                PlayerCharacter.Instance.GetHit(shoutDamage);
            }

            yield return new WaitForEndOfFrame();
        }

        Destroy(proj.gameObject);
    }

    void Slam() 
    {
        slamming = true;

        slamsTillWin++;

        curSlamTime = Random.Range(slamDelayTime, slamDelayTime * 2f);

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
            debrisObj.transform.position = (transform.position + (sRenderer.flipX ? transform.right : -transform.right) * 5f) + new Vector3((Time.frameCount % 10 == 0) ? PlayerCharacter.Instance.transform.position.x - transform.position.x : Random.Range(-10f, 10f), Random.Range(20f, 36f));
            debrisSprite.sortingOrder = 10;
            projectileTransforms.Add(debrisObj.transform);
        }

        Collider2D[] cols = Physics2D.OverlapBoxAll(transform.position, Vector2.one * 40f, 360f);

        for (int c = 0; c < cols.Length; c++) 
        {
            if (cols[c].GetComponent<Rigidbody2D>()) cols[c].GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-.4f, .4f), 1) * 4200f * Time.deltaTime);
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

                if (Vector2.Distance(projectileTransforms[d].position, PlayerCharacter.Instance.transform.position) < 1.2f)
                {
                    PlayerCharacter.Instance.GetHit(debrisDamage);
                }


                yield return new WaitForEndOfFrame();
            }

            warningObj.SetActive(false);
            Destroy(projectileTransforms[d].gameObject);
        }

        yield return new WaitForSeconds(.5f);
        slamming = false;

        if (slamsTillWin > 8)
        {
            GameOverManager.GameOver(true);
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
