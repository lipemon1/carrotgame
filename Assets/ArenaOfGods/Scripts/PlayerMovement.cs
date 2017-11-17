﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    public enum InputMode
    {
        Standard,
        CrossPlatform
    }

    [Header("Debug Options")]
    [SerializeField] private bool _showDebugMessages;

    [Header("Move Configs")]
    [SerializeField]
    private InputMode _inputType;

    [Header("Inputs Standard")]
    [SerializeField]
    private float _horizontalStandard;
    [SerializeField] private float _verticalStandard;

    [Header("Inputs CrossPlatform")]
    [SerializeField]
    private Joystick _joystick;
    [SerializeField] private float _horizontalCrossPlatform;
    [SerializeField] private float _verticalCrossPlatform;

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

    [Header("Animation")]
    [SerializeField]
    private Animator _modelAnimator;

    // Use this for initialization
    void Start()
    {
        if (_characterController == null)
        {
            _characterController = GetComponent<CharacterController>();
        }

        _joystick = GameObject.Find("ScreenJoystick").GetComponentInChildren<Joystick>();
    }

    // Update is called once per frame
    void Update()
    {
        RecieveInput();
        MovePlayer();
    }

    /// <summary>
    /// Movimenta o jogador
    /// </summary>
    void MovePlayer()
    {
        _characterController.Move(GetDirectionToMove() * _moveSpeed * Time.deltaTime);
        _characterController.SimpleMove(Physics.gravity);

        Jump();
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

                //new and maybe wrong but better
                float distance = 1;
                Vector3 directionOnScreen = new Vector3(_horizontalCrossPlatform, _verticalCrossPlatform);
                Vector3 directionOnWorld = Camera.main.ScreenPointToRay(directionOnScreen).GetPoint(distance) - Camera.main.ScreenPointToRay(Vector2.zero).GetPoint(distance);
                Vector3 directionOnPlane = Vector3.ProjectOnPlane(directionOnWorld, Vector3.up);
                directionToMove = directionOnPlane;
                break;

            default:
                if(_showDebugMessages) Debug.LogError("Input Type Unkown: " + _inputType);
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
                _horizontalStandard = Input.GetAxis("Horizontal");
                _verticalStandard = Input.GetAxis("Vertical");
                break;
            case InputMode.CrossPlatform:
                if (_joystick == null)
                {
                    if (_showDebugMessages) Debug.Log("Searching for Joystick script");
                    _joystick = GameObject.Find("ScreenJoystick").GetComponentInChildren<Joystick>();
                }
                else
                {
                    if (_showDebugMessages) Debug.Log("Getting input values");
                    _horizontalCrossPlatform = _joystick._inputValues.Horizontal;
                    _verticalCrossPlatform = _joystick._inputValues.Vertical;
                }
                break;

            default:
                if (_showDebugMessages) Debug.LogError("Input Type Unkown: " + _inputType);
                break;
        }
    }

    /// <summary>
    /// Método que tenta realizar o pulo do jogador
    /// </summary>
    private void Jump()
    {
        if (Input.GetButton("Jump"))
        {
            if (_characterController.isGrounded == true)
            {
                _vectorDirection.y = _jumpForce;
            }
        }

        _vectorDirection.y -= _jumpGravity * Time.deltaTime;
        _characterController.Move(_vectorDirection * Time.deltaTime);
    }
}
