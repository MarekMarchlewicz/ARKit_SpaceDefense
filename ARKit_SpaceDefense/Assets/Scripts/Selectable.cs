using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class Selectable : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private GameObject draggableItem;

    [SerializeField]
    private Camera dragCamera;

	[SerializeField]
	private float startScale = 0.2f;

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
		newDraggableItem.transform.localScale = newDraggableItem.transform.localScale * startScale;

        newDraggableItem.GetComponent<DragBehaviour>().StartDragging(eventData.pointerId, dragCamera);
    }
}
