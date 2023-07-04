using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    // top ui
    public Button                   pasueBtn;
    public GameObject[]             lifes;
    public TMPro.TextMeshProUGUI    scoreTMP;

    // popup ui
    public GameObject gameOverPopup;
    public GameObject gameClearPopup;

    private int nextHeart;

    private void Awake()
    {
        nextHeart = lifes.Length - 1;

        // 이벤트 구독
        Player.ePlayerHit += LoseHeart;
        GameManager.eGameStart += Init;
        GameManager.eGameOver += onGameOver;
        GameManager.eGameClear += onGameClear;
    }

    private void Init()
    {
        nextHeart = lifes.Length - 1;

        for(int i = 0; i < lifes.Length; i++) 
        {
            lifes[i].SetActive(true);
        }
    }

    private void onGameOver()
    {
        if(gameOverPopup != null)
            gameOverPopup.SetActive(true);
    }

    private void onGameClear()
    {
        if(gameClearPopup != null)
            gameClearPopup.SetActive(true);
    }

    public void UpdateScore(string _score)
    {
        Debug.Assert(scoreTMP, "Score TMP is null");
        if(scoreTMP != null)
            scoreTMP.text = _score;

    }

    public void LoseHeart()
    {
        if (nextHeart >= lifes.Length || nextHeart == -1)
            return;
       
        lifes[nextHeart].SetActive(false);
        nextHeart--;
    }

}
