using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private Map map;

    [SerializeField]
    private GameObject attackerPrefab;

	private int spawnWaveSize;

	private Vector3 targetAttackPosition;

	private List<GameObject> spawnedAttackers = new List<GameObject>();

	public void Spawn(Vector3 spawnPosition, Vector3 targetPosition, int waveSize, float delayBetweenWaves)
	{
		transform.position = spawnPosition;
		targetAttackPosition = targetPosition;

		spawnWaveSize = waveSize;

		InvokeRepeating("Spawn", 0f, delayBetweenWaves);
	}

	public void Stop()
	{
		CancelInvoke ("Spawn");

		for (int i = 0; i < spawnedAttackers.Count; i++) 
		{
			if (spawnedAttackers [i] != null) 
			{
				Destroy (spawnedAttackers [i]);
			}
		}
	}

    private void Spawn()
    {
		for (int i = 0; i < spawnWaveSize; i++)
		{
			Vector3 randomPosition = Random.onUnitSphere * 0.5f;
			randomPosition.y = 0f;

			GameObject attackerGO = Instantiate (attackerPrefab, transform.position + randomPosition, Quaternion.identity);

			spawnedAttackers.Add (attackerGO);

			attackerGO.GetComponent<RobotAttacker> ().Initialize (targetAttackPosition);
		}
    }
}
