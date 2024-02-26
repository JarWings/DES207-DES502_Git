using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Useable, Key, Weapon, Armor }
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")] 
public class ItemData_SO : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public int itemAmount;
    public bool stackable;
    [TextArea]
    public string description = "";
}
