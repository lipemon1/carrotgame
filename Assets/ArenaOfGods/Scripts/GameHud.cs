using System.Collections;
using System.Collections.Generic;
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

    void Awake()
    {
        Instance = this;
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
        _endGameMessage.text = GetRightEndMessage(win);
        _endGamePanel.SetActive(true);
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
