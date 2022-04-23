using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Entities;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreUIText;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject[] lifeIcons;
    private bool updateScore;
    private bool updateLives;
    private bool showGameOver;
    private void Start()
    {
        GameStateData.Instance.OnScoreChanged += UpdateScore;
        GameStateData.Instance.OnLiveLost += UpdateLives;
        GameStateData.Instance.OnGameOver += ShowGameOverScreen;
    }

    private void OnDisable()
    {
        GameStateData.Instance.OnScoreChanged -= UpdateScore;
        GameStateData.Instance.OnLiveLost -= UpdateLives;
        GameStateData.Instance.OnGameOver -= ShowGameOverScreen;
    }

    public void UpdateScore()
    {
        updateScore = true;
    }

    public void UpdateLives()
    {
        updateLives = true;
    }

    public void ShowGameOverScreen()
    {
        showGameOver = false;
    }

    private void Update()
    {
        if (updateScore)
        {
            updateScore = false;
            scoreUIText.text = "Score : " + GameStateData.Instance.Score;
        }

        if (updateLives)
        {
            updateLives = false;
            for (int i = 0; i < lifeIcons.Length; i++)
            {
                if (i < GameStateData.Instance.PlayerLives)
                {
                    lifeIcons[i].SetActive(true);
                }
                else
                {
                    lifeIcons[i].SetActive(false);
                }
            }
            // Debug.Log("Player remaining lives " + GameStateData.Instance.PlayerLives);
        }

        if (showGameOver)
        {
            showGameOver = false;
            gameOverScreen.SetActive(true);
        }
    }
}
