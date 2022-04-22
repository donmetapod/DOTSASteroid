using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Entities;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreUIText;
    private bool updateScore;
    private void Start()
    {
        GameStateData.Instance.OnScoreChanged.AddListener(UpdateScore);
    }

    private void OnDisable()
    {
        GameStateData.Instance.OnScoreChanged.RemoveListener(UpdateScore);
    }

    void UpdateScore()
    {
        updateScore = true;
    }

    private void Update()
    {
        if (updateScore)
        {
            updateScore = false;
            scoreUIText.text = "Score : " + GameStateData.Instance.Score;
        }
    }
}
