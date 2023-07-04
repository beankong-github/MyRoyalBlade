using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using static Cinemachine.DocumentationSortingAttribute;

public class PlayerUltEffectScript : MonoBehaviour
{
    private ParticleSystem particleSys;
    private AudioSource sound;
    private int level;
    public Sprite[] sprites;

    private void Awake()
    {
        particleSys = GetComponent<ParticleSystem>();
        sound = GetComponent<AudioSource>();

        GameManager.eLevelChanged += ChangeLevel;
        GameManager.eGameStart += InitLevel;
        GameManager.eGamePause += StopOnOff;
        GameManager.eGameContinue += StopOnOff;
            
    }

    void InitLevel()
    {
        level = 0;
    }

    void ChangeLevel(int _level)
    {
        level = _level;
    }

    private void StopOnOff()
    {
        if (sound == null)
            return;

        if (gameObject.activeSelf)
        {
            if (sound.isPlaying)
                sound.Stop();
            else
                sound.Play();
        }
    }

    [System.Obsolete]
    private void OnEnable()
    {
        Debug.Assert(particleSys, "ParticleSystem 컴포넌트 없음");

        if (sprites.Length > level)
            particleSys.textureSheetAnimation.SetSprite(0, sprites[level]);

        if (level == 1)
            particleSys.startSize = 0.2f;
        else
            particleSys.startSize = 0.6f;

    }

}
