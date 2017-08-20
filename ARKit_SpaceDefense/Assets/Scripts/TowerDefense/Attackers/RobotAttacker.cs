using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RobotAttacker : Attacker
{
    private NavMeshAgent m_NavMeshAgent;

    [SerializeField]
    private HealthBar healthBar;

    [SerializeField]
    private Transform target;

    [SerializeField, Range(0,1)]
    private float forwardStrength = .2f;
    
    private Animator m_Animator;
    private Transform m_Transform;

    private Vector3 lastPosition;
    private float lastRotation;

    private int forwardId = Animator.StringToHash("Forward");
    private int dieId = Animator.StringToHash("Die");
    private int takeDamageId = Animator.StringToHash("Take Damage");

    private void Awake()
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_Transform = GetComponent<Transform>();

        lastPosition = m_Transform.position;
        lastRotation = m_Transform.rotation.eulerAngles.y;
        
        Initialize();
    }

    public void Initialize()
    {
        healthBar.UpdateBar(1f);
    }

    private void Update()
    {
        if(m_NavMeshAgent.isOnNavMesh)
        {
            NavMeshHit hit;

            NavMesh.SamplePosition(target.position, out hit, float.MaxValue, -1);
            
            m_NavMeshAgent.destination = hit.position;
        }

        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        float forward = (m_Transform.position - lastPosition).magnitude * forwardStrength / Time.deltaTime;

        if (forward < 0.05f)
            forward = 0f;

        m_Animator.SetFloat(forwardId, forward);

        lastPosition = m_Transform.position;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        healthBar.UpdateBar(Health / (100f));

        m_Animator.SetTrigger(takeDamageId);
    }
}
