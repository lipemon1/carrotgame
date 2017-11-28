using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchTime : MonoBehaviour {

    public static MatchTime Instance { get; private set; }

    [Header("Debug")]
    [SerializeField] private bool _showDebugMessages;
    [SerializeField] private bool _canPassTime;

    [Header("Configuration")]
    [SerializeField] private float _timeToRun;

    [Header("Interface")]
    [SerializeField] private Text _timeOnScreen;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    // Update is called once per frame
    void Update() {
        if (_canPassTime)
            PassTime();
    }

    /// <summary>
    /// Inicia a contagem de uma nova partida
    /// </summary>
    public void StartMatchTimer(float newMatchTime)
    {
        if (_showDebugMessages) Debug.Log("Iniciando contagem local de tempo de partida");

        _timeToRun = newMatchTime;
        _canPassTime = true;
    }

    /// <summary>
    /// Atualiza o valor da partida que está sendo mostrada em tela
    /// </summary>
    /// <param name="newTime"></param>
    private void UpdateUI(float newTime)
    {
        _timeOnScreen.text = newTime.ToString("F0");
    }

    /// <summary>
    /// Reduz o tempo da partida
    /// </summary>
    private void PassTime()
    {
        _timeToRun -= Time.deltaTime;
        UpdateUI(_timeToRun);
    }
}
