using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class PoolManager : MonoBehaviour
{ 
    public  int     poolSize; // 풀 사이즈
    private int     curLevel; // 현재 레벨 정보 

    public GameObject uiParent;  // dynamic ui 부모 오브젝트

    public GameObject[] prefabs;    // 프리팹 배열
    public GameObject[] ui_prefabs;    // 프리팹 배열

    List<GameObject>[]  pools;              // pool 배열
    List<GameObject>[]  ui_pools;           // dynamic ui를 위한 풀 

    private void Awake()
    {
        // 현재 레벨 정보 등록
        curLevel = GameManager.Instance.Level;

        // Event 구독
        GameManager.eLevelChanged += OnChangeLevel;
        GameManager.eGameStart += FillPool;
        GameManager.eGameOver += ClearPool;
        GameManager.eGameOver += ClearUIPool;
        GameManager.eGameClear += ClearPool;
        GameManager.eGameClear += ClearUIPool;

        // go pool 초기화
        int size = (int)GO_POOL_TYPE._END_;
        pools = new List<GameObject>[size];
        for (int i = 0; i < size; ++i)
        {
            pools[i] = new List<GameObject>();
        }

        // ui pool 초기화
        size = (int)UI_POOL_TYPE._END_;
        ui_pools = new List<GameObject>[size];
        for (int i = 0; i < size; ++i)
        {
            ui_pools[i] = new List<GameObject>();
        }


        // 오브젝트 생성
        FillPool();
        FillUIPool();
    }


    private void FillPool()
    {
        if (pools[0].Count > 0)
            ClearPool();

        for (int i = 0; i < pools.Length; ++i)
        {
            for (int j = 0; j < poolSize; ++j)
            {
                string name = ((GO_POOL_TYPE)i).ToSafeString();

                Debug.Assert(prefabs[i], "PoolManager에" + name + " 프리팹이 등록되어 있지 않습니다!!");

                GameObject go = Instantiate(prefabs[i], transform);
                go.name = name + "_" + j.ToSafeString();
                go.SetActive(false);
                pools[i].Add(go);
            }
        }

    }

    private void FillUIPool()
    {
        if (ui_pools[0].Count > 0)
            ClearUIPool();

        for (int i = 0; i < ui_pools.Length; ++i)
        { 
            for (int j = 0; j < poolSize; ++j)
            {
                string name = ((UI_POOL_TYPE)i).ToSafeString();

                Debug.Assert(ui_prefabs[i], "PoolManager에" + name + " 프리팹이 등록되어 있지 않습니다!!");
                
                GameObject ui = Instantiate(ui_prefabs[i], uiParent.transform);
                ui.name = name + "_" + j.ToSafeString();
                ui.SetActive(false);
                ui_pools[i].Add(ui);
            }
        }
    }

    private void OnChangeLevel(int _level)
    {
        curLevel = _level;
    }


    public GameObject Generate(int _poolType ,GameObject _parent = null)
    {
        Debug.Assert(_poolType < (int)GO_POOL_TYPE._END_, "유효하지 않은 풀 오브젝트 생성 시도.");

        GameObject select = null;

        // pool에서 가장 먼저 만나는 비활성화된 게임 오브젝트를 반환한다
        foreach (var item in pools[_poolType])
        {
            if (!item.gameObject.activeSelf)
            {
                select = item;
                break;
            }
        }

        // 모든 풀의 오브젝트가 사용중이라면?
        if (select == null)
        {
            // list 재할당이 발생했음을 log로 알린다. poolSize 조정 필요.
            string objName = ((GO_POOL_TYPE)_poolType).ToString();
            Debug.Log("오브젝트 풀에서 재할당 발생: " + objName);

            // 새로운 오브젝트를 만들어 pool에 추가한 뒤 select에 담아 반환한다.
            select = Instantiate(prefabs[_poolType], transform);
            pools[_poolType].Add(select);
            select.name = objName + '_' + (pools[_poolType].Count - 1).ToString();
        }

        // 오브젝트 active 및 위치 초기화
        select.SetActive(true);
        select.transform.position = Vector3.zero;

        // 부모 오브젝트를 전달해줬다면 부모 오브젝트의 자식으로 설정
        if (_parent != null)
            select.transform.SetParent(_parent.transform);

        return select;
    }

    public GameObject GenerateUI(int _uiType)
    {
        Debug.Assert(_uiType < (int)UI_POOL_TYPE._END_, "유효하지 않은 UI 풀 오브젝트 생성 시도.");

        GameObject select = null;

        // pool에서 가장 먼저 만나는 비활성화된 게임 오브젝트를 반환한다
        foreach (var item in ui_pools[_uiType])
        {
            if (!item.gameObject.activeSelf)
            {
                select = item;
                break;
            }
        }

        // 모든 풀의 오브젝트가 사용중이라면?
        if (select == null)
        {
            string uiName = ((UI_POOL_TYPE)_uiType).ToString();
            Debug.Log("UI 오브젝트 풀에서 재할당 발생: " + uiName);

            // 새로운 오브젝트를 만들어 ui_pool에 추가한 뒤 select에 담아 반환한다.
            select = Instantiate(ui_prefabs[_uiType], uiParent.transform);
            ui_pools[_uiType].Add(select);
            select.name = uiName + '_' + (ui_pools[_uiType].Count - 1).ToString();
        }

        select.SetActive(true);
        return select;
    }

    void ClearPool()
    {
        for (int p = 0; p < pools.Length; ++p)
        {
            for (int i = 0; i < pools[p].Count; ++i)
            {
                Destroy(pools[p][i]);
            }
            pools[p].Clear();
        }
    }

    void ClearUIPool()
    {
        for (int p = 0; p < ui_pools.Length; ++p)
        {
            for (int i = 0; i < ui_pools[p].Count; ++i)
            {
                Destroy(ui_pools[p][i]);
            }
            ui_pools[p].Clear();
        }
    }
}
