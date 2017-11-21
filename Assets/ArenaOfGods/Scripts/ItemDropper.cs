using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropper : MonoBehaviour {

    [Header("Debug")]
    [SerializeField] private bool _showDebugMessages;
    [SerializeField] private bool _onMyArea;

    [Header("Scripts")]
    [SerializeField] private SetupLocalPlayer _playerConfig;

    /// <summary>
    /// Método que é chamado pelo botão de pegar cenoura
    /// </summary>
    public void TryToPlantCarrot()
    {
        int carrotIdRemovedFromInventory = _playerConfig.Inventory.TryToWithdrawCarrot();

        if (carrotIdRemovedFromInventory != -1)
        {
            if (_showDebugMessages) Debug.Log("Consegui retirar do inventário, mudar no servidor agora");
            _playerConfig.GameData.AddCarrotToArea(carrotIdRemovedFromInventory, _playerConfig.ActionButtonHandler.CurPlayerArea.Id, transform.position);
        }
    }
}
