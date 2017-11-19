using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActionButtonHandler : MonoBehaviour {

    [System.Serializable]
    public struct ButtonConfiguration
    {
        [Header("Interface")]
        public string ButtonText;
        public Sprite ButtonSprite;

        [Header("Core")]
        public Button.ButtonClickedEvent ButtonEvent;
    }

    public static ActionButtonHandler Instance { get; private set; }

    [Header("Debug")]
    [SerializeField] private Collider _curZone;

    [Header("Qualify Status")]
    [SerializeField] private bool _onMyArea;
    [SerializeField] private bool _haveCarrots;
    [SerializeField] private bool _fullCarrots;

    [Header("Buttons Configurations")]
    [SerializeField] private ButtonConfiguration _plantConfiguration;
    [SerializeField] private ButtonConfiguration _pickConfiguration;
    [SerializeField] private ButtonConfiguration _shootConfiguration;

    [Header("Interface Button")]
    [SerializeField] private Button _actionButton;
    [SerializeField] private Text _actionButtonText;
    [SerializeField] private Image _actionButtonImage;

    private void Awake()
    {
        Instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerArea"))
        {
            Debug.Log("Colliding with some player area: " + other.gameObject.name);
            _curZone = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerArea"))
        {
            if (_curZone != null)
            {
                _curZone = null;
            }
        }
    }
    
    /// <summary>
    /// Verifica qual botão deve ser mostrado para o jogador
    /// </summary>
    public void CheckButtonToShow()
    {
        if (_onMyArea)
        {
            if(_haveCarrots == false)
            {
                ChangeButton(_shootConfiguration);
            }
            else
            {
                ChangeButton(_plantConfiguration);
            }
        }
        else
        {
            if(_fullCarrots == false)
            {
                ChangeButton(_pickConfiguration);
            }
        }
    }

    /// <summary>
    /// Método que atualiza o botão que será liberado para o jogador
    /// </summary>
    /// <param name="newButtonConfiguration"></param>
    private void ChangeButton(ButtonConfiguration newButtonConfiguration)
    {
        _actionButton.onClick.RemoveAllListeners();
        _actionButton.onClick = newButtonConfiguration.ButtonEvent;

        _actionButtonText.text = newButtonConfiguration.ButtonText;

        _actionButtonImage.sprite = newButtonConfiguration.ButtonSprite;

        _actionButton.interactable = true;
    }

    /// <summary>
    /// Busca por novos status das variaveis de checagem
    /// </summary>
    private void UpdateStatus()
    {

    }
}
