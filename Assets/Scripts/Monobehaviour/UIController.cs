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
        GameManager.Instance.OnScoreChanged += UpdateScore;
        GameManager.Instance.OnLiveLost += UpdateLives;
        GameManager.Instance.OnGameOver += ShowGameOverScreen;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnScoreChanged -= UpdateScore;
        GameManager.Instance.OnLiveLost -= UpdateLives;
        GameManager.Instance.OnGameOver -= ShowGameOverScreen;
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
        showGameOver = true;
    }

    private void Update()
    {
        if (updateScore)
        {
            updateScore = false;
            scoreUIText.text = "Score : " + GameManager.Instance.Score;
        }

        if (updateLives)
        {
            updateLives = false;
            for (int i = 0; i < lifeIcons.Length; i++)
            {
                if (i < GameManager.Instance.PlayerLives)
                {
                    lifeIcons[i].SetActive(true);
                }
                else
                {
                    lifeIcons[i].SetActive(false);
                }
            }
        }

        if (showGameOver)
        {
            showGameOver = false;
            gameOverScreen.SetActive(true);
        }
    }
}
