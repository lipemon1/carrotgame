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
    [SerializeField] public static int PlayersGamingNow = 0;
    [SerializeField] public static List<PlayerConnected> PlayersConnectedsList = new List<PlayerConnected>();
    [SerializeField] public List<PlayerConnected> CopyOfPlayersConnectedsList = new List<PlayerConnected>();

    [Header("Game Data")]
    [SerializeField] public List<PlayerArea> PlayerAreaList = new List<PlayerArea>();
    [SerializeField] public List<Carrot> CarrotsList = new List<Carrot>();

    public override void OnStartClient()
    {
        base.OnStartClient();
        SearchCarrots();
        SearchPlayerAreas();
        NewPlayerOnServer(++PlayersGamingNow);
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

    private void NewPlayerOnServer(int newId)
    {
        int myNewId = PlayersGamingNow;
        PlayerConnected newPlayer = new PlayerConnected();
        newPlayer.PlayerId = myNewId;
        newPlayer.GameInstance = this.gameObject;
        newPlayer.IsConnected = true;
        PlayersConnectedsList.Add(newPlayer);
        CopyOfPlayersConnectedsList = PlayersConnectedsList;

        GetComponent<PlayerIdentity>().SetPlayerId(myNewId);
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
        if (_showDebugMessages) Debug.Log("Iniciando troca de dono de uma área(" + areaToChangeOwner + ")");

        if (isServer)
        {
            RpcChangePlayerAreaOwner(areaToChangeOwner, newOwner);
        }

        if (isClient)
        {
            CmdChangePlayerAreaOwner(areaToChangeOwner, newOwner);
            ChangePlayerAreaOwnerNow(areaToChangeOwner, newOwner);
        }
    }

    /// <summary>
    /// Retorna o ID do Dono dessa área, se for -1, ainda não tem dono
    /// </summary>
    /// <param name="areaId"></param>
    /// <returns></returns>
    public int GetMyPlayerAreaOwnerId(int areaId)
    {
        return PlayerAreaList.Where(pa => pa.Id == areaId).FirstOrDefault().PlayerOwnerId;
    }

    /// <summary>
    /// Retorna um player connected de acordo com o seu ID
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    public PlayerConnected GetPlayerById(int playerId)
    {
        return PlayersConnectedsList.Where(player => player.PlayerId == playerId).FirstOrDefault();
    }

    /// <summary>
    /// Marca que o jogador está pronto
    /// </summary>
    /// <param name="playerId"></param>
    public void SetPlayerAsReady(int playerId)
    {
        if (_showDebugMessages) Debug.Log("Iniciando troca de estado de pronto do jogador<" + playerId + ">");

        if (isServer)
        {
            RpcChangePlayerReadyValue(playerId);
        }

        if (isClient)
        {
            CmdChangePlayerReadyValue(playerId);
            ChangePlayerReadyValue(playerId);
        }
    }

    /// <summary>
    /// Verifica se o jogador recebido pode ser o dono da area que ele acabou de entrar
    /// </summary>
    /// <param name="areaThatIJustFound"></param>
    /// <param name="newOwnerId"></param>
    /// <returns></returns>
    public bool CanIBeTheOwner(int areaThatIJustFound, int newOwnerId)
    {
        if (_showDebugMessages) Debug.Log("Verificando se área já tem dono ou se o jogador já está pronto");
        int playerToTurnOwner = newOwnerId;
        if (GetMyPlayerAreaOwnerId(areaThatIJustFound) == -1 && !GetPlayerById(playerToTurnOwner).IsReady)
        {
            if (_showDebugMessages) Debug.Log("Nenhum, nem outro. O jogador " + playerToTurnOwner + " agora deve ser dono da área " + areaThatIJustFound);
            ChangePlayerAreaOwner(areaThatIJustFound, playerToTurnOwner);
            SetPlayerAsReady(playerToTurnOwner);

            return GetPlayerById(playerToTurnOwner).IsReady;
        }
        else
        {
            if (_showDebugMessages) Debug.Log("Ou o jogador já está pronto<" + GetPlayerById(playerToTurnOwner).IsReady + "> ou a área já tem dono<Dono: " + GetMyPlayerAreaOwnerId(areaThatIJustFound) + ">");
            return GetPlayerById(playerToTurnOwner).IsReady;
        }
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

    #region PLAYER AREAS HOOKS
    #region PlayerAreas Owners
    [Command]
    private void CmdChangePlayerAreaOwner(int playerAreaIdToChange, int newOwner)
    {
        if (_showDebugMessages) Debug.Log("COMAND > Mudando o dono da area: " + playerAreaIdToChange + ", para o jogador: " + newOwner);
        ChangePlayerAreaOwnerNow(playerAreaIdToChange, newOwner);
    }

    [ClientRpc]
    private void RpcChangePlayerAreaOwner(int playerAreaIdToChange, int newOwner)
    {
        if (_showDebugMessages) Debug.Log("RPC > Mudando o dono da area: " + playerAreaIdToChange + ", para o jogador: " + newOwner);
        ChangePlayerAreaOwnerNow(playerAreaIdToChange, newOwner);
    }

    void ChangePlayerAreaOwnerNow(int playerAreaIdToChange, int newOwner)
    {
        if (_showDebugMessages) Debug.Log("A área " + playerAreaIdToChange + " tem um novo dono: " + newOwner);
        PlayerAreaList.Where(pa => pa.Id == playerAreaIdToChange).FirstOrDefault().PlayerOwnerId = newOwner;
    }
    #endregion
    #region Players Ready
    [Command]
    private void CmdChangePlayerReadyValue(int playerToBeReady)
    {
        if (_showDebugMessages) Debug.Log("COMAND > Marcando jogador como pronto: " + playerToBeReady);
        ChangePlayerReadyValue(playerToBeReady);
    }

    [ClientRpc]
    private void RpcChangePlayerReadyValue(int playerToBeReady)
    {
        if (_showDebugMessages) Debug.Log("RPC > Marcando jogador como pronto: " + playerToBeReady);
        ChangePlayerReadyValue(playerToBeReady);
    }

    void ChangePlayerReadyValue(int playerToBeReady)
    {
        if (_showDebugMessages) Debug.Log("LOCAL > Marcando jogador como pronto: " + playerToBeReady);
        GetPlayerById(playerToBeReady).IsReady = true;
    }
    #endregion
    #endregion
}
