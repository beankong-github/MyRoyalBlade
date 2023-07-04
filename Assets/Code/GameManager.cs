using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public Player player;
    public MainUI ui;
    public PoolManager poolManager;

    // data
    private bool gameStop;

    [SerializeField]
    private LEVEL curLevel;
    private LEVEL prevLevel;

    [SerializeField]
    private int combo;
    [SerializeField]
    private long score;
    public long[] targetScore;
    public float progress; // 0 ~ 1

    public Vector3 genPos;
    public float genSpace;
    private float curTime;

    // event
    public delegate void GameEvent();
    public static event GameEvent eGameStart, eGamePause, eGameContinue,eGameOver, eGameClear;

    public delegate void LevelEvent(int _level);
    public static event LevelEvent eLevelChanged;


    // Properties 
    public static GameManager Instance { get => instance; }
    public int Level { get => (int)curLevel; }
    public long Score { get => score; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (poolManager == null)
        {
            poolManager = GetComponentInChildren<PoolManager>();
        }

        // Event 구독
        Player.ePlayerDead += GameOver;
        Player.ePlayerHit += ResetCombo;
        FallObject.eObjectDestroyed += OnFallObjectDestroyed;
    }

    private void Update()
    {
        if (gameStop)
            return;

        curTime += Time.deltaTime;

        if (curTime >= genSpace)
        {
            GameObject fallObject = poolManager.Generate((int)curLevel);
            fallObject.transform.position = genPos;

            curTime = 0;
        }

        genPos = player.transform.position;
        genPos.y += 15;
    }

    private void LateUpdate()
    {
        if (progress == 1 && !gameStop)
        {
            GameClear();
            Pause();
        }
        if (prevLevel != curLevel)
        {
            ChangeLevel((int)curLevel);
            prevLevel = curLevel;
        }
    }

    public void InitGame()
    {
        curLevel = LEVEL.LEVEL_0;
        Time.timeScale = 1;
        gameStop = false;
        combo = 0;
        score = 0;
        curTime = 0f;
        progress = 0f;
        prevLevel = curLevel;

        eGameStart();
    }
    public void Pause()
    {
        Time.timeScale = 0;
        gameStop = true;
        eGamePause();
    }

    public void Continue()
    {
        Time.timeScale = 1;
        gameStop = false;
        eGameContinue();
    }

    public void GameClear()
    {
        Debug.Log("GameClear");
        eGameClear();
    }

    public void GameOver()
    {
        Debug.Log("GameOver");
        eGameOver();
        gameStop = true;
    }
    public void ExitGame()
    {
        Debug.Log("Quit");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void OnFallObjectDestroyed(float _hp)
    {
        AddScore(_hp);
    }

    public void ResetCombo()
    {
        combo = 0;
    }

    private void AddScore(float _score)
    {
        // 콤보
        ++combo;
        GameObject comboUI = poolManager.GenerateUI((int)UI_POOL_TYPE.COMMBO);
        comboUI.GetComponent<TextMeshProUGUI>().text = combo.ToString() + " Combo!";
   
        // 점수 계산
        // 콤보 보너스
        _score += _score * (combo * 0.05f);

        // 공중 보너스
        if (!player.OnGround)
        {
            _score *= 1.5f;

            // 공중 보너스 UI
            poolManager.GenerateUI((int)UI_POOL_TYPE.AIR_SHOT);

        }

        // 점수 UI
        GameObject scoreUI = poolManager.GenerateUI((int)UI_POOL_TYPE.SCORE);
        scoreUI.GetComponent<TextMeshProUGUI>().text = _score.ToString();


        score += (long)_score;

        if (score >= targetScore[(int)curLevel])
        {
            if ((int)curLevel + 1 == (int)LEVEL._END_)
                GameClear();
            else
                ChangeLevel((int)curLevel + 1);
        }

        // UI에 반영
        ui.UpdateScore(score.ToString());

        // 진행도 계산 
        UpdateProgress();
    }

    private void UpdateProgress()
    {
        int lv = (int)curLevel;
        float start = lv > 0 ? score - targetScore[lv - 1] : score;
        float end = lv > 0 ? targetScore[lv] - targetScore[lv - 1] : targetScore[lv];

        progress = lv * 0.333f;
        progress += start / end * 0.333f;
        
        if (progress >= 0.999)
            progress = 1f;
    }


    public void ChangeLevel(int _level)
    {
        curLevel = (LEVEL)_level;
        eLevelChanged(_level);
    }
}
