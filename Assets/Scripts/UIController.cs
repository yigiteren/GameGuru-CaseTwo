using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private PlayerHandler playerHandler;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject lostScreen;
    [SerializeField] private TMP_Text winCoins;
    [SerializeField] private TMP_Text lostCoins;

    private void Start()
    {
        playerHandler.OnPlayerStop += OnGameOver;
    }

    private void OnGameOver(bool hasWon)
    {
        winCoins.text = $"Coins collected: {playerHandler.CollectedCoins}";
        lostCoins.text = $"Coins collected: {playerHandler.CollectedCoins}";

        victoryScreen.SetActive(hasWon);
        lostScreen.SetActive(!hasWon);
    }

    public void Replay()
    {
        SceneManager.LoadScene("Scene");
    }

}
