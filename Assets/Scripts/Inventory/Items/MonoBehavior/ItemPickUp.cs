using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemData_SO itemData;
    public AudioClip pickupSound;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AudioManager.PlayAudio(AudioType.soundFX, pickupSound, null, Vector2.zero, null, .6f, 1, 0, 0, 2600);

            JournalManager.FindEntry(itemData.JournalEntry);

            //TODO:����Ʒ��ӵ�����
            InventoryManager.Instance.inventoryData.AddItem(itemData, itemData.itemAmount);
            InventoryManager.Instance.inventoryUI.RefreshUI();
            Destroy(gameObject);
        }
    }
}
