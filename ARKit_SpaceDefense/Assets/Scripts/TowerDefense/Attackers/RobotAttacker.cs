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

    public void Initialize(Vector3 goal)
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_Transform = GetComponent<Transform>();

        lastPosition = m_Transform.position;
        lastRotation = m_Transform.rotation.eulerAngles.y;

        healthBar.UpdateBar(1f);

        m_NavMeshAgent.destination = goal;
    }

    private void Update()
    {
        if(m_Animator != null)
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

        if (IsAlive)
        {
            m_Animator.SetTrigger(takeDamageId);
        }
    }

    protected override void Die()
    {
        base.Die();

        m_Animator.SetTrigger(dieId);

        m_NavMeshAgent.isStopped = true;

        Destroy(gameObject, 1.3f);
    }
}
