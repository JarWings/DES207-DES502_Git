using UnityEngine;
using System.Collections;

public class ItemPickUp : MonoBehaviour
{
    public ItemData_SO itemData;
    public AudioClip pickupSound;

    private bool pickedup = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (pickedup) return;

        if (collision.CompareTag("Player"))
        {
            AudioManager.PlayAudio(AudioType.soundFX, pickupSound, null, Vector2.zero, null, .6f, 1, 0, 0, 2600);

            JournalManager.FindEntry(itemData.JournalEntry);

            //TODO:将物品添加到背包
            InventoryManager.Instance.inventoryData.AddItem(itemData, itemData.itemAmount);
            InventoryManager.Instance.inventoryUI.RefreshUI();

            GetComponent<Collider2D>().enabled = false;
            GetComponent<Rigidbody2D>().isKinematic = true;

            pickedup = true;

            StartCoroutine(PickUp());
        }
    }

    private IEnumerator PickUp()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.sortingOrder = 999;

        while (sprite.color != Color.clear) 
        {
            transform.Translate(2f * Time.deltaTime * new Vector3(0f, 1f));
            sprite.color = Color.Lerp(sprite.color, Color.clear, Time.deltaTime * 5f);
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }
}
