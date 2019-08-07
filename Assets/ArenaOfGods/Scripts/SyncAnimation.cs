using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SyncAnimation : MonoBehaviour {

    [Header("Debug")]
    [SerializeField] private bool _showDebugMessages;

    [Header("CharAnimations")]
    [SerializeField] private CharAnimationsController _charAnimController;

    public void SyncAnimations(float speed, bool planting, bool stoling, bool withGun)
    {
        if (_showDebugMessages) Debug.Log("Iniciando update de animações");

        _charAnimController.SyncAnimationsNow(speed, planting, stoling, withGun);
    }
}
