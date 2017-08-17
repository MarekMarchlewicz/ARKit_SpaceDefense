using UnityEngine;
using UnityEngine.EventSystems;

public class DragBehaviour : MonoBehaviour
{
    public event System.Action<DragBehaviour> OnDragStart, OnDragUpdate, OnDragStop;

    [SerializeField]
    private LayerMask raycastMask = 1;

    [SerializeField]
    private float maxRayDistance = 300f;

    [SerializeField]
    private float smoothing = 5f;

    private const float defaultCameraDepth = 5f;
    
    private Transform m_Transform;

    private int touchId = -1;
    private Camera dragCamera;

    private RaycastHit[] rayHitBuffer;

    public bool IsDragged { get { return touchId >=0; } }

    private void Awake()
    {
        m_Transform = GetComponent<Transform>();
        rayHitBuffer = new RaycastHit[10];
    }

    public void StartDragging(int newTouchId, Camera newDragCamera)
    {
        #if UNITY_STANDALONE
        touchId = 1;
        #else
        touchId = newTouchId;
        #endif
        dragCamera = newDragCamera;

        UpdateDragging(true);

        if (OnDragStart != null)
        {
            OnDragStart(this);
        }
    }

    public void StopDragging()
    {
        if (OnDragStop != null)
        {
            OnDragStop(this);
        }

        touchId = -1;
    }

    private void UpdateDragging(bool immediately = false)
    {
        Vector2? touch = GetTouchPosition();

        if (touch.HasValue)
        {
            int hits = Physics.RaycastNonAlloc(dragCamera.ScreenPointToRay(touch.Value), rayHitBuffer, maxRayDistance, raycastMask);

            float lerp = immediately ? 1 : Time.deltaTime * smoothing;

            Vector3 newPosition = Vector3.zero;

            if (hits > 0)
                newPosition = rayHitBuffer[0].point;
            else
                newPosition = dragCamera.ScreenToWorldPoint(new Vector3(touch.Value.x, touch.Value.y, defaultCameraDepth));
            
            m_Transform.position = Vector3.Lerp(m_Transform.position, newPosition, lerp);

            if(OnDragUpdate != null)
            {
                OnDragUpdate(this);
            }
        }
        else
        {
            StopDragging();
        }
    }

    private void Update()
    {
        if (IsDragged)
        {
            UpdateDragging();
        }
    }

    private Vector2? GetTouchPosition()
    {
        #if UNITY_STANDALONE
        if (Input.GetMouseButton(0))
        {
            return Input.mousePosition;
        }
        #else
        for (int t = 0; t < Input.touchCount; t++)
        {
            if (Input.touches[t].fingerId == touchId.Value)
            {
                return Input.touches[t].position;
            }
        }
        #endif

        return null;
    }
}
