    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharAnimationsController : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool _showDebugMessages;

    [SerializeField] private Animator _myAnimator;
    public float _speed;

    [Header("Controles")]
    public bool WithGun;
    public float TimePlanting = 3.0f;
    public float TimeStoling = 3.0f;
    public float cooldown = 0.5f;
    private bool canShoot = false;
    

    [Header("Objects")] public GameObject Gun;

    [Header("Particles")]
    public List<ParticleSystem> ShootVFX = new List<ParticleSystem>();

    [Header("Animation Sync")]
    [SerializeField] private SyncAnimation _syncAnimation;

    void Awake()
    {

    }

    // Use this for initialization
    void Start()
    {
        if (_myAnimator == null) _myAnimator = GetComponent<Animator>();
      

    }

    // Update is called once per frame
    void Update()
    {
        UpdateSpeed();
    }

    public void UpdateAnimations(float speed, bool planting, bool stoling, bool withGun)
    {
        _syncAnimation.SyncAnimations(speed, planting, stoling, withGun);
    }

    public void SyncAnimationsNow(float speed, bool planting, bool stoling, bool withGun)
    {
        if (_showDebugMessages) Debug.Log("LOCAL > Atualizando animações");
        _speed = speed;


        if (withGun)
        {
            print("Speed: " + speed);
            if (speed > 0.02f)
            {
                SetHasGun(false);
            }
            else
            {
                SetHasGun(true);
            }
        }
        else
        {
            SetHasGun(false);
        }

        SetPlanting(planting);
        SetStoling(stoling);
    }

    /// <summary>
    /// Atira
    /// TODO: retirar a checagem de input quando juntar com o script correto do personagem
    /// </summary>
    public void Shoot()
    {
        StartCoroutine(ShootRoutine());
    }

    private IEnumerator ShootRoutine()
    {
        _myAnimator.SetTrigger("Shoot");
        ShootVFX[0].Play(true);
        ParticleSystem _tempPart = ShootVFX[0];
        ShootVFX.RemoveAt(0);
        ShootVFX.Add(_tempPart);
        canShoot = false;
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }

    private void UpdateSpeed()
    {
        _myAnimator.SetFloat("Speed", _speed);
    }

    private void SetHasGun(bool hasGun)
    {
        Gun.SetActive(hasGun);
        WithGun = hasGun;
        
        _myAnimator.SetBool("WithGun", WithGun);
    }

    private void SetPlanting(bool planting)
    {
        _myAnimator.SetBool("Planting", planting);
        if (planting)
        {
            StartCoroutine(PlantingRoutine());
        }
    }

    private void SetStoling(bool stoling)
    {
        _myAnimator.SetBool("Stoling", stoling);
        if (stoling)
        {
            StartCoroutine(StolingRoutine());
        }
    }

    /// <summary>
    /// Quando personagem acaba de roubar e joga a cenoura para dentro do cesto
    /// </summary>
    private void StolingDone()
    {
        _myAnimator.SetTrigger("StolingDone");
        _myAnimator.SetBool("Stoling", false);

    }

    IEnumerator PlantingRoutine()
    {
        yield return new WaitForSeconds(TimePlanting);
        SetPlanting(false);

    }
    IEnumerator StolingRoutine()
    {
        yield return new WaitForSeconds(TimeStoling);
        //PlantingRoutine();
        StolingDone();
    }

    void OnValidate()
    {
        if (_myAnimator == null) _myAnimator = GetComponent<Animator>();
        Gun.SetActive(WithGun);
        SetHasGun(WithGun);
    }
}