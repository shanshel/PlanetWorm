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
    public Text bestScoreText, bestComboText, bestLevelText, yourScoreText, yourComboText, yourLevelText;

    public Image mainMenuPanel, gameOverPanel;

    // then where you want the Alpha setting


    private void Awake()
    {
        inst = this;
    }

    void Start()
    {
        bestScoreText.text = PlayerPrefs.GetInt("topScore", 0).ToString();
        bestComboText.text = PlayerPrefs.GetInt("topCombo", 0).ToString();
        bestLevelText.text = PlayerPrefs.GetInt("topLevel", 0).ToString();
    }

    void Update()
    {
        
    }

    public void setScore(int score)
    {
        scoreText.text = score.ToString();
    }

    bool onPlayButtonClicked_click = false;
    public void onPlayButtonClicked()
    {
        if (onPlayButtonClicked_click) return;
        onPlayButtonClicked_click = true;
        GameManager.inst.startGame();
        mainMenuPanel.GetComponent<CanvasGroup>().DOFade(0, .5f);
        Invoke("toggleMainMenuScreen", .5f);
    }

    bool onPlayAgainButtonClicked_click = false;
    public void onPlayAgainButtonClicked()
    {
        if (onPlayAgainButtonClicked_click) return;
        onPlayAgainButtonClicked_click = true;
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
        onPlayButtonClicked_click = false;
    }

    private void toggleGameOverScreen()
    {
     
        gameOverPanel.gameObject.SetActive(!gameOverPanel.gameObject.activeInHierarchy);
        onPlayAgainButtonClicked_click = false;
    }

    
    public void setGameOverScreenInfo(int score, int combo, int level)
    {
        yourScoreText.text = score.ToString();
        yourComboText.text = combo.ToString();
        yourLevelText.text = level.ToString();
    }


}
