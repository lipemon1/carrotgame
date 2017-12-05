using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StunBehaviour : NetworkBehaviour {

	[Header("Debug")]
    [SerializeField] private bool _showDebugMessages;

    [Header("Stats")]
    [SerializeField] private bool _isStuned;


    public bool GetStunedValue() { return _isStuned; }

    public void ChangeStunedValue(bool value)
    {
        if (_showDebugMessages) Debug.Log("Iniciando troca de status do Is Stuned");

        if (isServer)
        {
            RpcChangeStunedValue(value);
            return;
        }

        if (isClient)
        {
            CmdChangeStunedValue(value);
        }
    }

    #region Stuned Stats
    [Command]
    private void CmdChangeStunedValue(bool value)
    {
        if (_showDebugMessages) Debug.Log("COMAND > Mudando status do Is Stuned para: " + value);
        RpcChangeStunedValue(value);
    }

    [ClientRpc]
    private void RpcChangeStunedValue(bool value)
    {
        if (_showDebugMessages) Debug.Log("RPC > Mudando status do Is Stuned para: " + value);
        ChangeStunedValueNow(value);
    }

    void ChangeStunedValueNow(bool value)
    {
        if (_showDebugMessages) Debug.Log("LOCAL > Mudando status do Is Stuned para: " + value);
        _isStuned = value;
    }
    #endregion
}
