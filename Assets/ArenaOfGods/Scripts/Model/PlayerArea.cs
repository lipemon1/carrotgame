using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class PlayerArea : NetworkBehaviour{

	[Header("Player Area Instance")]
    [SerializeField] public GameObject GameInstance;

    [Header("Info")]
    [SerializeField] public int Id;

    [SyncVar(hook = "OnPlayerOwnerIdChanged")]
    [SerializeField] public int PlayerOwnerId;
    [SerializeField] public List<Carrot> CarrotsList = new List<Carrot>();

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
