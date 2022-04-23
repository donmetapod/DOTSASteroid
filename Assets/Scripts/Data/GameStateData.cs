using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public class GameStateData : MonoBehaviour
{
    private int score;
    private int playerLives = 3;
    private bool spaceshipHasShield;
    private bool spreadShotIsEnabled;
    private bool playerRespawning;
    private bool startRespawnRoutine;
    public static GameStateData Instance;
    public event Action OnScoreChanged;
    public event Action OnLiveLost;
    public event Action OnGameOver;
    public enum GameStateEnum
    {
        Playing,
        GameOver
    }

    public GameStateEnum GameState;
    public int Score
    {
        get => score;
        set
        {
            score = value;
            OnScoreChanged?.Invoke();
        }
    }

    public int PlayerLives
    {
        get => playerLives;
        set
        {
            playerLives = value;
            OnLiveLost?.Invoke();
            if (value == 0)
            {
                OnGameOver?.Invoke();
                GameState = GameStateEnum.GameOver;
            }
        }
    }

    public bool SpaceshipHasShield
    {
        get => spaceshipHasShield;
        set => spaceshipHasShield = value;
    }

    public bool SpreadShotIsEnabled
    {
        get => spreadShotIsEnabled;
        set => spreadShotIsEnabled = value;
    }

    public bool PlayerRespawning
    {
        get => playerRespawning;
        set
        {
            playerRespawning = value;
            startRespawnRoutine = true;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (startRespawnRoutine)
        {
            startRespawnRoutine = false;
            StartCoroutine(MakeVulnerableAfterRespawn());
        }
    }

    IEnumerator MakeVulnerableAfterRespawn()
    {
        yield return new WaitForSeconds(2);
        playerRespawning = false;
    }
}
