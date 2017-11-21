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

    [SyncVar(hook = "OnActiveChanged")]
    [SerializeField] public bool IsActive;

    [SerializeField] public List<Carrot> CarrotsList = new List<Carrot>();

    private void Start()
    {
        if (GameInstance == null)
            GameInstance = this.gameObject;

        IsActive = this.gameObject.activeInHierarchy;
    }

    #region PlayerOwnerChange
    private void OnPlayerOwnerIdChanged(int newId)
    {
        //Debug.Log("SERVER HOOK > Mudando dono da cenoura: " + Id);
        //CmdChangePlayerOwnerId(newId);
        //RpcChangePlayerOwnerId(newId);
        //ChangePlayerOwnerId(newId);
    }

    [ClientRpc]
    private void RpcChangePlayerOwnerId(int id)
    {
        Debug.Log("RPC OWNER ID");
        ChangePlayerOwnerId(id);
    }

    [Command]
    private void CmdChangePlayerOwnerId(int id)
    {
        Debug.Log("CMD OWNER ID");
        ChangePlayerOwnerId(id);
    }

    private void ChangePlayerOwnerId(int id)
    {
        Debug.Log("Mudando dono da cenoura: " + Id);
        PlayerOwnerId = id;
    }
    #endregion

    #region ActiveChange
    private void OnActiveChanged(bool newValue)
    {
        //Debug.Log("SERVER HOOK > Mudando status do objeto da cenoura: " + Id);
        //CmdChangeActiveValue(newValue);
        //RpcChangeActiveValue(newValue);
        //ChangeActiveValue(newValue);
    }

    [ClientRpc]
    private void RpcChangeActiveValue(bool value)
    {
        Debug.Log("RPC ACTIVE");
        ChangeActiveValue(value);
    }

    [Command]
    private void CmdChangeActiveValue(bool value)
    {
        Debug.Log("CMD ACTIVE");
        ChangeActiveValue(value);
    }

    private void ChangeActiveValue(bool value)
    {
        Debug.Log("Changing Active");
        IsActive = value;
        GameInstance.SetActive(IsActive);
    }
    #endregion
}
