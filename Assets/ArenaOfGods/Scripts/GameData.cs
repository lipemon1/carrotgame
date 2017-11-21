using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class GameData : NetworkBehaviour {

    [Header("Debug")]
    [SerializeField] private bool _showDebugMessages;

    [Header("Players Connected")]
    [SyncVar]
    [SerializeField] public PlayerConnected LastPlayerRegistered;
    [SyncVar]
    [SerializeField] public int LastPlayerId;
    [SerializeField] public List<PlayerConnected> PlayersConnectedsList = new List<PlayerConnected>();

    [Header("Game Data")]
    [SerializeField] public List<PlayerArea> PlayerAreaList = new List<PlayerArea>();
    [SerializeField] public List<Carrot> CarrotsList = new List<Carrot>();

    public override void OnStartClient()
    {
        base.OnStartClient();
        SearchCarrots();
        SearchPlayerAreas();
    }

    private void SearchCarrots()
    {
        Carrot[] carrotsFound = FindObjectsOfType(typeof(Carrot)) as Carrot[];
        CarrotsList = carrotsFound.ToList();
    }

    private void SearchPlayerAreas()
    {
        PlayerArea[] playerAreasFound = FindObjectsOfType(typeof(PlayerArea)) as PlayerArea[];
        PlayerAreaList = playerAreasFound.ToList();
    }

    public bool SomeoneHasThisIp(int idToCheck)
    {
        return PlayersConnectedsList.Where(pc => pc.PlayerId == idToCheck).FirstOrDefault() != null;
    }

    /// <summary>
    /// Retorna verdadeiro se o jogador passado por parametro é o dono da area passada por parametro
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="playerAreaId"></param>
    /// <returns></returns>
	public bool IsThisMyArea(int playerId, int playerAreaId)
    {
        if (_showDebugMessages) Debug.Log("Iniciando verificação de propriedade de uma área.\nÁrea: " + playerAreaId + "\nJogador:" + playerId);
        PlayerArea areaFromPlayer = PlayerAreaList.Where(pa => pa.Id == playerAreaId).FirstOrDefault();

        if (areaFromPlayer == null)
            return false;

        return areaFromPlayer.PlayerOwnerId == playerId;
    }

    /// <summary>
    /// Retorna o id da area dona da cenoura que voce pediu imbecil
    /// </summary>
    /// <param name="carrotId"></param>
    /// <returns></returns>
    private int AreaFromThisCarrot(int carrotId)
    {
        return PlayerAreaList.Where(pl => pl.CarrotsList.Contains(CarrotsList.Where(cr => cr.Id == carrotId).FirstOrDefault())).ToList().FirstOrDefault().Id;
    }

    /// <summary>
    /// Modifica o jogador que é o dono da cenoura
    /// </summary>
    /// <param name="carrotId"></param>
    /// <param name="newOwner"></param>
    public void ChangeCarrotPlayerOwner(int carrotId, int newOwner)
    {
        if (_showDebugMessages) Debug.Log("Iniciando troca de dono de cenoura");
        
        if (isServer)
        {
            RpcChangeCarrotPlayerArea(carrotId, newOwner);
            RpcChangeCarrotActiveValue(carrotId, false);
            RpcChangeCarrotOwner(carrotId, newOwner);
        }

        if (isClient)
        {
            CmdChangeCarrotPlayerArea(carrotId, newOwner);
            ChangeCarrotPlayerArea(carrotId, newOwner);

            CmdChangeCarrotActiveValue(carrotId, false);
            ChangeCarrotActiveValue(carrotId, false);

            CmdChangeCarrotOwner(carrotId, newOwner);
            ChangeCarrotOwner(carrotId, newOwner);
        }
    }

    /// <summary>
    /// Adiciona cenoura dentro de uma area de jogador
    /// </summary>
    /// <param name="carrotId"></param>
    /// <param name="newAreaId"></param>
    public void AddCarrotToArea(int carrotId, int newAreaId, Vector3 newPosition)
    {
        PlayerAreaList[newAreaId].CarrotsList.Add(CarrotsList.Where(cr => cr.Id == carrotId).ToList().FirstOrDefault());

        CarrotsList.Where(cr => cr.Id == carrotId).FirstOrDefault().GameInstance.transform.position = newPosition;
        CarrotsList.Where(cr => cr.Id == carrotId).FirstOrDefault().IsActive = true;
    }

    /// <summary>
    /// Troca o dono de uma área, usado na primeira vez que está plantando
    /// </summary>
    /// <param name="areaToChangeOwner"></param>
    /// <param name="newOwner"></param>
    public void ChangePlayerAreaOwner(int areaToChangeOwner, int newOwner)
    {
        PlayerAreaList[areaToChangeOwner].PlayerOwnerId = newOwner;
    }

    /// <summary>
    /// Retorna o ID do Dono dessa área, se for -1, ainda não tem dono
    /// </summary>
    /// <param name="areaToSearch"></param>
    /// <returns></returns>
    public int GetMyPlayerOwnerId(int areaToSearch)
    {
        return PlayerAreaList[areaToSearch].PlayerOwnerId;
    }

    /// <summary>
    /// Verifica se o jogador recebido pode ser o dono da area que ele acabou de entrar
    /// </summary>
    /// <param name="areaThatIJustFound"></param>
    /// <param name="newOwnerId"></param>
    /// <returns></returns>
    public bool CanIBeTheOwner(int areaThatIJustFound, int newOwnerId)
    {
        int playerId = newOwnerId - 1;
        if (GetMyPlayerOwnerId(areaThatIJustFound) == -1 && !PlayersConnectedsList[playerId].IsReady)
        {
            ChangePlayerAreaOwner(areaThatIJustFound, newOwnerId);
            PlayersConnectedsList[playerId].IsReady = true;

            return PlayersConnectedsList[playerId].IsReady;
        }

        return PlayersConnectedsList[playerId].IsReady;
    }

    #region CARROTS HOOKS
    #region Carrots PlayerArea
    [Command]
    private void CmdChangeCarrotPlayerArea(int carrotIdToChange, int newOwner)
    {
        if (_showDebugMessages) Debug.Log("COMAND > Mudando o dono da cenoura: " + carrotIdToChange + "para o jogador: " + newOwner);
        ChangeCarrotPlayerArea(carrotIdToChange, newOwner);
    }

    [ClientRpc]
    private void RpcChangeCarrotPlayerArea(int carrotIdToChange, int newOwner)
    {
        if (_showDebugMessages) Debug.Log("RPC >Mudando o dono da cenoura: " + carrotIdToChange + "para o jogador: " + newOwner);
        ChangeCarrotPlayerArea(carrotIdToChange, newOwner);
    }

    void ChangeCarrotPlayerArea(int carrotIdToChange, int newOwner)
    {
        if (_showDebugMessages) Debug.Log("LOCAL > Mudando o dono da cenoura: " + carrotIdToChange + "para o jogador: " + newOwner);
        //deletando da lista que estava
        int areaToRemoveFrom = AreaFromThisCarrot(carrotIdToChange);
        PlayerAreaList[areaToRemoveFrom].CarrotsList.Remove(CarrotsList.Where(cr => cr.Id == carrotIdToChange).FirstOrDefault());
    }
    #endregion
    #region Carrots Owners
    [Command]
    private void CmdChangeCarrotOwner(int carrotIdToChange, int newOwner)
    {
        if (_showDebugMessages) Debug.Log("COMAND > Mudando o dono da cenoura: " + carrotIdToChange + "para o jogador: " + newOwner);
        ChangeCarrotOwner(carrotIdToChange, newOwner);
    }

    [ClientRpc]
    private void RpcChangeCarrotOwner(int carrotIdToChange, int newOwner)
    {
        if (_showDebugMessages) Debug.Log("RPC >Mudando o dono da cenoura: " + carrotIdToChange + "para o jogador: " + newOwner);
        ChangeCarrotOwner(carrotIdToChange, newOwner);
    }

    void ChangeCarrotOwner(int carrotIdToChange, int newOwner)
    {
        if (_showDebugMessages) Debug.Log("A cenoura " + carrotIdToChange + " tem um novo dono: " + newOwner);
        //colocando novo dono
        CarrotsList.Where(cr => cr.Id == carrotIdToChange).FirstOrDefault().PlayerOwnerId = newOwner;
    }
    #endregion
    #region Carrots Active
    [Command]
    private void CmdChangeCarrotActiveValue(int carrotIdToChange, bool value)
    {
        if (_showDebugMessages) Debug.Log("COMAND > Mudando status do objeto da cenoura: " + carrotIdToChange);
        ChangeCarrotActiveValue(carrotIdToChange, value);
    }

    [ClientRpc]
    private void RpcChangeCarrotActiveValue(int carrotIdToChange, bool value)
    {
        if (_showDebugMessages) Debug.Log("RPC > Mudando status do objeto da cenoura: " + carrotIdToChange);
        ChangeCarrotActiveValue(carrotIdToChange, value);
    }

    void ChangeCarrotActiveValue(int carrotIdToChange, bool value)
    {
        if (_showDebugMessages) Debug.Log("LOCAL > Mudando status do objeto da cenoura: " + carrotIdToChange);
        CarrotsList.Where(cr => cr.Id == carrotIdToChange).FirstOrDefault().IsActive = value;
        CarrotsList.Where(cr => cr.Id == carrotIdToChange).FirstOrDefault().GameInstance.SetActive(value);
    }
    #endregion
    #endregion

    //#region PLAYER AREAS HOOKS

    //#endregion
}
