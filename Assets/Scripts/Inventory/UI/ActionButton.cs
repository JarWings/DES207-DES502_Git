using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    public KeyCode actionKey;
    public KeyCode controllerKey;
    private SlotHolder currentSlotHolder;

    private void Awake()
    {
        currentSlotHolder = GetComponent<SlotHolder>();
    }

    private void Update()
    {
        if ((Input.GetKeyDown(actionKey) || Input.GetKeyDown(controllerKey)) && currentSlotHolder.itemUI.GetItem()) 
        {
            currentSlotHolder.UseItem();
        }
    }
}
