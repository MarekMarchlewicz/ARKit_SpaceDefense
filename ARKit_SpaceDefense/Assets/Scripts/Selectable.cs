﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class Selectable : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private GameObject draggableItem;

    [SerializeField]
    private Camera dragCamera;

    private void OnValidate()
    {
        if(dragCamera == null)
        {
            dragCamera = Camera.main;
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject newDraggableItem = Instantiate(draggableItem);

        newDraggableItem.GetComponent<DragBehaviour>().StartDragging(eventData.pointerId, dragCamera);
    }
}
