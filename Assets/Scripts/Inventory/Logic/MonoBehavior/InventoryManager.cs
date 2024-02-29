using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public class DragData
    {
        public SlotHolder originalHolder;
        public RectTransform originalParent;
    }

    //TODO:最后添加模板用于保存数据
    [Header("Inventory Data")]
    public InventoryData_SO inventoryData;
    public InventoryData_SO actionData;

    [Header("Containers")]
    public ContainerUI inventoryUI;
    public ContainerUI actionUI;

    [Header("Drag Canvas")]
    public Canvas dragCanvas;
    public DragData currentDrag;


    [Header("UI Panel")]
    public GameObject bagPanel;

    bool isOpen = false;

    void Start()
    {
        inventoryUI.RefreshUI();
        actionUI.RefreshUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
            bagPanel.SetActive(isOpen);
        }
    }

    #region 检查拖拽物品是否在一个Slot范围内
    public bool CheckInInventoryUI(Vector3 position)
    {
        for(int i=0;i<inventoryUI.slotHolders.Length;i++)
        {
            RectTransform t = inventoryUI.slotHolders[i].transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }

        }
        return false;
    }

    public bool CheckInActionUI(Vector3 position)
    {
        for (int i = 0; i < actionUI.slotHolders.Length; i++)
        {
            RectTransform t = actionUI.slotHolders[i].transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }

        }
        return false;
    }
    #endregion
}
