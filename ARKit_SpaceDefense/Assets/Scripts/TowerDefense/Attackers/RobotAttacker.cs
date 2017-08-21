using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(LineRenderer))]
public class RobotAttacker : Attacker
{
    [SerializeField]
    private HealthBar healthBar;
    
    [SerializeField, Range(0,1)]
    private float forwardStrength = .2f;

    [SerializeField]
    private float aimRandom = 30f;

    [SerializeField, Range(0, 10)]
    private int shotsPerSecond = 1;
    
    private Animator m_Animator;
    private Transform m_Transform;
    private NavMeshAgent m_NavMeshAgent;
    private LineRenderer m_LineRenderer;

    private Vector3 lastPosition;
    private float lastRotation;

    private int forwardId = Animator.StringToHash("Forward");
    private int dieId = Animator.StringToHash("Die");
    private int takeDamageId = Animator.StringToHash("Take Damage");
    private int fireId = Animator.StringToHash("Fire");

    private ForceField forceField = null;

    public void Initialize(Vector3 goal)
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_Transform = GetComponent<Transform>();
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_LineRenderer = GetComponent<LineRenderer>();

        lastPosition = m_Transform.position;
        lastRotation = m_Transform.rotation.eulerAngles.y;

        healthBar.UpdateBar(1f);

        m_NavMeshAgent.destination = goal;
    }

    private void Update()
    {
        if(m_Animator != null)
            UpdateAnimator();

        if (forceField != null)
            Aim();
    }

    private void UpdateAnimator()
    {
        float forward = (m_Transform.position - lastPosition).magnitude * forwardStrength / Time.deltaTime;

        if (forward < 0.05f)
            forward = 0f;

        m_Animator.SetFloat(forwardId, forward);

        lastPosition = m_Transform.position;
    }

    private void Aim()
    {
        Vector3 delta = forceField.transform.position - m_Transform.position;
        delta.y = 0f;
        delta.Normalize();

        m_Transform.rotation = Quaternion.Lerp(m_Transform.rotation, Quaternion.Euler(delta), Time.deltaTime * 5f);
    }

    private void OnTriggerEnter(Collider collider)
    {
        forceField = collider.GetComponent<ForceField>();

        if(forceField != null)
        {
            m_NavMeshAgent.isStopped = true;

            float delay = 1f / shotsPerSecond;

            InvokeRepeating("Fire", delay, delay);
        }
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

        CancelInvoke();

        m_Animator.SetTrigger(dieId);

        m_NavMeshAgent.isStopped = true;

        Destroy(gameObject, 1.3f);
    }

    private void Fire()
    {
        m_Animator.SetTrigger(fireId);

        Invoke("TakeAShot", 1f);
    }

    private void TakeAShot()
    {
        Vector3 distance = forceField.transform.position - m_Transform.position;

        Vector3 aim = distance.normalized;

        //Vector2 rand = Random.insideUnitCircle * aimRandom;

        //Vector3 randomDirection = ()

        //Quaternion.LookRotation()

        Ray ray = new Ray(m_Transform.position, aim);
        RaycastHit rayHit;

        if (forceField.GetComponent<MeshCollider>().Raycast(ray, out rayHit, distance.magnitude))
        {
            forceField.Hit(rayHit.normal);
        }
    }
}
