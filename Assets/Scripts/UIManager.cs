using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{

    public static UIManager inst;
    [SerializeField]
    TextMeshProUGUI scoreText;


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

   
}
