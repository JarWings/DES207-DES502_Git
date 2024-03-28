using UnityEngine;

public class IconManager : MonoBehaviour
{
    private void Start()
    {
        CreateSprite();
    }

    public static SpriteRenderer CreateSprite()
    {
        GameObject sprite = new GameObject("interacticon");
        SpriteRenderer renderer =  sprite.AddComponent<SpriteRenderer>();

        renderer.sortingOrder = 100;

        return renderer;
    }

    public static void UpdateInteractIcon(SpriteRenderer renderer, Vector3 position, Sprite icon, float scale = .1f)
    {
        if (renderer == null) return;

        renderer.enabled = (icon != null);

        if (icon == null)
        {
            Destroy(renderer.gameObject);
            return;
        }

        renderer.transform.localScale = Vector3.one * scale;

        renderer.transform.position = position;
        renderer.sprite = icon;
    }
}