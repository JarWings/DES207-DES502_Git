using System.Collections;
using UnityEngine;

public class Student : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite rescuedSprite;

    public float fadeSpeed = 1.5f;

    public AudioClip rescueSound;

    private bool rescued = false;

    private void Start()
    {
        StudentManager.AddToTotalStudents();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || rescued)
        {
            return;
        }

        Rescued();
    }

    private void Rescued()
    {
        rescued = true;

        spriteRenderer.sprite = rescuedSprite;
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
