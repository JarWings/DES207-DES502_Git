using UnityEngine;

public class Crack : MonoBehaviour
{
    private SpriteRenderer sRender;
    public Sprite[] crackSprites;
    private int currentIndex = 0;

    private void Awake()
    {
        TryGetComponent(out sRender);
        UpdateSprite();

        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
    }

    public void NextCrack() 
    {
        currentIndex = currentIndex >= crackSprites.Length - 1 ? currentIndex : currentIndex + 1;
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        sRender.sprite = crackSprites[currentIndex];
    }
}
