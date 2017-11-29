using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameHud : MonoBehaviour {

    public static GameHud Instance { get; private set; }

    [Header("Debug")]
    [SerializeField] private bool _showDebugMessage;

    [Header("End Game Messages")]
    [SerializeField] private string _onWinning = "GANHOU";
    [SerializeField] private string _onLose = "PERDEU";

    [Header("Importante Message")]
    [SerializeField] private Text _importanteMessage;

    [Header("EndGame")]
    [SerializeField] private GameObject _endGamePanel;
    [SerializeField] private Text _endGameMessage;

    [Header("Final Labels")]
    [SerializeField] private string _winVarious = "Ganharam";
    [SerializeField] private string _lostVarious = "Perderam";

    [Header("Winner Interface")]
    [SerializeField] private Text _winLabelUI;
    [SerializeField] private Text _winnerListUI;
    [SerializeField] private string _winnersList;

    [Header("Losers Interface")]
    [SerializeField] private Text _lostLabelUI;
    [SerializeField] private Text _loserListUI;
    [SerializeField] private string _losersList;

    [Header("Menu Button")]
    [SerializeField] private Button _menuButton;

    void Awake()
    {
        Instance = this;

        _menuButton.onClick.AddListener(() => SceneChanger.Instance.RestartCurrentScene());
    }

    /// <summary>
    /// Mostra uma nova mensagem na tela
    /// </summary>
    /// <param name="newMessage"></param>
	public void SetNewMessage(string newMessage)
    {
        if (_showDebugMessage) Debug.Log("Trocando mensagem na tela para: " + newMessage);
        _importanteMessage.text = newMessage;
    }

    /// <summary>
    /// Método chamado para mostrar tela de final de jogo
    /// </summary>
    /// <param name="win"></param>
    public void GameIsOver(bool win)
    {
        if (_showDebugMessage) Debug.Log("Mostrando tela de gameover com final: " + GetRightEndMessage(win));
        _endGameMessage.text = GetRightEndMessage(win);
        _endGamePanel.SetActive(true);
    }

    /// <summary>
    /// Método que recebe os vencedores e perdedores para mostrar na tela de gameover
    /// </summary>
    /// <param name="winnersAreas"></param>
    /// <param name="losersAreas"></param>
    public void RecievePlayersLists(List<PlayerArea> winnersAreas, List<PlayerArea> losersAreas, GameData data)
    {
        _winnersList = MakePlayerNameList(data.GetPlayersNamesById(winnersAreas.Select(playerArea => playerArea.PlayerOwnerId).ToList()));
        _losersList = MakePlayerNameList(data.GetPlayersNamesById(losersAreas.Select(playerArea => playerArea.PlayerOwnerId).ToList()));

        UpdateGameOverUI();
    }

    /// <summary>
    /// Atualiza os valores na tela de gameover
    /// </summary>
    private void UpdateGameOverUI()
    {
        _winLabelUI.text = _winVarious;
        _winnerListUI.text = _winnersList;

        _lostLabelUI.text = _lostVarious;
        _loserListUI.text = _losersList;

        _menuButton.interactable = true;
        _endGamePanel.SetActive(true);
    }

    /// <summary>
    /// Retorna todos os nomes passados em forma de lista
    /// </summary>
    /// <param name="namesList"></param>
    /// <returns></returns>
    private string MakePlayerNameList(List<string> namesList)
    {
        string namesListAsString = "";

        foreach (string playerName in namesList)
        {
            namesListAsString += Environment.NewLine;
            namesListAsString += playerName;
        }

        return namesListAsString;
    }

    /// <summary>
    /// Retorna qual mensagem deverá ser mostrada no final do jogo
    /// </summary>
    /// <param name="win"></param>
    /// <returns></returns>
    private string GetRightEndMessage(bool win)
    {
        if (win)
            return _onWinning;
        else
            return _onLose;
    }
}
