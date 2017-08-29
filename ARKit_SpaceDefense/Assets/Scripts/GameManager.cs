using UnityEngine;

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

	[SerializeField] private Spawner spawner;

	[SerializeField] private int waveSize = 2;

	[SerializeField] private float delayBetweenSpawns = 10f;

	[SerializeField] private GameObject addTurretButton;

    private GameMode gameMode;
    public static GameMode GameMode { get { return instance.gameMode; } private set { instance.gameMode = value; } }

    private float timer;

    private static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

	private void Start()
	{
		ChangeMode(GameMode.FindPlane);
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
			spawner.Spawn (map.AttackersPosition, map.DefendersPosition, waveSize, delayBetweenSpawns);
			break;
		case GameMode.GameOver:
			UIManager.ShowMessage ("Game Over!", 1f);
			spawner.Stop ();
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
        ChangeMode(GameMode.GeneratingMap);
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

        if(Time.time - timer > matchTime)
        {
            ChangeMode(GameMode.GameOver);
        }
    }

    private void UpdateGameOver()
    {
        ChangeMode(GameMode.GeneratingMap);
    }

	public void SpawnEnemies(GameObject enemy, int count, Transform spawnPoint)
	{
		Debug.Log (count);
	}
}
