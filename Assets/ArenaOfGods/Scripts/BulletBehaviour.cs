using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletBehaviour : NetworkBehaviour {

    [Header("Debug")]
    [SerializeField] private bool _showDebugMessages;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<StunBehaviour>().StartStun();
        }

        if (_showDebugMessages) Debug.Log("Iniciando Destruição da Bala...");
        NetworkServer.Destroy(this.gameObject);
    }
}
