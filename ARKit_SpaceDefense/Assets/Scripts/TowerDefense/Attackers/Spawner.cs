using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private Map map;

    [SerializeField]
    private float spawnDelay = 5f;

    [SerializeField]
    private float spawnStart = 10f;

    [SerializeField]
    private GameObject attackerPrefab;

    private void Start()
    {
        transform.position = map.AttackersPosition;

        InvokeRepeating("Spawn", spawnStart, spawnDelay);
    }

    private void Spawn()
    {
        GameObject attackerGO = Instantiate(attackerPrefab, transform);

        attackerGO.GetComponent<RobotAttacker>().Initialize(map.DefendersPosition);
    }
}
