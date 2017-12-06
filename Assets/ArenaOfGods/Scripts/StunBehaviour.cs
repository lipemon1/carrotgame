using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StunBehaviour : NetworkBehaviour {

	[Header("Debug")]
    [SerializeField] private bool _showDebugMessages;

    [Header("Stats")]
    [SerializeField] private bool _isStuned;

    [Header("Configs")]
    [SerializeField] private float _stunTime;

    public bool GetStunedValue() { return _isStuned; }

    [Header("Particle")]
    [SerializeField]private GameObject _stunParticle;
    [SerializeField] private float _particleSpawnY;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.LogWarning("Start Stun");
            StartStun();
        }
    }

    /// <summary>
    /// Inicia o Stun
    /// </summary>
    public void StartStun()
    {
        if (_showDebugMessages) Debug.Log("Iniciando Stun");
        ChangeStunedValue(true);
        Invoke("StopStun", _stunTime);
        if (_showDebugMessages) Debug.Log("Terminando Stun em " + _stunTime.ToString("F0") + " segundos...");
    }

    /// <summary>
    /// Para o Stun
    /// </summary>
    private void StopStun()
    {
        ChangeStunedValue(false);
        if (_showDebugMessages) Debug.Log("Stun finalizado");
    }

    /// <summary>
    /// Modifica o valor de stun
    /// </summary>
    /// <param name="value"></param>
    private void ChangeStunedValue(bool value)
    {
        if (_showDebugMessages) Debug.Log("Iniciando troca de status do Is Stuned com valor: " + value);

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

        // JEFESU CHAMAR PARTICULA DE STUN AQUI
        if(value) SpawnParticle();
    }

    void SpawnParticle()
    {
        Vector3 position = new Vector3(transform.position.x, _particleSpawnY, transform.position.z);
        Instantiate(_stunParticle, position, Quaternion.identity);
    }
    #endregion
}
