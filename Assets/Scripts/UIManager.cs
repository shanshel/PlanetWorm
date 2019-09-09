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
    public Text bestScoreText;

    public Image mainMenuPanel;

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
        mainMenuPanel.GetComponent<CanvasGroup>().DOFade(0, .6f);
        Invoke("disableMainMenu", .6f);
        //mainMenuPanel.GetComponent<CanvasGroup>().alpha = 0;

    }

    private void disableMainMenu()
    {
        mainMenuPanel.gameObject.SetActive(false);
    }

    private void enableMainMenu()
    {
        mainMenuPanel.gameObject.SetActive(true);

    }


}
