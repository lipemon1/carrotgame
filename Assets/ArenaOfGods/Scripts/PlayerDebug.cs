using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDebug : MonoBehaviour {

	[Header("Interface Methods")]
    [SerializeField] private Text _playerIdUI;
    [SerializeField] private Text _readyUI;
    [SerializeField] private Text _playerAreadUI;
    [SerializeField] private Text _carrotsIdUI;
    [SerializeField] private Text _connectedPlayersUI;
    [SerializeField] private Text _everyoneReadyUI;

    [Header("Sufixes")]
    [SerializeField] private string _playerIdLabel;
    [SerializeField] private string _readyLabel;
    [SerializeField] private string _playerAreadLabel;
    [SerializeField] private string _carrotsIdLabel;
    [SerializeField] private string _connectedPlayersLabel;
    [SerializeField] private string _everyoneReadyLabel;

    public void DebugPlayer(int playerId, bool isReady, int playerArea, List<int> carrotsId, int connectedPlayersAmount, bool allReady)
    {
        _playerIdUI.text = _playerIdLabel + ": " + playerId.ToString();
        _readyUI.text = _readyLabel + ": " + isReady;
        _playerAreadUI.text = _playerAreadLabel + ": " + playerArea.ToString();
        _carrotsIdUI.text = _carrotsIdLabel + ": " + GetListAsString(carrotsId);
        _connectedPlayersUI.text = _connectedPlayersLabel + ": " + connectedPlayersAmount.ToString();
        _everyoneReadyUI.text = _everyoneReadyLabel + ": " + allReady;
    }

    private string GetListAsString(List<int> itensToGetString)
    {
        string itensAsString = "";

        if(itensToGetString.Count > 0)
        {
            foreach (int item in itensToGetString)
            {
                itensAsString += " [" + item + "],";
            }
        }
        else
        {
            itensAsString = "Nenhuma cenoura";
        }

        return itensAsString;
    }
}
