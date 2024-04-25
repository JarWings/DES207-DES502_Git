using System.Collections;
using UnityEngine;

public class Student : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] defaultSprites;
    public Sprite[] rescuedSprites;

    private int spriteIndex = 0;

    public float fadeSpeed = 1.5f;

    public AudioClip rescueSound;

    private bool rescued = false;

    private void Start()
    {
        spriteIndex = Random.Range(0, defaultSprites.Length);
        UpdateSprite();
        StudentManager.AddToTotalStudents();
    }

    private void UpdateSprite() 
    {
        spriteRenderer.sprite = rescued ? rescuedSprites[spriteIndex] : defaultSprites[spriteIndex];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || rescued) return;
        Rescued();
    }

    private void Rescued()
    {
        rescued = true;

        UpdateSprite();
        StartCoroutine(SpriteFade());
        AudioManager.PlayAudio(AudioType.soundFX, rescueSound, null, transform.position, null, 1, 1, 1, 0, 100);
        StudentManager.FindStudent();
    }

    IEnumerator SpriteFade()
    {
        yield return new WaitForSeconds(2f);
        while(spriteRenderer.color != Color.clear)
        {
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.clear, Time.deltaTime * fadeSpeed);
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }
}
