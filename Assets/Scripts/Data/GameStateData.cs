using UnityEngine;
using UnityEngine.Events;


public class GameStateData : MonoBehaviour
{
    private int score; 
    public static bool SpaceshipHasShield;
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
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
