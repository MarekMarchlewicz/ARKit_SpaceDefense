using UnityEngine;

public enum TurretState
{
    Inactive,
    Activating,
    Ready
}

public class Turret : MonoBehaviour
{
    [SerializeField]
    private Transform target;                   //The runner's transform

    [SerializeField]
    private Transform turretRotator;            //The transform of the turret that handles turning the gun

    [SerializeField]
    private Transform turrentGunArm;            //The transform of the turret that handles looking up and down

    [SerializeField]
    private Transform turrentGunArmCentre;      //The transform of gun arm's centre

    [SerializeField]
    private float turretRotationSpeed = 5f;     //How fast the turret rotates

    [SerializeField]
    private float poweredDownAngle = 25f;       //How far down does the turret look when unpowered

    private AudioSource audioSource;                            //Reference to audio source component
    private bool isPowered = true;                              //Is the turret currently powered?
    //TurretShooting turretShooting;                      //Reference to the TurretShooting script on the barrel of the gun

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //turretShooting = GetComponentInChildren<TurretShooting>();
    }

    private void Update()
    {
        if (!isPowered)
            return;

        //Get the vector between the turret and the runner, calculate the rotation needed to "look at" it, and "Linerally Interpolate" from the current rotation to the desired one
        Vector3 targetRotation = Quaternion.LookRotation(target.position - turretRotator.position).eulerAngles;

        Quaternion turretRotation = Quaternion.Euler(new Vector3(0f, targetRotation.y, 0f));
        turretRotator.localRotation = Quaternion.Lerp(turretRotator.localRotation, turretRotation, Time.deltaTime * turretRotationSpeed);

        targetRotation = Quaternion.LookRotation(target.position - turrentGunArmCentre.position).eulerAngles;

        Quaternion gunArmRotation = Quaternion.Euler(new Vector3(targetRotation.x, 0f, 0f));
        turrentGunArm.localRotation = Quaternion.Lerp(turrentGunArm.localRotation, gunArmRotation, Time.deltaTime * turretRotationSpeed);
    }

    //Public method called by the TurretPowerMarker
    public void TurnOff()
    {
        if (!isPowered)
            return;

        //Flag the turret as "off", disable the shooting capability, and begin rotating the gun downward
        isPowered = false;
        //turretShooting.enabled = false;
    }
}
