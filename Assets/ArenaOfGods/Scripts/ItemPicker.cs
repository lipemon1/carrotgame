using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPicker : MonoBehaviour {

    [Header("Debug")]
    [SerializeField] private bool _showDebugMessages;
    [SerializeField] private Carrot _targetToPick;
    [SerializeField] public bool IsTouchingCarrot;
    [SerializeField] private bool _onMyArea;

    [Header("Scripts")]
    [SerializeField] private ActionButtonHandler _actionButtonHandler;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private PlayerIdentity _playerIdentity;
    [SerializeField] private SetupLocalPlayer _setupLocalPlayer;

    private void Start()
    {
        if (_actionButtonHandler == null)
            _actionButtonHandler.GetComponent<ActionButtonHandler>();

        if (_inventory == null)
            _inventory = GetComponent<Inventory>();

        if (_playerIdentity == null)
            _playerIdentity = GetComponent<PlayerIdentity>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Carrot") && !_onMyArea && !_inventory.IsFull)
        {
            if(_showDebugMessages) Debug.Log("Colliding with some carrot: " + other.gameObject.name);
            _targetToPick = other.GetComponent<Carrot>();
            _targetToPick.GetComponent<CarrotBehaviour>().OnTargetEnter();
            IsTouchingCarrot = true;
            _actionButtonHandler.CheckButtonToShow();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Carrot") && !_onMyArea && !_inventory.IsFull)
        {
            GettingAwayFromCarrot();
        }
    }

    private void GettingAwayFromCarrot()
    {
        if (_targetToPick != null)
        {
            _targetToPick.GetComponent<CarrotBehaviour>().OnTargetExit();
            _targetToPick = null;
            IsTouchingCarrot = false;
            _actionButtonHandler.CheckButtonToShow();
        }
    }

    /// <summary>
    /// Método que é chamado pelo botão de pegar cenoura
    /// </summary>
    public void TryToPickCarrot()
    {
        int carrotToPick = _targetToPick.Id;

        if (_inventory.TryToStoreCarrot(carrotToPick))
        {
            if (_showDebugMessages) Debug.Log("Consegui guardar no inventário, mudar no servidor agora");
            GettingAwayFromCarrot();
            _setupLocalPlayer.GameData.ChangeCarrotPlayerOwner(carrotToPick, _playerIdentity.PlayerId);
        }
    }

    /// <summary>
    /// Altera o status de se estou na minha area
    /// </summary>
    /// <param name="value"></param>
    public void SetOnMyArea(bool value)
    {
        _onMyArea = value;
    }
}
