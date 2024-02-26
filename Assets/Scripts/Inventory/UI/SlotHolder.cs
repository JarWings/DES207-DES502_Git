using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlotType { BAG,USEABLE,KEY}

public class SlotHolder : MonoBehaviour
{
    public SlotType slotType;
    public ItemUI itemUI;

    public void UpdateItem()
    {
        switch (slotType)
        {
            case SlotType.BAG:
                itemUI.Bag = InventoryManager.Instance.inventoryData;
                break;
            case SlotType.USEABLE:
                break;
            case SlotType.KEY:
                break;
        }

        var item = itemUI.Bag.items[itemUI.Index];
        itemUI.SetupItemUI(item.ItemData, item.amount);
    }
}
