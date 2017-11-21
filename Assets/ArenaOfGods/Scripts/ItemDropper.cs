using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropper : MonoBehaviour {

    [Header("Debug")]
    [SerializeField] private bool _showDebugMessages;
    [SerializeField] private bool _onMyArea;

    [Header("Scripts")]
    [SerializeField] private ActionButtonHandler _actionButtonHandler;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private SetupLocalPlayer _setupLocalPlayer;

    private void Start()
    {
        if (_actionButtonHandler == null)
            _actionButtonHandler.GetComponent<ActionButtonHandler>();

        if (_inventory == null)
            _inventory = GetComponent<Inventory>();
    }

    /// <summary>
    /// Método que é chamado pelo botão de pegar cenoura
    /// </summary>
    public void TryToPlantCarrot()
    {
        int carrotIdRemovedFromInventory = _inventory.TryToWithdrawCarrot();

        if (carrotIdRemovedFromInventory != -1)
        {
            if (_showDebugMessages) Debug.Log("Consegui retirar do inventário, mudar no servidor agora");
            _setupLocalPlayer.GameData.AddCarrotToArea(carrotIdRemovedFromInventory, _actionButtonHandler.CurPlayerArea.Id, transform.position);
        }
    }
}
