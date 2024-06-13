using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    private InventoryItem Attached => GetComponentInChildren<InventoryItem>();

    public Action OnAttached;
    public Action OnDetached;

    public bool AddStack()
    {
        InventoryItem attached = Attached;

        if (attached == null)
            return false;

        if (attached.AddStack())
            return true;

        return false;
    }

    public bool RemoveStack()
    {
        InventoryItem attached = Attached;

        if (attached == null)
            return false;

        attached.RemoveStack();
        return true;
    }

    public void Attach(InventoryItem attached)
    {
        attached.transform.SetParent(transform);
        attached.transform.localPosition = Vector3.zero;

        OnAttached?.Invoke();


    }

    public void Detach()
    {
        InventoryItem attached = Attached;

        if (attached == null)
            return;

        OnDetached?.Invoke();

    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.TryGetComponent(out InventoryItem inventoryItem))
        {
            inventoryItem.transform.SetParent(transform);
            inventoryItem.transform.localPosition = Vector3.zero;
        }
    }
}
