using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    public enum InputMode
    {
        Standard,
        CrossPlatform,
        Controller
    }

    [Header("Debug Options")]
    [SerializeField]
    private bool _showDebugMessages;
    [SerializeField] private bool _isMoving;

    [Header("Move Configs")]
    [SerializeField]
    private InputMode _inputType;

    [Header("Animations Controller")]
    [SerializeField] private CharAnimationsController _animController;    

    [Header("Inputs Standard")]
    [SerializeField]
    private float _horizontalStandard;
    [SerializeField] private float _verticalStandard;
    public string HorizontalInputString;
    public string VerticalInputString;

    [Header("Character Controller")]
    [HideInInspector]
    private CharacterController _characterController;

    [Header("Move Params")]
    [SerializeField]
    private float _turnSpeed = 300.0f;
    [SerializeField] private Vector3 _vectorDirection = new Vector3(0, 0, 0);
    [SerializeField] private float _moveSpeed = 5.0f;
    [SerializeField] private float _jumpForce = 5.0f;
    [SerializeField] private float _jumpGravity = 3.0f;

    [Header("Status Markers")]
    [SerializeField] private bool _stolingCarrot;
    [SerializeField] private bool _hasGun;

    [Header("Shooting")] 
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _bulletSpawn;
    [SerializeField] private float _bulletLifetime = 2f;
    [SerializeField] private float _bulletSpeed = 5f;

    [Header("Animation")]
    [SerializeField]
    private Animator _modelAnimator;

    [Header("Stun Controller")]
    [SerializeField] private StunBehaviour _stunBehaviour;

    [Header("Setup")] public SetupLocalPlayer setupLocalPlayer;

    // Use this for initialization
    void Start()
    {
        if (_characterController == null)
        {
            _characterController = GetComponent<CharacterController>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        RecieveInput();

        if (_stunBehaviour.GetStunedValue() == false)
        {
            MovePlayer();
            UpdateCharAnimations();
        }

        // TODO Barulho dos passos
        //UpdateMoveStatus();
        SoundManager.Instance.UpdateRunningAudio(GetSpeedToUseOnMoveCheck());
    }

    /// <summary>
    /// Movimenta o jogador
    /// </summary>
    void MovePlayer()
    {
        Vector3 directionToMove = GetDirectionToMove();
        Vector3 pointToLook = new Vector3(directionToMove.x, transform.position.y, directionToMove.z);

        transform.LookAt(pointToLook);

        _characterController.Move(directionToMove * _moveSpeed * Time.deltaTime);
        _characterController.SimpleMove(Physics.gravity);
    }

    /// <summary>
    /// Passa os parametros para o controlador de animações para atualiza-la
    /// </summary>
    void UpdateCharAnimations()
    {
        float biggestInput = Mathf.Abs(GetBiggestCurInput(_horizontalStandard, _verticalStandard));

         if(_showDebugMessages) print("" + biggestInput);
         
        _animController.UpdateAnimations(biggestInput, false, _stolingCarrot, _hasGun);
    }

    private float GetBiggestCurInput(float x, float y)
    {
        if (x > y)
            return x;
        else
            return y;
    }

    /// <summary>
    /// Método que retorna qual a direção de movimento que será utilizada para se movimentar
    /// </summary>
    /// <returns></returns>
    private Vector3 GetDirectionToMove()
    {
        Vector3 directionToMove = Vector3.zero;

        switch (_inputType)
        {
            case InputMode.Standard:
                directionToMove = _verticalStandard * transform.TransformDirection(Vector3.forward);
                transform.Rotate(new Vector3(0, _horizontalStandard * _turnSpeed * Time.deltaTime, 0));
                break;

            case InputMode.CrossPlatform:
                //old and perfect
                //Vector2 directionOnScreen = new Vector2(_horizontalCrossPlatform, _verticalCrossPlatform);
                //Plane plane = new Plane(Vector3.up, Vector3.zero);

                //Debug.DrawRay(Vector3.zero, directionOnScreen * 100, Color.red);

                //Ray ray0 = Camera.main.ScreenPointToRay(Vector3.zero);
                //Ray ray1 = Camera.main.ScreenPointToRay(directionOnScreen);

                //float distance0;
                //float distance1;
                //if (plane.Raycast(ray0, out distance0) && plane.Raycast(ray1, out distance1))
                //{
                //    Vector3 directionOnPlane = ray1.GetPoint(distance1) - ray0.GetPoint(distance0);

                //    Debug.DrawRay(Vector3.zero, directionOnPlane * 100);

                //    forward = directionOnPlane.normalized;
                //}

                //new and better
//                Vector3 directionOnScreen = new Vector3(_horizontalCrossPlatform, _verticalCrossPlatform);
//                Vector3 directionOnWorld = Camera.main.ScreenToWorldPoint(directionOnScreen + Vector3.forward) - Camera.main.ScreenToWorldPoint(Vector3.forward);
//                Vector3 directionOnPlane = Vector3.ProjectOnPlane(directionOnWorld, Vector3.up);
//                directionToMove = directionOnPlane;
                break;
            
            case InputMode.Controller:
                Vector3 directionOnScreen = new Vector3(_horizontalStandard, _verticalStandard);
                Vector3 directionOnWorld = Camera.main.ScreenToWorldPoint(directionOnScreen + Vector3.forward) - Camera.main.ScreenToWorldPoint(Vector3.forward);
                Vector3 directionOnPlane = Vector3.ProjectOnPlane(directionOnWorld, Vector3.up);
                directionToMove = directionOnPlane;
                break;

            default:
                if (_showDebugMessages) Debug.LogError("Input Type Unkown: " + _inputType);
                break;
        }

        return directionToMove.normalized;
    }

    /// <summary>
    /// Recebendo o tipo de entrada do usuario
    /// </summary>
    private void RecieveInput()
    {
        switch (_inputType)
        {
            case InputMode.Standard:
                _horizontalStandard = Input.GetAxis("HorizontalStandard");
                _verticalStandard = Input.GetAxis("VerticalStandard");
                break;
            case InputMode.CrossPlatform:
//                if (_easyJoystick == null)
//                {
//                    if (_showDebugMessages) Debug.Log("Searching for EasyJoystick script");
//                    _easyJoystick = GameObject.Find("ScreenJoystick").GetComponentInChildren<EasyJoystick>();
//                }
//                else
//                {
//                    if (_showDebugMessages) Debug.Log("Getting input values");
//                    _horizontalCrossPlatform = _easyJoystick.MoveInput().x;
//                    _verticalCrossPlatform = _easyJoystick.MoveInput().z;
//                }
                break;

            case InputMode.Controller:
                _horizontalStandard = Input.GetAxis(HorizontalInputString);
                _verticalStandard = Input.GetAxis(VerticalInputString);
                break;
            
            default:
                if (_showDebugMessages) Debug.LogError("Input Type Unkown: " + _inputType);
                break;
        }
    }

    public void Fire()
    {
        if (_showDebugMessages) Debug.Log("Iniciando tiro");

        SpawnBullet();
    }

    /// <summary>
    /// Diz se o jogador está de movendo ou não
    /// </summary>
    private void UpdateMoveStatus()
    {
        _isMoving = Mathf.Abs(GetBiggestCurInput(_horizontalStandard, _verticalStandard)) > 0f && Mathf.Abs(GetBiggestCurInput(_horizontalStandard, _verticalStandard)) < 1f;
    }

    /// <summary>
    /// Retorna a soma das velocidades
    /// </summary>
    /// <returns></returns>
    private float GetSpeedToUseOnMoveCheck()
    {
        return Mathf.Abs(_horizontalStandard) + Mathf.Abs(_verticalStandard);
    }


    public void SetHasGun(bool hasGun)
    {
        _hasGun = hasGun;
    }

    public void SpawnBullet()
    {
        if (_showDebugMessages) Debug.LogWarning("LOCAL > Atirando");
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            _bulletPrefab,
            _bulletSpawn.position,
            _bulletSpawn.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * _bulletSpeed;

        // Destroy the bullet after _bulletLifetime seconds
        Destroy(bullet, _bulletLifetime);

        _animController.Shoot();

        SoundManager.Instance.Shoot();
    }
}
