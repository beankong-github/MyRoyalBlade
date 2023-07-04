using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactiveAfterAnimScript : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Debug.Assert(animator, "Animator Component 없음 : " + gameObject.name);
    }

    void Update()
    {
        if (gameObject.activeSelf)
        { 
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                gameObject.SetActive(false);
        }
    }
}
