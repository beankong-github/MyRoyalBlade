using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    public Image progressImg;
    public GameObject curPos;
    public Vector2 startPos;
    public Vector2 endPos;

    private RectTransform curRectPos;

    private void Awake()
    {
        if(curPos != null) 
        { 
            curRectPos = curPos.GetComponent<RectTransform>();
        }
        
    }
    private void FixedUpdate()
    {
        if(progressImg != null)
            progressImg.fillAmount = GameManager.Instance.progress;

        if (curRectPos != null)
        {
            float x = startPos.x + (GameManager.Instance.progress * (endPos.x - startPos.x));
            Vector3 pos = new Vector3(x, startPos.y);
            curRectPos.localPosition = pos;
        }
    }

}
