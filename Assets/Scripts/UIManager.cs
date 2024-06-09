using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject finishedPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Elements")]
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private TextMeshProUGUI levelText;

    private void Awake()
    {
        if (Instance != null) 
            Destroy(Instance);
        Instance = this;
    }

    public void InGame()
    {
        gamePanel.SetActive(true);
        finishedPanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    public void WinGame()
    {
        gamePanel.SetActive(false);
        finishedPanel.SetActive(false);
        winPanel.SetActive(true);
        losePanel.SetActive(false);
    }

    public void LoseGame()
    {
        gamePanel.SetActive(false);
        finishedPanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(true);
    }

    public void FinishedGame()
    {
        gamePanel.SetActive(false);
        finishedPanel.SetActive(true);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    public void UpdateTurnText(int turn)
    {
        turnText.text = turn.ToString();
    }

    public void UpdateLevelText(int level)
    {
        levelText.text = level.ToString();
    }
}
