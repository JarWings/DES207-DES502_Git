using UnityEngine;

public class Reflection : MonoBehaviour
{
    public float groundLevel;
    public SpriteRenderer TargetSprite;
    public SpriteRenderer spriteRender;

    private void Start()
    {
        TargetSprite = TargetSprite == null ? GameObject.FindWithTag("Player").GetComponent<SpriteRenderer>() : TargetSprite; // if null sets it to player sprite, added to fix issue with losing reference to player on scene changes
        spriteRender.sortingOrder = TargetSprite.sortingOrder;
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

        Vector3 pos = new (TargetSprite.transform.position.x, -(TargetSprite.transform.position.y - groundLevel), TargetSprite.transform.position.z);
        transform.position = pos;
    }
}
