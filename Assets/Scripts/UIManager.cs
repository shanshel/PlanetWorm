using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{

    public static UIManager inst;
    [SerializeField]
    TextMeshProUGUI scoreText;
    public Text bestScoreText, comboText;

    public Image mainMenuPanel, gameOverPanel;

    // then where you want the Alpha setting


    private void Awake()
    {
        inst = this;
    }

    void Start()
    {
    }

    void Update()
    {
        
    }

    public void setScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void onPlayButtonClicked()
    {
        GameManager.inst.startGame();
        mainMenuPanel.GetComponent<CanvasGroup>().DOFade(0, .5f);
        Invoke("toggleMainMenuScreen", .5f);
    }

    public void onPlayAgainButtonClicked()
    {

        gameOverPanel.GetComponent<CanvasGroup>().DOFade(0, .5f);
        Invoke("toggleGameOverScreen", .5f);
        GameManager.inst.resetGame(true);
    }

    public void onBackToMainMenuFromInGme()
    {
        gameOverPanel.GetComponent<CanvasGroup>().DOFade(0, .5f);
        GameManager.inst.resetGame(false);
    }

   

    public void onGameOver()
    {
        Invoke("onGameOverLate", 1.4f);
    }

    private void onGameOverLate()
    {
        toggleGameOverScreen();
        gameOverPanel.GetComponent<CanvasGroup>().DOFade(1f, .5f);
    }

    private void toggleMainMenuScreen()
    {

        mainMenuPanel.gameObject.SetActive(!mainMenuPanel.gameObject.activeInHierarchy);
    }

    private void toggleGameOverScreen()
    {
        gameOverPanel.gameObject.SetActive(!gameOverPanel.gameObject.activeInHierarchy);
    }


}
