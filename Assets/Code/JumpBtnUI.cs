using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpBtnUI : MonoBehaviour
{
    public Image coolTimeImgs;

    private void Awake()
    { 
        GameManager.eGameStart += Init;
        Player.eSuperJumpGaugeChanged += UpdateSuperJumpGauge;
    }
    public void Init()
    {
        Debug.Assert(coolTimeImgs, "이미지 설정 안됨" + gameObject.name);
        if (coolTimeImgs)
            coolTimeImgs.fillAmount = 0f;
    }

    public void UpdateSuperJumpGauge(float _gauge)
    {
        Debug.Assert(coolTimeImgs, "이미지 설정 안됨" + gameObject.name);
        if (coolTimeImgs)
            coolTimeImgs.fillAmount = (float)_gauge / 100;
    }
}
