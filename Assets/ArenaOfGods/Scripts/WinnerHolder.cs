using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class WinnerHolder : NetworkBehaviour {

    [Header("Debug")]
    [SerializeField] private bool _showDebugMessage;
    [SerializeField] private bool _showDebugMessageOnChecking;

    [Header("Status")]
    [SerializeField] private List<PlayerArea> _winners = new List<PlayerArea>();
    [SerializeField] private List<PlayerArea> _losers = new List<PlayerArea>();

    public void CheckNewWinners(List<PlayerArea> allPlayersAreas)
    {
        _winners = WhoHaveMoreCarrots(allPlayersAreas);
        _losers = FilterAreasTakeOutWinners(allPlayersAreas, _winners);
    }

    /// <summary>
    /// Retorna uma lista com as players areas com maior quantidade de cenouras
    /// </summary>
    /// <param name="playerAreas"></param>
    /// <returns></returns>
    private List<PlayerArea> WhoHaveMoreCarrots(List<PlayerArea> playerAreas)
    {
        if (_showDebugMessageOnChecking) Debug.Log("Iniciando verificação do Rank, verificando com " + playerAreas.Count + " área de jogadores.");
        List<PlayerArea> playerAreaAWithMoreCarrots = new List<PlayerArea>();

        int highestCarrotAmountFound = 0;

        foreach (PlayerArea playerArea in playerAreas)
        {
            if (_showDebugMessageOnChecking) Debug.Log("Maior quantidade de cenoura encontrada até o momento: " + highestCarrotAmountFound);

            if (_showDebugMessageOnChecking) Debug.Log("Iniciando checagem na Player Area: " + playerArea.Id + "...");

            if (playerArea.CarrotsList.Count > highestCarrotAmountFound)
            {
                if (_showDebugMessageOnChecking) Debug.Log("Nova maior quantidade de cenoura encontrada <" + playerArea.CarrotsList.Count + "> na Player Area " + playerArea.Id);
                highestCarrotAmountFound = playerArea.CarrotsList.Count;
            }
            else
            {
                if (_showDebugMessageOnChecking) Debug.Log("Player Area: " + playerArea.Id + " não tem uma quantidade de cenouras maior que: " + playerArea.CarrotsList.Count + ".");
            }

            if (_showDebugMessageOnChecking) Debug.Log("Finalizando checagem na Player Area: " + playerArea.Id + ". Indo para a próxima Player Area...");
        }

        playerAreaAWithMoreCarrots = playerAreas.Where(pa => pa.CarrotsList.Count == highestCarrotAmountFound).ToList();

        if (_showDebugMessageOnChecking) Debug.Log("Retornando lista de player areas < " + GetStringForDebug(playerAreaAWithMoreCarrots) + " > com maior quantidade de cenouras <" + highestCarrotAmountFound + ">");

        return playerAreaAWithMoreCarrots;
    }

    /// <summary>
    /// Retorna a lista de playerareas sem os vencedores
    /// </summary>
    /// <param name="allPlayersAreas"></param>
    /// <param name="winnersPlayersAreas"></param>
    /// <returns></returns>
    private List<PlayerArea> FilterAreasTakeOutWinners(List<PlayerArea> allPlayersAreas, List<PlayerArea> winnersPlayersAreas)
    {
        return allPlayersAreas.Except(winnersPlayersAreas).ToList();
    }

    /// <summary>
    /// Retrona string auxiliar para debug durante a checagem
    /// </summary>
    /// <param name="playerAreaToGetId"></param>
    /// <returns></returns>
    private string GetStringForDebug(List<PlayerArea> playerAreaToGetId)
    {
        string stringToReturn = "";

        foreach (PlayerArea playerArea in playerAreaToGetId)
        {
            stringToReturn += "[" + playerArea.Id.ToString() + "]";
        }

        return stringToReturn;
    }
}
