using UnityEngine;

public enum GameMode
{
    FindPlane,
    Wait,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static event System.Action<GameMode> OnGameModeChanged;

    private GameMode gameMode;
    public static GameMode GameMode { get { return instance.gameMode; } private set { instance.gameMode = value; } }

    private float timer;

    private static GameManager instance;

    private void Awake()
    {
        instance = this;

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

        Debug.Log(gameMode);
    }

    private void Update()
    {
        switch(gameMode)
        {
            case GameMode.FindPlane:
                UpdateFindPlane();
                break;
            case GameMode.Wait:
                UpdateWait();
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
        ChangeMode(GameMode.Wait);
    }

    private const float waitingTime = 5f;

    private void UpdateWait()
    {
        Debug.Log((waitingTime - Time.time - timer).ToString());

        if(Time.time - timer > waitingTime)
        {
            ChangeMode(GameMode.Playing);
        }
    }

    private const float matchTime = 60f * 3;

    private void UpdatePlaying()
    {
        if(Time.time - timer > matchTime)
        {
            ChangeMode(GameMode.GameOver);
        }
    }

    private void UpdateGameOver()
    {
        ChangeMode(GameMode.Wait);
    }
}
