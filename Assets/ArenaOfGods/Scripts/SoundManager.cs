using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioType
{
    MessWithDirt,
    CarrotPickup,
    CarrotPlaced,
    Shoot,
    StepOnDirt,
    Stun,
    WinGame,
    LoseGame,
    ClickedButton
}

[System.Serializable]
public class Sounds
{
    [Header("Plantar / Roubar")]
    [SerializeField]
    private AudioClip _messWithDirt;
    [SerializeField] private AudioClip _carrotPickup;
    [SerializeField] private AudioClip _carrotPlaced;

    [Header("Player")]
    [SerializeField]
    private AudioClip _shoot;
    [SerializeField] private AudioClip _stepOnDirt;
    [SerializeField] private AudioClip _stun;

    [Header("Victory")]
    [SerializeField]
    private AudioClip _win;
    [SerializeField] private AudioClip _lose;

    [Header("Interface")]
    [SerializeField] private AudioClip _clickedButton;

    public AudioClip GetAudioShot(AudioType audioType)
    {
        switch (audioType)
        {
            case AudioType.MessWithDirt:
                return _messWithDirt;
                break;
            case AudioType.CarrotPickup:
                return _carrotPickup;
                break;
                break;
            case AudioType.CarrotPlaced:
                return _carrotPlaced;
                break;
            case AudioType.Shoot:
                return _shoot;
                break;
            case AudioType.StepOnDirt:
                return _stepOnDirt;
                break;
            case AudioType.Stun:
                return _stun;
                break;
            case AudioType.WinGame:
                return _win;
                break;
            case AudioType.LoseGame:
                return _lose;

            case AudioType.ClickedButton:
                return _clickedButton;
             
            default:
                throw new ArgumentOutOfRangeException("audioType", audioType, null);
        }
        return null;
    }
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioSource SfxSource;

    public float MinSpeedRun = 0.1f;
    public Sounds GameSounds;
    
    private bool _planting;
    private bool _running;

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {

    }


    /// <summary>
    /// Atualiza os sons com base nas informações do personagem
    /// </summary>
    /// <param name="speed">Player Speed</param>
    /// <param name="stoling">If Player is stoling/planting</param>
    public void UpdateAudio(float speed, bool stoling)
    {
        #region Running
        if (!_running)
        {
            if (speed > MinSpeedRun)
            {
                _running = true;
                InvokeRepeating("Running", 0.1f, 0.3f);
            }
        }
        else
        {
            if (speed < MinSpeedRun)
            {
                _running = false;
                CancelInvoke("Running");
            }
        }

        #endregion

        #region Planting, Stoling

        if (_planting == false && stoling)
        {
            _planting = stoling;
            InvokeRepeating("Planting", 0.1f, 0.3f);
        }
        else if (_planting == true && !stoling)
        {
            _planting = stoling;
            CancelInvoke("Planting");
        }
        #endregion

    }

    /// <summary>
    /// Da play em um audio com base no tipo de audio escolhido
    /// </summary>
    /// <param name="audioType"></param>
    public void PlayAudio(AudioType audioType)
    {
        AudioClip clip = GameSounds.GetAudioShot(audioType);
        SfxSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Quando player Atira
    /// </summary>
    public void Shoot()
    {
        PlayAudio(AudioType.Shoot);
        print("SHOOT");
    }

    /// <summary>
    /// Quando player é stunado
    /// </summary>
    public void Stun()
    {
        PlayAudio(AudioType.Stun);
    }

    /// <summary>
    /// Quando jogador pega cenoura
    /// </summary>
    public void CarrotPickUp()
    {
        PlayAudio(AudioType.CarrotPickup);
    }

    /// <summary>
    /// Cenoura colocada no chao    
    /// </summary>
    public void CarrotPlaced()
    {
        PlayAudio(AudioType.CarrotPlaced);
    }

    /// <summary>
    /// Quando ganha o jogo
    /// </summary>
    public void WinGame()
    {
        PlayAudio(AudioType.WinGame);
    }

    /// <summary>
    /// Quando ganha o jogo
    /// </summary>
    public void LoseGame()
    {
        PlayAudio(AudioType.LoseGame);
    }

    public void ClickedButton()
    {
        PlayAudio(AudioType.ClickedButton);
    }


    private void Planting()
    {
        PlayAudio(AudioType.MessWithDirt);
    }

    private void Running()
    {
        PlayAudio(AudioType.StepOnDirt);
    }


}
