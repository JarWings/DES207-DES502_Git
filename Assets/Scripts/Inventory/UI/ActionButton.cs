using UnityEngine;

public enum DPadInput { none, north, east, south, west}

public class ActionButton : MonoBehaviour
{
    public KeyCode actionKey;
    public KeyCode controllerKey;
    public DPadInput dpadKey;
    private SlotHolder currentSlotHolder;

    private void Awake()
    {
        currentSlotHolder = GetComponent<SlotHolder>();
    }

    private void Update()
    {
        float dpadhoz = Input.GetAxis("DPADHorizontal");
        float dpadvert = Input.GetAxis("DPADVertical");

        if ((Input.GetKeyDown(actionKey) || Input.GetKeyDown(controllerKey) || dpadKey == DPadInput.north && dpadvert == 1f || dpadKey == DPadInput.south && dpadvert == -1f || dpadKey == DPadInput.east && dpadhoz == 1f || dpadKey == DPadInput.west && dpadhoz == -1f) && currentSlotHolder.itemUI.GetItem()) 
        {
            currentSlotHolder.UseItem();
        }
    }
}
