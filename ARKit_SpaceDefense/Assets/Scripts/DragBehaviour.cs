using UnityEngine;

public class DragBehaviour : MonoBehaviour
{
    public event System.Action<DragBehaviour> OnDragStart, OnDragUpdate, OnDragStop;

    [SerializeField]
    private LayerMask raycastMask = 1;

    [SerializeField]
    private float maxRayDistance = 300f;

    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private Vector3 positionOffset = Vector3.up * 0.5f;

    private const float defaultCameraDepth = 5f;
    
    private Transform m_Transform;

    private int touchId = -1;
    private Camera dragCamera;

    private RaycastHit[] rayHitBuffer;

    public bool IsDragged { get { return touchId >=0; } }

    private Node lastNode = null;

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

            float lerp = immediately ? 1 : Time.deltaTime * speed;

            Vector3 newPosition = Vector3.zero;

            Node node = null;

            if (hits > 0)
            {
                node = rayHitBuffer[0].collider.GetComponent<Node>();

                if (node != null)
                {
                    newPosition = node.transform.position + positionOffset;

                    lastNode = node;
                }
            }

            if (node == null)
            {
                newPosition = dragCamera.ScreenToWorldPoint(new Vector3(touch.Value.x, touch.Value.y, defaultCameraDepth));
            }

            MoveTo(newPosition, lerp);

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
        else if(lastNode != null)
        {
            m_Transform.position = lastNode.transform.position + positionOffset;

            Destroy(this);
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

    private void MoveTo(Vector3 position, float lerp)
    {
        m_Transform.position = Vector3.Lerp(m_Transform.position, position, lerp);
    }
}
