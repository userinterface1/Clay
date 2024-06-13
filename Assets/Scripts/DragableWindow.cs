using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragableWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Vector2 offset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 v = new Vector2(transform.position.x, transform.position.y);
        offset = v - eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }
}
