using UnityEngine;

public class Reflection : MonoBehaviour
{
    public float groundLevel;
    public SpriteRenderer TargetSprite;
    public SpriteRenderer spriteRender;

    private void Start()
    {
        if (TargetSprite == null)
        {
            spriteRender.sortingOrder = TargetSprite.sortingOrder;
        }
    }

    private void Update()
    {
        if(TargetSprite == null)
        {
            spriteRender.enabled = false;
            return;
        }

        spriteRender.transform.localScale = TargetSprite.transform.localScale;
        spriteRender.sprite = TargetSprite.sprite;

        Vector2 pos = new Vector2(TargetSprite.transform.position.x, -(TargetSprite.transform.position.y - groundLevel));
        transform.position = pos;
    }
}
