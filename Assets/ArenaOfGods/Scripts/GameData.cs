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
    [SerializeField] private PlayerDebug _playerDebug;

    [Header("Players Connected")]
    [SerializeField] public static int PlayersGamingNow = 0;
    [SerializeField] public static List<PlayerConnected> PlayersConnectedsList = new List<PlayerConnected>();
    [SerializeField] public List<PlayerConnected> CopyOfPlayersConnectedsList = new List<PlayerConnected>();

    [Header("Game Data")]
    [SerializeField] public List<PlayerArea> PlayerAreaList = new List<PlayerArea>();
    [SerializeField] public List<Carrot> CarrotsList = new List<Carrot>();

    [Header("Available Names")]
    [SerializeField] private List<string> _namesAvailable = new List<string>();

    [Header("Scripts")]
    [SerializeField] private SetupLocalPlayer _setupLocalPlayer;

    public override void OnStartClient()
    {
        base.OnStartClient();

        Invoke("StartWithDelay", 0.1f);
    }

    private void StartWithDelay()
    {
        _setupLocalPlayer.ConfigureAllScripts();

        ConfigureNewStart();
    }

    public void ConfigureNewStart()
    {
        SearchCarrots();
        SearchPlayerAreas();
        NewPlayerOnServer(++PlayersGamingNow);
        LobbyController.Instance.SetAsGameStarted();
        SearchPlayerDebug();

        if (!isServer)
            _setupLocalPlayer.GameCore.RecieveMessageFromServer();

        if (isServer)
            _setupLocalPlayer.StartLoop();
    }

    /// <summary>
    /// Retorna um nome aleatório para o jogador
    /// </summary>
    /// <returns></returns>
    private string GetRandomName(int playerId)
    {
        //Random.InitState(playerId);
        //int randomIndex = Random.Range(0, _namesAvailable.Count);

        //string newName = _namesAvailable[randomIndex];
        //_namesAvailable.Remove(newName);
        //return newName;

        return _namesAvailable[playerId];
    }

    private void Update()
    {
        if(_playerDebug != null)
        {
            if (isLocalPlayer && _playerDebug.gameObject.activeInHierarchy)
                CallOnAnyChange();
        }
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

    private void SearchPlayerDebug()
    {
        _playerDebug = GameObject.Find("DebugCanvas").GetComponent<PlayerDebug>();
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
        newPlayer.PlayerName = GetRandomName(myNewId);
        newPlayer.IsConnected = true;
        PlayersConnectedsList.Add(newPlayer);
        CopyOfPlayersConnectedsList = PlayersConnectedsList;

        _setupLocalPlayer.PlayerIdentity.SetPlayerId(myNewId);
    }

    #region PUBLIC METHODS
    #region Carrots
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

        if(isServer)
            RpcChangeCarrotActiveValue(carrotId, value);

        if(isClient)
            CmdChangeCarrotActiveValue(carrotId, value);
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
        }
    }

    /// <summary>
    /// Retorna o ID do Dono dessa área, se for -1, ainda não tem dono
    /// </summary>
    /// <param name="areaId"></param>
    /// <returns></returns>
    public int GetMyPlayerAreaOwnerId(int areaId)
    {
        if (_showDebugMessages) Debug.Log("Procurando uma player área com o ID: " + areaId);
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

    /// <summary>
    /// Retorna o id da área que o jogador é dono, -1 se ainda não tem área nenhuma
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    public int GetAreaIdFromSomePlayer(int playerId)
    {
        PlayerArea areaFounded = PlayerAreaList.Where(pa => pa.PlayerOwnerId == playerId).FirstOrDefault();

        if (areaFounded == null)
            return -1;
        else
            return areaFounded.Id;
    }

    /// <summary>
    /// Retorna a lista de id de cenouras que essa área contém
    /// </summary>
    /// <param name="playerArea"></param>
    /// <returns></returns>
    public List<int> GetCarrotsIdListFromPlayerArea(int playerArea)
    {
        List<int> zeroList = new List<int>();
        List<int> carrotsIdsToReturn = new List<int>();

        if (playerArea == -1)
            return zeroList;

        PlayerArea playerAreaFromPlayer = GetPlayerAreaById(playerArea);

        if (playerAreaFromPlayer.CarrotsList != null)
        {
            carrotsIdsToReturn = playerAreaFromPlayer.CarrotsList.Select(carrot => carrot.Id).ToList();
            return carrotsIdsToReturn;
        }
        else{
            return zeroList;
        }            
    }
    #endregion
    #region Players
    /// <summary>
    /// Retorna verdadeiro se não encontrou nenhuma jogador não pronto, ou seja, se todos estão prontos
    /// </summary>
    /// <returns></returns>
    public bool EveryoneIsReady()
    {
        return PlayersConnectedsList.Where(pc => pc.IsReady == false).FirstOrDefault() == null;
    }

    /// <summary>
    /// Retorna verdadeiro se o número de jogadores conectados é o número passado
    /// </summary>
    /// <returns></returns>
    public bool IsOnlyThisNumberOfPlayers(int numberOfConnectionsWanted)
    {
        return PlayersConnectedsList.Count == numberOfConnectionsWanted;
    }

    /// <summary>
    /// Retorna verdadeiro se o jogador já estiver pronto
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    public bool IsPlayerReady(int playerId)
    {
        return GetPlayerById(playerId).IsReady;
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
        }
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
    /// Retorna o nome do jogador de acordo com o seu id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string GetPlayerNameById(int id)
    {
        return GetPlayerById(id).PlayerName;
    }

    /// <summary>
    /// Retorna uma lista com todos os nomes de acordo com a lista de id passados
    /// </summary>
    /// <param name="playersId"></param>
    /// <returns></returns>
    public List<string> GetPlayersNamesById(List<int> playersId)
    {
        List<string> playersNames = new List<string>();

        foreach (int playerId in playersId)
        {
            playersNames.Add(GetPlayerNameById(playerId));
        }

        return playersNames;
    }
    #endregion
    #endregion

    #region INTERNAL UTIL METHODS
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
        return PlayerAreaList.FirstOrDefault(pa => pa.Id == id);
    }

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
    /// Retorna verdadeiro se a partida pode começar agora
    /// </summary>
    /// <returns></returns>
    public bool MatchCanStart()
    {
        return PlayerAreaList.Count == 4 && EveryoneIsReady();
    }
    #endregion

    #region CARROTS HOOKS
    #region Carrots PlayerArea
    [Command]
    private void CmdChangeCarrotPlayerArea(int carrotIdToChange, int newAreaOwner, Operation operation)
    {
        if (_showDebugMessages) Debug.Log("COMAND > Mudando a area da cenoura: " + carrotIdToChange + "para a área: " + newAreaOwner);
        RpcChangeCarrotPlayerArea(carrotIdToChange, newAreaOwner, operation);
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

            if (areaToRemoveFrom.CarrotsList.FirstOrDefault(cr => cr.Id == carrotIdToChange) == null)
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
        RpcChangeCarrotOwner(carrotIdToChange, newOwner);
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
        RpcChangeCarrotActiveValue(carrotIdToChange, value);
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
        RpcChangeCarrotPosition(carrotIdToMove, newPosition);
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
        RpcChangePlayerAreaOwner(playerAreaIdToChange, newOwner);
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
        RpcChangePlayerReadyValue(playerToBeReady);
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

        //UpdateGameStartCondition();
    }
    #endregion
    #endregion

    #region DEBUG
    /// <summary>
    /// Passa as informações para o debug do jogador
    /// </summary>
    private void CallOnAnyChange()
    {
        int playerId = _setupLocalPlayer.PlayerIdentity.PlayerId;
        _playerDebug.DebugPlayer(playerId, _setupLocalPlayer.GameData.GetPlayerNameById(playerId),_setupLocalPlayer.GameData.IsPlayerReady(playerId), GetAreaIdFromSomePlayer(playerId), GetCarrotsIdListFromPlayerArea(GetAreaIdFromSomePlayer(playerId)), GameData.PlayersConnectedsList.Count, MatchCanStart());
    }
    #endregion
}
