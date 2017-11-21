using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SetupLocalPlayer : NetworkBehaviour {

    [System.Serializable]
    public struct SetupConfig
    {
        public bool PlayerMovement;
        public bool ItemPicker;
        public bool ItemDropper;
        public bool Inventory;
        public bool ActionButtonHandler;
        public bool GameData;
        public bool PlayerIdentity;
    }

    [Header("Debug")]
    [SerializeField] private bool _showDebugMessages;

    [Header("Stay active")]
    [SerializeField] private SetupConfig _componentsToActive;

    [Header("Scripts")]
    [SerializeField] public PlayerMovement PlayerMovement;
    [SerializeField] public ItemPicker ItemPicker;
    [SerializeField] public ItemDropper ItemDropper;
    [SerializeField] public Inventory Inventory;
    [SerializeField] public ActionButtonHandler ActionButtonHandler;
    [SerializeField] public GameData GameData;
    [SerializeField] public PlayerIdentity PlayerIdentity;

    [Header("Spawn Configs")]
    [SerializeField] private float _spawnYOffset = 5f;

	// Use this for initialization
	void Start () {

        ConfigureAllScripts();

        transform.SetParent(GameObject.Find("Arena").gameObject.transform);
        transform.position = new Vector3(transform.position.x, transform.position.y + _spawnYOffset, transform.position.z);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        SetMyId();
    }

    void ConfigureAllScripts()
    {
        if (isLocalPlayer)
        {
            if(_componentsToActive.PlayerMovement) PlayerMovement.enabled = true;
            if (_componentsToActive.ItemPicker) ItemPicker.enabled = true;
            if (_componentsToActive.ItemDropper) ItemDropper.enabled = true;
            if (_componentsToActive.Inventory) Inventory.enabled = true;
            if (_componentsToActive.ActionButtonHandler) ActionButtonHandler.enabled = true;
            if (_componentsToActive.PlayerIdentity) PlayerIdentity.enabled = true;

            if (_showDebugMessages) Debug.Log("Configurando setup local player de: " + gameObject.name);
        }
        else
        {
            if (_componentsToActive.PlayerMovement) PlayerMovement.enabled = false;
            if (_componentsToActive.ItemPicker) ItemPicker.enabled = false;
            if (_componentsToActive.ItemDropper) ItemDropper.enabled = false;
            if (_componentsToActive.Inventory) Inventory.enabled = false;
            if (_componentsToActive.ActionButtonHandler) ActionButtonHandler.enabled = false;
            if (_componentsToActive.PlayerIdentity) PlayerIdentity.enabled = false;
        }
    }

    public void SetMyId()
    {
        int curPlayers = GameData.PlayersConnectedsList.Count;
        int newPlayerId = ++curPlayers;

        while (GameData.SomeoneHasThisIp(newPlayerId))
        {
            newPlayerId++;
        }

        if (isServer)
        {
            RpcNewPlayerConnected(newPlayerId);
        }

        if (isClient)
        {
            CmdNewPlayerConnected(newPlayerId);
            NewPlayerConnected(newPlayerId);
        }

        PlayerIdentity.SetPlayerId(GameData.LastPlayerRegistered.PlayerId);
    }

    #region PLAYERS HOOKS
    #region Registrando Novo Jogador
    [Command]
    private void CmdNewPlayerConnected(int newPlayersAmount)
    {
        if (_showDebugMessages) Debug.Log("COMAND > Novo jogador conectado: " + newPlayersAmount);
        NewPlayerConnected(newPlayersAmount);
    }

    [ClientRpc]
    private void RpcNewPlayerConnected(int newPlayersAmount)
    {
        if (_showDebugMessages) Debug.Log("RPC > Novo jogador conectado: " + newPlayersAmount);
        NewPlayerConnected(newPlayersAmount);
    }

    void NewPlayerConnected(int newPlayerId)
    {
        if (_showDebugMessages) Debug.Log("RPC > Novo jogador conectado: " + newPlayerId);
        if (GameData.PlayersConnectedsList.Where(pc => pc.PlayerId == newPlayerId).FirstOrDefault() == null)
        {
            if (_showDebugMessages) Debug.Log("Primeiro jogador com este id: " + newPlayerId);

            PlayerConnected newPlayer = new PlayerConnected();
            newPlayer.PlayerId = newPlayerId;

            GameData.PlayersConnectedsList.Add(newPlayer);

            GameData.LastPlayerRegistered = newPlayer;
            GameData.LastPlayerId = newPlayer.PlayerId;
        }
        else
        {
            if (_showDebugMessages) Debug.Log("Já existe um jogador com esse id: " + newPlayerId);
            GameData.LastPlayerRegistered = GameData.PlayersConnectedsList.Where(pc => pc.PlayerId == newPlayerId).FirstOrDefault();
        }
    }
    #endregion
    #endregion
}
