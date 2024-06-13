using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private Image icon_GUI;
    [SerializeField]
    private TextMeshProUGUI stack_GUI;

    public Action OnDestory;

    private ItemData itemData;

    private int stack;

    private Transform previousParent;

    public void Setup(ItemData itemData, int stack)
    {
        this.itemData = itemData;
        this.stack = stack;
    }

    public bool AddStack()
    {
        int added = stack + 1;

        if (added > itemData.MaxStack)
            return false;

        stack = added;
        return true;
    }

    public void RemoveStack()
    {
        int removed = stack - 1;

        if (removed <= 0)
            stack = removed;

        OnDestory?.Invoke();
        Destroy(gameObject);
    }

    public int GetStack() => stack;
    public ItemData GetItemData() => itemData;

    public void UpdateGUI()
    {
        if (itemData == null)
        {
            icon_GUI.sprite = null;
        }
        else
        {
            icon_GUI.sprite = itemData.Icon;
        }

        if (stack <= 1)
        {
            stack_GUI.text = string.Empty;
        }
        else
        {
            stack_GUI.text = stack.ToString();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            icon_GUI.raycastTarget = false;
            previousParent = transform.parent;
            transform.SetParent(transform.root);
        }
        else
        {
            InventoryItem inventoryItem = Instantiate(gameObject).GetComponent<InventoryItem>();
            inventoryItem.Setup(itemData, 1);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!transform.parent.TryGetComponent(out InventorySlot inventorySlot))
        {
            transform.SetParent(previousParent);
            transform.localPosition = Vector3.zero;
        }

        icon_GUI.raycastTarget = true;
    }
}
