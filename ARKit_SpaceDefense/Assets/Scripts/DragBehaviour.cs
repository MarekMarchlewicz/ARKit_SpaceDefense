using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DragBehaviour : MonoBehaviour
{
    public event System.Action<DragBehaviour> OnDragStart, OnDragUpdate, OnDragStop;

    [SerializeField]
    private LayerMask raycastMask = 1;

    [SerializeField]
    private float maxRayDistance = 300f;

    [SerializeField]
    private float speed = 5f;

	public Vector3 positionOffset = Vector3.up * 0.001f;

    private const float defaultCameraDepth = 5f;
    
    private Transform m_Transform;
    private AudioSource m_AudioSource;

    private int touchId = -1;
    private Camera dragCamera;

    private RaycastHit[] rayHitBuffer;

    public bool IsDragged { get { return touchId >=0; } }

    public Node lastNode = null;

    private void Awake()
    {
        m_Transform = GetComponent<Transform>();
        m_AudioSource = GetComponent<AudioSource>();
        rayHitBuffer = new RaycastHit[10];
    }

    public void StartDragging(int newTouchId, Camera newDragCamera)
    {
        #if UNITY_EDITOR
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

            Vector3? newPosition = null;

            if (hits > 0)
            {
                Node node = rayHitBuffer[0].collider.GetComponent<Node>();

                if (node != null && !node.isWalkable)
                {
                    newPosition = node.transform.position + positionOffset;

                    if (lastNode != node)
                    {
                        lastNode = node;

                        if(!m_AudioSource.isPlaying)
                            m_AudioSource.Play();
                    }
                }
            }

            if (!newPosition.HasValue)
            {
                newPosition = dragCamera.ScreenToWorldPoint(new Vector3(touch.Value.x, touch.Value.y, defaultCameraDepth));
            }

            MoveTo(newPosition.Value, lerp);

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
        #if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            return Input.mousePosition;
        }
        #else
        for (int t = 0; t < Input.touchCount; t++)
        {
            if (Input.touches[t].fingerId == touchId)
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
