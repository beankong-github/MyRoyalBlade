using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackBtnUI : MonoBehaviour
{
    public Image coolTimeImgs;

    private void Awake()
    { 
        Player.eUltGaugeChanged += UpdateUltGauge;
        GameManager.eGameStart += Init;
    }

    public void Init()
    {
        if (coolTimeImgs)
            coolTimeImgs.fillAmount = 0f;
    }

    public void UpdateUltGauge(float _gauge)
    {
        if (coolTimeImgs)
            coolTimeImgs.fillAmount = _gauge * 0.01f;
    }
}
