using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.iOS;

public enum GameMode
{
    FindPlane,
    GeneratingMap,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static event System.Action<GameMode> OnGameModeChanged;

	[SerializeField] private Map map;

	[SerializeField] private float matchTime = 60f;

	[SerializeField] private Spawner attackerSpawner;

	[SerializeField] private GameObject defender;

	[SerializeField] private int waveSize = 2;

	[SerializeField] private float delayBetweenSpawns = 10f;

	[SerializeField] private GameObject addTurretButton;

	[SerializeField] private GameMode gameMode;

	public static GameMode GameMode { get { return instance.gameMode; } private set { instance.gameMode = value; } }

	private Defender activeDefender;

	private List<GameObject> activeTurrets = new List<GameObject>(MAX_TURRETS);
	public int ActiveTurrets { get { return activeTurrets.Count; } }

	public const int MAX_TURRETS = 5;

    private float timer;

	public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

	private void Start()
	{
		ChangeMode(gameMode);
	}

    public void ChangeMode(GameMode newGameMode)
    {
        gameMode = newGameMode;

        timer = Time.time;

        if (OnGameModeChanged != null)
        {
            OnGameModeChanged(gameMode);
        }

		switch (newGameMode) 
		{
		case GameMode.FindPlane:
			UIManager.ShowMessage ("Find a flat surface", 10f);
			break;
		case GameMode.GeneratingMap:
			UIManager.ShowMessage ("Generating new map", 5f);
			map.GenerateNew ();
			break;
		case GameMode.Playing:
			UIManager.ShowMessage ("Start!", 1f);
			attackerSpawner.Spawn (map.transform, map.AttackersPosition, map.DefendersPosition, waveSize, delayBetweenSpawns);
			activeDefender = Instantiate (defender, map.DefendersPosition + Vector3.up * 0.3f, Quaternion.identity).GetComponent<Defender> ();

			addTurretButton.SetActive (true);
			break;
		case GameMode.GameOver:
			UIManager.ShowMessage ("Game Over!", 1f);
			attackerSpawner.Stop ();

			Destroy (activeDefender.gameObject);

			for (int i = 0; i < activeTurrets.Count; i++) 
			{
				Destroy (activeTurrets [i]);
			}

			activeTurrets.Clear ();

			addTurretButton.SetActive (false);
			break;
		}
    }

    private void Update()
    {
        switch(gameMode)
        {
            case GameMode.FindPlane:
                UpdateFindPlane();
                break;
            case GameMode.GeneratingMap:
                UpdateGeneratingMap();
                break;
            case GameMode.Playing:
                UpdatePlaying();
                break;
            case GameMode.GameOver:
                UpdateGameOver();
                break;
        }
    }

    private void UpdateFindPlane()
    {
		
    }


    private void UpdateGeneratingMap()
    {
		if (!map.IsBusy)
		{
			ChangeMode (GameMode.Playing);
		}
    }

    private void UpdatePlaying()
    {
		UIManager.UpdateTimer (Mathf.Clamp (matchTime - Mathf.Abs(timer - Time.time), 0f, float.MaxValue));

		if(Time.time - timer > matchTime || activeDefender.Health <= 0)
        {
            ChangeMode(GameMode.GameOver);
        }

		if (activeTurrets.Count >= MAX_TURRETS) 
		{
			addTurretButton.SetActive (false);
		}
    }

    private void UpdateGameOver()
    {
        ChangeMode(GameMode.GeneratingMap);
    }

	public void AddTurret(GameObject newTurret)
	{
		activeTurrets.Add (newTurret);
	}
}
