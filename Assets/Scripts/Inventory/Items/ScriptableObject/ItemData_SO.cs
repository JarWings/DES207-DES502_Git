using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum ItemType { Useable, Key, Weapon, Armor }
public enum ItemType { Useable, Key}
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")] 
public class ItemData_SO : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public int itemAmount;
    public bool stackable;

    public string JournalEntry = "";

    [TextArea]
    public string description = "";

    [Header("Usable Item")]
    public UseableItemData_SO useableData;
}
