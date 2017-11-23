using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class GameData : NetworkBehaviour {

    public enum Operation
    {
        Add,
        Remove
    }

    [Header("Debug")]
    [SerializeField] private bool _showDebugMessages;

    [Header("Players Connected")]
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

    /// <summary>
    /// Procura pelas cenouras que existem no mundo do jogo
    /// </summary>
    private void SearchCarrots()
    {
        Carrot[] carrotsFound = FindObjectsOfType(typeof(Carrot)) as Carrot[];
        CarrotsList = carrotsFound.ToList();
    }

    /// <summary>
    /// Procura pelas areas que existem no mundo do jogo
    /// </summary>
    private void SearchPlayerAreas()
    {
        PlayerArea[] playerAreasFound = FindObjectsOfType(typeof(PlayerArea)) as PlayerArea[];
        PlayerAreaList = playerAreasFound.ToList();
    }

    /// <summary>
    /// Registra um novo jogador dentro do servidor
    /// </summary>
    /// <param name="newId"></param>
    private void NewPlayerOnServer(int newId)
    {
        if (_showDebugMessages) Debug.Log("Registrando novo jogador com id: " + newId);
        int myNewId = PlayersGamingNow;
        PlayerConnected newPlayer = new PlayerConnected();
        newPlayer.PlayerId = myNewId;
        newPlayer.GameInstance = this.gameObject;
        newPlayer.IsConnected = true;
        PlayersConnectedsList.Add(newPlayer);
        CopyOfPlayersConnectedsList = PlayersConnectedsList;

        GetComponent<PlayerIdentity>().SetPlayerId(myNewId);
    }

    #region PUBLIC METHODS
    #region Carrots
    /// <summary>
    /// Retorna o id da area dona da cenoura que voce pediu imbecil
    /// </summary>
    /// <param name="carrotId"></param>
    /// <returns></returns>
    private int AreaFromThisCarrot(int carrotId)
    {
        if (_showDebugMessages) Debug.Log("Descobrindo a área que é dona dessa cenoura<" + carrotId + ">");
        Carrot carrotToFoundArea = GetCarrotById(carrotId);
        PlayerArea playerAreaFromThisCarrot = PlayerAreaList.Where(pl => pl.CarrotsList.Contains(carrotToFoundArea)).FirstOrDefault();

        if (playerAreaFromThisCarrot == null)
        {
            if (_showDebugMessages) Debug.Log("Cenoura está com algum jogador<Jogador Dono Atual:" + carrotToFoundArea.PlayerOwnerId + ">");
            return 0;
        }
        else
        {
            if (_showDebugMessages) Debug.Log("Area dona encontrada: <" + playerAreaFromThisCarrot.Id + ">");

            return playerAreaFromThisCarrot.Id;
        }
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
            RpcChangeCarrotOwner(carrotId, newOwner);
        }

        if (isClient)
        {
            CmdChangeCarrotOwner(carrotId, newOwner);
            ChangeCarrotOwner(carrotId, newOwner);
        }
    }

    /// <summary>
    /// Troca o estatus do objeto da cenoura
    /// </summary>
    /// <param name="carrotId"></param>
    /// <param name="value"></param>
    public void ChangeCarrotActiveValue(int carrotId, bool value)
    {
        if (_showDebugMessages) Debug.Log("Iniciando troca de estado de gameobject da cenoura<" + carrotId + ">");

        if (isServer)
        {
            RpcChangeCarrotActiveValue(carrotId, value);
        }

        if (isClient)
        {
            CmdChangeCarrotActiveValue(carrotId, value);
            ChangeCarrotActiveValueNow(carrotId, value);
        }
    }

    /// <summary>
    /// Modifica a area de jogador que é dona da cenoura
    /// </summary>
    /// <param name="carrotId"></param>
    /// <param name="newPlayerArea"></param>
    public void ChangeCarrotPlayerArea(int carrotId, int newPlayerArea, Operation operation)
    {
        if (_showDebugMessages) Debug.Log("Iniciando troca área dona da cenoura");

        if (isServer)
        {
            RpcChangeCarrotPlayerArea(carrotId, newPlayerArea, operation);
        }

        if (isClient)
        {
            CmdChangeCarrotPlayerArea(carrotId, newPlayerArea, operation);
            ChangeCarrotPlayerAreaNow(carrotId, newPlayerArea, operation);
        }
    }

    /// <summary>
    /// Muda a instancia de uma cenoura de posição
    /// </summary>
    /// <param name="carrotId"></param>
    /// <param name="newPosition"></param>
    public void ChangeCarrotGameInstancePosition(int carrotId, Vector3 newPosition)
    {
        if (_showDebugMessages) Debug.Log("Iniciando troca de posição de uma cenoura<" + carrotId + ">");

        if (isServer)
        {
            RpcChangeCarrotPosition(carrotId, newPosition);
        }

        if (isClient)
        {
            CmdChangeCarrotPosition(carrotId, newPosition);
            ChangeCarrotPositionNow(carrotId, newPosition);
        }
    }

    /// <summary>
    /// Adiciona cenoura dentro de uma area de jogador
    /// </summary>
    /// <param name="carrotId"></param>
    /// <param name="newAreaId"></param>
    public void AddCarrotToArea(int carrotId, int newAreaId, Vector3 newPosition)
    {
        ChangeCarrotActiveValue(carrotId, true);
        ChangeCarrotPlayerArea(carrotId, newAreaId, Operation.Add);
        ChangeCarrotGameInstancePosition(carrotId, newPosition);
    }
    #endregion
    #region Player Areas
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
    #endregion
    #endregion

    #region INTERNAL UTIL METHODS
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
    /// Retorna a cenoura atraves do seu ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Carrot GetCarrotById(int id)
    {
        return CarrotsList.Where(carrot => carrot.Id == id).FirstOrDefault();
    }

    /// <summary>
    /// Retorna uma player area atraves do seu ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public PlayerArea GetPlayerAreaById(int id)
    {
        return PlayerAreaList.Where(pa => pa.Id == id).FirstOrDefault();
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
    #endregion

    #region CARROTS HOOKS
    #region Carrots PlayerArea
    [Command]
    private void CmdChangeCarrotPlayerArea(int carrotIdToChange, int newAreaOwner, Operation operation)
    {
        if (_showDebugMessages) Debug.Log("COMAND > Mudando a area da cenoura: " + carrotIdToChange + "para a área: " + newAreaOwner);
        ChangeCarrotPlayerAreaNow(carrotIdToChange, newAreaOwner, operation);
    }

    [ClientRpc]
    private void RpcChangeCarrotPlayerArea(int carrotIdToChange, int newAreaOwner, Operation operation)
    {
        if (_showDebugMessages) Debug.Log("RPC > Mudando a area da cenoura: " + carrotIdToChange + "para a área: " + newAreaOwner);
        ChangeCarrotPlayerAreaNow(carrotIdToChange, newAreaOwner, operation);
    }

    void ChangeCarrotPlayerAreaNow(int carrotIdToChange, int newAreaOwner, Operation operation)
    {
        if (_showDebugMessages) Debug.Log("LOCAL > Mudando a area da cenoura<" + carrotIdToChange + ">");
        
        if(operation == Operation.Remove)
        {
            if (_showDebugMessages) Debug.Log("LOCAL > Mudando a area da cenoura: " + carrotIdToChange + "para a área: " + newAreaOwner);
            //deletando da lista que estava
            PlayerArea areaToRemoveFrom = GetPlayerAreaById(AreaFromThisCarrot(carrotIdToChange));

            if (areaToRemoveFrom.CarrotsList.Where(cr => cr.Id == carrotIdToChange).FirstOrDefault() == null)
            {
                if (_showDebugMessages) Debug.Log("LOCAL > A cenoura<" + carrotIdToChange + "> já não existe mais nessa área<" + areaToRemoveFrom + ">");
            }
            else
            {
                areaToRemoveFrom.CarrotsList.Remove(GetCarrotById(carrotIdToChange));
            }
        }

        if(operation == Operation.Add)
        {
            PlayerArea areaToAddCarrot = GetPlayerAreaById(newAreaOwner);

            if (areaToAddCarrot.CarrotsList.Where(cr => cr.Id == carrotIdToChange).FirstOrDefault() != null)
            {
                if (_showDebugMessages) Debug.Log("LOCAL > A cenoura<" + carrotIdToChange + "> já está nessa área<" + newAreaOwner + ">");
            }
            else
            {
                areaToAddCarrot.CarrotsList.Add(GetCarrotById(carrotIdToChange));
            }
        }
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
        if (_showDebugMessages) Debug.Log("RPC >Mudando o dono da cenoura<" + carrotIdToChange + "> para o jogador: " + newOwner);
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
        ChangeCarrotActiveValueNow(carrotIdToChange, value);
    }

    [ClientRpc]
    private void RpcChangeCarrotActiveValue(int carrotIdToChange, bool value)
    {
        if (_showDebugMessages) Debug.Log("RPC > Mudando status do objeto da cenoura: " + carrotIdToChange);
        ChangeCarrotActiveValueNow(carrotIdToChange, value);
    }

    void ChangeCarrotActiveValueNow(int carrotIdToChange, bool value)
    {
        if (_showDebugMessages) Debug.Log("LOCAL > Mudando status do objeto da cenoura: " + carrotIdToChange);
        CarrotsList.Where(cr => cr.Id == carrotIdToChange).FirstOrDefault().IsActive = value;
        CarrotsList.Where(cr => cr.Id == carrotIdToChange).FirstOrDefault().GameInstance.SetActive(value);
    }
    #endregion
    #region Carrots Position
    [Command]
    private void CmdChangeCarrotPosition(int carrotIdToMove, Vector3 newPosition)
    {
        if (_showDebugMessages) Debug.Log("COMAND > Mudando posição da cenoura<" + carrotIdToMove + ">");
        ChangeCarrotPositionNow(carrotIdToMove, newPosition);
    }

    [ClientRpc]
    private void RpcChangeCarrotPosition(int carrotIdToMove, Vector3 newPosition)
    {
        if (_showDebugMessages) Debug.Log("RPC > Mudando posição da cenoura<" + carrotIdToMove + ">");
        ChangeCarrotPositionNow(carrotIdToMove, newPosition);
    }

    void ChangeCarrotPositionNow(int carrotIdToMove, Vector3 newPosition)
    {
        if (_showDebugMessages) Debug.Log("LOCAL > Mudando posição da cenoura<" + carrotIdToMove + ">");
        GetCarrotById(carrotIdToMove).GameInstance.transform.position = newPosition;
        GetCarrotById(carrotIdToMove).GameInstance.GetComponent<CarrotBehaviour>().OnTargetExit();
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
