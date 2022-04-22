using UnityEngine;
using UnityEngine.Events;


public class GameStateData : MonoBehaviour
{
    private int score;
    private int playerLives = 3;
    public bool spaceshipHasShield;
    public bool spreadShotIsEnabled;
    public static GameStateData Instance;
    public UnityEvent OnScoreChanged;

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
            if (value == 0)
            {
                //Game Over
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
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
