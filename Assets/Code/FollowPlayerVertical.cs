using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class FollowPlayerVertical : MonoBehaviour
{
    public GameObject player;

    private float prevYPos = 0;
    private float minYPos = 0;

    private void Start()
    {
        Debug.Assert(player, "Player 연결 안됨" + gameObject.name);

        if (null != player)
        {
            prevYPos = player.transform.position.y;
            minYPos = transform.position.y;
        }
    }
    private void LateUpdate()
    {
        float curYPos = player.transform.position.y;

        if (curYPos != prevYPos)
        { 
            float diff = curYPos - prevYPos;

            UnityEngine.Vector3 pos = transform.position;
            pos.y += diff;

            if (pos.y < minYPos)
                pos.y = minYPos;

            transform.position = pos;
            prevYPos = curYPos;
        }
    }
}
