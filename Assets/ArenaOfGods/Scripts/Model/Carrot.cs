using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Carrot : NetworkBehaviour{

	[Header("Carrot Game Instance")]
    [SerializeField] public GameObject GameInstance;

    [Header("Info")]
    [SerializeField] public int Id;
    [SyncVar(hook = "OnPlayerOwnerIdChanged")]
    [SerializeField] public int PlayerOwnerId = -1;

    private void OnPlayerOwnerIdChanged(int newId)
    {
        RpcChangePlayerOwnerId(newId);
    }

    [ClientRpc]
    private void RpcChangePlayerOwnerId(int id)
    {
        PlayerOwnerId = id;
    }
}
