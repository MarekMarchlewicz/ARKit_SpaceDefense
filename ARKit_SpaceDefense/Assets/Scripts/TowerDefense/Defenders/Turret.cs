using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public enum TurretState
{
    Inactive,
    Activating,
    Ready
}

[RequireComponent(typeof(SphereCollider), typeof(AudioSource))]
public class Turret : MonoBehaviour
{
    [Header("Shooting")]

    [SerializeField]
    private Transform turretRotator;            //The transform of the turret that handles turning the gun

    [SerializeField]
    private Transform turrentGunArm;            //The transform of the turret that handles looking up and down

    [SerializeField]
    private Transform turretBarrel;      //The transform of gun's barrel

    [SerializeField]
    private float turretRotationSpeed = 5f;     //How fast the turret rotates

    [Header("Shooting")]

    [SerializeField, Range(1, 30)]
    private float shotsPerSecond = 1f;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private float bulletStartVelocity = 100f;

    [SerializeField, Range(0, 360)]
    private float aimPrecision = 25f;

    [SerializeField]
    private AudioClip shootSfx;

    [SerializeField]
    private AudioClip startSfx;

    private List<Attacker> attackers = new List<Attacker>();

    private TurretState turretState = TurretState.Inactive;

    private Transform target;

    private ObjectPooler bulletPool;

    private AudioSource m_AudioSource;

    private float lastTimeFired = 0f;

    private void Awake()
    {
        GetComponent<DragBehaviour>().OnDragStop += OnDragStop;
    }

    private void OnDragStop(DragBehaviour dragBehaviour)
    {
		if(dragBehaviour.lastNode != null && GameManager.instance.ActiveTurrets < GameManager.MAX_TURRETS)
        {
			dragBehaviour.lastNode.gameObject.SetActive (false);

			Initialize(dragBehaviour.lastNode.transform.position + dragBehaviour.positionOffset);

			GameManager.instance.AddTurret (gameObject);

            Destroy(dragBehaviour);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize(Vector3 position)
    {
        transform.position = position;

        m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.clip = startSfx;
        m_AudioSource.Play();

        ActivateTurret();

        bulletPool = gameObject.AddComponent<ObjectPooler>();
        bulletPool.Initialize(bulletPrefab, 30, false);
    }

    private void ActivateTurret()
    {
        PlayableDirector timeline = GetComponent<PlayableDirector>();
        timeline.Play();
        Invoke("OnTimelineFinished", (float)timeline.playableAsset.duration);

        turretState = TurretState.Activating;
    }

    private void OnTimelineFinished()
    {
        turretState = TurretState.Ready;
    }

    private void OnTriggerEnter(Collider collider)
    {
        Attacker attacker = collider.GetComponent<Attacker>();

        if (attacker != null)
        {
            attackers.Add(attacker);

            attacker.OnDead += Attacker_OnDead;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        Attacker attacker = collider.GetComponent<Attacker>();

        attackers.Remove(attacker);
    }

    private void Attacker_OnDead(Attacker attacker)
    {
        attackers.Remove(attacker);
    }

    private void Update()
    {
        if (turretState != TurretState.Ready)
            return;

        if (attackers.Count == 0)
            return;

        target = attackers[0].transform;

        //Get the vector between the turret and the runner, calculate the rotation needed to "look at" it, and "Linerally Interpolate" from the current rotation to the desired one
        Vector3 targetRotation = Quaternion.LookRotation(target.position - turretRotator.position).eulerAngles;
        targetRotation.x = 0f;
        targetRotation.z = 0f;
        Quaternion turretRotation = Quaternion.Euler(targetRotation);
        turretRotator.localRotation = Quaternion.Lerp(turretRotator.localRotation, turretRotation, Time.deltaTime * turretRotationSpeed);

        targetRotation = Quaternion.LookRotation(target.position - turretBarrel.position).eulerAngles;
        targetRotation.y = 0f;
        Quaternion gunArmRotation = Quaternion.Euler(targetRotation);
        turrentGunArm.localRotation = Quaternion.Lerp(turrentGunArm.localRotation, gunArmRotation, Time.deltaTime * turretRotationSpeed);

        float turretRotationDiff = Quaternion.Angle(turretRotation, turretRotator.localRotation);
        float gunRotationDiff = Quaternion.Angle(gunArmRotation, turrentGunArm.localRotation);

        if (turretRotationDiff < aimPrecision && gunRotationDiff < aimPrecision &&
            Time.time - lastTimeFired > 1 / shotsPerSecond)
        {
            Fire();
        }
    }

    private void Fire()
    {
        GameObject newBullet = bulletPool.GetPooledObject();

        if(newBullet != null)
        {
            newBullet.transform.position = turretBarrel.position;
            newBullet.transform.rotation = turretBarrel.rotation;
            newBullet.GetComponent<Rigidbody>().velocity = newBullet.transform.forward * bulletStartVelocity;
            newBullet.SetActive(true);

            m_AudioSource.clip = shootSfx;
            m_AudioSource.Play();

            lastTimeFired = Time.time;
        }
    }
}
