using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class GameData : NetworkBehaviour {

    [System.Serializable]
    public class PlayerConnected
    {
        public int PlayerId;
        public GameObject GameInstance;
        public bool IsReady;

        public PlayerConnected(int id, GameObject instance)
        {
            PlayerId = id;
            GameInstance = instance;
        }
    }

    [Header("Debug")]
    [SerializeField] private bool _showDebugMessages;

    [Header("Players Connected")]
    [SyncVar(hook = "OnChangePlayersAmount")]
    [SerializeField] private int _playersConnectedAmount = 0;
    [SerializeField] private int _playersCon;
    [SerializeField] private List<PlayerConnected> _playersConnectedsList = new List<PlayerConnected>();

    [Header("Game Data")]
    [SerializeField] public List<PlayerArea> PlayerAreaList = new List<PlayerArea>();
    [SerializeField] public List<Carrot> CarrotsList = new List<Carrot>();

    public override void OnStartClient()
    {
        base.OnStartClient();
        SearchCarrots();
        Invoke("LateShitHere", 2f);
    }

    private void SearchCarrots()
    {
        Carrot[] carrotsFound = FindObjectsOfType(typeof(Carrot)) as Carrot[];
        CarrotsList = carrotsFound.ToList();
    }

    private void LateShitHere()
    {
        OnChangePlayersAmount(_playersConnectedAmount);
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
        return PlayerAreaList.Where(pl => pl.CarrotsListId.Contains(carrotId)).ToList().FirstOrDefault().Id;
    }

    /// <summary>
    /// Modifica o jogador que é o dono da cenoura
    /// </summary>
    /// <param name="carrotId"></param>
    /// <param name="newOwner"></param>
    public void ChangeCarrotPlayerOwner(int carrotId, int newOwner)
    {
        if (_showDebugMessages) Debug.Log("Iniciando troca de dono de cenoura");
        //deletando da lista que estava
        int areaToRemoveFrom = AreaFromThisCarrot(carrotId);
        PlayerAreaList[areaToRemoveFrom].CarrotsListId.Remove(carrotId);
        CarrotsList[carrotId].GameInstance.SetActive(false);

        //colocando novo dono
        CarrotsList[carrotId].PlayerOwnerId = newOwner;
        if (_showDebugMessages) Debug.Log("A cenoura " + carrotId + " tem um novo dono: " + newOwner);
    }

    /// <summary>
    /// Adiciona cenoura dentro de uma area de jogador
    /// </summary>
    /// <param name="carrotId"></param>
    /// <param name="newAreaId"></param>
    public void AddCarrotToArea(int carrotId, int newAreaId, Vector3 newPosition)
    {
        PlayerAreaList[newAreaId].CarrotsListId.Add(carrotId);
        CarrotsList[carrotId].GameInstance.transform.position = newPosition;
        CarrotsList[carrotId].GameInstance.SetActive(true);
    }

    public int SomeoneConnected(GameObject instance)
    {
        int newamount = _playersCon + 1;
        CmdChangePlayersAmount(newamount);

        //int newPlayerId = _playersConnectedAmount;

        //PlayerConnected newPlayer = new PlayerConnected(newPlayerId, instance);
        //_playersConnectedsList.Add(newPlayer);
        //return newPlayer.PlayerId;
        return 9;
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
        if (GetMyPlayerOwnerId(areaThatIJustFound) == -1 && !_playersConnectedsList[playerId].IsReady)
        {
            ChangePlayerAreaOwner(areaThatIJustFound, newOwnerId);
            _playersConnectedsList[playerId].IsReady = true;

            return _playersConnectedsList[playerId].IsReady;
        }

        return _playersConnectedsList[playerId].IsReady;
    }

    #region NOW PASS INFO BITCH
    void OnChangePlayersAmount(int newValue)
    {
        SaveOffline(newValue);
    }

    [Command]
    public void CmdChangePlayersAmount(int newValue)
    {
        SaveOffline(newValue);
    }

    private void SaveOffline(int newValue)
    {
        _playersConnectedAmount = newValue;
        _playersCon = _playersConnectedAmount;
    }
    #endregion
}
