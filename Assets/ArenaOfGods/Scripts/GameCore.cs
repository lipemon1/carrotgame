using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameCore : NetworkBehaviour
{
    public enum LoopStatus
    {
        NotApplicable,
        WaitingPlayers,
        RoundStarting,
        PickingArea,
        WaitOthersToPick,
        MatchStarting,
        RoundPlaying,
        RoundEnding
    }

    [System.Serializable]
    public struct MovementEnableOptions
    {
        public bool onRoundStarting;
        public bool onWaitingPlayers;
        public bool onPickingArea;
        public bool onWaitOthersToPick;
        public bool onMatchStarting;
        public bool onRoundPlaying;
        public bool onRoundEnding;
    }

    [Header("Debug")]
    [SerializeField] private bool _showDebugMessage;
    [SerializeField] private bool _showDebugMessageGameLoop;

    [Header("Status")]
    [SerializeField] private LoopStatus _curLoopStatus = LoopStatus.NotApplicable;
    [SerializeField] private bool _alreadyStartedLoop;
    [SerializeField] public int PlayersAmountWanted = 4;

    [Header("Movement On Game Status")]
    [SerializeField] private MovementEnableOptions _moveOn;

    [Space(8)]
    public float CarrotYPos;


    
    [Header("Loop Configs")]
    [Space(8)]
    [HideInInspector] private WaitForSeconds _startGameWait;
    [HideInInspector] private WaitForSeconds _startMatchWait;
    [HideInInspector] private WaitForSeconds _endWait;
    [SerializeField] private float _startGameDelay = 3f;
    [SerializeField] private float _startMatchDelay = 3f;
    [SerializeField] private float _endDelay = 3f;

    [Header("Messages Config")]
    [SerializeField] private string _waitingConnections;
    [SerializeField] private string _chooseYourArea;
    [SerializeField] private string _waitingOthersArea;
    [SerializeField] private string _startMatch;
    [SerializeField] private string _endMessage;

    [Header("Time Configuration")]
    [Range(25, 60)]
    [SerializeField] private float _matchTime = 35f;

    [Header("Info")]
    [SerializeField] private static string _curMessageToShow;
    [SerializeField] private float _curTime = 1f;

    [Header("MyPlayer")]
    [SerializeField]
    private SetupLocalPlayer _setupLocalPlayer;

    private void Start()
    {
        _startGameWait = new WaitForSeconds(_startGameDelay);
        _startMatchWait = new WaitForSeconds(_startMatchDelay);
        _endWait = new WaitForSeconds(_endDelay);
    }

    public void StartGameLoop()
    {
        if (_showDebugMessage) Debug.Log("Tentando iniciar loop de jogo...");
        if(_alreadyStartedLoop == false)
        {
            _alreadyStartedLoop = true;
            StartCoroutine(GameLoop());
        }
        else
        {
            if (_showDebugMessage) Debug.Log("Loop não foi iniciado porque já existe um loop rodando");
        }
    }

    // This is called from start and will run each phase of the game one after another.
    private IEnumerator GameLoop()
    {
        if (_showDebugMessage) Debug.Log("Loop de jogo iniciado com sucesso!");

        // ESPERANDO TODOS CONECTAREM
        yield return StartCoroutine(WaitingPlayersConnect());

        // COMEÇANDO JOGO
        yield return StartCoroutine(RoundStarting());

        // libera jogadores para escolher suas áreas
        yield return StartCoroutine(RoundPickingArea());

        // fica esperando todo mundo escolher suas áreas
        yield return StartCoroutine(WaitingOthersToPick());

        // inicia a partida
        yield return StartCoroutine(MatchStarting());

        // partida rolando até que o gameover (tempo) seja chamado
        yield return StartCoroutine(RoundPlaying());

        // quando termina a partida
        yield return StartCoroutine(RoundEnding());

        RestartMatch();
    }

    #region LOOP ROUTINES
    private IEnumerator WaitingPlayersConnect()
    {
        ChangeCurStatus(LoopStatus.WaitingPlayers);
        ChangeMovementStatus(_moveOn.onWaitingPlayers);

        // Clear the text from the screen.
        ChangeCurMessage(_waitingConnections);

        // While there is not one tank left...
        while (!_setupLocalPlayer.GameData.IsOnlyThisNumberOfPlayers(PlayersAmountWanted))
        {
            if (_showDebugMessageGameLoop) Debug.Log("O número de jogadores ainda não é válido para começar a partida");
            // ... return on the next frame.
            yield return null;
        }
    }

    private IEnumerator RoundStarting()
    {
        ChangeCurStatus(LoopStatus.RoundStarting);
        ChangeMovementStatus(_moveOn.onRoundStarting);

        ChangeCurMessage(_chooseYourArea);

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return _startGameWait;
    }

    private IEnumerator RoundPickingArea()
    {
        ChangeCurStatus(LoopStatus.PickingArea);
        ChangeMovementStatus(_moveOn.onPickingArea);

        // Clear the text from the screen.
        ChangeCurMessage(string.Empty);

        // While there is not one tank left...
        while (!_setupLocalPlayer.GameData.IsPlayerReady(_setupLocalPlayer.PlayerIdentity.PlayerId))
        {
            if (_showDebugMessageGameLoop) Debug.Log("Ainda não selecionei uma área");
            // ... return on the next frame.
            yield return null;
        }
    }

    private IEnumerator WaitingOthersToPick()
    {
        ChangeCurStatus(LoopStatus.WaitOthersToPick);
        ChangeMovementStatus(_moveOn.onWaitOthersToPick);

        ChangeCurMessage(_waitingOthersArea);

        // While there is not one tank left...
        while (!_setupLocalPlayer.GameData.EveryoneIsReady())
        {
            if (_showDebugMessageGameLoop) Debug.Log("Ainda faltam jogadores a ficarem prontos");
            // ... return on the next frame.
            yield return null;
        }
    }

    private IEnumerator MatchStarting()
    {
        ChangeCurStatus(LoopStatus.MatchStarting);
        ChangeMovementStatus(_moveOn.onMatchStarting);
        ChangeCurMessage(_startMatch);

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return _startMatchWait;
    }

    private IEnumerator RoundPlaying()
    {
        ChangeCurStatus(LoopStatus.RoundPlaying);
        ChangeMovementStatus(_moveOn.onRoundPlaying);

        StartCoroutine(TimePass());

        // Clear the text from the screen.
        ChangeCurMessage(string.Empty);

        // While there is not one tank left...
        while (!GameIsEnded())
        {
            // ... return on the next frame.
            yield return null;
        }
    }

    private IEnumerator RoundEnding()
    {
        ChangeCurStatus(LoopStatus.RoundEnding);
        ChangeMovementStatus(_moveOn.onRoundEnding);

        //ChangeCurMessage(_endMessage);
        CallHudGameOver(false);

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return _endWait;
    }
    #endregion

    #region TIME ROUTINES
    private IEnumerator TimePass()
    {
        ConfigureMatchTime(_matchTime);

        while(_curTime >= 0f)
        {
            float _timeToReduce = Time.deltaTime;
            _curTime -= _timeToReduce;
            yield return null;
        }

        yield return null;
    }
    #endregion

    /// <summary>
    /// Modifica a mensagem que deve ser mostrada na tela
    /// </summary>
    /// <param name="newMessage"></param>
    private void ChangeCurMessage(string newMessage)
    {
        if (_showDebugMessage) Debug.Log("Mudando cur message para: " + newMessage);
        _curMessageToShow = newMessage;
        UpdateMessageUI(_curMessageToShow);
    }

    /// <summary>
    /// Retorna se o tempo da partida já acabou
    /// </summary>
    /// <returns></returns>
    private bool GameIsEnded()
    {
        return _curTime < 0f;
    }

    /// <summary>
    /// Modifica o status do loop
    /// </summary>
    /// <param name="newStatus"></param>
    private void ChangeCurStatus(LoopStatus newStatus)
    {
        _curLoopStatus = newStatus;
    }

    /// <summary>
    /// Configura o tempo de partida
    /// </summary>
    /// <param name="newTime"></param>
    private void ConfigureMatchTime(float newTime)
    {
        if (_showDebugMessage) Debug.Log("Configurando tempo de jogo para: " + newTime);
        CallTimerOnScreen(newTime);
        _curTime = newTime;
    }

    /// <summary>
    /// Método que reinicia a partida
    /// </summary>
    private void RestartMatch()
    {
        if (_showDebugMessage) Debug.Log("Restarting the match");
    }


    /// <summary>
    /// Muda o status de movimento do jogador
    /// </summary>
    /// <param name="newStatus"></param>
    private void ChangeMovementStatus(bool newStatus)
    {
        if (_showDebugMessage) Debug.Log("Iniciando a troca de status de movimento do jogador: " + newStatus);
        if (isServer)
        {
            RpcChangeMovementStatus(newStatus);
        }

        if (isClient)
        {
            CmdChangeMovementStatus(newStatus);
        }
    }

    /// <summary>
    /// Muda o status de movimento do jogador
    /// </summary>
    /// <param name="newStatus"></param>
    private void CallTimerOnScreen(float newMatchTime)
    {
        if (_showDebugMessage) Debug.Log("Iniciando chamada de tempo para Match Timer com tempo de: " + newMatchTime);

        if (isServer)
        {
            RpcCallTimerStart(newMatchTime);
        }

        if (isClient)
        {
            CmdCallTimerStart(newMatchTime);
        }
    }

    /// <summary>
    /// Modifica a mensagem que é mostrada na interface do jogo
    /// </summary>
    /// <param name="newMessage"></param>
    private void UpdateMessageUI(string newMessage)
    {
        if (_showDebugMessage) Debug.Log("Iniciando troca de message na tela: " + newMessage);
        if (isServer)
        {
            RpcUpdateMessageUI(newMessage);
        }

        if (isClient)
        {
            CmdUpdateMessageUI(newMessage);
        }
    }

    public void RecieveMessageFromServer()
    {
        if (isClient)
        {
            CmdUpdateMessageUIServerToClient();
        }
    }

    /// <summary>
    /// Chama o método de gameover na hud do jogo
    /// </summary>
    /// <param name="newStatus"></param>
    private void CallHudGameOver(bool win)
    {
        if (_showDebugMessage) Debug.Log("Iniciando chamada de gameover pro canvas com resultado: " + win);

        if (isServer)
        {
            RpcCallHudGameOver(win);
        }

        if (isClient)
        {
            CmdCallHudGameOver(win);
        }
    }

    #region GAME CORE HOOKS
    #region Control PlayerMovement
    [Command]
    private void CmdChangeMovementStatus(bool newStatus)
    {
        if (_showDebugMessage) Debug.Log("COMAND > Chamando troca de status de movimento: " + newStatus);
        RpcChangeMovementStatus(newStatus);
        ChangeMovementStatusNow(newStatus);
    }

    [ClientRpc]
    private void RpcChangeMovementStatus(bool newStatus)
    {
        if (_showDebugMessage) Debug.Log("RPC > Chamando troca de status de movimento: " + newStatus);
        ChangeMovementStatusNow(newStatus);
    }

    private void ChangeMovementStatusNow(bool newStatus)
    {
        if (_showDebugMessage) Debug.Log("LOCAL > Chamando troca de status de movimento: " + newStatus);
        if (isLocalPlayer)
            _setupLocalPlayer.PlayerMovement.enabled = newStatus;
        else
            if (_showDebugMessage) Debug.Log("Como não sou local player não vou trocar meu status de movimento para: " + newStatus);
    }
    #endregion
    #region UpdateMessage
    [Command]
    private void CmdUpdateMessageUI(string newMessage)
    {
        if (_showDebugMessage) Debug.Log("COMAND > Chamando update message: " + newMessage);
        RpcUpdateMessageUI(newMessage);
        UpdateMessageUINow(newMessage);
    }

    [Command]
    private void CmdUpdateMessageUIServerToClient()
    {
        if (_showDebugMessage) Debug.Log("COMAND > Chamando update message do servidor para clientes: " + _curMessageToShow);
        RpcUpdateMessageUI(_curMessageToShow);
    }

    [ClientRpc]
    private void RpcUpdateMessageUI(string newMessage)
    {
        if (_showDebugMessage) Debug.Log("RPC > Chamando update message: " + newMessage);
        UpdateMessageUINow(newMessage);
    }

    private void UpdateMessageUINow(string newMessage)
    {
        if (_showDebugMessage) Debug.Log("LOCAL > Chamando update message: " + newMessage);
        GameHud.Instance.SetNewMessage(newMessage);
    }
    #endregion
    #region TimerCall
    [Command]
    private void CmdCallTimerStart(float newTimer)
    {
        if (_showDebugMessage) Debug.Log("COMAND > Chamando início de contagem de tempo: " + newTimer);
        RpcCallTimerStart(newTimer);
    }

    [ClientRpc]
    private void RpcCallTimerStart(float newTimer)
    {
        if (_showDebugMessage) Debug.Log("RPC > Chamando início de contagem de tempo: " + newTimer);
        CallTimerStartNow(newTimer);
    }

    private void CallTimerStartNow(float newTimer)
    {
        if (_showDebugMessage) Debug.Log("LOCAL > Chamando início de contagem de tempo: " + newTimer);
        MatchTime.Instance.StartMatchTimer(newTimer);
    }
    #endregion
    #region GameOverCall
    [Command]
    private void CmdCallHudGameOver(bool win)
    {
        if (_showDebugMessage) Debug.Log("COMAND > Chamando tela de game over com resultado: " + win);
        RpcCallHudGameOver(win);
    }

    [ClientRpc]
    private void RpcCallHudGameOver(bool win)
    {
        if (_showDebugMessage) Debug.Log("RPC > Chamando tela de game over com resultado: " + win);
        CallHudGameOverNow(win);
    }

    private void CallHudGameOverNow(bool win)
    {
        if (_showDebugMessage) Debug.Log("LOCAL > Chamando tela de game over com resultado: " + win);
        //GameHud.Instance.GameIsOver(win);
        WinnerHolder.Instance.CheckNewWinners(_setupLocalPlayer.GameData);
    }
    #endregion
    #endregion
}
