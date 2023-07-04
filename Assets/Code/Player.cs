using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;


public class Player : MonoBehaviour
{
    [SerializeField]
    private PLAYER_STATE playerState;


    private int curLevel;
    public Vector3 startPos;
    
    [SerializeField]
    private bool onGround;

    public int maxLife;
    public int life;
    
    public float speed;
    public float damage;
    
    public int superJumpGaugeIncreseAmount;
    public int ultGaugeIncreseAmount;
    private int superJumpGauge;
    private int ultGauge;

    [SerializeField]
    private bool isShieldAvailable = true;
    public float shieldCoolTime;

    Rigidbody2D rigid2d;
    SpriteRenderer spriteRenderer;
    Animator animator;
    PoolManager poolMgr;

    public GameObject shadow;
    public GameObject ult;

    public AudioSource attackSound;
    public AudioSource hitSound;
    public AudioSource shieldSound;

    // event
    public delegate void PlayerStateEvent();
    public static event PlayerStateEvent ePlayerDead;
    public static event PlayerStateEvent ePlayerHit;

    public delegate void PlayerDataEvent(float _data);
    public static event PlayerDataEvent eUltGaugeChanged, eSuperJumpGaugeChanged;
    public static event PlayerDataEvent eShieldCoolUpdate;

    public bool OnGround { get => onGround; }

    private void Awake()
    {
        rigid2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer> ();
        animator = GetComponent<Animator>();
        poolMgr = GameManager.Instance.poolManager;

        // level change 이벤트 구독
        GameManager.eLevelChanged += OnChangeLevel;
        GameManager.eGameStart += Init;
    }

    private void Update()
    {
        // 애니메이션에 지면 상태 전달
        if(animator != null)
            animator.SetBool("OnGround", onGround);

        // 애니메이션에 궁극기 상태 전달
        if(ult != null)
            if (!ult.activeSelf)
            {
                animator.SetBool("OnUlt", false);
            }

    }

    private void OnCollisionEnter2D(Collision2D _collision)
    {
        if (playerState == PLAYER_STATE.DEAD)
            return;

        // 지면 충돌
        if (_collision.gameObject.CompareTag(TAGS.Ground.ToString()))
        {
            onGround = true;

            if(shadow != null)
                shadow.SetActive(true);
        }

        // 떨어지는 물체와 충돌
        if (_collision.gameObject.CompareTag(TAGS.Object.ToString()))
        {
            if (playerState == PLAYER_STATE.SUPER_JUMP)
                return;

            animator.SetTrigger("OnHit");
            animator.SetBool("isShieldAvailable", isShieldAvailable);

            // 쉴드 사용이 가능한 경우
            if (isShieldAvailable)
            {
                isShieldAvailable = false;
                StartCoroutine(WaitShieldCool());
                eShieldCoolUpdate(shieldCoolTime);

                // 사운드 재생
                if(shieldSound != null)
                    shieldSound.Play();
            }
            // 쉴드 사용이 불가능한 경우
            else
            {
                --life;
                if (life == -1)
                {
                    playerState = PLAYER_STATE.DEAD;
                    ePlayerDead();
                    animator.SetTrigger("Dead");
                    return;
                }

                // 사운드 재생
                if(hitSound != null) 
                    hitSound.Play();
                ePlayerHit();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D _collision)
    {
        if (_collision.gameObject.CompareTag(TAGS.Ground.ToString()))
        {
            onGround = false;

            if (shadow != null)
                shadow.SetActive(false);
        }
    }

    public void Init()
    {
        playerState = PLAYER_STATE.IDLE;
        gameObject.transform.position = startPos;
        curLevel = 0;

        life = maxLife;
        superJumpGauge = 0;
        ultGauge = 0;


        if (animator != null)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Die"))
            {
                // 사망 상태였다면
                animator.ResetTrigger("OnHit");
                animator.SetTrigger("Restart");
                return;
            }

            // 궁극기 상태였다면
            if (ult.activeSelf)
            {
                ult.SetActive(false);
                animator.Play("Idle");
            }
        }

    }

    IEnumerator WaitShieldCool()
    { 
        yield return new WaitForSeconds(shieldCoolTime);

        isShieldAvailable = true;
    }
    
    private void RestartCurrentAnimation()
    {
        if (animator != null)
        {
            animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, 0f);
        }
    }

    private void OnChangeLevel(int _level)
    {
        curLevel = _level;
    }

    public void Jump()
    {
        // 죽은 상태면 불가
        if (playerState == PLAYER_STATE.DEAD)
            return;

        if (onGround)
        {
            if (superJumpGauge >= 100)
            {
                SuperJump();
            }
            else 
            {
                Debug.Assert(rigid2d, "Ridigbody2D 컴포넌트 없음");

                Vector2 jumpSpeed = Vector2.up * speed;
                rigid2d.AddForce(jumpSpeed);

                playerState = PLAYER_STATE.JUMP;
                superJumpGauge = superJumpGauge + superJumpGaugeIncreseAmount > 100 ? 100 : superJumpGauge + superJumpGaugeIncreseAmount;
                eSuperJumpGaugeChanged(superJumpGauge);
            }
        }
    }

    private void SuperJump()
    {
        Vector2 jumpSpeed = Vector2.up * speed * 3;
        rigid2d.AddForce(jumpSpeed);

        playerState = PLAYER_STATE.SUPER_JUMP;
        superJumpGauge = 0;
        eSuperJumpGaugeChanged(superJumpGauge);

        animator.SetTrigger("SuperJump");
    }

    public void Attack()
    {
        Debug.Assert(animator, "Animator 컴포넌트 없음");

        // 죽은 상태면 불가
        if (playerState == PLAYER_STATE.DEAD)
            return;

        // 맞은 상태면 공격 불가
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Damage")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("DamageOnAir"))
            return;

        // 궁극기 사용 중엔 공격 불가
        if (ult.activeSelf)
            return;

        if (ultGauge >= 100)
        {
            UltAttack();
        }
        else
        {
            // 투사체 생성
            int projType = (int)GO_POOL_TYPE.PROJECTILE_0 + curLevel;
            GameObject proj = poolMgr.Generate(projType, gameObject);

            Debug.Assert(proj, "Player 투사체 생성 실패. 현재 단계 :" + curLevel.ToString()); 
                
            proj.transform.position = gameObject.transform.position + new Vector3(0, 1, 0);

            // 공격력 설정
            if(proj.gameObject.GetComponent<Bullet>())
                proj.gameObject.GetComponent<Bullet>().damage = damage;

            // 궁극기 게이지 증가
            ultGauge = ultGauge + ultGaugeIncreseAmount > 100 ? 100 : ultGauge + ultGaugeIncreseAmount;
            eUltGaugeChanged(ultGauge);

            // 사운드 재생
            if(attackSound != null)
                attackSound.Play();

            // 이미 Attack 애니메이션이 실행중이면 현재 애니메이션을 재실행한다.
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("AttackGround")
                || animator.GetCurrentAnimatorStateInfo(0).IsName("AttackOnAir"))
            {
                RestartCurrentAnimation();
            }
            else
            {
                animator.SetTrigger("Attack");
            }
        }
    }

    private void UltAttack()
    {
        // 궁극기 오브젝트 On
        ult.SetActive(true);

        // 궁극기 게이지 초기화
        ultGauge = 0;
        eUltGaugeChanged(ultGauge);

        // 궁 실행
        animator.SetBool("OnUlt", true);
    }

}
