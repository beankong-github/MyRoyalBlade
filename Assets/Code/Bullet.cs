using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float lifeTime;
    public float damage;
    private float time;

    void OnEnable()
    {
        time = 0;
    }
    void Update()
    {
        time += Time.deltaTime;
        transform.Translate(Vector3.up * speed * Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        if (time > lifeTime)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.gameObject.CompareTag(TAGS.Object.ToString()))
        {
            gameObject.SetActive(false);
        }

    }
}
  