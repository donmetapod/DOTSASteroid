using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public class GameManager : MonoBehaviour
{
    [SerializeField] private AudioController audioController;
    private int score;
    private int playerLives = 3;
    private bool spaceshipHasShield;
    private bool spreadShotIsEnabled;
    private bool playerRespawning;
    private bool startRespawnRoutine;
    private bool ufoInAction;
    public static GameManager Instance;
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
            audioController.SfxToPlay = "Explosion";
            audioController.PlaySFX();
            OnScoreChanged?.Invoke();
        }
    }

    public int PlayerLives
    {
        get => playerLives;
        set
        {
            playerLives = value;
            audioController.SfxToPlay = "LoseLife";
            audioController.PlaySFX();
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
        set
        {
            spaceshipHasShield = value;
            audioController.SfxToPlay = "ShieldPowerUp";
            audioController.PlaySFX();
            StartCoroutine(RemoveTemporaryShield());
        }
    }

    public bool SpreadShotIsEnabled
    {
        get => spreadShotIsEnabled;
        set
        {
            spreadShotIsEnabled = value;
            audioController.SfxToPlay = "SpreadShotPowerUp";
            audioController.PlaySFX();
            StartCoroutine(RemoveTemporarySpreadShot());
        }
    }

    public bool PlayerRespawning
    {
        get => playerRespawning;
        set
        {
            playerRespawning = value;
            startRespawnRoutine = true;
            spreadShotIsEnabled = false;
        }
    }

    public bool UfoInAction
    {
        get => ufoInAction;
        set => ufoInAction = value;
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

    public void PlayAudioClipWithName(string clipName)
    {
        audioController.PlayAudioClip(clipName);
    }

    IEnumerator RemoveTemporaryShield()
    {
        yield return new WaitForSeconds(10);
        spaceshipHasShield = false;
    }

    IEnumerator RemoveTemporarySpreadShot()
    {
        yield return new WaitForSeconds(10);
        spreadShotIsEnabled = false;
    }
}
