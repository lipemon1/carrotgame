using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletBehaviour : NetworkBehaviour {

    [Header("Debug")]
    [SerializeField] private bool _showDebugMessages;

    void OnCollisionEnter()
    {
        if (_showDebugMessages) Debug.Log("Iniciando Destruição da Bala...");
        if (_showDebugMessages) Debug.Log("I'm a Server: " + isServer);
        if (_showDebugMessages) Debug.Log("I'm a Client: " + isClient);

        NetworkServer.Destroy(this.gameObject);
    }
}
