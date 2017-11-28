using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using Vuforia;

public class LobbyController : MonoBehaviour {

    public static LobbyController Instance { get; private set; }

    [Header("Debug")]
    [SerializeField] private bool _showDebugMessage;
    [SerializeField] private bool _usingVuforia;

    [Header("Status")]
    [SerializeField] private bool _gameStarted;
    [SerializeField] private bool _isTracking;

    [Header("Network Discovery")]
    [SerializeField] private NetworkDiscovery _networkDiscovery;

    [Header("Lobby Canvas")]
    [SerializeField] private GameObject _lobbyGameObject;
    [SerializeField] private GameObject _joystickCanvas;
    [SerializeField] private GameObject _targetCanvas;
    [SerializeField] private GameObject _debugCanvas;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CheckCanvasControllers();
    }

    public void StartServerAndClient()
    {
        NetworkManager.singleton.StartHost();
        _networkDiscovery.Initialize();
        _networkDiscovery.StartAsServer();

        CheckCanvasControllers();
    }

    public void StartClient()
    {
        _networkDiscovery.Initialize();
        _networkDiscovery.StartAsClient();

        CheckCanvasControllers();
    }

    public void OnTargetLost()
    {
        if (_showDebugMessage) Debug.Log("Target perdido...");
        _isTracking = false;

        CheckCanvasControllers();
    }

    public void OnTargetFound()
    {
        if (_showDebugMessage) Debug.Log("Target encontrado..."); 
        _isTracking = true;

        CheckCanvasControllers();
    }

    public void SetAsGameStarted()
    {
        if (_showDebugMessage) Debug.Log("Marcando que o jogo já foi iniciado (server sendo criado)");
        _gameStarted = true;

        CheckCanvasControllers();
    }

    /// <summary>
    /// Escolhe qual os canvas que serão mostrados e escondidos
    /// </summary>
    private void CheckCanvasControllers()
    {
        if (_usingVuforia)
        {
            _targetCanvas.SetActive(!_isTracking);

            if (_gameStarted)
            {
                _lobbyGameObject.gameObject.SetActive(false);

                _joystickCanvas.gameObject.SetActive(_isTracking);
                _debugCanvas.gameObject.SetActive(true);
            }
            else
            {
                _lobbyGameObject.gameObject.SetActive(_isTracking);

                _joystickCanvas.gameObject.SetActive(_isTracking);
                _debugCanvas.gameObject.SetActive(true);
            }
        }
        else
        {
            _targetCanvas.SetActive(false);

            if (_gameStarted)
            {
                _lobbyGameObject.gameObject.SetActive(false);

                _joystickCanvas.gameObject.SetActive(true);
                _debugCanvas.gameObject.SetActive(true);
            }
            else
            {
                _lobbyGameObject.gameObject.SetActive(true);

                _joystickCanvas.gameObject.SetActive(false);
                _debugCanvas.gameObject.SetActive(true);
            }
        }
       
    }
}
