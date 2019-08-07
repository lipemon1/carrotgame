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
    [SerializeField] private SetupLocalPlayer _playerConfig;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Carrot") && !_onMyArea && !_playerConfig.Inventory.IsFull)
        {
            if(_showDebugMessages) Debug.Log("Colliding with some carrot: " + other.gameObject.name);
            _targetToPick = other.GetComponent<Carrot>();
            _targetToPick.GetComponent<CarrotBehaviour>().OnTargetEnter();
            IsTouchingCarrot = true;
            _playerConfig.ActionButtonHandler.CheckButtonToShow();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Carrot") && !_onMyArea && !_playerConfig.Inventory.IsFull)
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
            _playerConfig.ActionButtonHandler.CheckButtonToShow();
        }
    }

    /// <summary>
    /// Método que é chamado pelo botão de pegar cenoura
    /// </summary>
    public void TryToPickCarrot()
    {
        int carrotToPick = _targetToPick.Id;

        if (_playerConfig.Inventory.TryToStoreCarrot(carrotToPick))
        {
            if (_showDebugMessages) Debug.Log("Consegui guardar no inventário, mudar no servidor agora");
            GettingAwayFromCarrot();

            PickCarrot(carrotToPick, 0, _playerConfig.PlayerId);
            SoundManager.Instance.CarrotPickUp();
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

    /// <summary>
    /// Chama os método necessários do GameData para fazer o picking da cenoura em todo mundo
    /// </summary>
    /// <param name="carrotToPick"></param>
    /// <param name="areaToStore"></param>
    /// <param name="playerToPick"></param>
    private void PickCarrot(int carrotToPick, int areaToStore, int playerToPick)
    {
        GameData.Instance.ChangeCarrotPlayerOwner(carrotToPick, playerToPick);
        GameData.Instance.ChangeCarrotActiveValue(carrotToPick, false);
        GameData.Instance.ChangeCarrotPlayerArea(carrotToPick, areaToStore, GameData.Operation.Remove);
    }
}
