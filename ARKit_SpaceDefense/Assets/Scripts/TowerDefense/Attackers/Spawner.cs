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

	public void Spawn(Transform parentTransfrom, Vector3 spawnLocalPosition, Vector3 targetPosition, int waveSize, float delayBetweenWaves)
	{
		transform.parent = parentTransfrom;
		transform.position = spawnLocalPosition;
		transform.localScale = Vector3.one;
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
			Vector3 randomPosition = Vector3.zero;// Random.onUnitSphere * 0.5f;
			randomPosition.y = 0f;

			GameObject attackerGO = Instantiate (attackerPrefab, transform.position + randomPosition, Quaternion.identity);
			attackerGO.transform.parent = transform;
			attackerGO.transform.localScale = Vector3.one * 0.3f;

			spawnedAttackers.Add (attackerGO);

			attackerGO.GetComponent<RobotAttacker> ().Initialize (targetAttackPosition);
		}
    }
}
