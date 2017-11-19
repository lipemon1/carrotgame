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

    public struct ButtonData
    {
        public Button ActionButton;
        public Text ActionText;
        public Image ActionImage;
    }

    [Header("Debug")]
    [SerializeField] private bool _showDebugMessages;
    [SerializeField] private PlayerAreaIdentity _curZone;
    [SerializeField] private int _playerId;

    [Header("Qualify Status")]
    [SerializeField] private bool _onMyArea;
    [SerializeField] private bool _haveCarrots;
    [SerializeField] private bool _fullCarrots;
    [SerializeField] private bool _isTouchingCarrot;

    [Header("Buttons Configurations")]
    [SerializeField] private ButtonConfiguration _plantConfiguration;
    [SerializeField] private ButtonConfiguration _pickConfiguration;
    [SerializeField] private ButtonConfiguration _shootConfiguration;

    [Header("Interface Button")]
    [SerializeField] private Button _actionButton;
    [SerializeField] private Text _actionButtonText;
    [SerializeField] private Image _actionButtonImage;
    [Space]
    [SerializeField] private ButtonData _buttonData;

    [Header("Scripts")]
    [SerializeField] private Inventory _inventory;
    [SerializeField] private ItemPicker _itemPicker;

    private void Start()
    {
        if(_inventory == null)
            _inventory = GetComponent<Inventory>();

        if(_itemPicker == null)
            _itemPicker = GetComponent<ItemPicker>();

        if(_buttonData.ActionButton == null)
        {
            GameObject actionButton = GameObject.Find("ScreenJoystick").gameObject;
            _buttonData.ActionButton = actionButton.GetComponentInChildren<Button>();
            _buttonData.ActionImage = actionButton.GetComponentInChildren<Image>();
            _buttonData.ActionText = actionButton.GetComponentInChildren<Text>();

            UpdateActionButtonValues(_buttonData);
        }
    }

    private void Update()
    {
        CheckButtonToShow();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerArea"))
        {
            CheckButtonToShow();

            Debug.Log("Colliding with some player area: " + other.gameObject.name);
            _curZone = other.GetComponent<PlayerAreaIdentity>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerArea"))
        {
            CheckButtonToShow();

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
        UpdateStatus();

        if (_showDebugMessages) Debug.Log("Status verificados");

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
            if(_fullCarrots == false && _isTouchingCarrot)
            {
                ChangeButton(_pickConfiguration);
            }
            else
            {
                _actionButton.interactable = false;
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
        if(_curZone != null)
        {
            _onMyArea = GameData.Instance.IsThisMyArea(_playerId, _curZone.PlayerAreaId);
            if (_showDebugMessages) Debug.Log("Verificando area: " + _onMyArea);
        }
        else
        {
            _onMyArea = GameData.Instance.IsThisMyArea(_playerId, -2);
            if (_showDebugMessages) Debug.Log("Verificando area: " + _onMyArea);
        }

        _haveCarrots = _inventory.HaveOpenSlot;
        if (_showDebugMessages) Debug.Log("Verificando se tem cenouras: " + _haveCarrots);

        _fullCarrots = _inventory.IsFull;
        if (_showDebugMessages) Debug.Log("Verificando se o inventario esta cheio: " + _fullCarrots);

        _isTouchingCarrot = _itemPicker.IsTouchingCarrot;
        if (_showDebugMessages) Debug.Log("Verificando se esta tocando alguma cenoura: " + _isTouchingCarrot);
    }

    /// <summary>
    /// Recebe um novo botão como o botão de action que será utilizado
    /// </summary>
    /// <param name="newButton"></param>
    private void RecieveButtonData(ButtonData newButton)
    {
        _buttonData = newButton;

        UpdateActionButtonValues(_buttonData);
    }

    private void UpdateActionButtonValues(ButtonData newButtonData)
    {
        _actionButton = newButtonData.ActionButton;
        _actionButtonText = newButtonData.ActionText;
        _actionButtonImage = newButtonData.ActionImage;
    }
}
