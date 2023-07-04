using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldBtnUI : MonoBehaviour
{
    public Image coolTimeImgs;
    private float cooltime;
    private float time;

    private void Awake()
    {
        Player.eShieldCoolUpdate += SetCoolTime;
    }

    void SetCoolTime(float _cooltime)
    {
        if (coolTimeImgs != null)
        {
            coolTimeImgs.gameObject.SetActive(true);
            coolTimeImgs.fillAmount = 1;
            cooltime = _cooltime;
        }
    }

    private void Update()
    {
        if (coolTimeImgs == null)
            return;

        if (!coolTimeImgs.gameObject.activeSelf)
            return;

        time += Time.deltaTime;

        float scale = time / cooltime;
        if(scale > 1) 
        {
            coolTimeImgs.gameObject.SetActive(false);
            time = 0;
        }

        coolTimeImgs.fillAmount = 1 - scale;
    }


}