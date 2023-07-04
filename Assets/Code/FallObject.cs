using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FallObject : MonoBehaviour
{
    public float hp;
    public float maxhp;

    private Rigidbody2D rigid2D;
    private Vector2     velocity;

    public delegate void ObjectDestroyEvent(float _hp);
    public static event ObjectDestroyEvent eObjectDestroyed;

    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        Debug.Assert(rigid2D, "Rigidbody2D Component 없음 : " + gameObject.name);

        GameManager.eGamePause += OnPaused;
        GameManager.eGameContinue += OnContinue;
    }
    void OnEnable()
    {
        hp = maxhp;
    }
    void OnPaused()
    {
        if (rigid2D != null)
        {
            velocity = rigid2D.velocity;
            rigid2D.velocity = Vector2.zero;
        }
    }

    void OnContinue()
    {
        if (rigid2D != null)
        {
            rigid2D.velocity = velocity;
        }
    }

    private void LateUpdate()
    {
        if(hp <= 0) 
        { 
            gameObject.SetActive(false);
            eObjectDestroyed(maxhp);
            Explosion();
        }
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.CompareTag(TAGS.Attack.ToString()))
        {
            float damage = 0;
            if (_collision.GetComponent<Bullet>())
            {
                damage = _collision.GetComponent<Bullet>().damage;
            }

            hp -= damage; 
        }
    }

    private void OnCollisionEnter2D(Collision2D _collision)
    {
        if (_collision.gameObject.CompareTag(TAGS.Player.ToString()))
        {
            gameObject.SetActive(false);
            Explosion();

        }
    }

    private void OnParticleCollision(GameObject _collision)
    {
        if (_collision.CompareTag(TAGS.Attack.ToString()))
        {
            float damage = 100;
            hp -= damage;
        }
    }


    private void Explosion()
    {
        GameObject effect = GameManager.Instance.poolManager.Generate((int)GO_POOL_TYPE.EXPLOSION);
        Vector3 pos = this.transform.position;

        pos.x += Random.Range(-3, 3);

        effect.transform.position = pos;
    }
}
