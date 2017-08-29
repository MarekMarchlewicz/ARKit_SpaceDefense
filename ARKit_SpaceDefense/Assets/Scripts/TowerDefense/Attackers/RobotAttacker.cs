﻿using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(LineRenderer))]
public class RobotAttacker : Attacker
{
    [SerializeField]
    private HealthBar healthBar;
    
    [SerializeField, Range(0,1)]
    private float forwardStrength = .2f;

	[SerializeField, Range(0,100)]
    private int damagePerShot = 5;

    [SerializeField, Range(0, 10)]
    private int shotsPerSecond = 1;

	[SerializeField]
	private Transform rightHandTransform;
    
    private Animator m_Animator;
    private Transform m_Transform;
    private NavMeshAgent m_NavMeshAgent;
    private LineRenderer m_LineRenderer;

    private Vector3 lastPosition;
    private float lastRotation;

    private int forwardId = Animator.StringToHash("Forward");
    private int dieId = Animator.StringToHash("Die");
    private int takeDamageId = Animator.StringToHash("Take Damage");
	private int fireId = Animator.StringToHash("Right Blast Attack");
	private int aimId = Animator.StringToHash("Right Aim");

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
		ForceField newForceField = collider.GetComponent<ForceField>();

		if(newForceField != null)
        {
			forceField = newForceField;

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

		CancelInvoke("Fire");

        m_Animator.SetTrigger(dieId);

        m_NavMeshAgent.isStopped = true;

        Destroy(gameObject, 1.3f);
    }

    private void Fire()
    {
        m_Animator.SetTrigger(fireId);
		m_Animator.SetBool(aimId, true);

		TakeAShot ();
    }

    private void TakeAShot()
    {
		Vector3 distance = forceField.transform.position - rightHandTransform.position;

        Vector3 aim = distance.normalized;

        Ray ray = new Ray(m_Transform.position, aim);
        RaycastHit rayHit;

        if (forceField.GetComponent<MeshCollider>().Raycast(ray, out rayHit, distance.magnitude))
        {
			forceField.Hit(damagePerShot, rayHit.normal);
        }
    }
}
