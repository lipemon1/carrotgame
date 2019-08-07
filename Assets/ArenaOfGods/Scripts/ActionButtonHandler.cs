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
        public bool ShowAimSign;

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
    [SerializeField] private PlayerArea _curPlayerArea;
    [SerializeField] private PlayerArea _centerPlayerArea;

    [Header("Qualify Status")]
    [SerializeField] private bool _onMyArea;
    [SerializeField] private bool _haveCarrots;
    [SerializeField] private bool _fullCarrots;
    [SerializeField] private bool _isTouchingCarrot;

    [Header("Buttons Configurations")]
    [SerializeField] public ButtonConfiguration _plantConfiguration;
    [SerializeField] public ButtonConfiguration _pickConfiguration;
    [SerializeField] public ButtonConfiguration _shootConfiguration;

    [Header("Interface Button")] [SerializeField]
    private System.Action _action;
    [SerializeField] private Button _actionButton;
    [SerializeField] private Text _actionButtonText;
    [SerializeField] private Image _actionButtonImage;
    public ItemDropper itemDropper;
    public ItemPicker itemPicker;
    public PlayerMovement playerMovement;
    public string inputTrigger;
    [Space]

    [Header("Aim Sprite")]
    [SerializeField] private GameObject _aimSign;

    [Header("Scripts")]
    [SerializeField] private SetupLocalPlayer _playerConfig;

    private void Update()
    {
        CheckButtonToShow();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerArea"))
        {
            Debug.Log("Colliding with some player area: " + other.gameObject.name);
            _curPlayerArea = other.GetComponent<PlayerArea>();

            GameData.Instance.CanIBeTheOwner(_curPlayerArea.Id, _playerConfig.PlayerId);

            CheckButtonToShow();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerArea"))
        {
            CheckButtonToShow();

            if (_curPlayerArea != null)
            {
                _curPlayerArea = null;
            }
        }
    }
    
    /// <summary>
    /// Verifica qual botão deve ser mostrado para o jogador
    /// </summary>
    public void CheckButtonToShow()
    {
        _aimSign.SetActive(false);

        UpdateStatus();

        if (_showDebugMessages) Debug.Log("Status verificados");

        if (_onMyArea)
        {
            if (_haveCarrots == false)
            {
                ChangeGunStatus(true);
                ChangeButton(_shootConfiguration, () => playerMovement.Fire());
            }
            else
            {
                ChangeGunStatus(false);
                ChangeButton(_plantConfiguration, () => itemDropper.TryToPlantCarrot());
            }
        }
        else
        {
            ChangeGunStatus(false);
            if (_fullCarrots == false && _isTouchingCarrot)
            {
                ChangeButton(_pickConfiguration, () => itemPicker.TryToPickCarrot());
            }
            else
            {
                _action = null;
//                _actionButton.interactable = false;
            }
        }
            
        Debug.Log("Aguardando input de tiro");
        if (Input.GetButtonDown(inputTrigger))
        {
            _action?.Invoke();
            Debug.Log("Deve atirar");
        }
    }


    /// <summary>
    /// Método que atualiza o botão que será liberado para o jogador
    /// </summary>
    /// <param name="newButtonConfiguration"></param>
    private void ChangeButton(ButtonConfiguration newButtonConfiguration, System.Action action)
    {
        _action = action;
//        _actionButton.onClick.RemoveAllListeners();
//        _actionButton.onClick = newButtonConfiguration.ButtonEvent;
//
//        _actionButtonText.text = newButtonConfiguration.ButtonText;
//
//        _actionButtonImage.sprite = newButtonConfiguration.ButtonSprite;

//        _aimSign.SetActive(newButtonConfiguration.ShowAimSign);
        _aimSign.SetActive(false);

//        _actionButton.interactable = true;
    }

    public void ChangeGunStatus(bool hasGun)
    {
        _playerConfig.PlayerMovement.SetHasGun(hasGun);
    }

    /// <summary>
    /// Busca por novos status das variaveis de checagem
    /// </summary>
    private void UpdateStatus()
    {
        if(_curPlayerArea != null)
        {
            _onMyArea = GameData.Instance.IsThisMyArea(_playerConfig.PlayerId, _curPlayerArea.Id);
            _playerConfig.ItemPicker.SetOnMyArea(_onMyArea);
            if (_showDebugMessages) Debug.Log("Verificando area: " + _onMyArea);
        }
        else
        {
            _onMyArea = false;
            _playerConfig.ItemPicker.SetOnMyArea(_onMyArea);
            if (_showDebugMessages) Debug.Log("Verificando area: " + _onMyArea);
        }

        _haveCarrots = _playerConfig.Inventory.HaveSomeCarrot;
        if (_showDebugMessages) Debug.Log("Verificando se tem cenouras: " + _haveCarrots);

        _fullCarrots = _playerConfig.Inventory.IsFull;
        if (_showDebugMessages) Debug.Log("Verificando se o inventario esta cheio: " + _fullCarrots);

        _isTouchingCarrot = _playerConfig.ItemPicker.IsTouchingCarrot;
        if (_showDebugMessages) Debug.Log("Verificando se esta tocando alguma cenoura: " + _isTouchingCarrot);
    }

    private void UpdateActionButtonValues(ButtonData newButtonData)
    {
        _actionButton = newButtonData.ActionButton;
        _actionButtonText = newButtonData.ActionText;
        _actionButtonImage = newButtonData.ActionImage;
    }

    /// <summary>
    /// Retorna a Player Area atual do jogador
    /// </summary>
    /// <returns></returns>
    public PlayerArea GetCurPlayerArea()
    {
        if(_centerPlayerArea == null)
            _centerPlayerArea = GameData.Instance.GetPlayerAreaById(0);

        if (_curPlayerArea != null)
            return _curPlayerArea;
        else
            return _centerPlayerArea;
    }
}
