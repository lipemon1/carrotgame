using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel = 0, sendInterval = 1f)]
public class SyncAnimation : NetworkBehaviour {

    [Header("Debug")]
    [SerializeField] private bool _showDebugMessages;

    [Header("CharAnimations")]
    [SerializeField] private CharAnimationsController _charAnimController;

    public void SyncAnimations(float speed, bool planting, bool stoling, bool withGun)
    {
        if (_showDebugMessages) Debug.Log("Iniciando update de animações");

        if (isServer)
        {
            RpcSyncAnimations(speed, planting, stoling, withGun);
            return;
        }

        if (isClient)
        {
            CmdSyncAnimations(speed, planting, stoling, withGun);
        }
    }

    [Command]
    public void CmdSyncAnimations(float speed, bool planting, bool stoling, bool withGun)
    {
        if (_showDebugMessages) Debug.Log("COMAND > Atualizando animações");
        RpcSyncAnimations(speed, planting, stoling, withGun);
    }

    [ClientRpc]
    public void RpcSyncAnimations(float speed, bool planting, bool stoling, bool withGun)
    {
        if (_showDebugMessages) Debug.Log("RPC > Atualizando animações");
        _charAnimController.SyncAnimationsNow(speed, planting, stoling, withGun);
    }
}
