using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class LobbyController : MonoBehaviour {

    [Header("Network Discovery")]
    [SerializeField] private NetworkDiscovery _networkDiscovery;

    [Header("Functions to Call")]
    [SerializeField] private UnityEvent _onStart;
    [SerializeField] private UnityEvent _onEnd;

    public void StartServerAndClient()
    {
        NetworkManager.singleton.StartHost();
        _networkDiscovery.Initialize();
        _networkDiscovery.StartAsServer();

        _onStart.Invoke();
    }

    public void StartClient()
    {
        _networkDiscovery.Initialize();
        _networkDiscovery.StartAsClient();
        NetworkManager.singleton.StartClient();

        _onEnd.Invoke();
    }
}
